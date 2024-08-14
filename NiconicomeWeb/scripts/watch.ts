import * as esbuild from "https://deno.land/x/esbuild@v0.19.7/mod.js";
import "https://deno.land/std@0.222.0/dotenv/load.ts";
import { denoPlugins } from "https://deno.land/x/esbuild_deno_loader@0.9.0/mod.ts";

const isDevelopment = Deno.args[0] === "development";
const isWatch = Deno.args[0] === "watch";

console.log(`target directory: ${Deno.env.get("VIDEOLIST_DIR")}`);
console.log(`isDevelopment: ${isDevelopment}`);
console.log(`isWatch: ${isWatch}`);

let context: esbuild.BuildContext | undefined;

async function createContext() {
  return await esbuild.context({
    plugins: [...denoPlugins(), {
      name: "watcher",
      setup(build) {
        let count = 0;
        build.onEnd((result) => {
          if (result.errors.length > 0) {
            console.error("Build Error!!");
           watch();
          } else if (count === 0) {
            count++;
            console.log("First build is done. Watching files");
          } else {
            count++;
            console.log("Rebuild: ", count);
          }
        });
      },
    }],
    entryPoints: ["./NiconicomeWeb/src/watch/ui/main.tsx"],
    minify: false,
    bundle: true,
    sourcemap: "inline",
    outdir: Deno.env.get("DETAIL_DIR"),
    target: ["edge122"],
    format: "esm",
  });
}

async function watch() {
  if (context) {
    context.dispose();
    context = undefined;
    setTimeout(() => {
      console.log("Restart watching...")
      watch();
    }, 5000);
    return;
  }
  context = await createContext();
  await context.watch();
}

if (isWatch) {
  Deno.addSignalListener("SIGINT", () => {
    if (context) context.dispose();
    console.log("Good Bye!");
    Deno.exit();
  });

  watch();
} else {
  try {
    await esbuild.build({
      plugins: [...denoPlugins()],
      entryPoints: ["./NiconicomeWeb/src/watch/ui/main.tsx"],
      minify: !isDevelopment,
      bundle: true,
      sourcemap: isDevelopment ? "inline" : false,
      outdir: Deno.env.get("DETAIL_DIR"),
      target: ["edge122"],
      format: "esm",
    });
  } catch (e) {
    console.error(e);
    Deno.exit(1);
  }

  esbuild.stop();
}
