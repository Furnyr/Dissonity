/*

interface.rs exposes functionality to:

- Receive RPC data from with an rpc listener
- Send data to the app through the app sender (hash required)
- Send data to the JS level from the app (app hash required)

*/

use core::str;
use serde_json::to_string;
use wasm_bindgen::prelude::*;
use web_sys::js_sys::Function;

use crate::enums::ActionCode;
use crate::structs::{AppPayload, HashPayload, HiRpcPayload};

use super::facade::public::unlock_module;
use super::facade::state_code;
use super::facade::interface::*;
use super::public::{generate_hash, validate_hash};


/// Set the callback that will receive data to send to the game build/app. A hash is not required since only the first app sender passed works.
/// 
/// **Returns**: A boolean indicating success.
/// 
/// **Note**: There can only be one app sender at the same time.
#[wasm_bindgen(js_name=useAppSender)]
pub fn use_app_sender(sender: Function) -> bool {

    dispatch_app_sender(sender)
}

/// Remove the current app sender to add a new one. The app hash will also be invalidated.
/// 
/// **Returns**: A boolean indicating success.
/// 
/// **Note**: Requires the app hash.
#[wasm_bindgen(js_name=freeAppSender)]
pub fn expose_free_app_sender(app_hash_bytes: &[u8]) -> bool {

    if !validate_app_hash(app_hash_bytes) {
        return false;
    }

    unlock_module();

    free_app_sender();

    true
}

/// Send the app hash to the app sender. If there's no app sender yet, the message is left pending.
/// 
/// **Returns**: A boolean indicating success.
#[wasm_bindgen(js_name=dispatchAppHash)]
pub async fn dispatch_app_hash() -> bool {

    if app_sender().is_none() {
        app_sender_notified().await;
    }

    let new_app_hash: Vec<u8>;
    let hash_string: &str;

    //? Generate first app hash
    if app_hash().is_none() {

        new_app_hash = match generate_hash() {
            Some(data) => data,
            None => return false
        };
    
        hash_string = match str::from_utf8(&new_app_hash) {
            Ok(data) => data,
            Err(_) => return false
        };
    
        update_app_hash(hash_string);
    }

    else {
        hash_string = match str::from_utf8(app_hash().unwrap()) {
            Ok(data) => data,
            Err(_) => return false
        };
    }

    // Hash payload
    let hash_payload = HashPayload {
        hash: hash_string.to_owned()
    };

    // JS hash payload
    let js_hash_payload = match serde_wasm_bindgen::to_value(&hash_payload) {
        Ok(data) => data,
        Err(_) => {
            return false;
        }
    };

    // hiRPC Payload
    let hirpc_payload = HiRpcPayload {
        action_code: ActionCode::Hash as i32,
        data: Some(js_hash_payload)
    };

    // App payload
    let payload = AppPayload {
        hirpc_state: state_code() as i32,
        rpc_message: None,
        hirpc_message: Some(hirpc_payload)
    };

    let serialized_payload = to_string(&payload).unwrap();

    //\ Send to the game build
    if let Some(closure) = app_sender() {
        _ = closure.call1(&JsValue::NULL, &serialized_payload.into());
    }

    true
}

/// Calling this function will dispatch data to all the app listeners. This is usually called from the game build.
/// 
/// **Returns**: A boolean indicating success.
#[wasm_bindgen(js_name=sendToHiRpc)]
pub fn send_to_hirpc(app_hash_bytes: &[u8], data: &JsValue) -> bool {

    if !validate_app_hash(app_hash_bytes) {
        return false;
    }

    for function in app_listeners().iter() {
        _ = function.call1(&JsValue::NULL, data);
    }

    true
}

/// Calling this function will dispatch data to the game build. If the app sender isn't available, the message is left pending.
/// 
/// **Returns**: A boolean indicating success
#[wasm_bindgen(js_name=sendToApp)]
pub async fn send_to_app(hash_bytes: &[u8], data: &JsValue) -> bool {

    if !validate_hash(hash_bytes) {
        return false;
    }

    let closure = {
        if app_sender().is_none() {
            app_sender_notified().await;
        }

        app_sender().unwrap()
    };

    let hirpc_payload = HiRpcPayload {
        action_code: ActionCode::Interop as i32,
        data: Some(data.clone())
    };

    let payload = AppPayload {
        hirpc_state: state_code() as i32,
        rpc_message: None,
        hirpc_message: Some(hirpc_payload)
    };

    let stringified_payload = to_string(&payload).unwrap();

    _ = closure.call1(&JsValue::NULL, &stringified_payload.into());

    true
}

/// Add a callback that will receive data sent from the game build.
#[wasm_bindgen(js_name=addAppListener)]
pub fn expose_add_app_listener(hash_bytes: &[u8], listener: Function) {

    if !validate_hash(hash_bytes) {
        return;
    }

    add_app_listener(&listener);
}

/// Clear the app listeners. After this call every listener subscribed to the app will stop receiving data.
/// 
/// **Returns**: A boolean indicating success.
#[wasm_bindgen(js_name=clearAppListeners)]
pub fn expose_clear_app_listeners(hash_bytes: &[u8]) -> bool {

    if !validate_hash(hash_bytes) {
        return false;
    }

    clear_app_listeners();

    true
}

pub fn validate_app_hash(app_hash_bytes: &[u8]) -> bool {
    if let Some(hash) = app_hash() {
        hash == app_hash_bytes
    }

    else {
        false
    }
}