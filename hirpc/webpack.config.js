const path = require("path");
const TerserPlugin = require("terser-webpack-plugin");

module.exports = {
  entry: {
    main: {
      import: "./src/index.ts",
      filename: "dissonity_hirpc.js",
      library: {
        name: ["Dissonity", "HiRpc"],
        type: "global"
      }
    },
    buildVariables: {
      import: "./src/modules/build_variables.ts",
      filename: "dissonity_build_variables.js",
      library: {
        name: ["Dissonity", "BuildVariables"],
        type: "global"
      }
    }
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
  },
  output: {
    path: path.resolve(__dirname, "dist"),
  },
  mode: "production",
  optimization: {
    minimize: true,
    minimizer: [
      new TerserPlugin({
        terserOptions: {
          output: {
            quote_style: 3
          },
        },
      }),
    ],
  }
};