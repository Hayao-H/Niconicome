import { DotNetObjectReference } from "../shared/DotNetObjectReference";
import { ElementHandler, ElementHandlerImpl } from "../shared/ElementHandler";
import { SortHandler, SortHandlerImpl } from "./sortHandler/sortHandler";
import { WidthHandler, WidthHandlerImpl } from "./widthHandler/widthHandler";

export function main(blazorView: DotNetObjectReference) {
    const elmHandler: ElementHandler = new ElementHandlerImpl();
    const widthHandler: WidthHandler = new WidthHandlerImpl(elmHandler);
    const sortHandler: SortHandler = new SortHandlerImpl(elmHandler, blazorView)

    widthHandler.Initialize();
    sortHandler.Initialize();
}