/*
    - Move your local files to the Unity folder in the repository
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

    const proceed = rl("[ " + chalk.red("unity (repo)") + " <- " + chalk.green("LocalDissonity") + " ] Do you want to overwrite the repository with your local files? (Y/n): ");

    if (proceed.toLowerCase() != "y" && proceed.length != 0) {
        console.log("\nOperation canceled.");
        return;
    }

    // Sources
    const repoFolder = "../unity";

    //\ Clean up repository folder
    fs.rmSync(repoFolder, {
        recursive: true
    });

    fs.mkdirSync(repoFolder);

    //\ Copy target into repository
    fs.cpSync(pathToLocal, repoFolder, {
      recursive: true
    });

    //\ Update README.md
    fs.copyFileSync("../README.md", `${repoFolder}/README.md`);

    console.log(chalk.green(`\nLocal files in your Unity project moved to ${repoFolder} successfully!`));
}

main();