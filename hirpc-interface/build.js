import fs from "fs";

// Read content
let pluginContent = fs.readFileSync("./build/plugin.js").toString();
let appLoaderContent = fs.readFileSync("./build/app_loader.js").toString();

// Remove export from plugin
pluginContent = pluginContent.replace("export {};\n", "");

// Add disclaimer
const disclaimer = `/*
    This file has been generated from a TypeScript source.
    Don't modify it manually.

    https://github.com/Furnyr/Dissonity/
*/\n\n`;

pluginContent = disclaimer + pluginContent;
appLoaderContent = disclaimer + appLoaderContent;

// Write files
fs.writeFileSync("./build/plugin.js", Buffer.from(pluginContent));
fs.writeFileSync("./build/app_loader.js", Buffer.from(appLoaderContent));