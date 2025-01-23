/*
    - Add the version file
*/

const fs = require("fs");
const chalk = require("chalk");
const { version } = require("../package.json");

function main() {
    
    //? Build exists
    if (!fs.existsSync("./dist")) {
        console.log(chalk.red("No build found. Run pnpm build first."));
        return;
    }

    // Target
    const targetFile = `./dist/version.json`;

    fs.writeFileSync(targetFile, JSON.stringify({
        hirpc: version
    }, null, 4));

    console.log(chalk.green(`Added ${targetFile} successfully!`));
}

main();