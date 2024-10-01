using Newtonsoft.Json;
using Dissonity.Models.Interop;
using UnityEngine;
using Dissonity.Events;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Dissonity.Commands;
using Dissonity.Commands.Responses;
using Dissonity.Models;
using System.Linq;

namespace Dissonity
{
    /// <summary>
    /// This class receives messages from the JavaScript Bridge Library. <br/> <br/>
    /// You do not need to interact with this class directly.
    /// </summary>
    public class DissonityBridge : MonoBehaviour
    {
        #nullable enable

        internal Action<string>? queryAction = null;
        internal Action<BridgeStateCode>? stateAction = null;
        internal Action<MultiEvent>? multiEventAction = null;
        internal Action? patchUrlMappingsAction = null;

        private const string Dispatch = "DISPATCH";

        //# RECEIVE - - - - -
        public void ReceiveState(string stringifiedMessage)
        {
            if (stateAction == null) return;

            var message = JsonConvert.DeserializeObject<BridgeStateMessage>(stringifiedMessage);

            if (message == null) return;

            stateAction(message.Code);

            stateAction = null;
        }

        public void ReceiveQuery(string query)
        {
            if (queryAction == null) return;

            queryAction(query);

            queryAction = null;
        }

        // Sent by BridgeLib once the authentication process is done
        public void ReceiveMultiEvent(string stringifiedMessage)
        {
            if (multiEventAction == null) return;

            var payload = JsonConvert.DeserializeObject<RawMultiEvent>(stringifiedMessage);

            if (payload == null) return;

            //\ Get user data
            var userConfig = DissonityConfigAttribute.GetUserConfig();

            //\ Deserialize server response
            MethodInfo[] deserializeOverloads = typeof(JsonConvert).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "DeserializeObject")
                .ToArray();
            MethodInfo deserialize = deserializeOverloads.First(m => m.IsGenericMethod);
            MethodInfo genericDeserialize = deserialize.MakeGenericMethod(userConfig.ServerTokenResponse);
            var serverPayload = genericDeserialize.Invoke(null, new object[] { payload.ServerResponse });

            //\ Deserialize RPC events
            RpcMessage? readyMessage = JsonConvert.DeserializeObject<RpcMessage>(payload.ReadyMessage);
            RpcMessage? authorizeMessage = JsonConvert.DeserializeObject<RpcMessage>(payload.AuthorizeMessage);
            RpcMessage? authenticateMessage = JsonConvert.DeserializeObject<RpcMessage>(payload.AuthenticateMessage);
        
            if (readyMessage == null || authorizeMessage == null || authenticateMessage == null) throw new JsonException("Something went wrong trying to deserialize the first RPC payload");

            var readyEvent = (ReadyEvent) ((JObject) readyMessage.Data[1]).ToObject(typeof(ReadyEvent))!;
            var authorizeResponse = (AuthorizeResponse) ((JObject) authorizeMessage.Data[1]).ToObject(typeof(AuthorizeResponse))!;
            var authenticateResponse = (AuthenticateResponse) ((JObject) authenticateMessage.Data[1]).ToObject(typeof(AuthenticateResponse))!;
        
            //\ Get data and dispatch
            MultiEvent multiEvent = new MultiEvent()
            {
                ReadyData = readyEvent.Data,
                AuthorizeData = authorizeResponse.Data,
                AuthenticateData = authenticateResponse.Data,
                ServerResponse = serverPayload
            };

            multiEventAction(multiEvent);

            multiEventAction = null;
        }

        public void ReceivePatchUrlMappings(string _)
        {
            if (patchUrlMappingsAction == null) return;

            patchUrlMappingsAction();

            patchUrlMappingsAction = null;
        }


        //# HANDLE - - - - -
        public void HandleMessage(string stringifiedMessage)
        {
            var message = JsonConvert.DeserializeObject<RpcMessage>(stringifiedMessage);

            if (message == null) return;

            // Opcode
            Opcode opcode = (Opcode) (long) message.Data[0];

            // Payload
            JObject payload = (JObject) message.Data[1];

            switch (opcode)
            {
                case Opcode.Hello:
                    // No official implementation yet
                    break;

                case Opcode.Close:
                    // No official implementation yet
                    break;

                case Opcode.Handshake:
                    break;

                case Opcode.Frame:
                    try {
                        HandleFrame(payload);
                    } catch(Exception e)
                    {
                        Utils.DissonityLogError(e);
                    }
                    return;

                default:
                    throw new Exception("Invalid message format");
            }
        }

