mergeInto(LibraryManager.library, {
    Listen: function () {
        window.addEventListener("message", Bridge);
    },
    StopListening: function () {
        window.removeEventListener("message", Bridge);
        SendToRpcBridge({ command: "StopListening" });
    },
    RequestState: function () {
        SendToRpcBridge({ command: "RequestState" });
    },
    RequestPatchUrlMappings: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        SendToRpcBridge({ command: "RequestPatchUrlMappings", payload: message });
    },
    Send: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        SendToRpcBridge({ command: "Send", payload: message });
    },
    RequestQuery: function () {
        SendToRpcBridge({ command: "RequestQuery" });
    }
});