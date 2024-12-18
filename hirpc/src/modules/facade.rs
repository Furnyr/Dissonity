/*

facade.rs exposes
- state code
- utils (official functionality)
- build variables
- safe interaction with static mut(s)

*/

use core::str;
use tokio::sync::futures::Notified;
use wasm_bindgen::prelude::*;
use tokio::sync::Notify;
use web_sys::js_sys::{Reflect, Function};
use web_sys::Window;

use crate::enums::*;
use crate::structs::ClosureWrapper;

static mut UTILS: Option<JsValue> = None;
static mut BUILD_VARIABLES: Option<JsValue> = None;

// public.rs
static mut LOCKED: bool = false;                        // Controls hash generation
static mut HASHES: Option<Vec<&[u8]>> = None;           // Generated hashes

// interface.rs
static mut APP_HASH: Option<&[u8]> = None;              // Hash stored inside the game build
static mut APP_SENDER: Option<Function> = None;        // Function used to send data to the game build
static mut APP_SENDER_NOTIFY: Option<Notify> = None;   // Notifies when the app sender id set
static mut APP_LISTENERS: Option<Vec<Function>> = None; // Functions used to receive data from the game build

// rpc.rs
static mut READY: bool = false;
static mut READY_NOTIFY: Option<Notify> = None;         // Notifies when the initial connection is established
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
static mut STATE_CODE: Option<StateCode> = Some(StateCode::Loading);    // Represents the state of the connection
static mut RPC_CLOSURES: Option<Vec<ClosureWrapper>> = None;            // Closures added to the window event listeners
static mut RPC_LISTENERS: Option<Vec<Function>> = None;                 // Functions used to receive data from the RPC

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

/// Access `STATE_CODE`.
#[wasm_bindgen(js_name=getStateCode)]
pub fn state_code() -> StateCode {
    unsafe {
        if let Some(ref state) = STATE_CODE {
            *state
        }
        
        else {
            panic!();
        }
    }
}

/// Modify `STATE_CODE`.
pub fn update_state(state: StateCode) {
    unsafe {
        STATE_CODE = Some(state);
    }
}

// PUBLIC - Expose APIs securely
pub mod public {
    use super::*;

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

    /// Set `LOCKED` to false.
    pub fn unlock_module() {
        unsafe {
            LOCKED = false;
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
}

// INTERFACE - Interact with the game build securely
pub mod interface {
    use super::*;

    /// Access `APP_SENDER`.
    pub fn app_sender() -> Option<&'static Function> {
        unsafe {
            if let Some(ref closure_ref) = APP_SENDER {
                Some(closure_ref)
            }
    
            else {
                None
            }
        }
    }
    
    /// Update `APP_SENDER` and notify waiters.
    /// 
    /// This can happen multiple times per runtime, but there can only be one app sender at the same time.
    /// 
    /// Use `free_app_sender` to remove the current app sender (and hash).
    pub fn dispatch_app_sender(closure: Function) -> bool {
        unsafe {
            if APP_SENDER.is_none() {
                APP_SENDER = Some(closure);
    
                if let Some(ref notify_ref) = APP_SENDER_NOTIFY {
                    notify_ref.notify_waiters();
                }
    
                true
            }
    
            else {
                false
            }
        }
    }
    
    /// Get `APP_SENDER_NOTIFY.notified()`. The notification will be sent once the app sender is available.
    pub fn app_sender_notified() -> Notified<'static> {
        unsafe {
            if let Some(ref notify_ref) = APP_SENDER_NOTIFY {
                notify_ref.notified()
            }
    
            else {
                // Initialize notify
                APP_SENDER_NOTIFY = Some(Notify::new());
    
                // Return ref
                if let Some(ref notify_ref) = APP_SENDER_NOTIFY {
                    notify_ref.notified()
                }
    
                else {
                    panic!();
                }
            }
        }
    }
    
    /// Access `APP_LISTENERS`.
    pub fn app_listeners() -> &'static Vec<Function> {
        unsafe {
            if let Some(ref listeners_ref) = APP_LISTENERS {
                listeners_ref
            }
    
            else {
                // Initialize listeners
                let vec: Vec<Function> = Vec::new();
                APP_LISTENERS = Some(vec);
    
                // Return ref
                if let Some(ref listeners_ref) = APP_LISTENERS {
                    listeners_ref
                }
    
                else {
                    panic!();
                }
            }
        }
    }
    
    /// Add a new listener to `APP_LISTENERS`.
    pub fn add_app_listener(listener: &Function) {
        let owned_listener = listener.to_owned();
        
        unsafe {
            if let Some(ref mut listeners_ref) = APP_LISTENERS {
                listeners_ref.push(owned_listener);
            }
            else {
                // Initialize listeners
                let vec: Vec<Function> = vec![owned_listener];
    
                APP_LISTENERS = Some(vec);
            }
        }
    }
    
    /// Clear all listeners from `APP_LISTENERS`.
    pub fn clear_app_listeners() {
        unsafe {
            if let Some(ref mut listeners_ref) = APP_LISTENERS {
                listeners_ref.clear();
            }
        }
    }

    /// Access `APP_HASH`
    pub fn app_hash() -> Option<&'static [u8]> {
        unsafe {
            if let Some(closure_ref) = APP_HASH {
                Some(closure_ref)
            }
    
            else {
                None
            }
        }
    }
    
    /// Set the `APP_HASH`.
    /// 
    /// This can happen multiple times per runtime, but there can only be one app hash at the same time.
    /// 
    /// Use `free_app_sender` to remove the current app sender (and hash).
    pub fn update_app_hash(hash: &str) -> bool {
    
        let boxed_hash = Box::leak(Box::new(hash.to_string()));
    
        unsafe {
            if APP_HASH.is_none() {
                APP_HASH = Some(boxed_hash.as_bytes());
    
                true
            }
    
            else {
                false
            }
        }
    }

    /// Free the app sender to pass a new one.
    pub fn free_app_sender() {
        unsafe {
            APP_SENDER = None;
            APP_HASH = None;
        }
    }
}

