import { DotNetObjectReference } from "../shared/DotNetObjectReference";
import { ElementHandler, ElementHandlerImpl } from "../shared/ElementHandler";
import { StyleHandler, StyleHandlerImpl } from "../shared/StyleHandler";
import { SortHandler, SortHandlerImpl } from "./sortHandler/sortHandler";
import { WidthHandler, WidthHandlerImpl } from "./widthHandler/widthHandler";

export async function initialize(blazorView: DotNetObjectReference) {
    const elmHandler: ElementHandler = new ElementHandlerImpl();
    const styleHandler: StyleHandler = new StyleHandlerImpl(elmHandler);
    const widthHandler: WidthHandler = new WidthHandlerImpl(elmHandler, styleHandler, blazorView);
    const sortHandler: SortHandler = new SortHandlerImpl(elmHandler, blazorView)

    await widthHandler.initialize();
    sortHandler.initialize();
}

export async function setWidth(blazorView: DotNetObjectReference) {
    const elmHandler: ElementHandler = new ElementHandlerImpl();
    const styleHandler: StyleHandler = new StyleHandlerImpl(elmHandler);
    const widthHandler: WidthHandler = new WidthHandlerImpl(elmHandler, styleHandler, blazorView);

    await widthHandler.setWidth();
}
