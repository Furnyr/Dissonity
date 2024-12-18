
use wasm_bindgen::prelude::wasm_bindgen;

// Hashes
pub const HASH_RANDOM_BYTES: i32 = 1;   // Only controls left shift

// Handshake
pub const SDK_VERSION: &str = "1.8.0";  // SDK mirroring version
pub const HANDSHAKE_VERSION: i32 = 1;
pub const HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION: i32 = 250;
pub const HANDSHAKE_UNKNOWN_VERSION_NUMBER: i32 = -1;
pub const HANDSHAKE_ENCODING: &str = "json";

// Closures
pub const MAIN_CLOSURE_ID: &str = "main";
pub const CONNECTION_CLOSURE_ID: &str = "cono";

#[wasm_bindgen]
pub struct RpcCommands;

#[wasm_bindgen]
impl RpcCommands {

    #[wasm_bindgen(getter, js_name=DISPATCH)]
    pub fn dispatch() -> String {
        "DISPATCH".to_string()
    }

    #[wasm_bindgen(getter, js_name=AUTHORIZE)]
    pub fn authorize() -> String {
        "AUTHORIZE".to_string()
    }

    #[wasm_bindgen(getter, js_name=AUTHENTICATE)]
    pub fn authenticate() -> String {
        "AUTHENTICATE".to_string()
    }
}

#[wasm_bindgen]
pub struct RpcEvents;

#[wasm_bindgen]
impl RpcEvents {

    #[wasm_bindgen(getter, js_name=READY)]
    pub fn ready() -> String {
        "READY".to_string()
    }

    #[wasm_bindgen(getter, js_name=ERROR)]
    pub fn error() -> String {
        "ERROR".to_string()
    }
}

#[wasm_bindgen]
pub struct Platform;

#[wasm_bindgen]
impl Platform {

    #[wasm_bindgen(getter, js_name=MOBILE)]
    pub fn mobile() -> String {
        "mobile".to_string()
    }

    #[wasm_bindgen(getter, js_name=DESKTOP)]
    pub fn desktop() -> String {
        "desktop".to_string()
    }
}