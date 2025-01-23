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
  setupHiRpc: () => setupHiRpc
});
module.exports = __toCommonJS(index_exports);
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
// Annotate the CommonJS export names for ESM import in node:
0 && (module.exports = {
  setupHiRpc
});
