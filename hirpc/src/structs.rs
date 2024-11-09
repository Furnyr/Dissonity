use wasm_bindgen::JsValue;
use web_sys::js_sys::{Object, Reflect};

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

pub struct HandshakePayload<'a> {
    pub v: f64,
    pub encoding: &'a str,
    pub client_id: &'a str,
    pub frame_id: &'a str,
    pub sdk_version: Option<&'a str>
}

impl HandshakePayload<'_> {
    pub fn to_js(&self) -> JsValue {
        let obj = Object::new();

        _ = Reflect::set(&obj, &"v".into(), &self.v.into());
        _ = Reflect::set(&obj, &"encoding".into(), &self.encoding.into());
        _ = Reflect::set(&obj, &"client_id".into(), &self.client_id.into());
        _ = Reflect::set(&obj, &"frame_id".into(), &self.frame_id.into());

        if let Some(version) = self.sdk_version {
            _ = Reflect::set(&obj, &"sdk_version".into(), &version.into());
        }

        else {
            _ = Reflect::set(&obj, &"sdk_version".into(), &JsValue::UNDEFINED);
        }
        
        JsValue::from(obj)
    }
}