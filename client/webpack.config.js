const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const webpack = require("webpack");
const path = require("path");
const HtmlWebpackPlugin = require("html-webpack-plugin");


var statsConfig = {
	all: false,
	version: false,
	timings: false,
	modules: false,
	modulesSpace: 0,
	assets: false,
	assetsSpace: 0,
	errors: false,
	errorsCount: true,
	warnings: false,
	warningsCount: true,
	logging: "warn"
};

module.exports = env => { return {

  entry: "./src/index.tsx",

  mode: env.dev ? "development" : "production",

  devtool: env.dev ? "inline-source-map" : false,

  stats: statsConfig,

  devServer: {
    // noInfo: true,
    serveIndex: false,

    // stats: "minimal",
    stats: statsConfig,


    port: 32015,
    historyApiFallback: true
  },

  output: {
    path: path.resolve(__dirname, "dist"),
    publicPath: "",

    clean: true,

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
        use: [{
          loader: "ts-loader",
          options: {
            colors: false,
            silent: true,
            errorFormatter: (message, colors) =>
              `${message.file}(${message.line},${message.character}): ${message.severity} TS${message.code}: ${message.content}`
          }
        }],
        exclude: /node_modules/
      },
      // {
      //   test: /\.(png|svg)$/,
      //   use: "file-loader"
      // },
      {
        test: /icons\/.*\.svg$/,
        use: "raw-loader"
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
                plugins: [
                  "autoprefixer"
                ]
              }
            }
          },
          "sass-loader"
        ]
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

    {
      apply(compiler) {
        compiler.hooks.done.tap("fbu-format", c => {
          console.log("");
          c.compilation.errors.map(err => err.message).sort().forEach(msg =>
            console.log(msg));
          console.log("");
        });
      }
    }
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
