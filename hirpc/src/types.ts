/*

    The only trustworthy messages are those sent through the hiRPC channel "dissonity".

    - Discord sends a message through RPC
    - An RPC message has a few properties, one of those is the RPC payload, that is the main data
    - The hiRPC module sends the message downward to the application (InteropMessage)
    - This interop message contains an RPC or hiRPC message and the hiRPC state
    - A hiRPC message is sent from the JS layer, but not by Discord
    - A hiRPC message has a channel property that identifies who sent it, and a payload, that is the main data
    - (Payloads are usually a property named "data": <RpcMessage>.data, <HiRpcMessage>.data)

    Upward -> Downward
    Discord RPC -> Dissonity hiRPC -> JS layer or hiRPC Interface -> Application

*/

import { Opcode, StateCode } from "./enums"
import IBuildVariables from "./modules/build_variables";

export type RpcMessage = [Opcode, RpcPayload];

export type RpcPayload = {
    evt?: string,
    cmd?: string,
    nonce?: string,
    data?: unknown
};

// Sent when the downward flow is opened.
export type DissonityChannelHandshake = {
    raw_multi_event: MultiEvent
    hash: string
};

// Used in the hiRPC interface. Normal payload in the dissonity channel.
export type DissonityChannelPayload = {
    nonce?: string, // Only included in a response
    query?: string,
    formatted_price?: string
};

export type HiRpcMessage = {
    channel: string,
    data: unknown | DissonityChannelPayload | DissonityChannelHandshake, // Just for documentation: anything can go through hiRPC, but the data sent through {channel:"dissonity"} can be typed.
    opening?: boolean // True in the first payload sent through {channel:"dissonity"}
}

export type InteropMessage = {
    hirpc_state: StateCode,
    hirpc_message?: HiRpcMessage,   // Mutually exclusive
    rpc_message?: RpcMessage        // Mutually exclusive
}

export type ParseVariableType = "string" | "boolean" | "string[]" | "json" | "json_array";

export type BuildVariables = InstanceType<typeof IBuildVariables>;

export type MultiEvent = {
    ready: string,
    authorize: string,
    authenticate: string,
    response: string
}