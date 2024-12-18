/*

This is the hiRPC, it handles the communication between Discord's RPC protocol and a game build, while
allowing external JS code to securely use the API.

Its purpose is:
- Being able to use a standalone SDK instead of the npm package
- Being compatible with JS plugins

std is only used for essential functionality.

- - - - - -

modules/rpc.rs -> Connection with the RPC protocol
modules/public.rs -> Hashes for secure communication
modules/interface.rs -> Communication with the game build
modules/facade.rs -> Unsafe code and main JS interoperation

*/

include!(concat!(env!("CARGO_MANIFEST_DIR"), "/src/_version.rs"));

mod utils;
mod constants;
pub mod structs;
pub mod enums;
pub mod modules;
pub mod ci;