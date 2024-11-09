import type * as hirpc from "../../../pkg/dissonity_hirpc";

const wasm = await globalThis["dso_hirpc" as keyof typeof globalThis] as typeof hirpc;

// Test interacting with the hiRPC here!

wasm.connect();