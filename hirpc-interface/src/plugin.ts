/*

This is the hiRPC Interface for Unity. It allows C# to use the hiRPC from the window.

Function names are PascalCase, C# method conventions.

*/

import type { HiRpcModule, DissonityChannelPayload } from "./types";

mergeInto(LibraryManager.library, {

    // Allow messages to get from the JS layer to the Unity app
    //@unity-api
    DsoOpenDownwardFlow: function(): void {

        //todo remove logs
        console.log("Opening downward flow");

        const hiRpc = window.dso_hirpc as HiRpcModule;

        // Load module now if LAZY_HIRPC_LOAD is set to true.
        // This loads the module with zero hash accesses. Hash access is already locked at this point either way.
        if (hiRpc.getBuildVariables().LAZY_HIRPC_LOAD) {
            hiRpc.load(0)
                .then(openFlow)
                .catch(err => {

                    // So if something weird happens we can see what's going on
                    console.log(err);
                });
        }

        else {
            openFlow();
        }

        function openFlow() {
            console.log("Setting app sender");
            hiRpc.openDownwardFlow((stringifiedData: string) => {
                SendMessage("_DissonityBridge", "_HiRpcInput", stringifiedData);
            });
        }
    },

    //@unity-bridge
    DsoEmptyRequest: function (stringifiedMessage: string): void {

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
    DsoSendToJs: function (stringifiedMessage: string): void {

        const { data, channel, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.sendToJs(app_hash, channel, data);
    },

    //@unity-bridge
    DsoPatchUrlMappings: function (stringifiedMessage: string): void {

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
    DsoFormatPrice: function (stringifiedMessage: string): void {

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
    DsoGetQueryObject: function (stringifiedMessage: string): void {

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
    DsoSendToRpc: function (stringifiedMessage: string): void {

        const { data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        // The array is formed again at the hiRPC layer
        hiRpc.sendToRpc(app_hash, data[0], data[1]);
    },

    //@unity-api
    DsoExpandCanvas: function (): void {

        if (typeof window.dso_expand_canvas == "undefined") return;

        // These times have been tested to work as nicely as possible
        window.dso_expand_canvas();
        setTimeout(window.dso_expand_canvas, 15);
        setTimeout(window.dso_expand_canvas, 60);
    },

    //@unity
    DsoLog: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        console.log(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },

    //@unity
    DsoWarn: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        console.warn(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },

    //@unity
    DsoError: function (stringifiedMessage: string): void {

        const message = UTF8ToString(stringifiedMessage);

        console.error(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },

    //@unity-api
    DsoLocalStorageSetItem: function (stringifiedMessage: string): void {

        const { data } = JSON.parse(UTF8ToString(stringifiedMessage));

        localStorage.setItem(data[0], data[1]);
    },

    //@unity-bridge
    DsoLocalStorageGetItem: function (stringifiedMessage: string): void {

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
    DsoLocalStorageClear: function (): void {

        localStorage.clear();
    },

    // End current communication
    //@unity-api
    DsoCloseDownwardFlow: function (stringifiedMessage: string): void {

        const { app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.closeDownwardFlow(app_hash);
    }
});