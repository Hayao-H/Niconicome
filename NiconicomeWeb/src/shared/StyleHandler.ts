import { AttemptResultWidthData, AttemptResultWidthDataImpl } from "./AttemptResult";
import { ElementHandler } from "./ElementHandler";

export interface StyleHandler {
    /**
     * 要素のスタイルを取得
     */
    GetComputedStyle(query: string): AttemptResultWidthData<CSSStyleDeclaration>;
}

export class StyleHandlerImpl implements StyleHandler {
    constructor(elementHandler: ElementHandler) {
        this._elmHandler = elementHandler;
    }

    private readonly _elmHandler: ElementHandler;

    public GetComputedStyle(query: string): AttemptResultWidthData<CSSStyleDeclaration> {

        const result: AttemptResultWidthData<Element> = this._elmHandler.Get(query);
        if (!result.IsSucceeded || result.Data === null) {
            return AttemptResultWidthDataImpl.Fail(result.Message ?? "");
        }

        try {
            const style = window.getComputedStyle(result.Data);
            return AttemptResultWidthDataImpl.Succeeded(style);
        } catch (ex) {
            return AttemptResultWidthDataImpl.Fail("スタイルを取得できませんでした。");
        }
    }
}