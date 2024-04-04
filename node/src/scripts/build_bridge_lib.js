
const { build } = require("esbuild");
const glob = require("glob");
const fs = require("fs");

const entryPoints = glob.globSync("./src/client/_unity_bridge/*.ts");

//? build
if (!fs.existsSync("./_unity_structures")) {
    fs.mkdirSync("./_unity_structures");
}

//# BRIDGE LIBRARY - - - - -
// Build into ./_unity_structures
build({
    bundle: true,
    entryPoints,
    outbase: './src/client/_unity_bridge',
    outdir: './_unity_structures',
});

console.log("BridgeLib compiled to _unity_structures/BridgeLibrary.js");