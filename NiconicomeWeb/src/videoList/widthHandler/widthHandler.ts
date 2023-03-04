import { AttemptResultWidthData } from '../../shared/AttemptResult';
import { ElementHandler } from '../../shared/ElementHandler';
import { ElementIDs } from './ElementIds';
import { Dictionary } from '../../shared/Collection/dictionary';
import { StyleHandler } from '../../shared/StyleHandler';

export interface WidthHandler {
    Initialize(): void;
}

export class WidthHandlerImpl implements WidthHandler {

    constructor(elementHandler: ElementHandler, styleHandler: StyleHandler) {
        this._elmHandler = elementHandler;
        this._styleHandler = styleHandler;
        this._columnIDs = {
            '0': 'CheckBoxColumn',
            '1': 'ThumbnailColumn',
            '2': 'TitleColumn',
            '3': 'UploadedDateTimeColumn',
            '4': 'IsDownloadedColumn',
            '5': 'ViewCountColumn',
            '6': 'CommentCountColumn',
            '7': 'MylistCountColumn',
            '8': 'LikeCountColumn',
            '9': 'MessageColumn',
        };
        this._separatorIDs = {
            '0': '#CheckBoxColumnSeparator',
            '1': '#ThumbnailColumnSeparator',
            '2': '#TitleColumnSeparator',
            '3': '#UploadedDateTimeColumnSeparator',
            '4': '#IsDownloadedColumnSeparator',
            '5': '#ViewCountColumnSeparator',
            '6': '#CommentCountColumnSeparator',
            '7': '#MylistCountColumnSeparator',
            '8': '#LikeCountColumnSeparator',
        };
    }

    //#region  field

    private readonly _elmHandler: ElementHandler;

    private readonly _styleHandler: StyleHandler;

    private readonly _columnIDs: Dictionary<string>;

    private readonly _separatorIDs: Dictionary<string>;

    private _isResizing = false;

    private _resizingIndex: null | string;

    //#endregion

    public Initialize(): void {

        let left = 0;
        for (const key in this._separatorIDs) {

            const sepResult: AttemptResultWidthData<Element> = this._elmHandler.Get(this._separatorIDs[key]);
            if (!sepResult.IsSucceeded || sepResult.Data === null) continue;

            const elm = sepResult.Data;
            if (!(elm instanceof HTMLElement)) continue;

            const styleResult = this._styleHandler.GetComputedStyle(`#${this._columnIDs[key]}`);
            if (styleResult.IsSucceeded && styleResult.Data !== null) {
                const style: CSSStyleDeclaration = styleResult.Data;

                const rawResult = this._elmHandler.GetAll(`.${this._columnIDs[key]}`);
                if (!rawResult.IsSucceeded || rawResult.Data === null) {
                    continue;
                }

                if (style.display === "none") {
                    //ヘッダーが非表示ならセパレーターも非表示
                    elm.style.display = "none";

                    //リスト側も非表示にする
                    rawResult.Data.forEach(raw => {
                        if (raw instanceof HTMLElement) {
                            raw.style.display = "none";
                        }
                    });

                    continue;
                } else {
                    left += Number(style.width.match(/\d+/));
                    elm.style.left = `${left}px`;

                    //リスト側の幅をヘッダーに合わせる
                    rawResult.Data.forEach(raw => {
                        if (raw instanceof HTMLElement) {
                            raw.style.width = style.width;
                        }
                    });
                }
            }

            const indexS = elm.dataset.index;

            if (indexS == undefined) continue;

            elm.addEventListener('mousedown', _ => this.OnMouseDown(indexS));
        }

        const pageResult = this._elmHandler.Get(ElementIDs.PageContent);
        if (!pageResult.IsSucceeded || pageResult.Data === null || !(pageResult.Data instanceof HTMLElement)) return;
        pageResult.Data.addEventListener('mouseup', _ => this.OnMouseUp());

        const headerResult = this._elmHandler.Get(ElementIDs.VideoListHeader);
        if (!headerResult.IsSucceeded || headerResult.Data === null || !(headerResult.Data instanceof HTMLElement)) return;
        headerResult.Data.addEventListener('mousemove', e => this.OnMouseMove(e));

    }

    //#region private

    private OnMouseDown(index: string): void {
        this._isResizing = true;
        this._resizingIndex = index;
    }

    private OnMouseUp(): void {
        this._isResizing = false;
        this._resizingIndex = null;
    }

    private OnMouseMove(e: MouseEvent): void {
        if (!this._isResizing || this._resizingIndex === null) return;

        const nextIndex = Number(this._resizingIndex) + 1;

        const headerResult = this._elmHandler.Get(`#${this._columnIDs[this._resizingIndex]}`);
        const nextHeaderResult = this._elmHandler.Get(`#${this._columnIDs[`${nextIndex}`]}`);
        const headerWrapperResult = this._elmHandler.Get(ElementIDs.VideoListHeader);
        const columnResult = this._elmHandler.GetAll(`.${this._columnIDs[this._resizingIndex]}`);
        const nextColumnResult = this._elmHandler.GetAll(`.${this._columnIDs[`${nextIndex}`]}`);
        const sepResult = this._elmHandler.Get(this._separatorIDs[this._resizingIndex]);

        if (!headerResult.IsSucceeded || headerResult.Data === null || !columnResult.IsSucceeded || columnResult.Data === null || !sepResult.IsSucceeded || sepResult.Data === null || !headerWrapperResult.IsSucceeded || headerWrapperResult.Data === null || !nextHeaderResult.IsSucceeded || nextHeaderResult.Data === null || !nextColumnResult.IsSucceeded || nextColumnResult.Data === null) return;

        if (!(headerResult.Data instanceof HTMLElement)) return;
        if (!(nextHeaderResult.Data instanceof HTMLElement)) return;

        const headerRect: DOMRect = headerResult.Data.getBoundingClientRect();
        const headerWrapperRect: DOMRect = headerWrapperResult.Data.getBoundingClientRect();


        const width = e.clientX - headerRect.left;
        const deltaWidth = width - headerResult.Data.offsetWidth;
        const nextWidth = nextHeaderResult.Data.offsetWidth - deltaWidth;

        headerResult.Data.style.width = `${width}px`;
        nextHeaderResult.Data.style.width = `${nextWidth}px`;

        columnResult.Data.forEach(elm => {
            if (!(elm instanceof HTMLElement)) return;

            elm.style.width = `${width}px`;
        });

        nextColumnResult.Data.forEach(elm => {
            if (!(elm instanceof HTMLElement)) return;

            elm.style.width = `${nextWidth}px`;
        });

        if (!(headerWrapperResult.Data instanceof HTMLElement)) return;
        if (!(sepResult.Data instanceof HTMLElement)) return;

        const left = headerRect.left - headerWrapperRect.left + width - 10;
        sepResult.Data.style.left = `${left}px`;
    }

    //#endregion
}

interface ClassNamesDict {
    [index: number]: string;
}