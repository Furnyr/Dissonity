// src/index.ts
var RpcOpcode = /* @__PURE__ */ ((RpcOpcode2) => {
  RpcOpcode2[RpcOpcode2["Handshake"] = 0] = "Handshake";
  RpcOpcode2[RpcOpcode2["Frame"] = 1] = "Frame";
  RpcOpcode2[RpcOpcode2["Close"] = 2] = "Close";
  RpcOpcode2[RpcOpcode2["Hello"] = 3] = "Hello";
  return RpcOpcode2;
})(RpcOpcode || {});
async function setupHiRpc(_hiRpcVersion) {
  if (typeof window == "undefined") {
    throw new Error("Cannot load hiRPC Module outside of a web environment");
  }
  return new Promise((resolve, reject) => {
    if (typeof window.dso_hirpc == "object") {
      resolve(window.dso_hirpc);
      return;
    }
    import("dso_proxy_bridge/dissonity_hirpc.js").then(() => {
      import("dso_proxy_bridge/dissonity_build_variables.js").then(() => {
        sessionStorage.setItem("dso_needs_prefix", "true");
        sessionStorage.setItem("dso_outside_discord", "false");
        window.dso_hirpc = new window.Dissonity.HiRpc.default();
        clearRpcSessionStorage();
        resolve(window.dso_hirpc);
      }).catch((err) => {
        reject(err);
      });
    }).catch((_) => {
      import("dso_bridge/dissonity_hirpc.js").then(() => {
        import("dso_bridge/dissonity_build_variables.js").then(() => {
          sessionStorage.setItem("dso_needs_prefix", "false");
          sessionStorage.setItem("dso_outside_discord", "true");
          window.dso_hirpc = new window.Dissonity.HiRpc.default();
          clearRpcSessionStorage();
          resolve(window.dso_hirpc);
        }).catch((err) => {
          reject(err);
        });
      }).catch((err) => {
        reject(err);
      });
    });
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
  const confirmedNeedsPrefix = sessionStorage.getItem("dso_needs_prefix") == "true";
  if (confirmedNeedsPrefix && !src.startsWith("./") && !src.startsWith(".proxy/")) {
    src = ".proxy/" + src;
  }
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
