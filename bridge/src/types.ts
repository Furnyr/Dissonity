export type RpcBridgeCommands = "StopListening" | "Send" | "RequestQuery" | "RequestState" | "RequestPatchUrlMappings";
export type DissonityBridgeMethods = "ReceiveQuery" | "HandleMessage" | "ReceiveState" | "ReceiveMultiEvent" | "ReceivePatchUrlMappings";
export type ParseVariableType = "string" | "boolean" | "string[]" | "map" | "json";

export type RpcFramePayload = {
    cmd: string,
    nonce?: string,
    evt?: string,
    data: any
};