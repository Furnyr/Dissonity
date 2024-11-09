
require("dotenv").config();
const { build } = require("esbuild");
const glob = require("glob");
const fs = require("fs");

console.log("\n----- Building project -----");

const entryPoints = glob.globSync("./src/client/**/*.ts");

//# CLIENT TYPESCRIPT - - - - -
//? build
if (!fs.existsSync("./build")) {
  fs.mkdirSync("./build");
}

//? build/client
if (!fs.existsSync("./build/client")) {
  fs.mkdirSync("./build/client");
}

// Inject .env vars
const define = {};
for (const k in process.env) {

  //\ Only inject public properties
  if (!k.startsWith("PUBLIC_")) continue;
  
  console.log(` Injecting .env: ${k}`);
  define[`process.env.${k}`] = JSON.stringify(process.env[k]);
}

// Build into ./build
build({
  bundle: true,
  entryPoints,
  outbase: './src/client',
  outdir: './build/client',
  platform: 'browser',
  external: [],
  define,
});


//# OTHER FILES - - - - -
// .env
const envBuffer = fs.readFileSync("./.env");
fs.writeFileSync("./build/.env", envBuffer);

// client/index.html
const htmlBuffer = fs.readFileSync("./src/client/index.html");
fs.writeFileSync("./build/client/index.html", htmlBuffer);

// client/discord.check.js
const checkBuf = fs.readFileSync("./src/client/discord.check.js");
fs.writeFileSync("./build/client/discord.check.js", checkBuf);

//? client files
if (!fs.existsSync("./build/client/Files")) {
  fs.mkdirSync("./build/client/Files");
}

//? client build
if (!fs.existsSync("./build/client/Files/Build")) {
  fs.mkdirSync("./build/client/Files/Build");
}

//? client scripts
if (!fs.existsSync("./build/client/Files/Scripts")) {
  fs.mkdirSync("./build/client/Files/Scripts");
}

/*const nestedHtmlBuffer = fs.readFileSync("./src/client/Files/iframe_index.html");
fs.writeFileSync("./build/client/Files/iframe_index.html", nestedHtmlBuffer);*/

const rpcBridgeBf = fs.readFileSync("./src/client/Files/Scripts/rpc_bridge.js");
fs.writeFileSync("./build/client/Files/Scripts/rpc_bridge.js", rpcBridgeBf);

const officialUtilsBf = fs.readFileSync("./src/client/Files/Scripts/official_utils.js");
fs.writeFileSync("./build/client/Files/Scripts/official_utils.js", officialUtilsBf);

for (const fileName of fs.readdirSync("./src/client/Files/Build")) {

  const fileBuffer = fs.readFileSync(`./src/client/Files/Build/${fileName}`);
  fs.writeFileSync(`./build/client/Files/Build/${fileName}`, fileBuffer);
}

console.log("Other files have been included in the build folder");
console.log("----- Project build ready -----\n");