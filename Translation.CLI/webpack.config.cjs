const path = require('path');

module.exports = {
  mode: 'production', // or 'development' for easier debugging
  entry: './dist/browser/index.js', // Ensure this is the correct entry point
  output: {
    path: path.resolve(__dirname, 'dist/bundle'), // Output directory for your bundle
    filename: 'audio-translation-service-client.js', // Name of the bundled file
    library: {
      name: 'AudioTranslationServiceClient', // Name of the global variable to export
      type: 'umd'
    },
    globalObject: 'this' // Ensures the library is accessible in different environments
  },
  resolve: {
    extensions: ['.js', '.json', '.ts', '.tsx'] // Include TypeScript extensions if needed
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: ['@babel/preset-env'],
            plugins: ['@babel/plugin-transform-runtime']
          }
        }
      },
      {
        test: /\.tsx?$/, // Add rule for TypeScript files
        use: 'ts-loader',
        exclude: /node_modules/
      }
    ]
  },
  externals: [
    // Add any libraries or dependencies that you don't want to bundle
  ]
};