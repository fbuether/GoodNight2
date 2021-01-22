const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const webpack = require("webpack");
const path = require("path");
const HtmlWebpackPlugin = require("html-webpack-plugin");

module.exports = env => { return {

  entry: "./src/index.tsx",

  mode: env.dev ? "development" : "production",

  devtool: env.dev ? "inline-source-map" : false,

  devServer: {
    // noInfo: true,
    serveIndex: false,
    stats: "minimal",
    port: 32015,
  },

  output: {
    path: path.resolve(__dirname, "dist"),
    publicPath: "",

    filename: function(chunk) {
      return chunk.chunk.name == "main"
        ? "goodnight-[contenthash].js"
        : "[name]-[contenthash].js";
    }
  },

  module: {
    rules: [
      {
        test: /\.tsx?$/,
        loader: "ts-loader",
        exclude: /node_modules/
      },
      {
        test: /\.(png|svg)$/,
        use: "file-loader"
      },
      {
        test: /\.scss$/,
        use: [
          MiniCssExtractPlugin.loader,
          "css-loader",
          {
            loader: "postcss-loader",
            options: {
              postcssOptions: {
                // postcss plugins, can be exported to postcss.config.js
                plugins: function () {
                  return [
                    require("autoprefixer")
                  ];
                }
              }
            }
          },
          "sass-loader"]
      }
    ]
  },

  resolve: {
    extensions: [".ts", ".tsx", ".js"]
  },

  plugins: [
    new HtmlWebpackPlugin({
      template: "src/index.html",
      favicon: "assets/night-sleep-icon.png"
    }),
    new webpack.DefinePlugin({
      "__REACT_DEVTOOLS_GLOBAL_HOOK__": "({ isDisabled: true })"
    }),
    new MiniCssExtractPlugin({
      filename: "[name]-[contenthash].css",
    }),
  ],


  optimization: {
    runtimeChunk: "single",
    splitChunks: {
      chunks: "all",
      minSize: 0,
      cacheGroups: {
        nodeModuleGroup: {
          test: /\/node_modules\//,
          name: function(module) {
            // return npm module name as name.
            return module.context.match(
              /\/node_modules\/(.*?)(\/|$)/)[1];
          }
        }
      }
    }
  },
}};
