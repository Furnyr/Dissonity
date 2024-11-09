

//# HIRPC - - - - -
import type * as _hiRpcModule from "../../hirpc/pkg/dissonity_hirpc";
export type hiRpcModule = typeof _hiRpcModule;

export type HiRpcCommands = "close" | "send" | "query" | "state" | "patch_url_mappings" | "format_price";

// Used in hirpc-sdk? Nonce will probably be passed to the hiRPC module directly
/*export type RpcBridgeMessage = {
    command: HiRpcCommands,
    nonce?: string,
    payload?: string
};*/