
import { config } from "dotenv";
  config();
import { build } from "esbuild";
import glob from "glob";
import fs from "fs";

const args = process.argv.slice(2);
const clean = args[0] == "clean";

//? Delete build folder
if (clean && fs.existsSync("./build")) {
  fs.rmSync("./build", {
    recursive: true
  });
}

console.log("\n----- Building project... -----\n");

//? Create main folders
if (!fs.existsSync("./build")) {
  fs.mkdirSync("./build");
}

if (!fs.existsSync("./build/client")) {
  fs.mkdirSync("./build/client");
}


//# ENV VARIABLES - - - - -
const entryPoints = glob.globSync("./src/client/**/*.ts");

const define = {};
for (const k in process.env) {

  // Only inject public properties
  if (!k.startsWith("PUBLIC_")) continue;
  
  console.log(`Injecting to .env: ${k}`);
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
//const envBuffer = fs.readFileSync("./.env");
//fs.writeFileSync("./build/.env", envBuffer);

//\ Copy client folder into build
fs.cpSync("./src/client", "./build/client", {
  recursive: true,
  filter: src => {
    if (src.endsWith(".ts")) return false;
    return true;
  }
});

console.log("\n----- Project build ready -----\n");