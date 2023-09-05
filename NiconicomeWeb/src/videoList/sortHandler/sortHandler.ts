import { DotNetObjectReference } from "../../shared/DotNetObjectReference";
import { ElementHandler, ElementHandlerImpl } from "../../shared/ElementHandler";
import { ElementIDs } from "./elementIDs";

export interface SortHandler {
    initialize(registeredList: string[]): void;
}

export class SortHandlerImpl implements SortHandler {

    constructor(elmHandler: ElementHandler, dotnetHelper: DotNetObjectReference) {
        this._elmHandler = elmHandler;
        this._dotnetHelper = dotnetHelper;
    }

    private readonly _elmHandler: ElementHandler;

    private readonly _dotnetHelper: DotNetObjectReference;

    private _sourceNiconicoID: string | null = null;

    private _sourceID: string | null = null;

    private _lastOverElement: HTMLElement | null = null;

    public initialize(registeredList: string[]): void {
        const rowResult = this._elmHandler.GetAll(ElementIDs.VideoListRow);
        if (!rowResult.IsSucceeded || rowResult.Data === null) {
            return;
        }

        rowResult.Data.forEach(elm => {

            if (elm instanceof HTMLElement) {

                const niconicoID = elm.dataset['niconicoid'];
                const playlistID = elm.dataset['playlistid'];

                if (niconicoID === undefined || playlistID === undefined) {
                    return;
                }

                const key: string = `${niconicoID}-${playlistID}`;
                if (registeredList.includes(key)) {
                    return;
                } else {
                    registeredList.push(key);
                }

                elm.addEventListener('dragstart', e => {
                    if (!(e.target instanceof HTMLElement)) {
                        return;
                    }

                    const row: HTMLElement | null = this.GetParentByClassName(e.target, ElementIDs.VideoListRowClassName);
                    if (row === null) {
                        return;
                    }

                    this._sourceNiconicoID = row.dataset['niconicoid'] ?? null;
                    this._sourceID = row.id;
                });

                elm.addEventListener('dragover', e => {
                    e.preventDefault();
                    if (!(e.target instanceof HTMLElement)) {
                        return;
                    }

                    const row: HTMLElement | null = this.GetParentByClassName(e.target, ElementIDs.VideoListRowClassName);
                    if (row === null) {
                        return;
                    }

                    if (!row.classList.contains(ElementIDs.DropTargetClassName)) {
                        row.classList.add(ElementIDs.DropTargetClassName);
                    }
                    this._lastOverElement = row;
                });

                elm.addEventListener('dragleave', e => {
                    e.preventDefault();
                    if (!(e.target instanceof HTMLElement)) {
                        return;
                    }

                    const row: HTMLElement | null = this.GetParentByClassName(e.target, ElementIDs.VideoListRowClassName);
                    if (row === null) {
                        return;
                    }

                    if (row.classList.contains(ElementIDs.DropTargetClassName)) {
                        row.classList.remove(ElementIDs.DropTargetClassName);
                    }
                });

                elm.addEventListener('drop', async e => {
                    e.preventDefault();

                    if (this._sourceNiconicoID === null) {
                        return;
                    }

                    const sourceResult = this._elmHandler.Get(`#${this._sourceID}`);
                    if (!sourceResult.IsSucceeded || sourceResult.Data === null) {
                        return;
                    }

                    if (!(e.target instanceof HTMLElement)) {
                        return;
                    }

                    let parent: ParentNode | null = e.target.parentNode;
                    let dropTarget: HTMLElement = e.target;

                    while (parent !== null) {
                        if (!(parent instanceof HTMLElement)) {
                            return;
                        }

                        if (!parent.classList.contains(ElementIDs.VideoListBodyClassName)) {
                            dropTarget = parent;
                            parent = parent.parentNode;
                            continue;
                        }

                        parent.insertBefore(sourceResult.Data, dropTarget);
                        await this._dotnetHelper.invokeMethodAsync("MoveVideo", this._sourceNiconicoID, dropTarget.dataset['niconicoid']!);
                        parent = null;
                    }


                    if (this._lastOverElement !== null) {
                        if (this._lastOverElement.classList.contains(ElementIDs.DropTargetClassName)) {
                            this._lastOverElement.classList.remove(ElementIDs.DropTargetClassName);
                        }
                    }
                });
            }
        });
    }

    private GetParentByClassName(currentElement: HTMLElement, className: string): HTMLElement | null {
        let parent: ParentNode | null = currentElement;

        while (parent !== null) {
            if (!(parent instanceof HTMLElement)) {
                return null;
            }

            if (!parent.classList.contains(className)) {
                parent = parent.parentNode;
                continue;
            }

            return parent;
        }

        return null;
    }
}