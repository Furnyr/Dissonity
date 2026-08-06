/*
    - Add the version file
*/

import fs from "fs";
import chalk from "chalk";
import packageJson from "../package.json" with { type: "json" };

function main() {
    
    //? Build exists
    if (!fs.existsSync("./dist")) {
        console.log(chalk.red("No build found. Run pnpm build first."));
        return;
    }

    // Target
    const versionJsFile = `./dist/version.js`;
    const versionTsFile = `./dist/version.d.ts`;

    fs.writeFileSync(versionJsFile, `export const version = "${packageJson.version}";`);
    fs.writeFileSync(versionTsFile, `export declare const version = "${packageJson.version}";`);

    console.log(chalk.green(`Added ${versionJsFile} successfully!`));
    console.log(chalk.green(`Added ${versionTsFile} successfully!`));
}

main();