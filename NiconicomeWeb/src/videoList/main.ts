import { DotNetObjectReference } from "../shared/DotNetObjectReference";
import { ElementHandler, ElementHandlerImpl } from "../shared/ElementHandler";
import { StyleHandler, StyleHandlerImpl } from "../shared/StyleHandler";
import { SelectionHandlerImpl } from "./SelectionHandler/selectionHandler";
import { SortHandler, SortHandlerImpl } from "./sortHandler/sortHandler";
import { WidthHandler, WidthHandlerImpl } from "./widthHandler/widthHandler";

export async function initialize(blazorView: DotNetObjectReference, isFirstRender: boolean) {
    const elmHandler: ElementHandler = new ElementHandlerImpl();
    const styleHandler: StyleHandler = new StyleHandlerImpl(elmHandler);
    const widthHandler: WidthHandler = new WidthHandlerImpl(elmHandler, styleHandler, blazorView);
    const sortHandler: SortHandler = new SortHandlerImpl(elmHandler, blazorView)

    if (isFirstRender) {
        await widthHandler.initialize();
    }

    sortHandler.initialize(registeredList);
}

export async function setWidth(blazorView: DotNetObjectReference) {
    const elmHandler: ElementHandler = new ElementHandlerImpl();
    const styleHandler: StyleHandler = new StyleHandlerImpl(elmHandler);
    const widthHandler: WidthHandler = new WidthHandlerImpl(elmHandler, styleHandler, blazorView);

    await widthHandler.setWidth();
}

export function getSelectedIOfInput(): string {
    const elmHandler: ElementHandler = new ElementHandlerImpl();
    const handler = new SelectionHandlerImpl(elmHandler);

    return handler.getSelected();
}

let registeredList: string[] = [];
