/*
        This file has been generated from a TypeScript source.
        Don't modify it manually.
    
        https://github.com/Furnyr/Dissonity/
*/

mergeInto(LibraryManager.library, {
    OpenDownwardFlow: function () {
        const hiRpc = window.dso_hirpc;
        hiRpc.openDownwardFlow((stringifiedData) => {
            SendMessage("_DissonityBridge", "_HiRpcInput", stringifiedData);
        });
    },
    EmptyRequest: function (stringifiedMessage) {
        const { nonce, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        const payload = {
            nonce
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    SendToJs: function (stringifiedMessage) {
        const { payload, channel, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        hiRpc.sendToJs(app_hash, channel, payload);
    },
    PatchUrlMappings: function (stringifiedMessage) {
        const { nonce, data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const { mappings, config } = data;
        const hiRpc = window.dso_hirpc;
        hiRpc.patchUrlMappings(app_hash, mappings, config);
        const payload = {
            nonce
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    FormatPrice: function (stringifiedMessage) {
        const { nonce, data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const { amount, currency, locale } = data;
        const hiRpc = window.dso_hirpc;
        const formattedPrice = hiRpc.formatPrice(app_hash, {
            amount,
            currency
        }, locale);
        const payload = {
            nonce,
            formatted_price: formattedPrice
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    GetQueryObject: function (stringifiedMessage) {
        const { nonce, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        const query = JSON.stringify(hiRpc.getQueryObject());
        const payload = {
            nonce,
            query
        };
        hiRpc.sendToApp(app_hash, "dissonity", payload);
    },
    SendToRpc: function (stringifiedMessage) {
        const { data, app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        hiRpc.sendToRpc(app_hash, data[0], data[1]);
    },
    ExpandCanvas: function () {
        if (typeof window.dso_expand_canvas == "undefined")
            return;
        window.dso_expand_canvas();
        setTimeout(window.dso_expand_canvas, 15);
        setTimeout(window.dso_expand_canvas, 60);
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
    CloseDownwardFlow: function (stringifiedMessage) {
        const { app_hash } = JSON.parse(UTF8ToString(stringifiedMessage));
        const hiRpc = window.dso_hirpc;
        hiRpc.closeDownwardFlow(app_hash);
    }
});
