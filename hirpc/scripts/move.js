/*
    - Move build files to the Unity resources
*/

const fs = require("fs");
const chalk = require("chalk");

function main() {
    
    //? Build exists
    if (!fs.existsSync("./dist") || !fs.existsSync("./dist/dissonity_hirpc.js")) {
        console.log(chalk.red("No build found. Run pnpm build first."));
        return;
    }

    // Sources
    const sourcePath = "./dist/";

    // Targets
    const templateTargetPath = "../unity/Resources/WebGLTemplateSource/Dissonity/Bridge/";

    // Move main files
    fs.renameSync(`${sourcePath}dissonity_hirpc.js`, `${templateTargetPath}dissonity_hirpc.js.txt`);
    fs.renameSync(`${sourcePath}dissonity_build_variables.js`, `${templateTargetPath}dissonity_build_variables.js.txt`);
    fs.renameSync(`${sourcePath}version.json`, `${templateTargetPath}version.json.txt`);
    console.log(chalk.green(`Files moved to ${templateTargetPath} successfully!`));
}

main();