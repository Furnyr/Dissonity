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
    /// This class receives messages from the JavaScript layer (Dissonity hiRPC or Discord RPC). <br/> <br/>
    /// You do not need to interact with this class directly.
    /// </summary>
    public class DissonityBridge : MonoBehaviour
    {
        #nullable enable

        internal Dictionary<string, object> pendingTasks = new(); // TaskCompletionSource<>

        private const string Dispatch = "DISPATCH";
        private const string MultiId = "MULTI";
        private const string DissonityChannel = "dissonity";

        //# HIRPC INTERFACE - - - - -
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void SaveAppHash(string hash);

        [DllImport("__Internal")]
        private static extern void EmptyRequest(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void GetQueryObject(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void PatchUrlMappings(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void FormatPrice(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void SendToJs(string stringifiedMessage);
#endif

#if !UNITY_WEBGL
        private static void SaveAppHash(string _) {}
        private static void EmptyRequest(string _) {}
        private static void GetQueryObject(string _) {}
        private static void PatchUrlMappings(string _) {}
        private static void FormatPrice(string _) {}
        private static void SendToJs(string _) {}
#endif


        //# SEND (INTERACTIONS) - - - - -
        // Bridge interactions occur between C# and JS through hiRPC messages. These messages don't get to the Discord RPC protocol,
        // they are used to get data to the API level or execute actions.
        // Any interction might send data through hiRPC.
        internal Task<StateCode> ExeState()
        {
            string nonce = Guid.NewGuid().ToString(); 
            var tcs = new TaskCompletionSource<StateCode>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage data = new()
            {
                Nonce = nonce
            };

            EmptyRequest(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<string> ExeQuery()
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<string>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage data = new()
            {
                Nonce = nonce
            };

            GetQueryObject(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<StateCode> ExePatchUrlMappings(PatchUrlMappings payload)
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<StateCode>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage data = new()
            {
                Nonce = nonce,
                StringifiedData = JsonConvert.SerializeObject(payload)
            };

            PatchUrlMappings(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<string> ExeFormatPrice(FormatPrice payload)
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<string>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage data = new()
            {
                Nonce = nonce,
                StringifiedData = JsonConvert.SerializeObject(payload)
            };

            FormatPrice(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<MultiEvent> ExeMultiEvent()
        {
            var tcs = new TaskCompletionSource<MultiEvent>();
            pendingTasks.Add(MultiId, tcs); // Only one listener per multi event

            return tcs.Task;
        }

        //# RECEIVE (INTERACTIONS) - - - - -
        private void HandleMultiEvent(RawMultiEvent payload)
        {
            //? No task
            if (!pendingTasks.ContainsKey(MultiId)) return;

            var tcs = (TaskCompletionSource<MultiEvent>) pendingTasks[MultiId];

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
            ReadyEventData? readyEventData = JsonConvert.DeserializeObject<ReadyEventData>(payload.ReadyMessage);
            AuthorizeData? authorizeData = JsonConvert.DeserializeObject<AuthorizeData>(payload.AuthorizeMessage);
            AuthenticateData? authenticateData = JsonConvert.DeserializeObject<AuthenticateData>(payload.AuthenticateMessage);     
        
            if (readyEventData == null || authorizeData == null || authenticateData == null) throw new JsonException("Something went wrong trying to deserialize the first hiRPC payload");

            //\ Get data and dispatch
            MultiEvent multiEvent = new MultiEvent()
            {
                ReadyData = readyEventData,
                AuthorizeData = authorizeData,
                AuthenticateData = authenticateData,
                ServerResponse = serverPayload
            };

            tcs.TrySetResult(multiEvent);
            pendingTasks.Remove(MultiId);
        }

        // PatchUrlMappings, State
        private void HandleState(string nonce, StateCode stateCode)
        {
            //? No task
            if (!pendingTasks.ContainsKey(nonce)) return;

            var tcs = (TaskCompletionSource<StateCode>) pendingTasks[nonce];
            tcs.TrySetResult(stateCode);

            pendingTasks.Remove(nonce);
        }

        // Query and FormatPrice
        private void HandleString(string nonce, string stringData)
        {
            //? No task
            if (!pendingTasks.ContainsKey(nonce)) return;

            var tcs = (TaskCompletionSource<string>) pendingTasks[nonce];
            tcs.TrySetResult(stringData);

            pendingTasks.Remove(nonce);
        }

        // App hash
        private void HandleHash(string hash)
        {
            SaveAppHash(hash);
        }


        //# HANDLE - - - - -
        /// <summary>
        /// This method is used for communication outside Unity. You don't have to call it.
        /// </summary>
        public void _HiRpcInput(string stringifiedData)
        {
            InteropMessage? message = JsonConvert.DeserializeObject<InteropMessage>(stringifiedData);

            if (message == null) throw new JsonException("Something went wrong trying to deserialize the hiRPC input");

            //? Is RPC message
            if (message.RpcMessage != null)
            {
                HandleRpcMessage(message.RpcMessage);
                return;
            }

            //? Is not a hiRPC message
            if (message.HiRpcMessage == null) return;

            //? Not dissonity channel
            if (message.HiRpcMessage.Channel != DissonityChannel)
            {
                //todo implementation to receive hirpc messages

                return;
            }

            //? First payload
            if (message.HiRpcMessage.Opening == true)
            {
                JObject objectPayload = (JObject) message.HiRpcMessage.Data;
                DissonityChannelHandshake? payload = (DissonityChannelHandshake?) objectPayload.ToObject(typeof(DissonityChannelHandshake));
            
                if (payload == null) throw new JsonException("Something went wrong trying to deserialize the hiRPC input");

                //? Hash
                if (payload.Hash != null)
                {
                    HandleHash(payload.Hash);
                }

                //? Multi Event
                if (payload.RawMultiEvent != null)
                {
                    HandleMultiEvent(payload.RawMultiEvent);
                }
            }

            //? Normal payload
            else
            {
                JObject objectPayload = (JObject) message.HiRpcMessage.Data;
                DissonityChannelPayload? payload = (DissonityChannelPayload?) objectPayload.ToObject(typeof(DissonityChannelPayload));
            
                if (payload == null) throw new JsonException("Something went wrong trying to deserialize the hiRPC input");

                HandleHiRpcMessage(payload, message.HiRpcState);
            }
        }

        private void HandleHiRpcMessage(DissonityChannelPayload payload, StateCode stateCode)
        {
            //? No nonce
            if (payload.Nonce == null)
            {
                throw new Exception("No nonce found in hiRPC message with channel 'dissonity'");
            }

            //? Query
            else if (payload.Query != null)
            {
                HandleString(payload.Nonce, payload.Query);
            }

            //? Formatted price
            else if (payload.FormattedPrice != null)
            {
                HandleString(payload.Nonce, payload.FormattedPrice);
            }

            //? Empty
            else
            {
                HandleState(payload.Nonce, stateCode);
            }
        }

        private void HandleRpcMessage(object[] discordEvent)
        {
            // Opcode
            Opcode opcode = (Opcode) (long) discordEvent[0];

            // Payload
            JObject payload = (JObject) discordEvent[1];

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