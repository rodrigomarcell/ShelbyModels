const { BundleAnalyzerPlugin } = require('webpack-bundle-analyzer');
const WebpackBar = require('webpackbar');
const CracoAlias = require('craco-alias');

process.env.BROWSER = 'none';

module.exports = {
  webpack: {
    plugins: [
      new WebpackBar({ profile: true }),
      ...(process.env.NODE_ENV === 'development' ? [new BundleAnalyzerPlugin({ openAnalyzer: false })] : []),
    ],
    configure: (webpackConfig) => {
      webpackConfig.watchOptions = {
        aggregateTimeout: 300,
        poll: 1000,
      };
      return webpackConfig;
    },
  },
  plugins: [
    {
      plugin: CracoAlias,
      options: {
        source: 'tsconfig',
        baseUrl: './src/',
        tsConfigPath: './tsconfig.paths.json',
      },
    },
  ],
};
