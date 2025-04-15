"use strict";
var __create = Object.create;
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __getProtoOf = Object.getPrototypeOf;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toESM = (mod, isNodeMode, target) => (target = mod != null ? __create(__getProtoOf(mod)) : {}, __copyProps(
  // If the importer is in node compatibility mode or this is not an ESM
  // file that has been converted to a CommonJS file using a Babel-
  // compatible transform (i.e. "__esModule" has not been set), then set
  // "default" to the CommonJS "module.exports" for node compatibility.
  isNodeMode || !mod || !mod.__esModule ? __defProp(target, "default", { value: mod, enumerable: true }) : target,
  mod
));
var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);

// src/index.ts
var index_exports = {};
__export(index_exports, {
  RpcOpcode: () => RpcOpcode,
  loadIframe: () => loadIframe,
  setupHiRpc: () => setupHiRpc
});
module.exports = __toCommonJS(index_exports);
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
    let skipPrefixCheck = false;
    let useProxyImport = false;
    if (window.location.hostname.endsWith(".discordsays.com")) {
      sessionStorage.setItem("dso_outside_discord", "false");
    } else {
      skipPrefixCheck = true;
      sessionStorage.setItem("dso_outside_discord", "true");
    }
    if (skipPrefixCheck || window.location.pathname.startsWith("/.proxy")) {
      sessionStorage.setItem("dso_needs_prefix", "false");
    } else {
      useProxyImport = true;
      sessionStorage.setItem("dso_needs_prefix", "true");
    }
    if (useProxyImport) {
      tryProxyImport().then((module2) => {
        resolve(module2);
      }).catch(() => {
        tryDirectImport().then((module2) => {
          resolve(module2);
        }).catch((err) => {
          reject(err);
        });
      });
    } else {
      tryDirectImport().then((module2) => {
        resolve(module2);
      }).catch(() => {
        tryProxyImport().then((module2) => {
          resolve(module2);
        }).catch((err) => {
          reject(err);
        });
      });
    }
    function tryProxyImport() {
      return new Promise((resolve2, reject2) => {
        import("dso_proxy_bridge/dissonity_hirpc.js").then(() => {
          import("dso_proxy_bridge/dissonity_build_variables.js").then(() => {
            sessionStorage.setItem("dso_needs_prefix", "true");
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
    function tryDirectImport() {
      return new Promise((resolve2, reject2) => {
        import("dso_bridge/dissonity_hirpc.js").then(() => {
          import("dso_bridge/dissonity_build_variables.js").then(() => {
            sessionStorage.setItem("dso_needs_prefix", "false");
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
// Annotate the CommonJS export names for ESM import in node:
0 && (module.exports = {
  RpcOpcode,
  loadIframe,
  setupHiRpc
});
