
/*

Functions used in the IframeBridge.

*/

// Methods
declare const SendMessage: (gameObject: string, method: string, value: any) => void;

// Types
import type { RpcBridgeCommands } from "./types";


//# FUNCTIONS - - - - -
//@iframe-bridge.lib
function SendToRpcBridge(message: { command: RpcBridgeCommands, nonce?: string, payload?: string }): void {

    const source: Window = window.parent.opener ?? window.parent;
    const sourceOrigin = !!document.referrer ? document.referrer : "*";

    source.postMessage({ iframeBridge: true, ...message }, sourceOrigin);
};

//@rpc-bridge
function Bridge({ data }: MessageEvent): void {

    const { method, payload } = data;

    // Since this function is called frm the RpcBridge, it's always save to send to Unity
    SendMessage("_DissonityBridge", method, payload);
};