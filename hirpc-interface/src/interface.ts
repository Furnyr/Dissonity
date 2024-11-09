
/*

This is the hiRPC Interface. It receives messages from the DissonityBridge inside the Unity build
and interacts with the hiRPC.

Function names are PascalCase, C# method conventions.

*/

// Types
import type { RpcBridgeMessage } from "./old.types";
type RpcBridgeInterface = (message: RpcBridgeMessage) => void;


//# FUNCTIONS - - - - -
mergeInto(LibraryManager.library, {
    
    //@unity-api
    // Imports the RpcBridge to enable communication
    LoadInterface: function (): void {

        // Function used in this file
        this.InterfaceWrapper = async (data: unknown) => {

            if (!this.rpcInterface) {
                await (this.rpcInterfacePromise as unknown as Promise<unknown>);
                
                if (!this.rpcInterface) return;
            }

            this.rpcInterface(data);
        }

        this.rpcInterfacePromise = new Promise((resolve, reject) => {

            if (window.dso_outside_discord) {

                import("web_bridge" as string)
                .then(module => {

                    this.rpcInterface = module.InterfaceBridgeListener;
                    resolve(true);
                })
                .catch (error => {
                    reject(error);
                });
            }

            else {

                import("bridge" as string)
                .then(module => {
                    this.rpcInterface = module.InterfaceBridgeListener;
                    resolve(true);
                })
                .catch(error => {
                    reject(error);
                });
            }
        }) as any;
    },

    //@unity-api
    StopListening: function (): void {
        (this.InterfaceWrapper as RpcBridgeInterface)({ command: "StopListening" });
    },

    //@unity-bridge
    RequestState: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce } = JSON.parse(message);

        (this.InterfaceWrapper as RpcBridgeInterface)({ command: "RequestState", nonce });
    },

    //@unity-bridge
    RequestPatchUrlMappings: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce, payload } = JSON.parse(message);

        (this.InterfaceWrapper as RpcBridgeInterface)({ command: "RequestPatchUrlMappings", nonce, payload: JSON.stringify(payload) });
    },

    //@unity-api
    // Called from Unity. Send data to the client RPC
    Send: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        (this.InterfaceWrapper as RpcBridgeInterface)({ command: "Send", payload: message });
    },

    //@unity-bridge
    RequestFormatPrice: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce, payload } = JSON.parse(message);

        (this.InterfaceWrapper as RpcBridgeInterface)({ command: "RequestFormatPrice", nonce, payload: JSON.stringify(payload) });
    },

    //@unity-bridge
    RequestQuery: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce } = JSON.parse(message);

        (this.InterfaceWrapper as RpcBridgeInterface)({ command: "RequestQuery", nonce });
    }
});