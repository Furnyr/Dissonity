/*

The only trustworthy messages are those sent through the hiRPC channel "dissonity".
All the others could be falsified.

- Discord sends a message through RPC
- An RPC message has a few properties, one of those is the RPC payload, that is the main data
- The hiRPC module sends the message downward to the application (InteropMessage)
- This interop message contains an RPC or hiRPC message and the hiRPC state
- A hiRPC message is sent from the JS level, but not by Discord
- A hiRPC message has a channel property that identifies who sent it, and a payload, that is the main data
- (Payloads are usually a property named "data": <RpcMessage>.data, <HiRpcMessage>.data)

Upward -> Downward
Discord RPC -> Dissonity hiRPC -> JS Level or hiRPC Interface -> Application

*/

import { Opcode, StateCode } from "./enums"
import BuildVariables from "./modules/build_variables";

export type RpcMessage = [Opcode, RpcPayload];

//todo check
export type RpcPayload = {
    evt?: string,
    cmd?: string,
    nonce?: string,
    data?: unknown
};

export type HiRpcMessage = {
    channel: string,
    data: unknown
}

export type InteropMessage = {
    hirpc_state: StateCode,
    hirpc_message?: HiRpcMessage,
    rpc_message?: RpcMessage
}

export type ParseVariableType = "string" | "boolean" | "string[]" | "json";

export type BuildVariables = InstanceType<typeof BuildVariables>

/*
    {
        hirpc_state: Stable,
        hirpc_message: {
            channel: "dissonity",
            data: {
                hash: string
            }
        },
        rpc_message: [Opcode, Payload]
    }
*/