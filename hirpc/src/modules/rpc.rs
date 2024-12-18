/*

rpc.rs exposes functionality to send data to RPC using a hash.

The only way to receive data from RPC is using an app sender.

*/

use core::str;
use serde_json::to_string;
use wasm_bindgen::prelude::*;
use web_sys::js_sys::{self, Function};
use web_sys::{MessageEvent, Window};

use crate::constants::*;
use crate::structs::*;
use crate::enums::*;
use super::public::*;
use super::facade::{initialize, log_object, state_code, update_state};
use super::facade::rpc::*;
use super::facade::build::*;
use super::facade::interface::*;


#[wasm_bindgen]
extern "C" {

    #[wasm_bindgen(js_namespace = console, js_name = error)]
    fn log_error(s: &str);
}

/// Attempts to establish a connection with the Discord RPC protocol.
/// 
/// **Returns**: A boolean indicating success.
/// 
/// **Note**: Check if there's already a established connection through `globalThis.dso_connected`
#[wasm_bindgen]
pub async fn connect(hash_bytes: &[u8]) -> bool {

    if !validate_hash(hash_bytes) {
        return false;
    }

    //? Ready
    if ready() {
        return false;
    }

    //? Not browser
    let window = match web_sys::window() {
        Some(window) => window,
        None => {
            update_state(StateCode::Unfunctional);

            hirpc_log_error("Not in a web environment");

            return false;
        }
    };

    //\ Import utils
    let hash = match generate_hash() {
        Some(data) => data,
        None => return false
    };

    let hash_string = match str::from_utf8(&hash) {
        Ok(data) => data,
        Err(_) => return false
    };

    initialize(&window, hash_string);

    if let Some(false) = disable_info_logs() {
        greet();
    }

    //\ Get global variables
    let is_web = match js_sys::Reflect::get(&window, &JsValue::from_str("dso_outside_discord")) {
        Ok(data) => data,
        Err(_) => {
            update_state(StateCode::Errored);

            hirpc_log_error("Can't access global variables");

            return false;
        }
    };

    let is_connected = match js_sys::Reflect::get(&window, &JsValue::from_str("dso_connected")) {
        Ok(data) => data,
        Err(_) => {
            update_state(StateCode::Errored);

            hirpc_log_error("Can't access global variables");

            return false;
        }
    };

    //? Outside Discord
    if is_web.is_truthy() {

        update_state(StateCode::OutsideDiscord);

        return false;
    }

    // Query parameters needed for handshake
    let query = match query_object(&window) {
        Some(data) => data,
        None => {

            update_state(StateCode::Errored);

            return false;
        }
    };

    let frame_id: String;
    let platform: String;

    //? No query params
    if query.frame_id.is_none() || query.instance_id.is_none() || query.platform.is_none() {

        update_state(StateCode::OutsideDiscord);

        return false;
    }

    //\ Capture query parameters
    else {

        frame_id = match query.frame_id {
            Some(data) => data,
            None => return false
        };

        platform = match query.platform {
            Some(data) => data,
            None => return false
        };
    }

    // Create main closure
    let main_closure = Closure::wrap(Box::new(move |event: MessageEvent| {
        wasm_bindgen_futures::spawn_local(async move {
            rpc_receive(event).await;
        });
    }) as Box<dyn FnMut(_)>);

    //\ Add main closure as listener
    let main_wrapper = ClosureWrapper {
        closure: main_closure,
        id: MAIN_CLOSURE_ID
    };

    wrap_add_closure(&window, main_wrapper, "Something went wrong adding the main closure");

    //? No connection
    if is_connected.is_falsy() {

        // Create connection closure
        let connection_closure = Closure::wrap(Box::new(|_: MessageEvent| {

            let window = match web_sys::window() {
                Some(window) => window,
                None => {
                    update_state(StateCode::Unfunctional);

                    return;
                }
            };

            //\ Remove connection closure
            wrap_remove_closure(CONNECTION_CLOSURE_ID);
            
            //\ Set global connected variable to true
            match js_sys::Reflect::set(&window, &"dso_connected".into(), &true.into()) {
                Ok(_) => (),
                Err(_) => {
                    update_state(StateCode::Unfunctional);
                }
            }

            update_state(StateCode::Stable);
            set_ready();
            dispatch_ready(); // Only dispatch in the first connection (here)
        }) as Box<dyn FnMut(_)>);

        //\ Add connection closure as listener
        let connection_wrapper = ClosureWrapper {
            closure: connection_closure,
            id: CONNECTION_CLOSURE_ID
        };

        wrap_add_closure(&window, connection_wrapper, "Something went wrong adding the connection closure");

        //\ Do handshake
        let client_id = client_id().unwrap();

        let handshake_sdk_version = SDK_VERSION;
        let major_mobile_version = parse_major_mobile_version(query.mobile_app_version);
    
        let mut payload = HandshakePayload {
            v: HANDSHAKE_VERSION,
            encoding: HANDSHAKE_ENCODING,
            client_id: &client_id,
            frame_id: &frame_id,
            sdk_version: None
        };
    
        //? Send SDK version
        if platform == Platform::desktop() || major_mobile_version >= HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION {
            payload.sdk_version = Some(handshake_sdk_version);
        }

        // Serialize and send
        let serialized_payload = match serde_wasm_bindgen::to_value(&payload) {
            Ok(data) => data,
            Err(_) => return false
        };

        if let Some(false) = disable_info_logs() {
            hirpc_log("Connecting...");
        }

        rpc_send(&(Opcode::Handshake as i32).into(), &serialized_payload);
        ready_notified().await;
    }

    else {
        update_state(StateCode::Stable);
        set_ready();
    }

    true
}

