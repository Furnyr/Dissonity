//! Test suite for the Web and headless browsers.

#![cfg(target_arch = "wasm32")]

extern crate wasm_bindgen_test;
use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use wasm_bindgen::JsValue;
use wasm_bindgen_test::*;
use dissonity_hirpc::modules::*;
use dissonity_hirpc::enums::*;
use dissonity_hirpc::structs::*;
use web_sys::js_sys::Function;
use core::str;

wasm_bindgen_test_configure!(run_in_browser);

#[wasm_bindgen_test]
pub fn build_variables() {

    let window = match web_sys::window() {
        Some(window) => window,
        None => {
            panic!("Not in a web environment");
        }
    };

    //\ Generate hash
    let hash = match public::generate_hash() {
        Some(data) => data,
        None => panic!("Hash can't be generated")
    };

    let hash_string = match str::from_utf8(&hash) {
        Ok(data) => data,
        Err(_) => panic!("Can't convert hash to string")
    };

    assert_eq!(facade::initialize(&window, &hash_string), true);

    assert_eq!(facade::build::disable_info_logs(), Some(false));

    assert_eq!(facade::build::client_id(), Some(String::from("123456789987654321")));

    assert_eq!(facade::build::mappings().is_none(), true);
    
    assert_eq!(facade::build::patch_url_mappings_config().unwrap().patch_src_attributes, false);

    assert_eq!(facade::build::oauth_scopes().unwrap()[0], String::from("identify"));

    assert_eq!(facade::build::token_request_path(), Some(String::from("/api/token")));

    assert_eq!(facade::build::server_request(), Some(String::from("")));
}

#[wasm_bindgen_test]
pub fn lock_hashes() {

    //\ Generate hash
    let hash = match public::request_hash() {
        Some(data) => data,
        None => panic!("Hash can't be generated")
    };

    public::lock_hashes(&hash);

    match public::request_hash() {
        Some(_) => panic!("Hash shouldn't have been generated"),
        None => ()
    };
}

#[wasm_bindgen_test]
pub fn set_state() {

    facade::update_state(StateCode::Stable);
    assert_eq!(facade::state_code() as i32, StateCode::Stable as i32);

    facade::update_state(StateCode::Unfunctional);
    assert_eq!(facade::state_code() as i32, StateCode::Unfunctional as i32);
}

#[wasm_bindgen_test]
pub async fn app_communication() {

    //\ Generate hash
    let hash = match public::generate_hash() {
        Some(data) => data,
        None => panic!("Hash can't be generated")
    };

    let closure = Closure::wrap(Box::new(move |data: JsValue| {

        let parsed: serde_json::Value = serde_wasm_bindgen::from_value(data).unwrap();

        let stringified_data = match parsed.as_str() {
            Some(data) => data,
            None => panic!("Data sent to the app isn't a string")
        };

        let payload: AppPayload = match serde_json::from_str(stringified_data) {
            Ok(data) => data,
            Err(_) => panic!("Couldn't deserialize payload sent to the app")
        };

        let hirpc_message = match payload.hirpc_message {
            Some(data) => data,
            None => panic!("No hirpc_message property")
        };

        assert_eq!(hirpc_message.data, Some("data".into()));
    }) as Box<dyn FnMut(JsValue)>);

    let function = closure.as_ref().unchecked_ref::<Function>().clone();
    let clone = closure.as_ref().unchecked_ref::<Function>().clone();

    assert_eq!(
        interface::use_app_sender(function),
        true
    );

    assert_eq!(
        interface::use_app_sender(clone),
        false
    );

    assert_eq!(
        interface::send_to_app(&hash, &"data".into()).await,
        true
    );

    assert_eq!(
        interface::send_to_hirpc(&hash, &"data".into()),
        false
    );
}