var path = require("path");
var apiExtractor = require("@microsoft/api-extractor");
var fs = require("fs");

const apiExtractorJsonPath = path.join(__dirname, '../config/api-extractor.json');
const rootDir = path.join(__dirname, "../");
const jsonName = "page-analyze-plugin.api.json";

if (!fs.existsSync(path.join(rootDir,"etc"))){
    fs.mkdirSync(path.join(rootDir,"etc"));
}
if (!fs.existsSync(path.join(rootDir,"input"))){
    fs.mkdirSync(path.join(rootDir,"input"));
}

// Load and parse the api-extractor.json file
const extractorConfig = apiExtractor.ExtractorConfig.loadFileAndPrepare(apiExtractorJsonPath);

// Invoke API Extractor
const extractorResult = apiExtractor.Extractor.invoke(extractorConfig, {
    // Equivalent to the "--local" command-line parameter
    localBuild: true,

    // Equivalent to the "--verbose" command-line parameter
    showVerboseMessages: true
});

if (!extractorResult.succeeded) {
    console.error(`API Extractor completed with ${extractorResult.errorCount} errors`
        + ` and ${extractorResult.warningCount} warnings`);
    process.exit(1);
}

if (!fs.existsSync(path.join(rootDir, "temp", jsonName))) {
    console.error("*.api.jsonファイルが存在しません。");
    process.exit(1);
}

fs.copyFileSync(path.join(rootDir, "temp", jsonName), path.join(rootDir, "input", jsonName));
fs.rmSync(path.join(rootDir, "dist"), { recursive: true, force: true });
fs.rmSync(path.join(rootDir, "etc"), { recursive: true, force: true });
//fs.rmSync(path.join(rootDir, "temp"), { recursive: true, force: true });


