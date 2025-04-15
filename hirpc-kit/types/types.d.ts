import { Opcode, StateCode } from "./enums";
import IBuildVariables from "./modules/build_variables";
export type RpcMessage = [Opcode, RpcPayload];
export type RpcPayload = {
    evt?: string;
    cmd?: string;
    nonce?: string;
    data?: unknown;
};
export type RpcInputPayload = {
    evt?: string;
    cmd?: string;
    nonce?: string;
    args?: unknown;
};
export type DissonityChannelError = {
    message: string;
};
export type DissonityChannelHandshake = {
    raw_multi_event: MultiEvent | null;
    hash: string;
};
export type DissonityChannelPayload = {
    nonce?: string;
    response?: string | null;
    nullable_response?: boolean;
};
export type HiRpcMessage = {
    channel: string;
    data: unknown | DissonityChannelPayload | DissonityChannelHandshake | DissonityChannelError;
    opening?: boolean;
    error?: boolean;
};
export type InteropMessage = {
    hirpc_state: StateCode;
    hirpc_message?: HiRpcMessage;
    rpc_message?: RpcMessage;
};
export type ParseVariableType = "string" | "boolean" | "string[]" | "json" | "json_array";
export type BuildVariables = InstanceType<typeof IBuildVariables>;
export type MultiEvent = {
    ready: string;
    authorize: string;
    authenticate: string;
    response: string;
};
