/*
    - Move the Unity folder in the repository to your local Unity project
*/

const fs = require("fs");
const chalk = require("chalk");
const { config } = require("dotenv");
const prompt = require("prompt-sync");
const { getLocalPath } = require("./utils.js");

config();

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