// src/index.ts
async function setupHiRpc(_hiRpcVersion) {
  return new Promise((resolve, reject) => {
    if (typeof window.dso_hirpc == "object") {
      resolve(window.dso_hirpc);
    }
    import("dso_proxy_bridge/dissonity_hirpc.js").then(() => {
      import("dso_proxy_bridge/dissonity_build_variables.js").then(() => {
        window.dso_hirpc = new window.Dissonity.HiRpc.default();
        resolve(window.dso_hirpc);
      }).catch((err) => {
        reject(err);
      });
    }).catch((_) => {
      import("dso_bridge/dissonity_hirpc.js").then(() => {
        import("dso_bridge/dissonity_build_variables.js").then(() => {
          window.dso_hirpc = new window.Dissonity.HiRpc.default();
          resolve(window.dso_hirpc);
        }).catch((err) => {
          reject(err);
        });
      }).catch((err) => {
        reject(err);
      });
    });
  });
}
export {
  setupHiRpc
};
