/*
        This file has been generated from a TypeScript source.
        Don't modify it manually.
    
        https://github.com/Furnyr/Dissonity/
*/

mergeInto(LibraryManager.library, {
    OpenDownwardFlow: function () {
        this.Channel = "dissonity";
        const hiRpc = window.dso_hirpc;
        hiRpc.openDownwardFlow((stringifiedData) => {
            SendMessage("_DissonityBridge", "_HiRpcInput", stringifiedData);
        });
    },
    SaveAppHash: function (utf8Hash) {
        const hash = UTF8ToString(utf8Hash);
        this.AppHash = hash;
    },
    EmptyRequest: function (stringifiedMessage) {
        const { nonce } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        const payload = {
            nonce
        };
        hiRpc.sendToApp(this.AppHash, this.Channel, payload);
    },
    SendToJs: function (stringifiedMessage) {
        const { payload, channel } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        hiRpc.sendToJs(this.AppHash, channel, payload);
    },
    PatchUrlMappings: function (stringifiedMessage) {
        const { nonce, stringified_data } = JSON.parse(UTF8ToString(stringifiedMessage));
        const parsedData = JSON.parse(stringified_data);
        const { mappings, config } = parsedData;
        const hiRpc = window.dso_hirpc;
        hiRpc.patchUrlMappings(this.AppHash, mappings, config);
        const payload = {
            nonce
        };
        hiRpc.sendToApp(this.AppHash, this.Channel, payload);
    },
    FormatPrice: function (stringifiedMessage) {
        const { nonce, stringified_data } = JSON.parse(UTF8ToString(stringifiedMessage));
        const parsedData = JSON.parse(stringified_data);
        const { amount, currency, locale } = parsedData;
        const hiRpc = window.dso_hirpc;
        const formattedPrice = hiRpc.formatPrice(this.AppHash, {
            amount,
            currency
        }, locale);
        const payload = {
            nonce,
            formatted_price: formattedPrice
        };
        hiRpc.sendToApp(this.AppHash, this.Channel, payload);
    },
    GetQueryObject: function (stringifiedMessage) {
        const { nonce } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        const query = JSON.stringify(hiRpc.getQueryObject());
        const payload = {
            nonce,
            query
        };
        hiRpc.sendToApp(this.AppHash, this.Channel, payload);
    },
    SendToRpc: function (stringifiedMessage) {
        const dataArray = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        hiRpc.sendToRpc(this.AppHash, dataArray[0], dataArray[1]);
    },
    DissonityLog: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        console.log(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },
    DissonityWarn: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        console.warn(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },
    DissonityError: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        console.error(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },
    CloseDownwardFlow: function () {
        const hiRpc = window.dso_hirpc;
        hiRpc.closeDownwardFlow(this.AppHash);
    }
});
