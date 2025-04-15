/*
    - Update build variables in the dist or ci folder
*/

const prompt = require("prompt-sync");
const fs = require("fs");
const chalk = require("chalk");

const VARIABLE_SEPARATOR = "§"; // (alt 21 win) (\u00A7)

function main() {

    const args = process.argv.slice(2);
    const ci = args[0] == "ci";

    const path = ci
        ? "./ci/src/modules/build_variables.js"
        : "./dist/dissonity_build_variables.js";
    
    //? Build exists
    if (!fs.existsSync(path)) {

        if (ci) {
            console.log(chalk.red("No build found. Run pnpm tsc -p ci.tsconfig.json first."));
        }

        else {
            console.log(chalk.red("No build found. Run pnpm build first."));
        }
        return;
    }

    const rl = prompt({
        sigint: true
    });
    
    //\ Import variables file
    let buildBuffer = fs.readFileSync(path);

    const variables = {};
    const required = chalk.red("*");
    const hasDefault = chalk.green("[D]");

    //? No CI
    if (!ci) {

        console.log(chalk.cyan("Please input your build variables:"));
        console.log(chalk.blue("- Enter to set empty\n- N to not modify\n- D for default value, if applicable\n"));
        
        variables.DISABLE_INFO_LOGS = rl(
            hasDefault + required + chalk.yellow("DISABLE_INFO_LOGS (True/False): "));
    
        variables.CLIENT_ID = rl(
            required + chalk.yellow("CLIENT_ID (snowflake): "));
    
        variables.DISABLE_CONSOLE_LOG_OVERRIDE = rl(
            hasDefault + required + chalk.yellow("DISABLE_CONSOLE_LOG_OVERRIDE (True/False): "));
    
        variables.MAPPINGS = rl(
            chalk.yellow('MAPPINGS [{"prefix":"/foo","target":"foo.com"}]: '));
    
        variables.PATCH_URL_MAPPINGS_CONFIG = rl(
            hasDefault + chalk.yellow(`PATCH_URL_MAPPINGS_CONFIG {"patchFetch":true...} ${chalk.blueBright("see src/official_types.d.ts")}: `));
    
        variables.OAUTH_SCOPES = rl(
            required + chalk.yellow("OAUTH_SCOPES (identify,guilds): "));
    
        variables.TOKEN_REQUEST_PATH = rl(
            required + chalk.yellow("TOKEN_REQUEST_PATH (/api/token): "));
    
        variables.SERVER_REQUEST = rl(
            chalk.yellow("SERVER_REQUEST ({}): "));
    }

    //? CI
    else {
        variables.DISABLE_INFO_LOGS = "d";
        variables.CLIENT_ID = "123456789987654321";
        variables.DISABLE_CONSOLE_LOG_OVERRIDE = "d";
        variables.MAPPINGS = "";
        variables.PATCH_URL_MAPPINGS_CONFIG = "d";
        variables.OAUTH_SCOPES = "identify";
        variables.TOKEN_REQUEST_PATH = "/api/token";
        variables.SERVER_REQUEST = "";
    }

    // Update algorithm
    let buildString = buildBuffer.toString();
    for (const key of Object.keys(variables)) {

        if (variables[key].toLowerCase() == "n") continue;

        let index = buildString.indexOf(`[[[ ${key} ]]]`);
        let endIndex = buildString.indexOf(VARIABLE_SEPARATOR, index);
        let substring = buildString.substring(index, endIndex);

        //? Default value
        if (variables[key].toLowerCase() == "d") {
            if (key == "DISABLE_INFO_LOGS") variables[key] = "False";
            if (key == "DISABLE_CONSOLE_LOG_OVERRIDE") variables[key] = "True";
            if (key == "PATCH_URL_MAPPINGS_CONFIG") variables[key] = '{"patchFetch":true,"patchWebSocket":true,"patchXhr":true,"patchSrcAttributes":false}';
        }

        buildString = buildString.replace(substring, `[[[ ${key} ]]] ${variables[key]}`);
    }

    buildBuffer = Buffer.from(buildString);

    fs.writeFileSync(path, buildBuffer);

    console.log(chalk.green("\nVariables updated successfully!"));
}

main();