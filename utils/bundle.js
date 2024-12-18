/*
pnpm bundle:

- Build utils
- Build hiRPC
- Build hiRPC interface
- Move to the Unity resources
*/

import fs from "fs";
import chalk from "chalk";

const VARIABLE_SEPARATOR = "§"; // alt 21 win

function main() {
    
    //? Build exists
    if (!fs.existsSync("../hirpc/pkg") || !fs.existsSync("../hirpc/pkg/build_variables.js")) {
        console.log(chalk.red("No build found. Run pnpm build first."));
        return;
    }

    //? Rust build exists
    if (!fs.existsSync("../hirpc/pkg/dissonity_hirpc.js")) {
        console.log(chalk.red("No Rust build found. Compile the Rust code first in ../hirpc"));
        return;
    }

    // Sources
    const baseHirpcSourcePath = "../hirpc/pkg/";
    const baseInterfaceSourcePath = "../hirpc-interface/build/"

    // Targets
    const baseHirpcTargetPath = "../unity/Resources/WebGLTemplateSource/Dissonity/Files/Bridge/";
    const basePluginsTargetPath = "../unity/Plugins/"
    const baseTemplateTargetPath = "../unity/Resources/WebGLTemplateSource/Dissonity/";

    // Move main files
    fs.renameSync(`${baseHirpcSourcePath}dissonity_hirpc.js`, `${baseHirpcTargetPath}dissonity_hirpc.js.txt`);
    fs.renameSync(`${baseHirpcSourcePath}dissonity_hirpc_bg.wasm`, `${baseHirpcTargetPath}dissonity_hirpc_bg.wasm.txt`);
    fs.renameSync(`${baseHirpcSourcePath}build_variables.js`, `${baseHirpcTargetPath}build_variables.js.txt`);
    fs.renameSync(`${baseInterfaceSourcePath}plugin.js`, `${basePluginsTargetPath}HiRpcPlugin.jslib`);
    fs.renameSync(`${baseInterfaceSourcePath}app_loader.js`, `${baseTemplateTargetPath}app_loader.js.txt`);
    
    // Make utils into .txt
    const hirpcFolder = fs.readdirSync(`${baseHirpcSourcePath}snippets`)[0];
    const relativePathToUtils = `snippets/${hirpcFolder}/pkg/`;
    const pathToUtils = `${baseHirpcSourcePath}${relativePathToUtils}`;

    fs.renameSync(`${pathToUtils}utils.js`, `${pathToUtils}utils.js.txt`);
    
    //? Folder doesn't exist
    if (!fs.existsSync(`${baseHirpcTargetPath}snippets`)) {
        fs.renameSync(`${baseHirpcSourcePath}snippets`, `${baseHirpcTargetPath}snippets`);
    }

    //? Just rename file
    else {
        fs.renameSync(`${pathToUtils}utils.js.txt`, `${baseHirpcTargetPath}${relativePathToUtils}utils.js.txt`);
    }

    console.log(chalk.green("\nFiles moved to ../unity/Plugins successfully!"));
    console.log(chalk.green("Files moved to ../unity/Resources/WebGLTemplateSource/Dissonity successfully!"));
}

main();