const esbuild = require('esbuild');
const isDevelopment = process.env.MODE === 'development';
require('dotenv').config();

esbuild.build({
    entryPoints: ['./src/videoList/main.ts'],
    bundle: true,
    minify: !isDevelopment,
    sourcemap: isDevelopment ? 'inline' : false,
    outdir: process.env.VIDEOLIST_DIR,
    target: "es2021",
    format:"esm",
}).catch((e) => console.error(e));
