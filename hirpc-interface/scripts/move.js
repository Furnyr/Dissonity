/*
    - Move the app loader to the Unity resources
    - Move the plugin to the Unity plugins folder
*/

import fs from "fs";
import chalk from "chalk";

function main() {
    
    //? Build exists
    if (!fs.existsSync("./dist") || !fs.existsSync("./dist/plugin.js")) {
        console.log(chalk.red("No build found. Run pnpm build first."));
        return;
    }

    // Sources
    const sourcePath = "./dist/";

    // Targets
    const pluginTargetPath = "../unity/Plugins/"
    const templateTargetPath = "../unity/Resources/WebGLTemplateSource/Dissonity/";

    // Move main files
    fs.renameSync(`${sourcePath}plugin.js`, `${pluginTargetPath}HiRpcPlugin.jslib`);
    console.log(chalk.green(`\nFiles moved to ${pluginTargetPath} successfully!`));

    fs.renameSync(`${sourcePath}app_loader.js`, `${templateTargetPath}app_loader.js.txt`);
    console.log(chalk.green(`Files moved to ${templateTargetPath} successfully!`));
}

main();