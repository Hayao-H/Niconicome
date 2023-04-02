import { ElementHandler } from "../../shared/ElementHandler";

export interface SelectionHandler {

    /**
     * 入力欄で選択されている文字列を取得する
     */
    getSelected(): string;
}

export class SelectionHandlerImpl implements SelectionHandler {

    constructor(elmHandler: ElementHandler) {
        this._elmHandler = elmHandler;
    }

    private readonly _elmHandler: ElementHandler;

    public getSelected(): string {

        const elmResult = this._elmHandler.Get("#InputBox");
        if (!elmResult.IsSucceeded || elmResult.Data === null) {
            return "";
        }

        const elm: Element = elmResult.Data;
        if (!(elm instanceof HTMLInputElement)) {
            return "";
        }

        if (!this.IsValidIndex(elm.value, elm.selectionStart)) {
            return "";
        }

        if (!this.IsValidIndex(elm.value, elm.selectionEnd, true)) {
            return "";
        }

        return elm.value.substring(elm.selectionStart!, elm.selectionEnd!)

    }

    private IsValidIndex(value: string, index: number | null, isEnd: boolean = false): boolean {
        if (index === null) {
            return false;
        }

        if (isEnd && index > value.length) {
            return false;
        }

        if (!isEnd && index > value.length - 1) {
            return false;
        }

        return true;
    }
}