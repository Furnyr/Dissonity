#![allow(dead_code)] //todo remove

use core::str;
use wasm_bindgen::prelude::*;
use web_sys::{js_sys::Reflect, Window};

use crate::constants::HANDSHAKE_UNKNOWN_VERSION_NUMBER;
use crate::enums::*;
use crate::structs::ListenerWrapper;

// public.rs
static mut LOCKED: bool = false;
static mut HASHES: Option<Vec<&[u8]>> = None;

// rpc.rs
static mut READY: bool = false;
static mut ALLOWED_ORIGINS: Option<[&str; 11]> = Some([
    "origin_ph",
    "https://discord.com",
    "https://discordapp.com",
    "https://ptb.discord.com",
    "https://ptb.discordapp.com",
    "https://canary.discord.com",
    "https://canary.discordapp.com",
    "https://staging.discord.co",
    "http://localhost:3333",
    "https://pax.discord.com",
    "null"
]);
static mut STATE_CODE: Option<StateCode> = Some(StateCode::Loading);
static mut UTILS: Option<JsValue> = None;
static mut BUILD_VARIABLES: Option<JsValue> = None;
static mut MOBILE_APP_VERSION: Option<&str> = None;
static mut LISTENERS: Option<Vec<ListenerWrapper>> = None;

// JavaScript utils
#[wasm_bindgen(module = "/pkg/utils.js")]
extern "C" {
    #[wasm_bindgen(js_name="importModule")]
    fn utils_import(key: &str) -> JsValue;

    #[wasm_bindgen(js_name="logObject")]
    pub fn log_object(data: &JsValue);
}


/// Import the JavaScript utils and update allowed origins.
/// 
/// Mobile app version must be updated separately.
pub fn initialize(window: &Window, key: &str) -> bool {
    unsafe {
        let imported = utils_import(key);

        // Already imported
        if imported.is_falsy() {
            return false;
        }

        UTILS = Some(imported);

        if let Some(ref utils) = UTILS {
            BUILD_VARIABLES = match Reflect::get(utils, &"buildVariables".into()) {
                Ok(data) => Some(data),
                Err(_) => return false
            }
        }

        initialize_allowed_origins(window)
    }
}

/// Update `ALLOWED_ORIGINS` with the current window origin.
fn initialize_allowed_origins(window: &Window) -> bool {
    let origin = match window.location().origin() {
        Ok(data) => data,
        Err(_) => {
            return false;
        }
    };

    let boxed = Box::leak(Box::new(origin));

    unsafe {
        if let Some(ref mut origins_ref) = ALLOWED_ORIGINS {
            origins_ref[0] = boxed;
        }
    }

    true
}

/// Access `READY`.
pub fn ready() -> bool {
    unsafe {
        READY
    }
}

/// Set `READY` to true.
pub fn set_ready() {
    unsafe {
        READY = true;
    }
}

/// Access `LOCKED`.
pub fn locked() -> bool {
    unsafe {
        LOCKED
    }
}

/// Set `LOCKED` to true.
pub fn lock_module() {
    unsafe {
        LOCKED = true;
    }
}

/// Access `HASHES`.
pub fn hashes() -> &'static Vec<&'static [u8]> {
    unsafe {
        if let Some(ref hashes_ref) = HASHES {
            hashes_ref
        }

        else {
            // Initialize hashes
            let vec: Vec<&[u8]> = Vec::new();
            HASHES = Some(vec);

            // Return ref
            if let Some(ref hashes_ref) = HASHES {
                hashes_ref
            }

            else {
                panic!();
            }
        }
    }
}

/// Add a new hash to `HASHES`.
pub fn add_hash(hash: &str) {

    let boxed_hash = Box::leak(Box::new(hash.to_string()));
    
    unsafe {
        if let Some(ref mut hashes_ref) = HASHES {
            hashes_ref.push(boxed_hash.as_bytes());
        }
        else {
            // Initialize hashes
            let vec: Vec<&[u8]> = vec![boxed_hash.as_bytes()];

            HASHES = Some(vec);
        }
    }
}

/// Access `ALLOWED_ORIGINS`.
pub fn allowed_origins() -> Option<&'static [&'static str; 11]> {
    unsafe {
        if let Some(ref origins_ref) = ALLOWED_ORIGINS {
            Some(origins_ref)
        }

        else {
            None
        }
    }
}

/// Access `STATE_CODE`.
pub fn state_code() -> Option<&'static StateCode> {
    unsafe {
        if let Some(ref state) = STATE_CODE {
            Some(state)
        }
        
        else {
            None
        }
    }
}

/// Modify `STATE_CODE`.
pub fn update_state(state: StateCode) {
    unsafe {
        STATE_CODE = Some(state);
    }
}

/// Update `MOBILE_APP_VERSION`.
pub fn update_mobile_app_version(string: &'static str) {
    unsafe {
        MOBILE_APP_VERSION = Some(string);
    }
}

/// Access `MOBILE_APP_VERSION`.
pub fn mobile_app_version() -> Option<&'static str> {
    unsafe {
        MOBILE_APP_VERSION
    }
}

