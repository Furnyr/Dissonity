#![allow(dead_code)] //todo remove

// Hashes
pub const HASH_RANDOM_BYTES: i32 = 1;   // Only controls left shift

// Close
pub const CLOSE_CODE_NORMAL: i32 = 1000;

// Handled Commands
pub const COMMAND_DISPATCH: &str = "DISPATCH";
pub const COMMAND_AUTHORIZE: &str = "AUTHORIZE";
pub const COMMAND_AUTHENTICATE: &str = "AUTHENTICATE";

// Handled events
pub const EVENT_READY: &str = "READY";
pub const EVENT_ERROR: &str = "ERROR";

// Handshake
pub const SDK_VERSION: &str = "1.6.0";  // SDK mirroring version
pub const HANDSHAKE_VERSION: i32 = 1;
pub const HANDSHAKE_SDK_MINIMUM_MOBILE_VERSION: i32 = 250;
pub const HANDSHAKE_UNKNOWN_VERSION_NUMBER: i32 = -1;
pub const HANDSHAKE_ENCODING: &str = "json";
pub const PLATFORM_DESKTOP: &str = "desktop";

// Authorize
pub const AUTHORIZE_RESPONSE_TYPE: &str = "code";
pub const AUTHORIZE_PROMPT: &str = "none";

// Listeners
pub const TEST_LISTENER_ID: &str = "myid";