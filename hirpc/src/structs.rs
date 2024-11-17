#![allow(dead_code)] //todo remove

use wasm_bindgen::prelude::Closure;
use web_sys::MessageEvent;
use serde::{Serialize, Deserialize};

pub struct QueryData {
    pub keys: Vec<String>,
    pub values: Vec<String>
}

impl QueryData {
    pub fn insert(&mut self, key: String, value: String) {
        self.keys.push(key);
        self.values.push(value);
    }

    pub fn find(&self, key: &str) -> Option<&str> {
        for i in 0..self.keys.len() {

            if self.keys[i] != key { continue; }

            if i >= self.values.len() {
                return None;
            }

            else {
                return Some(&self.values[i]);
            }
        }

        None
    }

    pub fn has(&self, key: &str) -> bool {
        for i in 0..self.keys.len() {

            if self.keys[i] != key { continue; }

            return i < self.values.len()
        }

        false
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct HandshakePayload<'a> {
    pub v: i32,
    pub encoding: &'a str,
    pub client_id: &'a str,
    pub frame_id: &'a str,
    pub sdk_version: Option<&'a str>
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Mapping {
    pub prefix: String,
    pub target: String
}

#[derive(Serialize, Deserialize, Debug)]
pub struct PatchUrlMappingsConfig {
    #[serde(rename = "patchFetch")]
    pub patch_fetch: bool,

    #[serde(rename = "patchWebSocket")]
    pub patch_web_socket: bool,

    #[serde(rename = "patchXhr")]
    pub patch_xhr: bool,

    #[serde(rename = "patchSrcAttributes")]
    pub patch_src_attributes: bool
}

pub struct ListenerWrapper {
    pub closure: Closure<dyn FnMut(MessageEvent)>,
    pub id: &'static str
}