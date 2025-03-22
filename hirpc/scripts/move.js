/*
    - Move build files to the Unity WebGL Template
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
    const templateTargetPath = "../unity/Editor/Assets/Template/Dissonity/Bridge/";

    // Move main files
    fs.renameSync(`${sourcePath}dissonity_hirpc.js`, `${templateTargetPath}dissonity_hirpc.js.txt`);
    fs.renameSync(`${sourcePath}dissonity_build_variables.js`, `${templateTargetPath}dissonity_build_variables.js.txt`);
    fs.renameSync(`${sourcePath}version.js`, `${templateTargetPath}version.js.txt`);
    fs.renameSync(`${sourcePath}version.d.ts`, `${templateTargetPath}version.d.ts.txt`);
    console.log(chalk.green(`Files moved to ${templateTargetPath} successfully!`));
}

main();