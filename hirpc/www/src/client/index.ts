import type * as hirpc from "../../../pkg/dissonity_hirpc";

const wasm = await globalThis["dso_hirpc" as keyof typeof globalThis] as typeof hirpc;

// Test interacting with the hiRPC here!
// If you are using the testing url in a browser, everything sent to the RPC is received back.

const hash = wasm.requestHash()!;

console.log( await wasm.connect(hash) );