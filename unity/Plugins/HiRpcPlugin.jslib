/*
    This file has been generated from a TypeScript source.
    Don't modify it manually.

    https://github.com/Furnyr/Dissonity/
*/

mergeInto(LibraryManager.library, {
    OpenDownwardCommunication: function () {
        this.Channel = "dissonity";
        const hiRpc = globalThis.dso_hirpc;
        hiRpc.useAppSender((stringifiedData) => {
            SendMessage("_DissonityBridge", "_HiRpcInput", stringifiedData);
        });
        hiRpc.dispatchAppHash();
    },
    SaveAppHash: function (utf8Hash) {
        const stringHash = UTF8ToString(utf8Hash);
        const hash = new TextEncoder().encode(stringHash);
        this.AppHash = hash;
    },
    RequestEmpty: function (stringifiedMessage) {
        const { nonce } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = globalThis.dso_hirpc;
        hiRpc.sendToApp(this.AppHash, {
            channel: this.Channel,
            nonce
        });
    },
    SendHiRpc: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const hiRpc = globalThis.dso_hirpc;
        hiRpc.sendToHiRpc(this.AppHash, message);
    },
    RequestPatchUrlMappings: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce, stringified_data } = JSON.parse(message);
        const parsedData = JSON.parse(stringified_data);
        const { mappings, config } = parsedData;
        const hiRpc = globalThis.dso_hirpc;
        hiRpc.patchUrlMappings(this.AppHash, mappings, config);
        hiRpc.sendToApp(this.AppHash, {
            channel: this.Channel,
            nonce
        });
    },
    RequestFormatPrice: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce, stringified_data } = JSON.parse(message);
        const parsedData = JSON.parse(stringified_data);
        const { amount, currency, locale } = parsedData;
        const hiRpc = globalThis.dso_hirpc;
        const formattedPrice = hiRpc.formatPrice(this.AppHash, {
            amount,
            currency
        }, locale);
        hiRpc.sendToApp(this.AppHash, {
            channel: this.Channel,
            nonce,
            formatted_price: formattedPrice
        });
    },
    RequestQuery: function (stringifiedMessage) {
        const { nonce } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = globalThis.dso_hirpc;
        const query = hiRpc.getStringifiedQuery();
        hiRpc.sendToApp(this.AppHash, {
            channel: this.Channel,
            nonce,
            query
        });
    },
    SendRpc: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const dataArray = JSON.parse(message);
        const hiRpc = globalThis.dso_hirpc;
        hiRpc.sendToRpc(this.AppHash, dataArray[0], dataArray[1]);
    },
    StopListening: function () {
        const hiRpc = globalThis.dso_hirpc;
        hiRpc.clearRpcListeners(this.AppHash);
    }
});
