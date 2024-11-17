#![cfg(feature = "ci")]

use wasm_bindgen::prelude::*;

#[wasm_bindgen(module = "/pkg/build_variables.js")]
extern "C" {
    pub fn loadBuildVariables();
}