// RPC - Communicate with Discord
pub mod rpc {
    use crate::modules::rpc::wrap_remove_closure;

    use super::*;

    /// Access `READY`.
    pub fn ready() -> bool {
        unsafe {
            READY
        }
    }

    /// Set `READY` to true.
    /// 
    /// This means that an initial connection was established, not that the bridge is necessarily stable.
    /// 
    /// The `STATE_CODE` is what determines the state of the connection.
    pub fn set_ready() {
        unsafe {
            READY = true;
        }
    }

    /// Access `ALLOWED_ORIGINS`.
    pub fn allowed_origins() -> &'static [&'static str; 11] {
        unsafe {
            if let Some(ref origins_ref) = ALLOWED_ORIGINS {
                origins_ref
            }
    
            else {
                panic!();
            }
        }
    }
    
    /// Access `RPC_CLOSURES`.
    pub fn rpc_closures() -> &'static Vec<ClosureWrapper> {
        unsafe {
            if let Some(ref closures_ref) = RPC_CLOSURES {
                closures_ref
            }
    
            else {
                // Initialize closures
                let vec: Vec<ClosureWrapper> = Vec::new();
                RPC_CLOSURES = Some(vec);
    
                // Return ref
                if let Some(ref closures_ref) = RPC_CLOSURES {
                    closures_ref
                }
    
                else {
                    panic!();
                }
            }
        }
    }
    
    /// Add a new closure to `RPC_CLOSURES`.
    pub fn add_rpc_closure(cb: ClosureWrapper) {
        unsafe {
            if let Some(ref mut closures_ref) = RPC_CLOSURES {
                closures_ref.push(cb);
            }
            else {
                // Initialize closures
                let vec: Vec<ClosureWrapper> = vec![cb];
    
                RPC_CLOSURES = Some(vec);
            }
        }
    }
    
    /// Remove a closure from `RPC_CLOSURES` given its id.
    pub fn remove_rpc_closure(id: &str) {
        unsafe {
            if let Some(ref mut closures_ref) = RPC_CLOSURES {
                closures_ref.retain(|l| l.id != id);
            }
        }
    }
    
    /// Clear all closures from `RPC_CLOSURES`.
    pub fn clear_rpc_closures() {
        unsafe {
            if let Some(ref mut closures_ref) = RPC_CLOSURES {
                for listener in closures_ref.iter() {
                    wrap_remove_closure(listener.id);
                }
            }
        }
    }

    /// Access `RPC_LISTENERS`.
    pub fn rpc_listeners() -> &'static Vec<Function> {
        unsafe {
            if let Some(ref listeners_ref) = RPC_LISTENERS {
                listeners_ref
            }
    
            else {
                // Initialize listeners
                let vec: Vec<Function> = Vec::new();
                RPC_LISTENERS = Some(vec);
    
                // Return ref
                if let Some(ref listeners_ref) = RPC_LISTENERS {
                    listeners_ref
                }
    
                else {
                    panic!();
                }
            }
        }
    }
    
    /// Add a new listener to `RPC_LISTENERS`.
    pub fn add_rpc_listener(listener: &Function) {
        let owned_listener = listener.to_owned();
        
        unsafe {
            if let Some(ref mut listeners_ref) = RPC_LISTENERS {
                listeners_ref.push(owned_listener);
            }
            else {
                // Initialize listeners
                let vec: Vec<Function> = vec![owned_listener];
    
                RPC_LISTENERS = Some(vec);
            }
        }
    }
    
    /// Clear all listeners from `RPC_LISTENERS`.
    pub fn clear_rpc_listeners() {
        unsafe {
            if let Some(ref mut listeners_ref) = RPC_LISTENERS {
                listeners_ref.clear();
            }
        }
    }

    /// Notify waiters for `READY_NOTIFY`. This should only happen once per runtime.
    pub fn dispatch_ready() {
        unsafe {
            if let Some(ref notify_ref) = READY_NOTIFY {
                notify_ref.notify_waiters();

                READY_NOTIFY = None;
            }
        }
    }

    /// Get `READY_NOTIFY.notified()`. The notification will be sent once the first connection is established.
    pub fn ready_notified() -> Notified<'static> {
        unsafe {
            if let Some(ref notify_ref) = READY_NOTIFY {
                notify_ref.notified()
            }

            else {
                // Initialize notify
                READY_NOTIFY = Some(Notify::new());

                // Return ref
                if let Some(ref notify_ref) = READY_NOTIFY {
                    notify_ref.notified()
                }

                else {
                    panic!();
                }
            }
        }
    }

}

