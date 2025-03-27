/*
        This file has been generated from a TypeScript source.
        Don't modify it manually.
    
        https://github.com/Furnyr/Dissonity/
*/

mergeInto(LibraryManager.library, {
    DsoOpenDownwardFlow: function () {
        const hiRpc = window.dso_hirpc;
        hiRpc.openDownwardFlow((stringifiedData) => {
            SendMessage("_DissonityBridge", "_HiRpcInput", stringifiedData);
        });
    },
    DsoEmptyRequest: function (stringifiedMessage) {
        const { nonce, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        const payload = {
            nonce
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    DsoSendToJs: function (stringifiedMessage) {
        const { data, channel, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        hiRpc.sendToJs(app_hash, channel, data);
    },
    DsoPatchUrlMappings: function (stringifiedMessage) {
        const { nonce, data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const { mappings, config } = data;
        const hiRpc = window.dso_hirpc;
        hiRpc.patchUrlMappings(app_hash, mappings, config);
        const payload = {
            nonce
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    DsoFormatPrice: function (stringifiedMessage) {
        const { nonce, data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const { amount, currency, locale } = data;
        const hiRpc = window.dso_hirpc;
        const formattedPrice = hiRpc.formatPrice(app_hash, {
            amount,
            currency
        }, locale);
        const payload = {
            nonce,
            response: formattedPrice
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    DsoGetQueryObject: function (stringifiedMessage) {
        const { nonce, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        const query = JSON.stringify(hiRpc.getQueryObject());
        const payload = {
            nonce,
            response: query
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    DsoSendToRpc: function (stringifiedMessage) {
        const { data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        hiRpc.sendToRpc(app_hash, data[0], data[1]);
    },
    DsoExpandCanvas: function () {
        if (typeof window.dso_expand_canvas == "undefined")
            return;
        window.dso_expand_canvas();
        setTimeout(window.dso_expand_canvas, 15);
        setTimeout(window.dso_expand_canvas, 60);
    },
    DsoLog: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        console.log(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },
    DsoWarn: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        console.warn(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },
    DsoError: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        console.error(`%c[Dissonity]%c ${message}`, "color:#8177f6;font-weight: bold;", "color:initial;");
    },
    DsoLocalStorageSetItem: function (stringifiedMessage) {
        const { data } = JSON.parse(UTF8ToString(stringifiedMessage));
        localStorage.setItem(data[0], data[1]);
    },
    DsoLocalStorageGetItem: function (stringifiedMessage) {
        const { nonce, app_hash, data } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        const response = localStorage.getItem(data);
        const payload = {
            nonce,
            response,
            nullable_response: true
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    DsoLocalStorageClear: function () {
        localStorage.clear();
    },
    DsoCloseDownwardFlow: function (stringifiedMessage) {
        const { app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        hiRpc.closeDownwardFlow(app_hash);
    }
});
