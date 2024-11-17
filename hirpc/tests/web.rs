//! Test suite for the Web and headless browsers.

#![cfg(target_arch = "wasm32")]

extern crate wasm_bindgen_test;
use wasm_bindgen_test::*;
use dissonity_hirpc::modules::*;
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

    assert_eq!(facade::build::mappings().unwrap().len(), 0);
    
    assert_eq!(facade::build::patch_url_mappings_config().unwrap().patch_xhr, false);

    assert_eq!(facade::build::oauth_scopes().unwrap()[0], String::from("identify"));

    assert_eq!(facade::build::token_request_path(), Some(String::from("/api/token")));

    assert_eq!(facade::build::server_request(), Some(String::from("")));
}