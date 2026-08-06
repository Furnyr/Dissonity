import chalk from "chalk";
import "dotenv/config";
import { getLocalPath } from"./utils.js";

function main() {

    const pathToLocal = getLocalPath();
    if (!pathToLocal) return;

    console.log(chalk.blueBright("Attempting to build all hiRPC-related files and move them to your local project..."));
}

main();