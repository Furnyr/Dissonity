export type RpcBridgeCommands = "StopListening" | "Send" | "RequestQuery" | "RequestState" | "RequestPatchUrlMappings" | "RequestFormatPrice";
export type DissonityBridgeMethods = "_ReceiveString" | "_HandleMessage" | "_ReceiveState" | "_ReceiveMultiEvent" | "_ReceiveEmpty";
export type ParseVariableType = "string" | "boolean" | "string[]" | "map" | "json";

export type RpcFramePayload = {
    cmd: string,
    nonce?: string,
    evt?: string,
    data: any
};