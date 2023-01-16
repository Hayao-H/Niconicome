import { ElementHandler, ElementHandlerImpl } from "../shared/ElementHandler";
import { WidthHandler, WidthHandlerImpl } from "./widthHandler/widthHandler";

declare global {
    interface Window {
        main: () => void;
    }
}

window.main = main;

function main() {
    console.log("Hello World!!");

    const elmHandler: ElementHandler = new ElementHandlerImpl();
    const widthHandler: WidthHandler = new WidthHandlerImpl(elmHandler);

    widthHandler.Initialize();

}