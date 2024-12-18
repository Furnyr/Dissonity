use wasm_bindgen::prelude::wasm_bindgen;

/// hiRPC State
#[wasm_bindgen]
#[derive(Debug, Copy, Clone)]
pub enum StateCode {
    Unfunctional = 0,       // Outside a web environment
    OutsideDiscord = 1,     // In a web environment, connected to the game but not Discord
    Errored = 2,            // Something went wrong
    Loading = 3,            // Not errored but not ready
    Stable = 4,             // Up and running! A connection was established
}

/// hiRPC Action code
#[wasm_bindgen]
#[derive(Debug, Copy, Clone)]
pub enum ActionCode {
    Hash = 0,           // Sends the hash used for authentication between the game and hiRPC
    Interop = 1,        // Message communication between the JS level and hiRPC
}

/// RPC Opcode
#[wasm_bindgen]
pub enum Opcode {
    Handshake = 0,
    Frame = 1,
    Close = 2,
    Hello = 3
}

/// RPC Close code
#[wasm_bindgen]
pub enum CloseCode {
    CloseNormal = 1000,
    CloseUnsupported = 1003,
    CloseAbnormal = 1006,
    InvalidClientid = 4000,
    InvalidOrigin = 4001,
    Ratelimited= 4002,
    TokenRevoked = 4003,
    InvalidVersion = 4004,
    InvalidEncoding = 4005,
}