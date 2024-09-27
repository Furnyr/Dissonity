function SendToRpcBridge(message) {
    const source = window.parent.opener ?? window.parent;
    const sourceOrigin = !!document.referrer ? document.referrer : "*";
    source.postMessage({ iframeBridge: true, ...message }, sourceOrigin);
}
;
function Bridge({ data }) {
    const { method, payload } = data;
    SendMessage("_DissonityBridge", method, payload);
}
;