// UTILS - Official functionality
pub mod utils {
    use super::*;
    use crate::modules::public::{hirpc_log_error, validate_hash};

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

    /// Util that wraps around the official function.
    #[wasm_bindgen(js_name=patchUrlMappings)]
    pub fn expose_patch_url_mappings(hash_bytes: &[u8], mappings: &JsValue, config: &JsValue) {

        if !validate_hash(hash_bytes) {
            return;
        }

        let prop_name = "patchUrlMappings";

        let patch_function = get_imported_function(prop_name);

        if let Some(function) = patch_function {
            _ = function.call2(&JsValue::NULL, mappings, config);
        }

        else {
            hirpc_log_error("Tried to access utils without attempting to connect");
        }
    }

    /// Util that wraps around the official function.
    #[wasm_bindgen(js_name=formatPrice)]
    pub fn expose_format_price(hash_bytes: &[u8], price: &JsValue, locale: &JsValue) -> Result<JsValue, JsValue> {

        if !validate_hash(hash_bytes) {
            return Err("Invalid hash".into());
        }

        let prop_name = "formatPrice";

        let format_function = get_imported_function(prop_name);

        if let Some(function) = format_function {
            function.call2(&JsValue::NULL, price, locale)
        }

        else {
            hirpc_log_error("Tried to access utils without attempting to connect");

            Err("No function found".into())
        }
    }
}

// BUILD VARIABLES - Information defined in the game engine
pub mod build {
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

    /// Access the `CLIENT_ID` build variable.
    #[wasm_bindgen(js_name=getClientId)]
    pub fn client_id() -> Option<String> {
        let prop_name = "CLIENT_ID";

        get_build_variable::<String>(prop_name)
    }

    /// Access the `DISABLE_INFO_LOGS` build variable.
    #[wasm_bindgen(js_name=getDisableInfoLogs)]
    pub fn disable_info_logs() -> Option<bool> {
        let prop_name = "DISABLE_INFO_LOGS";

        get_build_variable::<bool>(prop_name)
    }

    /// Access the `DISABLE_CONSOLE_LOG_OVERRIDE` build variable.
    #[wasm_bindgen(js_name=getDisableConsoleLogOverride)]
    pub fn disable_console_log_override() -> Option<bool> {
        let prop_name = "DISABLE_CONSOLE_LOG_OVERRIDE";

        get_build_variable::<bool>(prop_name)
    }

    /// Access the `MAPPINGS` build variable.
    #[wasm_bindgen(js_name=getMappings)]
    pub fn expose_mappings() -> Option<String> {
        let mapping_data = mappings();

        match mapping_data {
            Some(data) => {
                match serde_json::to_string(&data) {
                    Ok(string) => Some(string),
                    Err(_) => None
                }
            },
            None => None
        }
    }

    pub fn mappings() -> Option<Vec<Mapping>> {
        let prop_name = "MAPPINGS";

        get_build_variable::<Vec<Mapping>>(prop_name)
    }

    /// Access the `PATCH_URL_MAPPINGS_CONFIG` build variable.
    #[wasm_bindgen(js_name=getPatchUrlMappingsConfig)]
    pub fn expose_patch_url_mappings_config() -> Option<String> {
        let config_data = patch_url_mappings_config();

        match config_data {
            Some(data) => {
                match serde_json::to_string(&data) {
                    Ok(string) => Some(string),
                    Err(_) => None
                }
            },
            None => None
        }
    }

    pub fn patch_url_mappings_config() -> Option<PatchUrlMappingsConfig> {
        let prop_name = "PATCH_URL_MAPPINGS_CONFIG";

        get_build_variable::<PatchUrlMappingsConfig>(prop_name)
    }

    /// Access the `OAUTH_SCOPES` build variable.
    #[wasm_bindgen(js_name=getOauthScopes)]
    pub fn oauth_scopes() -> Option<Vec<String>> {
        let prop_name = "OAUTH_SCOPES";

        get_build_variable::<Vec<String>>(prop_name)
    }

    /// Access the `TOKEN_REQUEST_PATH` build variable.
    #[wasm_bindgen(js_name=getTokenRequestPath)]
    pub fn token_request_path() -> Option<String> {
        let prop_name = "TOKEN_REQUEST_PATH";

        get_build_variable::<String>(prop_name)
    }

    /// Access the `SERVER_REQUEST` build variable.
    #[wasm_bindgen(js_name=getServerRequest)]
    pub fn server_request() -> Option<String> {
        let prop_name = "SERVER_REQUEST";

        get_build_variable::<String>(prop_name)
    }
}