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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonity
{
    /// <summary>
    /// This class receives messages from the JavaScript Bridge Library. <br/> <br/>
    /// You do not need to interact with this class directly.
    /// </summary>
    public class DissonityBridge : MonoBehaviour
    {
        #nullable enable

        internal Dictionary<string, object> pendingTasks = new(); // TaskCompletionSource<>

        [Obsolete] internal Action<string>? queryAction = null;
        [Obsolete] internal Action<BridgeStateCode>? stateAction = null;
        [Obsolete] internal Action<MultiEvent>? multiEventAction = null;
        [Obsolete] internal Action? patchUrlMappingsAction = null;
        [Obsolete] internal Action<string>? formatPriceAction = null;

        private const string Dispatch = "DISPATCH";
        private const string Multi = "MULTI";

        //# JAVASCRIPT - - - - -
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void RequestState(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void RequestQuery(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void RequestPatchUrlMappings(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void RequestFormatPrice(string stringifiedMessage);
#endif

#if !UNITY_WEBGL
        private static void RequestState(string _) {}
        private static void RequestQuery(string _) {}
        private static void RequestPatchUrlMappings(string _) {}
        private static void RequestFormatPrice(string _) {}
#endif


        //# INTERACTIONS - - - - -
        // Bridge interactions occur between C# and JS, but they don't get to the client RPC protocol.
        // They are useful to get data to the API level or execute actions,
        // but any interaction can and will alter the RpcBridge state.
        internal Task<BridgeStateCode> ExeState()
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<BridgeStateCode>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage<object?> data = new()
            {
                Nonce = nonce,
                Payload = null
            };

            RequestState(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<string> ExeQuery()
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<string>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage<object?> data = new()
            {
                Nonce = nonce,
                Payload = null
            };

            RequestQuery(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task ExePatchUrlMappings(PatchUrlMappings payload)
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<object?>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage<BridgeStringPayload> data = new()
            {
                Nonce = nonce,
                Payload = new()
                {
                    Str = JsonConvert.SerializeObject(payload)
                }
            };

            RequestPatchUrlMappings(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<string> ExeFormatPrice(FormatPrice payload)
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<string>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage<BridgeStringPayload> data = new()
            {
                Nonce = nonce,
                Payload = new()
                {
                    Str = JsonConvert.SerializeObject(payload)
                }
            };

            RequestFormatPrice(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<MultiEvent> ExeMultiEvent()
        {
            var tcs = new TaskCompletionSource<MultiEvent>();
            pendingTasks.Add(Multi, tcs); // Only one listener per multi event

            return tcs.Task;
        }

        //# RECEIVE (INTERACTIONS) - - - - -
        /// <summary>
        /// This method is used internally but must be public. <b> Don't </b> call it.
        /// </summary>
        public void _ReceiveMultiEvent(string stringifiedMessage)
        {
            var message = JsonConvert.DeserializeObject<BridgeMessage<BridgeMultiPayload>>(stringifiedMessage);

            if (message == null) throw new JsonException("Something went wrong trying to deserialize the bridge message (multi)");

            var payload = message.Payload.RawMultiEvent;

            if (payload == null) throw new JsonException("Something went wrong trying to deserialize the bridge message (multi)");

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

            //? No task
            if (!pendingTasks.ContainsKey(Multi)) return;

            var tcs = (TaskCompletionSource<MultiEvent>) pendingTasks[Multi];
            tcs.TrySetResult(multiEvent);
            pendingTasks.Remove(Multi);
        }


        /// <summary>
        /// This method is used internally but must be public. <b> Don't </b> call it.
        /// </summary>
        public void _ReceiveState(string stringifiedMessage)
        {
            var message = JsonConvert.DeserializeObject<BridgeMessage<BridgeStatePayload>>(stringifiedMessage);

            if (message == null) throw new JsonException("Something went wrong trying to deserialize the bridge message");

            //? No task
            if (!pendingTasks.ContainsKey(message.Nonce!)) return;

            var tcs = (TaskCompletionSource<BridgeStateCode>) pendingTasks[message.Nonce!];
            tcs.TrySetResult(message.Payload.Code);

            pendingTasks.Remove(message.Nonce!);
        }


        // PatchUrlMappings
        /// <summary>
        /// This method is used internally but must be public. <b> Don't </b> call it.
        /// </summary>
        public void _ReceiveEmpty(string stringifiedMessage)
        {
            var message = JsonConvert.DeserializeObject<BridgeMessage<object>>(stringifiedMessage);

            if (message == null) throw new JsonException("Something went wrong trying to deserialize the bridge message");

            //? No task
            if (!pendingTasks.ContainsKey(message.Nonce!)) return;

            var tcs = (TaskCompletionSource<object?>) pendingTasks[message.Nonce!];
            tcs.TrySetResult(null);

            pendingTasks.Remove(message.Nonce!);
        }


        // Query and FormatPrice
        /// <summary>
        /// This method is used internally but must be public. <b> Don't </b> call it.
        /// </summary>
        public void _ReceiveString(string stringifiedMessage)
        {
            var message = JsonConvert.DeserializeObject<BridgeMessage<BridgeStringPayload>>(stringifiedMessage);

            if (message == null) throw new JsonException("Something went wrong trying to deserialize the bridge message");

            //? No task
            if (!pendingTasks.ContainsKey(message.Nonce!)) return;

            var tcs = (TaskCompletionSource<string>) pendingTasks[message.Nonce!];
            tcs.TrySetResult(message.Payload.Str);

            pendingTasks.Remove(message.Nonce!);
        }


        //# HANDLE - - - - -
        /// <summary>
        /// This method is used internally but must be public. <b> Don't </b> call it.
        /// </summary>
        public void _HandleMessage(string stringifiedMessage)
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
                : EventUtility.GetTypeFromString(rawDiscordEvent.Event!);

            var typedEvent = payload.ToObject(eventType);


            //\ Check type
            if (typedEvent is not DiscordEvent discordEvent) throw new JsonException("Something went wrong trying to deserialize the RPC payload");

            //? Not deserialized data
            if (discordEvent.Data == null)
            {
                var dataType = command
                    ? CommandUtility.GetDataTypeFromString(rawDiscordEvent.Command)
                    : EventUtility.GetDataTypeFromString(rawDiscordEvent.Event!);

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
        internal void MockDispatch(string eventString, object eventData)
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