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
        private static extern void DsoEmptyRequest(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoGetQueryObject(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoPatchUrlMappings(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoFormatPrice(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoSendToJs(string stringifiedMessage);

        [DllImport("__Internal")]
        private static extern void DsoLocalStorageGetItem(string stringifiedMessage);
#endif

#if !UNITY_WEBGL
        private static void DsoEmptyRequest(string _) {}
        private static void DsoGetQueryObject(string _) {}
        private static void DsoPatchUrlMappings(string _) {}
        private static void DsoFormatPrice(string _) {}
        private static void DsoSendToJs(string _) {}
        private static void DsoLocalStorageGetItem(string _) {}
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
                Nonce = nonce,
                AppHash = Api._appHash!
            };

            DsoEmptyRequest(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<string> ExeQuery()
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<string>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage data = new()
            {
                Nonce = nonce,
                AppHash = Api._appHash!
            };

            DsoGetQueryObject(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<string?> ExeLocalStorageGetItem(string key)
        {
            string nonce = Guid.NewGuid().ToString();            
            var tcs = new TaskCompletionSource<string?>();
            pendingTasks.Add(nonce, tcs);

            BridgeMessage data = new()
            {
                Nonce = nonce,
                AppHash = Api._appHash!,
                Data = key
            };

            DsoLocalStorageGetItem(JsonConvert.SerializeObject(data));

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
                AppHash = Api._appHash!,
                Data = payload
            };

            DsoPatchUrlMappings(JsonConvert.SerializeObject(data));

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
                AppHash = Api._appHash!,
                Data = payload
            };

            DsoFormatPrice(JsonConvert.SerializeObject(data));

            return tcs.Task;
        }

        internal Task<MultiEvent?> ExeMultiEvent()
        {
            var tcs = new TaskCompletionSource<MultiEvent?>();
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

            //\ End task
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

        // DsoLocalStorageGetItem
        private void HandleNullableString(string nonce, string? stringData)
        {
            //? No task
            if (!pendingTasks.ContainsKey(nonce)) return;

            var tcs = (TaskCompletionSource<string?>) pendingTasks[nonce];
            tcs.TrySetResult(stringData);

            pendingTasks.Remove(nonce);
        }

        // App hash
        private void HandleHash(string hash)
        {
            Api._appHash = hash;
        }


        //# HANDLE - - - - -
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
                Api.hiRpcMessageBus.DispatchEvent(message.HiRpcMessage);

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

                // The multi event task is completed whether it's null or not.
                // A null value indicates an outside Discord state.
                else
                {
                    //? No task
                    if (!pendingTasks.ContainsKey(MultiId)) return;
                    var tcs = (TaskCompletionSource<MultiEvent?>) pendingTasks[MultiId];

                    //\ End task
                    tcs.TrySetResult(null);
                    pendingTasks.Remove(MultiId);
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

            //? Nullable string
            else if (payload.NullableResponse == true)
            {
                HandleNullableString(payload.Nonce, payload.Response);
            }

            //? String
            else if (payload.Response != null)
            {
                HandleString(payload.Nonce, payload.Response);
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
                Api.discordMessageBus.DispatchEvent(deserializedEvent);
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
                Api.discordMessageBus.DispatchEvent(deserializedEvent);
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

            //? No Response
            Type responseType = CommandUtility.GetResponseFromString(responseEvent.Command);
            if (responseType == typeof(NoResponse))
            {
                // Cast, as we know this is a NoResponse
                ((TaskCompletionSource<NoResponse>) commandTask).TrySetResult(new NoResponse());
            }

            else
            {
                // At this point we already have access to the command task and command response.
                // Reflections are used since to call TrySetResult we would need to cast (TaskCompletionSource<T>) but we have typeof(T)

                //\ Access TrySetResult
                MethodInfo trySetResult = commandTask.GetType().GetMethod(nameof(TaskCompletionSource<object>.TrySetResult));
                trySetResult.Invoke(commandTask, new object[] { responseEvent });
            }

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
        internal void MockDiscordDispatch(string eventString, object eventData)
        {
            //? Not mock mode
            if (!Api.IsMock) return;
            
            Type eventType = EventUtility.GetTypeFromString(eventString);
            var eventInstance = Activator.CreateInstance(eventType);

            if (eventInstance is not DiscordEvent typedEventInstance) return;

            typedEventInstance.Data = eventData;
            typedEventInstance.Event = eventString;

            Api.discordMessageBus.DispatchEvent(typedEventInstance);
        }

        internal void MockHiRpcDispatch(string channel, object data)
        {
            //? Not mock mode
            if (!Api.IsMock) return;
            
            HiRpcMessage message = new()
            {
                Channel = channel,
                Data = data
            };

            Api.hiRpcMessageBus.DispatchEvent(message);
        }
    }
}