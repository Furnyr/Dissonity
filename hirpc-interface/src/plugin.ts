/*

This is the hiRPC Interface for Unity. It allows C# to use the hiRPC from the window.

Function names are PascalCase, C# method conventions.

*/

import type { HiRpcModule, DissonityChannelPayload } from "./types";

mergeInto(LibraryManager.library, {

    // Allow messages to get from the JS layer to the Unity app
    //@unity-api
    OpenDownwardFlow: function(): void {

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.openDownwardFlow((stringifiedData: string) => {

            SendMessage("_DissonityBridge", "_HiRpcInput", stringifiedData);
        });
    },

    //@unity-bridge
    EmptyRequest: function (stringifiedMessage: string): void {

        const { nonce, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        // The same nonce is sent down. The state is always included
        // in the game payload.
        const payload: DissonityChannelPayload = {
            nonce
        };

        hiRpc.sendToApp(app_hash, DISSONITY_CHANNEL, payload);
    },

    // Send data to the JS layer
    //@unity
    SendToJs: function (stringifiedMessage: string): void {

        const { data, channel, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.sendToJs(app_hash, channel, data);
    },

    //@unity-bridge
    PatchUrlMappings: function (stringifiedMessage: string): void {

        const { nonce, data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        //\ Parse data
        const { mappings, config } = data;

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.patchUrlMappings(app_hash, mappings, config);

        const payload: DissonityChannelPayload = {
            nonce
        };

        hiRpc.sendToApp(app_hash, DISSONITY_CHANNEL, payload);
    },

    //@unity-bridge
    FormatPrice: function (stringifiedMessage: string): void {

        const { nonce, data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        //\ Parse data
        const { amount, currency, locale } = data;

        const hiRpc = window.dso_hirpc as HiRpcModule;

        const formattedPrice = hiRpc.formatPrice(app_hash, {
            amount,
            currency
        }, locale);

        const payload: DissonityChannelPayload = {
            nonce,
            response: formattedPrice
        };

        hiRpc.sendToApp(app_hash, DISSONITY_CHANNEL, payload);
    },

    //@unity-bridge
    GetQueryObject: function (stringifiedMessage: string): void {

        const { nonce, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        const query = JSON.stringify(hiRpc.getQueryObject());

        const payload: DissonityChannelPayload = {
            nonce,
            response: query
        };

        hiRpc.sendToApp(app_hash, DISSONITY_CHANNEL, payload);
    },

    // Send data to the client RPC
    //@unity-api
    SendToRpc: function (stringifiedMessage: string): void {

        const { data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        // The array is formed again at the hiRPC layer
        hiRpc.sendToRpc(app_hash, data[0], data[1]);
    },

    //@unity-api
    ExpandCanvas: function (): void {
        
        if (typeof window.dso_expand_canvas == "undefined") return;

        // These times have been tested to work as nicely as possible
        window.dso_expand_canvas();
        setTimeout(window.dso_expand_canvas, 15);
        setTimeout(window.dso_expand_canvas, 60);
    },

    //@unity
    DissonityLog: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        console.log(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },

    //@unity
    DissonityWarn: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        console.warn(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },

    //@unity
    DissonityError: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        console.error(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },

    //@unity-api
    LocalStorageSetItem: function (stringifiedMessage: string): void {

        const { data } = JSON.parse(UTF8ToString(stringifiedMessage));

        localStorage.setItem(data[0], data[1]);
    },

    //@unity-bridge
    LocalStorageGetItem: function (stringifiedMessage: string): void {

        const { nonce, app_hash, data } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        const response = localStorage.getItem(data);

        const payload: DissonityChannelPayload = {
            nonce,
            response,
            nullable_response: true
        };

        hiRpc.sendToApp(app_hash, DISSONITY_CHANNEL, payload);
    },

    //@unity-api
    LocalStorageClear: function (): void {

        localStorage.clear();
    },

    // End current communication
    //@unity-api
    CloseDownwardFlow: function (stringifiedMessage: string): void {

        const { app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.closeDownwardFlow(app_hash);
    }
});