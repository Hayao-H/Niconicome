import { OutputHandler } from "./lib/output/output";

const logger = new OutputHandler();

if (logger.canUse) {
    logger.write("Hello World!!");
    const dt = new Date();
    logger.write(dt.toString());
    log();
}

function log() {
    logger.write("Hello!!");

    setTimeout(() => {
        log();
    }, 5 * 1000);
}
