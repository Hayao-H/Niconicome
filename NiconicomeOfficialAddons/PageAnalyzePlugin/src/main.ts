import { Response } from "../@types/net/http/fetch/Response";
import { OutputHandler } from "../lib/output/output";

main();
async function main() {

    const logger = new OutputHandler();
    if (logger.canUse) {
        logger.write("Hello World!!");

        let res: Response;
        let content= "";

        try {
            res = await fetch("http://nicovideo.jp/");
            content = await res.text();
        } catch (ex) {
            logger.write(ex.message);
        }


    }
}

