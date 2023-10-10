import { DotNetObjectReference } from "../../shared/DotNetObjectReference";
import { ElementHandler } from "../../shared/ElementHandler";

export interface UserEventHandler {

    /**
     * 初期化
     */
    Initialize(registered: string[]): void;
}

export class UserEventHandler implements UserEventHandler {

    constructor(dotnet: DotNetObjectReference, element: ElementHandler) {
        this._dotnet = dotnet;
        this._element = element;
    }

    //#region  field

    private readonly _dotnet: DotNetObjectReference;

    private readonly _element: ElementHandler;

    //#endregion

    public Initialize(registered: string[]): void {
        const elmResult = this._element.GetAll('.VideoListRow');
        if (!elmResult.IsSucceeded || elmResult.Data === null) return;

        elmResult.Data.forEach(elm => {

            if (!(elm instanceof HTMLElement)) return;
            const niconicoID = elm.dataset['niconicoid'];
            const playlistID = elm.dataset['playlistid'];

            if (niconicoID === undefined || playlistID === undefined) {
                return;
            }

            const key: string = `${niconicoID}-${playlistID}`;

            if (registered.includes(key)) {
                return;
            } else {
                registered.push(key);
            }
        });
    }

}