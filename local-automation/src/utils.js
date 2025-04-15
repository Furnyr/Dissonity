
const fs = require("fs");
const chalk = require("chalk");

module.exports.getLocalPath = () => {

    //? Path set in .env
    if (!process.env.UNITY_PROJECT_PATH || process.env.UNITY_PROJECT_PATH == "") {
        console.log(chalk.red("Unity project path not found. Please set it in a .env file."));
        return;
    }

    //? Valid path
    if (!fs.existsSync(process.env.UNITY_PROJECT_PATH)) {
        console.log(process.env.UNITY_PROJECT_PATH)
        console.log(chalk.yellow("Unity project path is set in .env, but it wasn't found. Please check if the path is correct."));
        return;
    }

    const pathToAssets = `${process.env.UNITY_PROJECT_PATH}/Assets`;

    //? Valid path
    if (!fs.existsSync(pathToAssets)) {
        console.log(chalk.yellow("Unity project path is set in .env, but there's no Assets folder within it."));
        return;
    }

    const pathToLocal = `${pathToAssets}/LocalDissonity`;

    //? Valid path
    if (!fs.existsSync(pathToLocal)) {
        console.log(chalk.yellow("Please create a folder named LocalDissonity inside your Assets folder."));
        return;
    }

    return pathToLocal;
};