
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


//# NESTED HTML CONFIG - - - - -
//? Unity build found
if (!fs.existsSync("./src/client/nested/index.html") || !fs.existsSync("./src/client/nested/Build")) {
  throw "[ ❌ ] No Unity build found inside src/client/nested";
}

//? Template data found
if (fs.existsSync("./src/client/nested/TemplateData")) {
  throw "[ ❌ ] This base project only supports the Unity 'WebGL Template' set to 'Minimal'";
}

let htmlString = fs.readFileSync("./src/client/nested/index.html").toString();

//# RESOLUTION SET ALGORITHM - - - - -
if (htmlString.includes("px; background:")) {

  const startIndex = htmlString.indexOf("width:");
  const endIndex = htmlString.indexOf("; background:");
  const replaceSubstring = htmlString.substring(startIndex, endIndex);

  htmlString = htmlString.replace(replaceSubstring, "width: 100vw; height: 100vh")
}

//# COLOR RESET ALGORITHM
if (!htmlString.includes("background: #000000")) {

  const startIndex = htmlString.indexOf("; background:");
  const endIndex = startIndex + 21;
  const replaceSubstring = htmlString.substring(startIndex, endIndex);

  htmlString = htmlString.replace(replaceSubstring, "; background: #000000")
}

//? Unity instance
const tab = "  ";
if (!htmlString.includes("var unityInstance;")) {

  // (Initialize var)
  htmlString = htmlString.replace("createUnityInstance", `var unityInstance;\n`
    + `${tab}${tab}${tab}createUnityInstance`);
  
  // (Set var)
  htmlString = htmlString.replace("})", "}).then(instance => {\n"
    + `${tab}${tab}${tab}${tab}unityInstance = instance;\n`
    + `${tab}${tab}${tab}})`);
}

fs.writeFileSync("./src/client/nested/index.html", htmlString);

console.log("Nested HTML configuration ready");


//# OTHER FILES - - - - -
// .env
const envBuffer = fs.readFileSync("./.env");
fs.writeFileSync("./build/.env", envBuffer);

// client/index.html
const htmlBuffer = fs.readFileSync("./src/client/index.html");
fs.writeFileSync("./build/client/index.html", htmlBuffer);

// client/nested/index.html
//? nested
if (!fs.existsSync("./build/client/nested")) {
  fs.mkdirSync("./build/client/nested");
}

const nestedHtmlBuffer = fs.readFileSync("./src/client/nested/index.html");
fs.writeFileSync("./build/client/nested/index.html", nestedHtmlBuffer);

// client/nested/Build
//? nested/Build
if (!fs.existsSync("./build/client/nested/Build")) {
  fs.mkdirSync("./build/client/nested/Build");
}

for (const fileName of fs.readdirSync("./src/client/nested/Build")) {

  const fileBuffer = fs.readFileSync(`./src/client/nested/Build/${fileName}`);
  fs.writeFileSync(`./build/client/nested/Build/${fileName}`, fileBuffer);
}

console.log("Other files have been included in the build folder");
console.log("----- Project build ready -----\n");