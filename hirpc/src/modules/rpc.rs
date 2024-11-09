
//todo testing function (callback, wrap_query)
//todo clean up and handle errors properly (internal_send, connect, )

use core::str;
use wasm_bindgen::prelude::*;
use web_sys::{js_sys, MessageEvent};
use js_sys::Reflect;

use crate::structs::*;
use crate::enums::*;
use crate::constants::*;
use crate::modules::public::*;

// Module data
static mut ALLOWED_ORIGINS: [&str; 11] = [
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
];
static mut LISTENERS: Vec<Closure<dyn FnMut(MessageEvent)>> = Vec::new();
static mut STATE_CODE: StateCode = StateCode::Loading;
static mut UTILS: Option<JsValue> = None;
static mut BUILD_VARIABLES: Option<JsValue> = None;
static mut MOBILE_APP_VERSION: Option<&str> = None;

#[wasm_bindgen]
extern "C" {
    #[wasm_bindgen(js_namespace = console)]
    fn log(s: &str);
}

// JavaScript utils
#[wasm_bindgen(module = "/pkg/utils.js")]
extern "C" {
    #[wasm_bindgen(js_name="setKey")]
    fn utils_set_key(key: &str);

    #[wasm_bindgen(js_name="importModule")]
    fn utils_import(key: &str) -> JsValue;

    #[wasm_bindgen(js_name="logObject")]
    fn utils_log_object(data: &JsValue);
}

/// Attempts to establish a connection with the Discord RPC protocol.
/// 
/// **Returns**: A boolean indicating success.
/// 
/// **Tip**: Check if there's already a established connection through `globalThis.dso_hirpc`
#[wasm_bindgen]
pub fn connect() -> bool {

    //todo not connect if there's already a connection established

    //? Not browser
    let window = match web_sys::window() {
        Some(window) => window,
        None => {

            unsafe {
                STATE_CODE = StateCode::Unfunctional
            }

            return false;
        }
    };

    log("inside a browser!");

    //\ Update allowed origins
    unsafe {

        let origin = match window.location().origin() {
            Ok(data) => data,
            Err(_) => {
    
                STATE_CODE = StateCode::Unfunctional;
    
                return false;
            }
        };

        let boxed = Box::leak(Box::new(origin));

        ALLOWED_ORIGINS[0] = boxed;

        log(&("updated allowed origins with ".to_owned() + boxed));
    }


    //\ Get global variable
    let is_web = match Reflect::get(&window, &JsValue::from_str("dso_outside_discord")) {
        Ok(data) => data,
        Err(_) => {

            unsafe {
                STATE_CODE = StateCode::Unfunctional
            }

            return false;
        }
    };

    log("outside discord:");
    log(&is_web.is_truthy().to_string());

    //? Outside Discord
    if is_web.is_truthy() {

        unsafe {
            STATE_CODE = StateCode::OutsideDiscord;
        }

        return false;
    }

    let query = match query() {
        Ok(data) => data,
        Err(_) => {

            unsafe {
                STATE_CODE = StateCode::Unfunctional;
            }

            return false;
        }
    };

    let frame_id: &str;
    let platform: &str;

    //? No query params
    if !query.has("frame_id") || !query.has("instance_id") || !query.has("platform") {

        log("no query");

        unsafe {
            STATE_CODE = StateCode::OutsideDiscord;
        }

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
        unsafe {
            let boxed = Box::leak(Box::new(v.to_owned()));
            MOBILE_APP_VERSION = Some(boxed);
        }
    }

    log("importing utils");

    //\ Import utils
    let hash = match generate_hash() {
        Some(data) => data,
        None => return false
    };

    let hash_string = match str::from_utf8(&hash) {
        Ok(data) => data,
        Err(_) => return false
    };

    utils_set_key(hash_string);

    let disable_info_logs: bool;
    let client_id: String;
    let handshake_sdk_version: String;

    unsafe {

        UTILS = Some(utils_import(hash_string));

        if let Some(ref utils) = UTILS {
            BUILD_VARIABLES = match Reflect::get(utils, &"buildVariables".into()) {
                Ok(data) => Some(data),
                Err(_) => return false
            }
        }

        if let Some(ref vars) = BUILD_VARIABLES {
            disable_info_logs = match Reflect::get(vars, &"DISABLE_INFO_LOGS".into()) {
                Ok(data) => match data.as_bool() {
                    Some(b) => b,
                    None => return false
                },
                Err(_) => return false
            };

            client_id = match Reflect::get(vars, &"CLIENT_ID".into()) {
                Ok(data) => match data.as_string() {
                    Some(s) => s,
                    None => return false
                },
                Err(_) => return false
            };

            handshake_sdk_version = match Reflect::get(vars, &"HANDSHAKE_SDK_VERSION".into()) {
                Ok(data) => match data.as_string() {
                    Some(s) => s,
                    None => return false
                },
                Err(_) => return false
            };
        }

        else { return false; }

    }

    log("utils imported");

    log("logging data:");
    log(&(disable_info_logs.to_string()));
    log(&(client_id.to_string()));
    log(&(handshake_sdk_version.to_string()));

    let major_mobile_version = parse_major_mobile_version();

    let mut payload = HandshakePayload {
        v: HANDSHAKE_VERSION,
        encoding: HANDSHAKE_ENCODING,
        client_id: &client_id,
        frame_id,
        sdk_version: None
    };

    if platform == PLATFORM_DESKTOP || major_mobile_version >= HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION {
        payload.sdk_version = Some(&handshake_sdk_version);
    }

    let cb = Closure::wrap(Box::new(callback) as Box<dyn FnMut(_)>);
    
    _ = window.add_event_listener_with_callback("message", cb.as_ref().unchecked_ref());

    unsafe {
        LISTENERS.push(cb);
    }

    log("listener added");

    let arr = js_sys::Array::new();
    arr.push(&0.into());
    arr.push(&payload.to_js());

    log("sending first payload");

    internal_send(arr.into());

    true
}

