mergeInto(LibraryManager.library, {
    Listen: function () {
        window.addEventListener("message", Bridge);
    },
    StopListening: function () {
        window.removeEventListener("message", Bridge);
        SendToRpcBridge({ command: "StopListening" });
    },
    RequestState: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce } = JSON.parse(message);
        SendToRpcBridge({ command: "RequestState", nonce });
    },
    RequestPatchUrlMappings: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce, payload } = JSON.parse(message);
        SendToRpcBridge({ command: "RequestPatchUrlMappings", nonce, payload: JSON.stringify(payload) });
    },
    Send: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        SendToRpcBridge({ command: "Send", payload: message });
    },
    RequestFormatPrice: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce, payload } = JSON.parse(message);
        SendToRpcBridge({ command: "RequestFormatPrice", nonce, payload: JSON.stringify(payload) });
    },
    RequestQuery: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce } = JSON.parse(message);
        SendToRpcBridge({ command: "RequestQuery", nonce });
    }
});