

//# HIRPC - - - - -
import type * as _hiRpcModule from "../../hirpc/pkg/dissonity_hirpc";
export type HiRpcModule = typeof _hiRpcModule;

export type RpcPayload = {
    cmd: string,
    nonce?: string,
    evt?: string,
    data: any
};

export type HiRpcPayload = {
    action_code: number,
    data?: DissonityHiRpcData
};

export type DissonityHiRpcData = {
    channel: string
};

export type GamePayload = {
    hirpc_state: number,
    rpc_message?: [number, RpcPayload],
    hirpc_message?: HiRpcPayload
};

export type BridgeMessage = {
    nonce?: string,
    stringified_data?: string
};

export type AuthenticationPromise = {
    hash: Uint8Array
}