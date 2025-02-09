import fs from "fs";
import chalk from "chalk";

function main() {

    // Read content
    let pluginContent = fs.readFileSync("./dist/plugin.js").toString();
    let appLoaderContent = fs.readFileSync("./dist/app_loader.js").toString();
    
    // Remove export from plugin
    pluginContent = pluginContent.replace("export {};\n", "");

    // Add dissonity channel to plugin
    pluginContent = pluginContent.replace(/DISSONITY_CHANNEL/g, '"dissonity"');
    
    // Add disclaimer
    const disclaimer = `/*
        This file has been generated from a TypeScript source.
        Don't modify it manually.
    
        https://github.com/Furnyr/Dissonity/
*/\n\n`;
    
    pluginContent = disclaimer + pluginContent;
    appLoaderContent = disclaimer + appLoaderContent;
    
    // Write files
    fs.writeFileSync("./dist/plugin.js", Buffer.from(pluginContent));
    fs.writeFileSync("./dist/app_loader.js", Buffer.from(appLoaderContent));

    console.log(chalk.green("Build finished successfully!"));
}

main();