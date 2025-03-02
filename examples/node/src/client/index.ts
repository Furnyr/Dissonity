import { setupHiRpc, loadIframe } from "@dissonity/hirpc-kit";
import { version } from "./Unity/Bridge/version";

async function main () {

    // Create hiRPC instance
    const hiRpc = await setupHiRpc(version);

    // Load hiRPC with one hash access
    const authPromise = hiRpc.load(1);

    // Request hash. Keep it safe!
    const hash = (await hiRpc.requestHash())!;

    // (You can use hiRPC functionality here)

    // Begin loading the Unity game
    loadIframe("Unity/index.html", "dissonity-child");

    // Any RPC-related commands need to run after this promise resolution
    await authPromise;
}

main();