/// Add a callback that will receive data sent from the Discord RPC.
#[wasm_bindgen(js_name=addRpcListener)]
pub fn expose_add_rpc_listener(hash_bytes: &[u8], listener: Function) {

    if !validate_hash(hash_bytes) {
        return
    }

    add_rpc_listener(&listener);
}

/// Clear the RPC event listeners. After this call, you will stop receiving data, but the RPC connection won't be closed.
/// 
/// **Returns**: A boolean indicating success.
/// 
/// **Note**: To close the connection with the RPC use `sendToRpc`
#[wasm_bindgen(js_name=clearRpcListeners)]
pub fn expose_clear_rpc_listeners(hash_bytes: &[u8]) -> bool {

    if !validate_hash(hash_bytes) {
        return false;
    }

    clear_rpc_listeners();

    true
}

/// Send data to the Discord RPC. If the connection isn't established, the message is left pending.
/// 
/// **Returns**: A boolean indicating success.
/// 
/// **Note**: To receive data from RPC, call `useAppSender` once in the runtime.
#[wasm_bindgen(js_name=sendToRpc)]
pub async fn expose_rpc_send(hash_bytes: &[u8], opcode: &JsValue, payload: &JsValue) -> bool {

    if !validate_hash(hash_bytes) {
        return false;
    }

    //? Not ready
    if !ready() {
        ready_notified().await;
    }

    rpc_send(opcode, payload);

    true
}

fn rpc_send(opcode: &JsValue, payload: &JsValue) {

    // Needs access to: window, window.parent (source), document and source origin

    let window = match web_sys::window() {
        Some(window) => window,
        None => {
            update_state(StateCode::Unfunctional);

            return;
        }
    };

    let source = match window.parent() {
        Ok(option) => {
            match option {
                Some(data) => data,
                None => {
                    hirpc_log_error("No parent window exists");

                    return;
                }
            }
        }
        Err(_) => return
    };

    let doc = match window.document() {
        Some(data) => data,
        None => {
            hirpc_log_error("Can't access document");

            return;
        }
    };

    let source_origin: String = if doc.referrer().is_empty() {
        String::from("*")
    }

    else {
        doc.referrer()
    };

    // Format message for RPC:
    // [Opcode, Payload]
    let array = js_sys::Array::new();
    array.push(opcode);
    array.push(payload);

    match source.post_message(&array, &source_origin) {
        Ok(data) => data,
        Err(_) => {
            hirpc_log_error("Something went wrong sending a message to RPC");
        }
    };
}

