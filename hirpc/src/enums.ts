
/**
 * Dissonity HiRpc State code
 */
export enum StateCode {
    Unfunctional = 0,       // Outside a web environment
    OutsideDiscord = 1,     // In a web environment, connected to the game but not Discord
    Errored = 2,            // Something went wrong
    Loading = 3,            // Not errored but not ready
    Stable = 4,             // Up and running! A connection was established
}

/**
 * Discord RPC Opcode
 */
export enum Opcode {
    Handshake = 0,
    Frame = 1,
    Close = 2,
    Hello = 3
}

export const RpcCommands = Object.freeze({
    DISPATCH: "DISPATCH",
    AUTHORIZE: "AUTHORIZE",
    AUTHENTICATE: "AUTHENTICATE"
});

export const RpcEvents = Object.freeze({
    READY: "READY",
    ERROR: "ERROR"
});

export const Platform = Object.freeze({
    MOBILE: "mobile",
    DESKTOP: "desktop"
});