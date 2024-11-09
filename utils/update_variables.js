
import prompt from "prompt-sync";
import fs from "fs";
import chalk from "chalk";

const VARIABLE_SEPARATOR = "§"; // alt 21 win

function main() {
    
    //? Build exists
    if (!fs.existsSync("../hirpc/pkg") || !fs.existsSync("../hirpc/pkg/build_variables.js")) {
        console.log(chalk.red("No build found. Run pnpm build first."));
        return;
    }

    const rl = prompt({
        sigint: true
    });
    
    //\ Import variables file
    let buildBuffer = fs.readFileSync("../hirpc/pkg/build_variables.js");

    const variables = {};
    const required = chalk.red("*");
    const hasDefault = chalk.green("[D]");

    //? No CI
    const args = process.argv.slice(2);

    if (args[0] != "ci") {

        console.log(chalk.cyan("Please input your build variables:"));
        console.log(chalk.blue("- Enter to set empty\n- N to not modify\n- D for default value, if applicable\n"));
        
        variables.SDK_VERSION = rl(
            required + chalk.yellow("SDK_VERSION (x.y.z): "));
    
        variables.DISABLE_INFO_LOGS = rl(
            hasDefault + required + chalk.yellow("DISABLE_INFO_LOGS (True/False): "));
    
        variables.CLIENT_ID = rl(
            required + chalk.yellow("CLIENT_ID (snowflake): "));
    
        variables.DISABLE_CONSOLE_LOG_OVERRIDE = rl(
            hasDefault + required + chalk.yellow("DISABLE_CONSOLE_LOG_OVERRIDE (True/False): "));
    
        variables.MAPPINGS = rl(
            chalk.yellow("MAPPINGS (/foo,foo.com,/example,example.com): "));
    
        variables.PATCH_URL_MAPPINGS_CONFIG = rl(
            hasDefault + chalk.yellow("PATCH_URL_MAPPINGS_CONFIG ({ ./src/official_types.d.ts [PatchUrlMappingsConfig] }): "));
    
        variables.OAUTH_SCOPES = rl(
            required + chalk.yellow("OAUTH_SCOPES (identify,guilds): "));
    
        variables.TOKEN_REQUEST_PATH = rl(
            required + chalk.yellow("TOKEN_REQUEST_PATH (/api/token): "));
    
        variables.SERVER_REQUEST = rl(
            chalk.yellow("SERVER_REQUEST ({}): "));
    }

    //? CI
    else {
        variables.SDK_VERSION = "x.y.z";
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
            if (key == "PATCH_URL_MAPPINGS_CONFIG") variables[key] = "{patchFetch:true,patchWebSocket:true,patchXhr:false,patchSrcAttributes:true}";
        }

        buildString = buildString.replace(substring, `[[[ ${key} ]]] ${variables[key]}`);
    }

    buildBuffer = Buffer.from(buildString);

    fs.writeFileSync("../hirpc/pkg/build_variables.js", buildBuffer);

    console.log(chalk.green("\nVariables updated successfully!"));
}

main();