        private void HandleFrame(JObject payload)
        {
            //\ Deserialize raw payload
            DiscordEvent? rawDiscordEvent = (DiscordEvent?) payload.ToObject(typeof(DiscordEvent));

            if (rawDiscordEvent == null) throw new JsonException("Something went wrong trying to deserialize the initial RPC payload");

            var deserializedEvent = (rawDiscordEvent.Event == null)
                ? Deserialize(true, payload, rawDiscordEvent)
                : Deserialize(false, payload, rawDiscordEvent);
                

            if (deserializedEvent.Command == Dispatch)
            {
                Api.messageBus.DispatchEvent(deserializedEvent);
                return;
            }

            if (deserializedEvent.Event == DiscordEventType.Error)
            {
                //? In response to a command
                if (deserializedEvent.Nonce != null)
                {
                    RejectCommand((ErrorEvent) deserializedEvent);
                    return;
                }

                // General error
                Api.messageBus.DispatchEvent(deserializedEvent);
            }

            //? Command
            if (deserializedEvent.Nonce == null)
            {
                throw new Exception("Missing nonce");
            }

            ConsumeCommand(deserializedEvent);
        }

        private DiscordEvent Deserialize(bool command, JObject payload, DiscordEvent rawDiscordEvent)
        {
            //\ Deserialize whole command payload
            var eventType = command
                ? CommandUtility.GetResponseFromString(rawDiscordEvent.Command)
                : EventUtility.GetTypeFromString(rawDiscordEvent.Event);

            var typedEvent = payload.ToObject(eventType);


            //\ Check type
            if (typedEvent is not DiscordEvent discordEvent) throw new JsonException("Something went wrong trying to deserialize the RPC payload");

            //? Not deserialized data
            if (discordEvent.Data == null)
            {
                var dataType = command
                    ? CommandUtility.GetDataTypeFromString(rawDiscordEvent.Command)
                    : EventUtility.GetDataTypeFromString(rawDiscordEvent.Event);

                //? Not no-response
                if (dataType != typeof(NoResponse))
                {
                    var serializedData = ((JToken) rawDiscordEvent.Data).ToObject(dataType);

                    //? Still not deserialized
                    if (serializedData == null) throw new JsonException("An event with null data was detected. This shouldn't happen under normal circumstances");

                    discordEvent.Data = serializedData;
                }
            }

            return discordEvent;
        }

        private void ConsumeCommand(DiscordEvent responseEvent)
        {
            string nonce = responseEvent.Nonce!;

            //\ Get task completion source
            if (!Api.pendingCommands.ContainsKey(nonce)) return;
            var commandTask = Api.pendingCommands[nonce];

            // At this point we already have access to the command task and command response.
            // Reflections are used since to call TrySetResult we would need to cast (TaskCompletionSource<T>) but we have typeof(T)

            //\ Access TrySetResult
            MethodInfo trySetResult = commandTask.GetType().GetMethod(nameof(TaskCompletionSource<object>.TrySetResult));
            trySetResult.Invoke(commandTask, new object[] { responseEvent });

            //¡ Remove the pending command
            Api.pendingCommands.Remove(nonce);
        }

        private void RejectCommand(ErrorEvent errorEvent)
        {
            string nonce = errorEvent.Nonce!;
            int code = errorEvent.Data.Code;
            string message = $"({errorEvent.Command}) {errorEvent.Data.Message}";

            //\ Get task completion source
            var commandTask = Api.pendingCommands[nonce];

            // At this point we already have access to the command task.
            // Reflections are used since to call TrySetException we would need to cast (TaskCompletionSource<T>) but we have typeof(T)

            //\ Access TrySetException
            MethodInfo trySetException = commandTask.GetType().GetMethod(nameof(TaskCompletionSource<object>.TrySetException));
            trySetException.Invoke(commandTask, new object[] { new CommandException(message, code) });

            //¡ Remove the pending command
            Api.pendingCommands.Remove(nonce);
        }
    

        //# MOCK - - - - -
        public void MockDispatch(string eventString, object eventData)
        {
            //? Not mock mode
            if (!Api.IsMock) return;
            
            Type eventType = EventUtility.GetTypeFromString(eventString);
            var eventInstance = Activator.CreateInstance(eventType);

            if (eventInstance is not DiscordEvent typedEventInstance) return;

            typedEventInstance.Data = eventData;
            typedEventInstance.Event = eventString;

            Api.messageBus.DispatchEvent(typedEventInstance);
        }
    }
}