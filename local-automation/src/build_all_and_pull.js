/*
    - Move the Unity folder in the repository to your local Unity project
*/

const chalk = require("chalk");
const { config } = require("dotenv");
const { getLocalPath } = require("./utils.js");

config();

function main() {

    const pathToLocal = getLocalPath();
    if (!pathToLocal) return;

    console.log(chalk.blueBright("Attempting to build all hiRPC-related files and move them to your local project..."));
}

main();