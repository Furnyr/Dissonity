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
    const versionJsFile = `./dist/version.js`;
    const versionTsFile = `./dist/version.d.ts`;

    fs.writeFileSync(versionJsFile, `export const version = "${version}";`);
    fs.writeFileSync(versionTsFile, `export declare const version = "${version}";`);

    console.log(chalk.green(`Added ${versionJsFile} successfully!`));
    console.log(chalk.green(`Added ${versionTsFile} successfully!`));
}

main();