use wasm_bindgen::{prelude::Closure, JsValue};
use web_sys::MessageEvent;
use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize, Debug)]
pub struct QueryData {
    pub instance_id: Option<String>,
    pub location_id: Option<String>,
    pub channel_id: Option<String>,
    pub guild_id: Option<String>,
    pub frame_id: Option<String>,
    pub platform: Option<String>,
    pub mobile_app_version: Option<String>,
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

pub struct ClosureWrapper {
    pub closure: Closure<dyn FnMut(MessageEvent)>,
    pub id: &'static str
}

#[derive(Serialize, Deserialize, Debug)]
pub struct AppPayload {
    pub hirpc_state: i32,

    #[serde(serialize_with = "serialize_object", deserialize_with = "deserialize_object")]
    pub rpc_message: Option<JsValue>,

    pub hirpc_message: Option<HiRpcPayload>
}

#[derive(Serialize, Deserialize, Debug)]
pub struct HiRpcPayload {
    pub action_code: i32,

    #[serde(serialize_with = "serialize_object", deserialize_with = "deserialize_object")]
    pub data: Option<JsValue>
}

#[derive(Serialize, Deserialize, Debug)]
pub struct HashPayload {
    pub hash: String,
}

fn serialize_object<S>(value: &Option<JsValue>, serializer: S) -> Result<S::Ok, S::Error> where S: serde::Serializer {
    match value {
        Some(value) => {
            let serde_value: serde_json::Value = serde_wasm_bindgen::from_value(value.clone()).unwrap();
            serde_value.serialize(serializer)
        }
        None => serializer.serialize_none()
    }
}

fn deserialize_object<'de, D>(deserializer: D) -> Result<Option<JsValue>, D::Error> where D: serde::Deserializer<'de> {
    let serde_value: Option<serde_json::Value> = Option::deserialize(deserializer)?;
    if let Some(value) = serde_value {
        let js_value: JsValue = serde_wasm_bindgen::to_value(&value).map_err(serde::de::Error::custom)?;
        Ok(Some(js_value))
    } else {
        Ok(None)
    }
}