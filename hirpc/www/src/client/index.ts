// Run pnpm build/kit:types in the main hiRPC folder.
import type IHiRpc from "#../../../../hirpc-kit/types/index.d.ts"

const hiRpc = await globalThis["dso_hirpc" as keyof typeof globalThis] as IHiRpc;

// Test interacting with the hiRPC here!
// If you are using the testing url in a browser, everything sent to the RPC is received back.

const hash = await hiRpc.requestHash()!;
await hiRpc.load();