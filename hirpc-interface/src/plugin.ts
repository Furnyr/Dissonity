/*

This is the hiRPC Interface for Unity. It allows C# to use the hiRPC from the window.

Function names are PascalCase, C# method conventions.

*/

import type { HiRpcModule, DissonityChannelPayload } from "./types";


mergeInto(LibraryManager.library, {

    // Allow messages to get from the JS layer to the Unity app
    //@unity-api
    OpenDownwardFlow: function(): void {

        this.Channel = "dissonity";

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.openDownwardFlow((stringifiedData: string) => {

            SendMessage("_DissonityBridge", "_HiRpcInput", stringifiedData);
        });
    },

    // Save the app hash for future use
    //@unity-bridge
    SaveAppHash: function (utf8Hash: string): void {

        const hash = UTF8ToString(utf8Hash);

        this.AppHash = hash;
    },

    //@unity-bridge
    EmptyRequest: function (stringifiedMessage: string): void {

        const { nonce } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        // The same nonce is sent down. The state is always included
        // in the game payload.
        const payload: DissonityChannelPayload = {
            nonce
        };

        hiRpc.sendToApp(this.AppHash as string, this.Channel as string, payload);
    },

    // Send data to the JS layer
    //@unity
    SendToJs: function (stringifiedMessage: string): void {

        const { payload, channel } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.sendToJs(this.AppHash as string, channel, payload);
    },

    //@unity-bridge
    PatchUrlMappings: function (stringifiedMessage: string): void {

        const { nonce, stringified_data } = JSON.parse(UTF8ToString(stringifiedMessage));

        //\ Parse data
        const parsedData = JSON.parse(stringified_data);
        const { mappings, config } = parsedData;

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.patchUrlMappings(this.AppHash as any, mappings, config);

        const payload: DissonityChannelPayload = {
            nonce
        };

        hiRpc.sendToApp(this.AppHash as string, this.Channel as string, payload);
    },

    //@unity-bridge
    FormatPrice: function (stringifiedMessage: string): void {

        const { nonce, stringified_data } = JSON.parse(UTF8ToString(stringifiedMessage));

        //\ Parse data
        const parsedData = JSON.parse(stringified_data);
        const { amount, currency, locale } = parsedData;

        const hiRpc = window.dso_hirpc as HiRpcModule;

        const formattedPrice = hiRpc.formatPrice(this.AppHash as any, {
            amount,
            currency
        }, locale);

        const payload: DissonityChannelPayload = {
            nonce,
            formatted_price: formattedPrice
        };

        hiRpc.sendToApp(this.AppHash as any, this.Channel as string, payload);
    },

    //@unity-bridge
    GetQueryObject: function (stringifiedMessage: string): void {

        const { nonce } = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        const query = JSON.stringify(hiRpc.getQueryObject());

        const payload: DissonityChannelPayload = {
            nonce,
            query
        };

        hiRpc.sendToApp(this.AppHash as string, this.Channel as string, payload);
    },

    // Send data to the client RPC
    //@unity-api
    SendToRpc: function (stringifiedMessage: string): void {

        const dataArray = JSON.parse(UTF8ToString(stringifiedMessage));

        const hiRpc = window.dso_hirpc as HiRpcModule;

        // The array is formed again at the hiRPC layer
        hiRpc.sendToRpc(this.AppHash as string, dataArray[0], dataArray[1]);
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

    // End current communication
    //@unity-api
    CloseDownwardFlow: function (): void {

        const hiRpc = window.dso_hirpc as HiRpcModule;

        hiRpc.closeDownwardFlow(this.AppHash as string);
    }
});