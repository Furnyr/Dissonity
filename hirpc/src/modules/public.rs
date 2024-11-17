
//todo hash checking isn't properly implemented yet to public functions
//todo exposed for testing (greet, check, validate_hash)
//todo comms with game build will use a special hash

use core::str;
use wasm_bindgen::prelude::*;
use web_sys::js_sys;
use sha256::digest;

use crate::constants::*;
use super::facade::*;

#[wasm_bindgen]
extern "C" {
    #[wasm_bindgen(js_namespace = console)]
    fn log(s: &str);
}

#[wasm_bindgen]
pub fn validate_hash(bytes: &[u8]) -> bool {

    let mut found = false;

    for hash in hashes().iter() {
        if *hash == bytes {
            found = true;
        }
    }

    found
}

#[wasm_bindgen]
pub fn generate_hash() -> Option<Vec<u8>> {

    if locked() {
        return None;
    }

    let window = match web_sys::window() {
        Some(window) => window,
        None => return None
    };

    let crypto = match window.crypto() {
        Ok(crypto) => crypto,
        Err(_) => return None
    };

    let mut random_array = [0u8];

    match crypto.get_random_values_with_u8_array(&mut random_array) {
        Ok(obj) => obj,
        Err(_) => return None
    };

    let now = js_sys::Date::now();

    // [timestamp] [pseudo-random byte]
    let key = ((now as i64) << (HASH_RANDOM_BYTES * 8)) | random_array[0] as i64;
    let hash = digest(key.to_string());

    add_hash(&hash);

    Some(hash.as_bytes().to_vec())
}

#[wasm_bindgen]
pub fn check() -> Result<(), JsValue> {

    // Use `web_sys`'s global `window` function to get a handle on the global
    // window object.
    let window = web_sys::window().expect("no global `window` exists");
    let document = window.document().expect("should have a document on window");
    let _body = document.body().expect("document should have a body");

    let a = js_sys::Reflect::get(&window, &JsValue::from_str("outsideDiscord"))?;

    let a = a.is_truthy();


    if a {
        log("outside discord!");
    } else {
        log("not outside discord!");
    }

    Ok(())
}

#[wasm_bindgen]
pub fn greet() {
    log("Hello, hirpc!");
}