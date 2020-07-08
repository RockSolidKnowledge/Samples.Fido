const path = require("path");
const webpack = require("webpack");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");


module.exports = {

    target: "web",

    resolve: {
        // Add ".ts" and ".tsx" as resolvable extensions.
        extensions: [".ts", ".tsx", ".js", ".json", ".html"],
    },

    module: {
        rules: [
            // All files with a ".ts" or ".tsx" extension will be handled by "awesome-typescript-loader".
            { test: /.ts$/, loader: "ts-loader" },

            // All output ".js" files will have any sourcemaps re-processed by "source-map-loader".
            { enforce: "pre", test: /\.js$/, loader: "source-map-loader" }
        ]
    },

    plugins: ([
        // make sure we allow any jquery usages outside of our webpack modules
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            "window.jQuery": "jquery"
        }),

        // Clean dist folder.
        new CleanWebpackPlugin({
            "verbose": true // Write logs to console.
        }),

    ]),

    // pretty terminal output
    stats: { colors: true },

    // Building mode
    mode: 'development',

    // Entry point of the application
    entry: './ts/authenticate.ts',

    output: {
        // Target application
        path: path.resolve(__dirname, 'dist'),
        filename: 'bundle.js',
        // Defining a global var that can used to call functions from within ASP.NET Razor pages.
        library: "aspAndWebpack",
        libraryTarget: "var"
    },

    devtool: "source-map"

};