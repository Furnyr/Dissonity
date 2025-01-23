/**
 * Dissonity HiRpc State code
 */
export declare enum StateCode {
    Unfunctional = 0,// Outside a web environment
    OutsideDiscord = 1,// In a web environment, connected to the game but not Discord
    Errored = 2,// Something went wrong
    Loading = 3,// Not errored but not ready
    Stable = 4
}
/**
 * Discord RPC Opcode
 */
export declare enum Opcode {
    Handshake = 0,
    Frame = 1,
    Close = 2,
    Hello = 3
}
export declare const RpcCommands: Readonly<{
    DISPATCH: "DISPATCH";
    AUTHORIZE: "AUTHORIZE";
    AUTHENTICATE: "AUTHENTICATE";
}>;
export declare const RpcEvents: Readonly<{
    READY: "READY";
    ERROR: "ERROR";
}>;
export declare const Platform: Readonly<{
    MOBILE: "mobile";
    DESKTOP: "desktop";
}>;
