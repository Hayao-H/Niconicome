const esbuild = require("esbuild");

esbuild.build({
    entryPoints: ['./src/main.ts'],
    bundle: true,
    minify: false,
    sourcemap: "inline",
    outdir: 'G:\\Projects\\C#\\06_niconicome\\Niconicome\\Niconicome\\bin\\Debug\\net5.0-windows10.0.19041.0\\win-x64\\addons\\5147f547-c273-411c-a8ae-49d4fc6ffa7e\\scripts',
    platform: 'node',
    format: 'iife',
}).catch((e) => console.error(e));
