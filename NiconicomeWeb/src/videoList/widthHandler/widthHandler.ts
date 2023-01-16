import { AttemptResultWidthData } from '../../shared/AttemptResult';
import { ElementHandler } from '../../shared/ElementHandler';
import { ElementIDs } from './ElementIds';
import { Dictionary } from '../../shared/Collection/dictionary';

export interface WidthHandler {
    Initialize(): void;
}

export class WidthHandlerImpl implements WidthHandler {

    constructor(elementHandler: ElementHandler) {
        this._elmHandler = elementHandler;
        this._headerColumnIDs = {
            '0': '#CheckBoxColumn',
            '1': '#ThumbnailColumn',
            '2': '#TitleColumn',
            '3': '#UploadedDateTimeColumn',
            '4': '#ViewCountColumn',
            '5': '#CommentCountColumn',
            '6': '#MylistCountColumn',
        };
        this._columnClassNames = {
            '0': '.CheckBoxColumn',
            '1': '.ThumbnailWrapper',
            '2': '.TitleColumn',
            '3': '.UploadedDateTimeColumn',
            '4': '.ViewCountColumn',
            '5': '.CommentCountColumn',
            '6': '.MylistCountColumn',
        };
        this._separatorIDs = {
            '0': '#CheckBoxColumnSeparator',
            '1': '#ThumbnailColumnSeparator',
            '2': '#TitleColumnSeparator',
            '3': '#UploadedDateTimeColumnSeparator',
            '4': '#ViewCountColumnSeparator',
            '5': '#CommentCountColumnSeparator',
            '6': '#MylistCountColumnSeparator',
        };
    }

    //#region  field

    private readonly _elmHandler: ElementHandler;

    private readonly _headerColumnIDs: Dictionary<string>;

    private readonly _separatorIDs: Dictionary<string>;

    private readonly _columnClassNames: Dictionary<string>;

    private _isResizing = false;

    private _resizingIndex: null | string;

    //#endregion

    public Initialize(): void {

        const sepResult: AttemptResultWidthData<NodeListOf<Element>> = this._elmHandler.GetAll(ElementIDs.Separator);
        if (!sepResult.IsSucceeded || sepResult.Data === null) return;

        sepResult.Data.forEach(elm => {
            if (!(elm instanceof HTMLElement)) return;

            const indexS = elm.dataset.index;

            if (indexS == undefined) return;

            elm.addEventListener('mousedown', _ => this.OnMouseDown(indexS));
        });

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

        const headerResult = this._elmHandler.Get(this._headerColumnIDs[this._resizingIndex]);
        const nextHeaderResult = this._elmHandler.Get(this._headerColumnIDs[`${nextIndex}`]);
        const headerWrapperResult = this._elmHandler.Get(ElementIDs.VideoListHeader);
        const columnResult = this._elmHandler.GetAll(this._columnClassNames[this._resizingIndex]);
        const nextColumnResult = this._elmHandler.GetAll(this._columnClassNames[`${nextIndex}`]);
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