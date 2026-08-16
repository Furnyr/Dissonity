// src/index.ts
var RpcOpcode = /* @__PURE__ */ ((RpcOpcode2) => {
  RpcOpcode2[RpcOpcode2["Handshake"] = 0] = "Handshake";
  RpcOpcode2[RpcOpcode2["Frame"] = 1] = "Frame";
  RpcOpcode2[RpcOpcode2["Close"] = 2] = "Close";
  RpcOpcode2[RpcOpcode2["Hello"] = 3] = "Hello";
  return RpcOpcode2;
})(RpcOpcode || {});
async function setupHiRpc(_hiRpcVersion, bridgePath) {
  if (typeof window == "undefined") {
    throw new Error("Cannot load hiRPC Module outside of a web environment");
  }
  return new Promise((resolve, reject) => {
    if (typeof window.dso_hirpc == "object") {
      resolve(window.dso_hirpc);
      return;
    }
    tryDirectImport().then((module) => {
      resolve(module);
    }).catch((err) => {
      reject(err);
    });
    function tryDirectImport() {
      const bridgeImport = bridgePath ?? sessionStorage.getItem("dso_bridge");
      if (!bridgeImport) {
        throw new Error("You must specify a bridge path using the 'bridgePath' parameter or the 'dso_bridge' session storage item.");
      }
      return new Promise((resolve2, reject2) => {
        import(`${bridgeImport}dissonity_hirpc.js`).then(() => {
          import(`${bridgeImport}dissonity_build_variables.js`).then(() => {
            mountInstance();
            resolve2(window.dso_hirpc);
          }).catch((err) => {
            reject2(err);
          });
        }).catch((err) => {
          reject2(err);
        });
      });
    }
    function mountInstance() {
      const instance = new window.Dissonity.HiRpc.default();
      if (window.dso_hirpc != instance) {
        window.dso_hirpc = instance;
      }
      clearRpcSessionStorage();
    }
    function clearRpcSessionStorage() {
      const hiRpc = window.dso_hirpc;
      const query = hiRpc.getQueryObject();
      sessionStorage.removeItem("dso_connected");
      sessionStorage.removeItem("dso_authenticated");
      sessionStorage.setItem("dso_instance_id", query.instance_id);
    }
  });
}
function loadIframe(src, id) {
  const iframe = document.createElement("iframe");
  iframe.id = id;
  iframe.src = src;
  iframe.height = "100vh";
  iframe.width = "100vw";
  iframe.style.display = "block";
  iframe.style.border = "0px";
  iframe.style.overflow = "hidden";
  iframe.style.height = "100vh";
  iframe.style.width = "100vw";
  document.body.appendChild(iframe);
}
export {
  RpcOpcode,
  loadIframe,
  setupHiRpc
};
