/*

This is the hiRPC, it handles the communication between Discord's RPC protocol and a game build, while
allowing external JS code to securely use the API.

Its purpose is:
- Being able to use a standalone SDK instead of the npm package
- Being compatible with JS plugins

std is only used for essential functionality.

- - - - - -

modules/rpc.rs -> Connection with the RPC protocol
modules/public.rs -> Exposed API and hashes
modules/interface.rs -> Communication with the game build

*/

//todo exposed functions will follow js conventions

mod utils;
mod constants;
mod structs;
mod enums;
mod modules;