
//todo testing function (callback, wrap_query)
//todo clean up and handle errors properly (internal_send, connect, )

use core::str;
use wasm_bindgen::prelude::*;
use web_sys::js_sys;
use web_sys::{MessageEvent, Window};
use serde::Serialize;

use crate::constants::*;
use crate::structs::*;
use crate::enums::*;
use super::public::*;
use super::facade::*;


#[wasm_bindgen]
extern "C" {
    #[wasm_bindgen(js_namespace = console)]
    fn log(s: &str);

    #[wasm_bindgen(js_namespace = console, js_name = error)]
    fn log_error(s: &str);
}

/// Attempts to establish a connection with the Discord RPC protocol.
/// 
/// **Returns**: A boolean indicating success.
/// 
/// **Tip**: Check if there's already a established connection through `globalThis.dso_connected`
#[wasm_bindgen]
pub fn connect() -> bool {

    //? Ready
    if ready() {
        return false;
    }

    //? Not browser
    let window = match web_sys::window() {
        Some(window) => window,
        None => {

            update_state(StateCode::Unfunctional);

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

    //\ Get global variables
    let is_web = match js_sys::Reflect::get(&window, &JsValue::from_str("dso_outside_discord")) {
        Ok(data) => data,
        Err(_) => {

            update_state(StateCode::Errored);

            return false;
        }
    };

    let is_connected = match js_sys::Reflect::get(&window, &JsValue::from_str("dso_connected")) {
        Ok(data) => data,
        Err(_) => {

            update_state(StateCode::Errored);

            return false;
        }
    };

    //? Outside Discord
    if is_web.is_truthy() {

        update_state(StateCode::OutsideDiscord);

        return false;
    }    

    // Query parameters needed for handshake
    let query = match query(&window) {
        Ok(data) => data,
        Err(err) => {

            update_state(StateCode::Errored);

            log_error(err);

            return false;
        }
    };

    let frame_id: &str;
    let platform: &str;

    //? No query params
    if !query.has("frame_id") || !query.has("instance_id") || !query.has("platform") {

        update_state(StateCode::OutsideDiscord);

        return false;
    }

    else {

        frame_id = match query.find("frame_id") {
            Some(data) => data,
            None => return false
        };

        platform = match query.find("platform") {
            Some(data) => data,
            None => return false
        };
    }

    //\ Update mobile app version
    if let Some(v) = query.find("mobile_app_version") {
        let boxed = Box::leak(Box::new(v.to_owned()));
        update_mobile_app_version(boxed);
    }

    //\ Setup listener
    //todo change listener if ready
    let cb = Closure::wrap(Box::new(callback) as Box<dyn FnMut(_)>);

    match window.add_event_listener_with_callback("message", cb.as_ref().unchecked_ref()) {
        Ok(data) => data,
        Err(_) => {
            log("[hirpc] Something went wrong in an event listener");
        }
    };

    let wrapper = ListenerWrapper {
        closure: cb,
        id: TEST_LISTENER_ID
    };

    add_listener(wrapper);

    //? No connection
    if is_connected.is_falsy() {

        let client_id = build::client_id().unwrap();
        let handshake_sdk_version = SDK_VERSION;
    
        log(&(client_id.to_string()));
        log(handshake_sdk_version);
    
        let major_mobile_version = parse_major_mobile_version();
    
        let mut payload = HandshakePayload {
            v: HANDSHAKE_VERSION,
            encoding: HANDSHAKE_ENCODING,
            client_id: &client_id,
            frame_id,
            sdk_version: None
        };
    
        if platform == PLATFORM_DESKTOP || major_mobile_version >= HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION {
            payload.sdk_version = Some(handshake_sdk_version);
        }

        rpc_send(Opcode::Handshake, payload);
    }

    set_ready();

    true
}

fn rpc_send<T: Serialize>(opcode: Opcode, payload: T) {

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
                None => return
            }
        }
        Err(_) => return
    };

    let doc = match window.document() {
        Some(data) => data,
        None => return
    };

    let source_origin: String = if doc.referrer().is_empty() {
        String::from("*")
    }

    else {
        doc.referrer()
    };

    let serialized_payload = match serde_wasm_bindgen::to_value(&payload) {
        Ok(data) => data,
        Err(_) => return
    };

    // Format message for RPC:
    // [Opcode, Payload]
    let array = js_sys::Array::new();
    array.push(&(opcode as i32).into());
    array.push(&serialized_payload);

    match source.post_message(&array, &source_origin) {
        Ok(data) => data,
        Err(_) => {
            log_error("[hirpc] Something went wrong sending a message to RPC");
        }
    };
}

fn callback(e: MessageEvent) {

    log("received message from the RPC!");

    log_object(&e.data());

    //let data: JsValue = js_sys::Reflect::get(&e.data(), &JsValue::from_str("field")).unwrap();
}

fn query(window: &Window) -> Result<QueryData, &'static str> {
    let search: String = window.location().search().unwrap_or_default();

    let is_parent = search.contains('?');

    let search =
        if is_parent { search.replace('?', "") }
        else {

            let parent_window = match window.parent() {
                Ok(window) => match window {
                    Some(window) => window,
                    None => return Err("[hirpc] No parent window exists")
                },
                Err(_) => return Err("[hirpc] Can't get parent window")
            };

            let parent_search = match parent_window.location().search() {
                Ok(search) => search,
                Err(_) => return Err("[hirpc] Can't get parent search")
            };

            parent_search.replace('?', "")
        };

    let query_key_value = search.split('&');

    let mut query_data = QueryData {
        keys: Vec::new(),
        values: Vec::new()
    };

    for pair in query_key_value {

        let split: Vec<&str> = pair.split('=').collect();
        if split.len() == 2 {
            query_data.insert(split[0].to_string(), split[1].to_string());
        }
    }

    Ok(query_data)
}