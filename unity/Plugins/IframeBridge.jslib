mergeInto(LibraryManager.library, {
    LoadInterface: function () {
        this.InterfaceWrapper = async (data) => {
            if (!this.rpcInterface) {
                await this.rpcInterfacePromise;
                if (!this.rpcInterface)
                    return;
            }
            this.rpcInterface(data);
        };
        this.rpcInterfacePromise = new Promise((resolve, reject) => {
            if (window.outsideDiscord) {
                import("web_bridge")
                    .then(module => {
                    this.rpcInterface = module.InterfaceBridgeListener;
                    resolve(true);
                })
                    .catch(error => {
                    reject(error);
                });
            }
            else {
                import("bridge")
                    .then(module => {
                    this.rpcInterface = module.InterfaceBridgeListener;
                    resolve(true);
                })
                    .catch(error => {
                    reject(error);
                });
            }
        });
    },
    StopListening: function () {
        this.InterfaceWrapper({ command: "StopListening" });
    },
    RequestState: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce } = JSON.parse(message);
        this.InterfaceWrapper({ command: "RequestState", nonce });
    },
    RequestPatchUrlMappings: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce, payload } = JSON.parse(message);
        this.InterfaceWrapper({ command: "RequestPatchUrlMappings", nonce, payload: JSON.stringify(payload) });
    },
    Send: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        this.InterfaceWrapper({ command: "Send", payload: message });
    },
    RequestFormatPrice: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce, payload } = JSON.parse(message);
        this.InterfaceWrapper({ command: "RequestFormatPrice", nonce, payload: JSON.stringify(payload) });
    },
    RequestQuery: function (stringifiedMessage) {
        const message = UTF8ToString(stringifiedMessage);
        const { nonce } = JSON.parse(message);
        this.InterfaceWrapper({ command: "RequestQuery", nonce });
    }
});