fn internal_send(message: JsValue) {

    let window = match web_sys::window() {
        Some(window) => window,
        None => {

            unsafe {
                STATE_CODE = StateCode::Unfunctional
            }

            return;
        }
    };

    let source = window.parent().unwrap().unwrap();

    let doc = window.document().unwrap();

    let source_ori: String = if doc.referrer().is_empty() {
        "*".to_owned()
    }

    else {
        doc.referrer()
    };

    _ = source.post_message(&message, &source_ori);
}

fn parse_major_mobile_version() -> f64 {

    unsafe {

        if let Some(version) = MOBILE_APP_VERSION {

            if version.contains('.') {

                let major_version = match version.split('.').next(){
                    Some(data) => data,
                    None => return HANDSHAKE_UNKNOWN_VERSION_NUMBER
                };

                let num: f64 = match major_version.parse() {
                    Ok(data) => data,
                    Err(_) => HANDSHAKE_UNKNOWN_VERSION_NUMBER
                };

                return num;
            }
        }

        HANDSHAKE_UNKNOWN_VERSION_NUMBER
    }
}

fn callback(e: MessageEvent) {

    log("received message from the RPC!");

    utils_log_object(&e.data());

    //let data: JsValue = js_sys::Reflect::get(&e.data(), &JsValue::from_str("field")).unwrap();
}

#[wasm_bindgen(js_name = getQuery)]
pub fn wrap_query() -> Option<String> {

    let mut str: String = String::new();

    let query = match query() {
        Ok(data) => data,
        Err(_) => return None
    };

    for i in 0..query.keys.len() {
        let element = (query.keys.get(i).expect("no key"), query.values.get(i).expect("no value"));
        str.push_str(&("[ ".to_string() + element.0 + " ]: " + element.1 + "\n"));
    }

    Some(str)
}

fn query() -> Result<QueryData, &'static str> {

    let window = match web_sys::window() {
        Some(window) => window,
        None => return Err("no global window exists")
    };

    let search: String = window.location().search().unwrap_or_default();

    let is_parent = search.contains('?');

    let search =
        if is_parent { search.replace('?', "") }
        else {

            let parent_window = match window.parent() {
                Ok(window) => match window {
                    Some(window) => window,
                    None => return Err("no parent window exists")
                },
                Err(_) => return Err("can't get parent window")
            };

            let parent_search = match parent_window.location().search() {
                Ok(search) => search,
                Err(_) => return Err("can't get parent search")
            };

            parent_search.replace('?', "")
        };

    let query_key_value = search.split('&');

    let mut map = QueryData {
        keys: Vec::new(),
        values: Vec::new()
    };

    for pair in query_key_value {

        let split: Vec<&str> = pair.split('=').collect();
        if split.len() == 2 {
            map.insert(split[0].to_string(), split[1].to_string());
        }
    }

    Ok(map)
}