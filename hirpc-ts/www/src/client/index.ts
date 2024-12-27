//import type HiRpc from "../../../src/index";
import type IHiRpc from "../../../src/shape";

const hiRpc = await globalThis["dso_hirpc" as keyof typeof globalThis] as IHiRpc;

// Test interacting with the hiRPC here!
// If you are using the testing url in a browser, everything sent to the RPC is received back.

//const hash = wasm.requestHash()!;

//console.log( await wasm.connect(hash) );

const hash = await hiRpc.test();
console.log(hash);