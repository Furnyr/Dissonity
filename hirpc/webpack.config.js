import path from "path";
import { fileURLToPath } from "url";
import TerserPlugin from "terser-webpack-plugin";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

export default {
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
    extensionAlias: {
      ".js": [".ts"]
    }
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