/// Get the major mobile version as an i32.
pub fn parse_major_mobile_version() -> i32 {

    let mobile_app_version: Option<&str>;

    unsafe {
        mobile_app_version = if let Some(version) = MOBILE_APP_VERSION {
            Some(version)
        }

        else {
            None
        }
    }

    match mobile_app_version {
        Some(version) => {
            if version.contains('.') {
        
                let major_version = match version.split('.').next(){
                    Some(data) => data,
                    None => return HANDSHAKE_UNKNOWN_VERSION_NUMBER
                };
        
                let num: i32 = match major_version.parse() {
                    Ok(data) => data,
                    Err(_) => HANDSHAKE_UNKNOWN_VERSION_NUMBER
                };
        
                num
            }

            else {
                HANDSHAKE_UNKNOWN_VERSION_NUMBER
            }
        }
        None => HANDSHAKE_UNKNOWN_VERSION_NUMBER
    }

}

/// Access `LISTENERS`.
pub fn listeners() -> &'static Vec<ListenerWrapper> {
    unsafe {
        if let Some(ref listeners_ref) = LISTENERS {
            listeners_ref
        }

        else {
            // Initialize listeners
            let vec: Vec<ListenerWrapper> = Vec::new();
            LISTENERS = Some(vec);

            // Return ref
            if let Some(ref listeners_ref) = LISTENERS {
                listeners_ref
            }

            else {
                panic!();
            }
        }
    }
}

/// Add a new listener to `LISTENERS`.
pub fn add_listener(cb: ListenerWrapper) {
    unsafe {
        if let Some(ref mut listeners_ref) = LISTENERS {
            listeners_ref.push(cb);
        }
        else {
            // Initialize listeners
            let vec: Vec<ListenerWrapper> = vec![cb];

            LISTENERS = Some(vec);
        }
    }
}

/// Remove a listener from `LISTENERS` given its id.
pub fn remove_listener(id: &str) {
    unsafe {
        if let Some(ref mut listeners_ref) = LISTENERS {
            listeners_ref.retain(|l| l.id != id);
        }
    }
}

/// Clear all listeners from `LISTENERS`.
pub fn clear_listeners() {
    unsafe {
        if let Some(ref mut listeners_ref) = LISTENERS {
            listeners_ref.clear();
        }
    }
}

// UTILS
pub mod utils {
    use wasm_bindgen::JsValue;
    use web_sys::js_sys::{Function, Reflect};

    use crate::structs::{Mapping, PatchUrlMappingsConfig};

    use super::UTILS;

    fn get_imported_function(prop_name: &str) -> Option<Function> {
        let js_value: Option<JsValue>;

        unsafe {
            js_value = if let Some(ref vars) = UTILS {
                match Reflect::get(vars, &prop_name.into()) {
                    Ok(data) => Some(data),
                    Err(_) => None
                }
            }
            else { None };
        }

        match js_value {
            Some(generic_value) => {

                if !generic_value.is_function() {
                    None
                }

                else {
                    Some(Function::from(generic_value))
                }
            }
            None => None
        }
    }

    pub fn patch_url_mappings(mappings: &Vec<Mapping>, config: &PatchUrlMappingsConfig) {

        let prop_name = "patchUrlMappings";

        let patch_function = get_imported_function(prop_name);

        if let Some(function) = patch_function {
            let mappings_js = match serde_wasm_bindgen::to_value(&mappings) {
                Ok(data) => data,
                Err(_) => return
            };

            let config_js = match serde_wasm_bindgen::to_value(&config) {
                Ok(data) => data,
                Err(_) => return
            };

            _ = function.call2(&JsValue::NULL, &mappings_js, &config_js);
        }
    }
}

// BUILD VARIABLES
pub mod build {
    use web_sys::js_sys::Reflect;

    use super::*;
    use crate::structs::{Mapping, PatchUrlMappingsConfig};

    fn get_build_variable<T: for<'de> serde::Deserialize<'de>>(prop_name: &str) -> Option<T> {
        let js_value: Option<JsValue>;

        unsafe {
            js_value = if let Some(ref vars) = BUILD_VARIABLES {
                match Reflect::get(vars, &prop_name.into()) {
                    Ok(data) => Some(data),
                    Err(_) => None
                }
            }
            else { None };
        }

        match js_value {
            Some(generic_value) => {
                let deserialized_data: Result<T, _> = serde_wasm_bindgen::from_value(generic_value);

                match deserialized_data {
                    Ok(build_variable) => Some(build_variable),
                    Err(_) => None
                }
            }
            None => None
        }
    }

    pub fn disable_info_logs() -> Option<bool> {
        let prop_name = "DISABLE_INFO_LOGS";

        get_build_variable::<bool>(prop_name)
    }

    pub fn client_id() -> Option<String> {
        let prop_name = "CLIENT_ID";

        get_build_variable::<String>(prop_name)
    }

    pub fn disable_console_log_override() -> Option<bool> {
        let prop_name = "DISABLE_CONSOLE_LOG_OVERRIDE";

        get_build_variable::<bool>(prop_name)
    }

    pub fn mappings() -> Option<Vec<Mapping>> {
        let prop_name = "MAPPINGS";

        get_build_variable::<Vec<Mapping>>(prop_name)
    }

    pub fn patch_url_mappings_config() -> Option<PatchUrlMappingsConfig> {
        let prop_name = "PATCH_URL_MAPPINGS_CONFIG";

        get_build_variable::<PatchUrlMappingsConfig>(prop_name)
    }

    pub fn oauth_scopes() -> Option<Vec<String>> {
        let prop_name = "OAUTH_SCOPES";

        get_build_variable::<Vec<String>>(prop_name)
    }

    pub fn token_request_path() -> Option<String> {
        let prop_name = "TOKEN_REQUEST_PATH";

        get_build_variable::<String>(prop_name)
    }

    pub fn server_request() -> Option<String> {
        let prop_name = "SERVER_REQUEST";

        get_build_variable::<String>(prop_name)
    }
}