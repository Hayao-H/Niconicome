import * as esbuild from "https://deno.land/x/esbuild@v0.19.7/mod.js";
import  "https://deno.land/std@0.222.0/dotenv/load.ts";

const isDevelopment = Deno.args[0] === "development";

console.log(`target directory: ${Deno.env.get("VIDEOLIST_DIR")}`);
console.log(`isDevelopment: ${isDevelopment}`);

try {
  await esbuild.build({
    entryPoints: ["./NiconicomeWeb/src/videoList/main.ts"],
    minify: !isDevelopment,
    bundle: true,
    sourcemap: isDevelopment ? "inline" : false,
    outdir: Deno.env.get('VIDEOLIST_DIR'),
    target: ["edge122"],
    format: "esm",
  });
} catch (e) {
  console.error(e);
  Deno.exit(1);
}

esbuild.stop();
