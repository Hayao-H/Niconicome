import { ElementHandler, ElementHandlerImpl } from "../shared/ElementHandler";
import { WidthHandler, WidthHandlerImpl } from "./widthHandler/widthHandler";

export function main() {
    console.log("Hello World!!");

    const elmHandler: ElementHandler = new ElementHandlerImpl();
    const widthHandler: WidthHandler = new WidthHandlerImpl(elmHandler);

    widthHandler.Initialize();

}