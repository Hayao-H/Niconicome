const esbuild = require('esbuild');
const isDevelopment = process.env.MODE === 'development';
require('dotenv').config();

esbuild.build({
    entryPoints: ['./src/videoList/main.ts'],
    bundle: true,
    minify: !isDevelopment,
    sourcemap: isDevelopment ? 'inline' : false,
    outdir: process.env.SCRIPT_DIR,
    target: "es2021",
}).catch((e) => console.error(e));