async fn rpc_receive(event: MessageEvent) {

    if !allowed_origins().iter().any(|&x| x == event.origin()) {
        return;
    }

    //todo remove once hiRPC is stable (maybe add a debug option?)
    log_object(&event.data());

    let data = event.data();

    for function in rpc_listeners().iter() {
        _ = function.call1(&JsValue::NULL, &data);
    }

    send_to_app(data).await;
}

async fn send_to_app(rpc_message: JsValue) {

    let closure = {
        if app_sender().is_none() {
            app_sender_notified().await;
        }

        app_sender().unwrap()
    };

    let stringified_payload = to_string(&AppPayload {
        hirpc_state: state_code() as i32,
        rpc_message: Some(rpc_message),
        hirpc_message: None
    }).unwrap();

    _ = closure.call1(&JsValue::NULL, &stringified_payload.into());
}

// CLOSURES

fn wrap_add_closure(window: &Window, wrapper: ClosureWrapper, error_message: &str) {

    //\ Add listener
    match window.add_event_listener_with_callback("message", wrapper.closure.as_ref().unchecked_ref()) {
        Ok(_) => (),
        Err(_) => {
            update_state(StateCode::Errored);

            hirpc_log_error(error_message);
            
            return;
        }
    };
    
    //\ Remove closure to facade
    add_rpc_closure(wrapper);
}

pub fn wrap_remove_closure(closure_id: &str) {
    // Window isn't passed because this function can be called much later in the process

    let window = match web_sys::window() {
        Some(window) => window,
        None => {
            update_state(StateCode::Unfunctional);
            
            return;
        }
    };

    let listener_wrapper = match rpc_closures().iter().find(|c| c.id == closure_id) {
        Some(data) => data,
        None => {
            return;
        }
    };

    //\ Remove listener
    match window.remove_event_listener_with_callback("message", listener_wrapper.closure.as_ref().unchecked_ref()) {
        Ok(_) => (),
        Err(_) => {
            update_state(StateCode::Errored);

            return;
        }
    };

    //\ Remove closure from facade
    remove_rpc_closure(closure_id);
}

// QUERY

#[wasm_bindgen(js_name=getStringifiedQuery)]
/// Get a stringified JSON object containing the query parameters.
pub fn expose_raw_query() -> Result<String, String> {

    let window = match web_sys::window() {
        Some(window) => window,
        None => {
            update_state(StateCode::Unfunctional);

            return Err("Not in a web environment".to_string());
        }
    };

    raw_query(&window)
}

pub fn raw_query(window: &Window) -> Result<String, String> {
    let search: String = window.location().search().unwrap_or_default();

    let is_parent = search.contains('?');

    let search =
        if is_parent { search.replace('?', "") }
        else {

            let parent_window = match window.parent() {
                Ok(window) => match window {
                    Some(window) => window,
                    None => return Err("No parent window exists".to_string())
                },
                Err(_) => return Err("Can't get parent window".to_string())
            };

            let parent_search = match parent_window.location().search() {
                Ok(search) => search,
                Err(_) => return Err("Can't get parent search".to_string())
            };

            parent_search.replace('?', "")
        };

    //\ Generate JSON string
    let query_key_value = search.split('&');

    let mut stringified_query_object = String::from("{\"_\":0");

    for pair in query_key_value {

        let split: Vec<&str> = pair.split('=').collect();
        if split.len() == 2 {
            stringified_query_object.push_str(",\"");
            stringified_query_object.push_str(split[0]);
            stringified_query_object.push_str("\":\"");
            stringified_query_object.push_str(split[1]);
            stringified_query_object.push('"');
        }
    }

    stringified_query_object.push('}');
    stringified_query_object = stringified_query_object.replace("\"_\":0,", "");

    Ok(stringified_query_object)
}

fn query_object(window: &Window) -> Option<QueryData> {

    match raw_query(window) {
        Ok(data) => {
            let query_data: QueryData = match serde_json::from_str(&data) {
                Ok(data) => data,
                Err(_) => {
                    hirpc_log_error("Something went wrong deserializing the query object");
                    return None;
                }
            };

            Some(query_data)
        },
        Err(err) => {
            log_error(&err);
            None
        }
    }
}

// OTHER

/// Get the major mobile version as an i32.
pub fn parse_major_mobile_version(mobile_app_version: Option<String>) -> i32 {

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