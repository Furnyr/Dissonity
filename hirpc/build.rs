use std::fs;

fn main() {
    let cargo_toml = fs::read_to_string("Cargo.toml").expect("Cargo.toml not found");

    let value: toml::Value = toml::from_str(&cargo_toml).expect("Something went wrong parsing Cargo.toml");
    let version = value["package"]["version"].as_str().expect("Version not found");

    fs::write("src/_version.rs", format!("pub const HIRPC_VERSION: &str = \"{}\";", version))
        .expect("Failed to write _version.rs");
}