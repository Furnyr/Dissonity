/*

public.rs exposes general hash functionality

*/

use core::str;
use wasm_bindgen::prelude::*;
use web_sys::js_sys;
use sha256::digest;

use crate::HIRPC_VERSION;
use crate::constants::*;
use super::facade::public::*;
use super::interface::validate_app_hash;


#[wasm_bindgen]
extern "C" {
    #[wasm_bindgen(js_namespace = console, js_name=log)]
    fn style_log(string: &str, style1: &str, style2: &str);

    #[wasm_bindgen(js_namespace = console, js_name=error)]
    fn style_error(string: &str, style1: &str, style2: &str);
}

// META

pub fn hirpc_log_error(string: &str) {
    style_error(&format!("%c[DissonityHiRpc]%c {}", string), "color:#8177f6;font-weight: bold;", "color:initial;");
}

pub fn hirpc_log(string: &str) {
    style_log(&format!("%c[DissonityHiRpc]%c {}", string), "color:#8177f6;font-weight: bold;", "color:initial;");
}

/// Log the hiRPC version.
#[wasm_bindgen]
pub fn greet() {
    hirpc_log(&format!("hiRPC! version {}", HIRPC_VERSION));
}

/// Get the hiRPC version.
#[wasm_bindgen]
pub fn version() -> String {
    HIRPC_VERSION.to_owned()
}

// HASHES

/// Generate a hash. The exposed function is `request_hash`.
pub fn generate_hash() -> Option<Vec<u8>> {
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

/// Request a hash to access restricted functionality.
#[wasm_bindgen(js_name=requestHash)]
pub fn request_hash() -> Option<Vec<u8>> {

    if locked() {
        return None;
    }

    generate_hash()
}

/// Lock hash generation.
#[wasm_bindgen(js_name=lockHashes)]
pub fn lock_hashes(bytes: &[u8]) {

    if !validate_hash(bytes) {
        return;
    }

    lock_module();
}

/// Check if the passed hash is valid.
pub fn validate_hash(bytes: &[u8]) -> bool {

    if validate_app_hash(bytes) {
        return true;
    }

    let mut found = false;

    for hash in hashes().iter() {
        if *hash == bytes {
            found = true;
        }
    }

    found
}