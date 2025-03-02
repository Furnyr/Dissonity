//@ts-nocheck

import { setupHiRpc, loadIframe } from "@dissonity/hirpc-kit";
import { DiscordSDK } from "@discord/embedded-app-sdk";
import { version } from "./Unity/Bridge/version";

/**
 * This example uses the Dissonity hiRPC alongside the official Embedded App SDK.
 * 
 * @dissonity/hirpc-kit — Facilitates interoperation with C#
 * @discord/embedded-app-sdk — Allows you to interact with the Discord client
 */

async function main () {

    // Create hiRPC instance (starts listening)
    const hiRpc = await setupHiRpc(version);

    // Use the Embedded App SDK to send the handshake (ready event)
    const discordSdk = new DiscordSDK(process.env.PUBLIC_CLIENT_ID!, { disableConsoleLogOverride: true });
    await discordSdk.ready();

    // Load hiRPC with one hash access
    const authPromise = hiRpc.load(1);

    // Request hash. Keep it safe!
    const hash = (await hiRpc.requestHash())!;

    // (You can use hiRPC functionality here)
    hiRpc.addAppListener(hash, "my-channel", message => {
       
        console.log(`Unity sent ${message} through the my-channel hiRPC channel!`);

        hiRpc.sendToApp(hash, "my-channel", "Hello C#!");
    });
    
    // Begin loading the Unity game
    loadIframe("Unity/index.html", "dissonity-child");

    // Any RPC-related commands need to run after this promise resolution
    await authPromise;

    discordSdk.commands.openExternalLink({ url: "https://google.com" });
}

main();