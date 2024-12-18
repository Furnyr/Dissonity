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
    /// This class receives messages from the JavaScript level (Dissonity hiRPC or Discord RPC). <br/> <br/>
    /// You do not need to interact with this class directly.
    /// </summary>
    public class DissonityBridge : MonoBehaviour
    {
        #nullable enable

        internal Dictionary<string, object> pendingTasks = new(); // TaskCompletionSource<>

        private const string Dispatch = "DISPATCH";
        private const string MultiId = "MULTI";
        private const string HashId = "HASH";
        private const string DissonityChannel = "dissonity";

        //# JAVASCRIPT - - - - -
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void SaveAppHash(string hash);

        [DllImport("__Internal")]
        private static extern void RequestEmpty(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void RequestQuery(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void RequestPatchUrlMappings(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void RequestFormatPrice(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void SendHiRpc(string stringifiedMessage);
#endif

#if !UNITY_WEBGL
        private static void SaveAppHash(string _) {}
        private static void RequestEmpty(string _) {}
        private static void RequestQuery(string _) {}
        private static void RequestPatchUrlMappings(string _) {}
        private static void RequestFormatPrice(string _) {}
        private static void SendHiRpc(string _) {}
#endif


        //# SEND (INTERACTIONS) - - - - -
        // Bridge interactions occur between C# and JS through hiRPC messages. These messages don't get to the Discord RPC protocol,
        // they are used to get data to the API level or execute actions.
        // Any interaction can and will alter the state of the connection.
        internal Task<StateCode> ExeState()
        {
            string nonce = Guid.NewGuid().ToString(); 
            var tcs = new TaskCompletionSource<StateCode>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage data = new()
            {
                Nonce = nonce
            };

            // Get state
            RequestEmpty(JsonConvert.SerializeObject(data));

            // Notify Unity to be ready by sending empty data
            SendHiRpc("");

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

            RequestQuery(JsonConvert.SerializeObject(data));

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

            RequestPatchUrlMappings(JsonConvert.SerializeObject(data));

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

            RequestFormatPrice(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<MultiEvent> ExeMultiEvent()
        {
            var tcs = new TaskCompletionSource<MultiEvent>();
            pendingTasks.Add(MultiId, tcs); // Only one listener per multi event

            return tcs.Task;
        }

        internal Task ExeHash()
        {
            var tcs = new TaskCompletionSource<object?>();
            pendingTasks.Add(HashId, tcs);

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

            foreach (string key in pendingTasks.Keys)
            {
                Debug.Log(key);
            }

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

            //? No task
            if (!pendingTasks.ContainsKey(HashId)) return;

            var tcs = (TaskCompletionSource<object?>) pendingTasks[HashId];
            tcs.TrySetResult(null);

            pendingTasks.Remove(HashId);
        }


        //# HANDLE - - - - -
        /// <summary>
        /// This method is used for communication outside Unity. You don't have to call it.
        /// </summary>
        public void _HiRpcInput(string stringifiedData)
        {
            AppPayload? message = JsonConvert.DeserializeObject<AppPayload>(stringifiedData);

            if (message == null) throw new JsonException("Something went wrong trying to deserialize the hiRPC input");

            //? Is RPC message
            if (message.RpcMessage != null)
            {
                HandleRpcMessage(message.RpcMessage);
                return;
            }

            //? Is hiRPC message
            if (message.HiRpcMessage == null) return;

            // Handle app hash
            if (message.HiRpcMessage.ActionCode == ActionCode.Hash)
            {
                HandleHash(message.HiRpcMessage.Data.Hash!);
                return;
            }

            //? Not dissonity channel
            if (message.HiRpcMessage.Data.Channel != DissonityChannel)
            {
                //todo implementation to receive hirpc messages

                return;
            }

            HandleHiRpcMessage(message.HiRpcMessage.Data, message.HiRpcState);
        }

        private void HandleHiRpcMessage(HiRpcData message, StateCode stateCode)
        {
            //? Multi Event
            if (message.RawMultiEvent != null)
            {
                HandleMultiEvent(message.RawMultiEvent);
                return;
            }

            // From this point, a nonce is needed
            if (message.Nonce == null)
            {
                throw new Exception("No nonce found in hiRPC message with channel 'dissonity'");
            }

            //? Query
            else if (message.Query != null)
            {
                HandleString(message.Nonce, message.Query);
            }

            //? Formatted price
            else if (message.FormattedPrice != null)
            {
                HandleString(message.Nonce, message.FormattedPrice);
            }

            //? Empty
            else
            {
                HandleState(message.Nonce, stateCode);
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