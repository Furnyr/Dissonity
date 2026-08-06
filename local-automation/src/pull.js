/*
    - Move the Unity folder in the repository to your local Unity project
*/

import fs from "fs";
import chalk from "chalk";
import "dotenv/config";
import prompt from "prompt-sync";
import { getLocalPath } from "./utils.js";

function main() {

    const pathToLocal = getLocalPath();
    if (!pathToLocal) return;

    const rl = prompt({
        sigint: true
    });
    
    const proceed = rl("[ " + chalk.red("LocalDissonity") + " <- " + chalk.green("unity (repo)") + " ] Do you want to overwrite your local project with the repository contents? (Y/n): ");

    if (proceed.toLowerCase() != "y" && proceed.length != 0) {
        console.log("\nOperation canceled.");
        return;
    }

    // Sources
    const sourcePath = "../unity";

    //\ Clean up target folder
    fs.rmSync(pathToLocal, {
        recursive: true
    });

    fs.mkdirSync(pathToLocal);

    //\ Copy Unity into target
    fs.cpSync(sourcePath, pathToLocal, {
      recursive: true
    });

    console.log(chalk.green(`\nUnity files in the repository moved to ${pathToLocal} successfully!`));
}

main();