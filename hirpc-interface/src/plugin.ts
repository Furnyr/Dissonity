/*

This is the hiRPC Interface. It receives messages from the DissonityBridge inside the Unity build
and interacts with the hiRPC.

It is used as a Unity plugin.

Function names are PascalCase, C# method conventions.

*/

import type { HiRpcModule } from "./types";


mergeInto(LibraryManager.library, {

    // Allow messages to get from the JS level to the Unity app
    //@unity-api
    OpenDownwardCommunication: function(): void {

        this.Channel = "dissonity";

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        hiRpc.useAppSender((stringifiedData: string) => {

            SendMessage("_DissonityBridge", "_HiRpcInput", stringifiedData);
        });

        hiRpc.dispatchAppHash();
    },

    // Save the app hash for future use
    //@unity-bridge
    SaveAppHash: function (utf8Hash: string): void {

        const stringHash = UTF8ToString(utf8Hash);
        const hash = new TextEncoder().encode(stringHash);

        this.AppHash = hash;
    },

    //@unity-bridge
    RequestEmpty: function (stringifiedMessage: string): void {

        const { nonce } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        // The same nonce is sent down. The state is always included
        // in the game payload.
        hiRpc.sendToApp(this.AppHash as any, {
            channel: this.Channel as any,
            nonce
        });
    },

    // Send data to the JS level
    //@unity
    SendHiRpc: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        // The array is formed again at the hiRPC level
        hiRpc.sendToHiRpc(this.AppHash as any, message);
    },

    //@unity-bridge
    RequestPatchUrlMappings: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce, stringified_data } = JSON.parse(message);

        //\ Parse data
        const parsedData = JSON.parse(stringified_data);
        const { mappings, config } = parsedData;

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        hiRpc.patchUrlMappings(this.AppHash as any, mappings, config);

        hiRpc.sendToApp(this.AppHash as any, {
            channel: this.Channel as any,
            nonce
        });
    },

    //@unity-bridge
    RequestFormatPrice: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const { nonce, stringified_data } = JSON.parse(message);

        //\ Parse data
        const parsedData = JSON.parse(stringified_data);
        const { amount, currency, locale } = parsedData;

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        const formattedPrice = hiRpc.formatPrice(this.AppHash as any, {
            amount,
            currency
        }, locale);

        hiRpc.sendToApp(this.AppHash as any, {
            channel: this.Channel as any,
            nonce,
            formatted_price: formattedPrice
        });
    },

    //@unity-bridge
    RequestQuery: function (stringifiedMessage: string): void {

        const { nonce } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        const query = hiRpc.getStringifiedQuery();

        hiRpc.sendToApp(this.AppHash as any, {
            channel: this.Channel as any,
            nonce,
            query
        });
    },

    // Send data to the client RPC
    //@unity-api
    SendRpc: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);
        const dataArray = JSON.parse(message);

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        // The array is formed again at the hiRPC level
        hiRpc.sendToRpc(this.AppHash as any, dataArray[0], dataArray[1]);
    },

    // End communication
    //@unity-api
    StopListening: function (): void {

        const hiRpc = (globalThis as unknown as { dso_hirpc: HiRpcModule }).dso_hirpc;

        hiRpc.clearRpcListeners(this.AppHash as any);
    }
});