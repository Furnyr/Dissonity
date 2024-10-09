
/*

This is the "IframeBridge". It can send messages to the "DissonityBridge" inside the Unity build,
so it is the last step in JavaScript.

It can't interact directly with the Discord RPC because it is inside a nested iframe. Instead,
it can communicate with the "RpcBridge".

Function names are PascalCase, C# method conventions.

*/

// Unity methods
declare const UTF8ToString: (str: any) => string;

// Jspre
declare const Bridge: (message: MessageEvent) => void;
declare const SendToRpcBridge: (message: { command: RpcBridgeCommands, nonce?: string, payload?: string }) => void;

// Plugin
declare const LibraryManager: { library: string };
declare const mergeInto: (arg1: string, arg2: Record<string, (...args: any[]) => void>) => void;

// Types
import type { RpcBridgeCommands } from "./types";


//# FUNCTIONS - - - - -
mergeInto(LibraryManager.library, {

    //@unity
    Listen: function (): void {
        window.addEventListener("message", Bridge);
    },

    //@unity
    StopListening: function (): void {
        window.removeEventListener("message", Bridge);
        SendToRpcBridge({ command: "StopListening" });
    },

    //@unity
    RequestState: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce } = JSON.parse(message);

        SendToRpcBridge({ command: "RequestState", nonce });
    },

    //@unity
    RequestPatchUrlMappings: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce, payload } = JSON.parse(message);

        SendToRpcBridge({ command: "RequestPatchUrlMappings", nonce, payload: JSON.stringify(payload) });
    },

    //@unity
    // Called from Unity. Send data to the client RPC
    Send: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        SendToRpcBridge({ command: "Send", payload: message });
    },

    //@unity
    RequestFormatPrice: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce, payload } = JSON.parse(message);

        SendToRpcBridge({ command: "RequestFormatPrice", nonce, payload: JSON.stringify(payload) });
    },

    //@unity
    // This could be the same function that the RpcBridge has
    // since it can access the query from a nested iframe,
    // but it's not to make sure both bridges have the same implementation.
    RequestQuery: function (stringifiedMessage: string): void{

        const message = UTF8ToString(stringifiedMessage);
        const { nonce } = JSON.parse(message);

        SendToRpcBridge({ command: "RequestQuery", nonce });
    }
});