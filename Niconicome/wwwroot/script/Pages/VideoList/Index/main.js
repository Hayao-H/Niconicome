// src/shared/AttemptResult.ts
var AttemptResultWidthDataImpl = class {
  constructor(isSucceeded, data, message) {
    this.IsSucceeded = isSucceeded;
    this.Data = data;
    this.Message = message;
  }
  static Succeeded(data) {
    return new AttemptResultWidthDataImpl(true, data, null);
  }
  static Fail(message) {
    return new AttemptResultWidthDataImpl(false, null, message);
  }
};

// src/shared/ElementHandler.ts
var ElementHandlerImpl = class {
  Get(query) {
    let result;
    try {
      result = document.querySelector(query);
    } catch (e) {
      return AttemptResultWidthDataImpl.Fail(`\u8981\u7D20\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002(\u8A73\u7D30\uFF1A${e.message})`);
    }
    return result == null ? AttemptResultWidthDataImpl.Fail("\u6307\u5B9A\u3055\u308C\u305F\u8981\u7D20\u304C\u898B\u3064\u304B\u308A\u307E\u305B\u3093\u3002") : AttemptResultWidthDataImpl.Succeeded(result);
  }
  GetAll(query) {
    let result;
    try {
      result = document.querySelectorAll(query);
    } catch (e) {
      return AttemptResultWidthDataImpl.Fail(`\u8981\u7D20\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002(\u8A73\u7D30\uFF1A${e.message})`);
    }
    return AttemptResultWidthDataImpl.Succeeded(result);
  }
};

// src/videoList/widthHandler/ElementIds.ts
var ElementIDs = class {
};
ElementIDs.PageContent = ".PageContent";
ElementIDs.VideoListHeader = "#VideoListHeader";
ElementIDs.Separator = ".Separator";

// src/videoList/widthHandler/widthHandler.ts
var WidthHandlerImpl = class {
  constructor(elementHandler) {
    this._isResizing = false;
    this._elmHandler = elementHandler;
    this._headerColumnIDs = {
      "0": "#CheckBoxColumn",
      "1": "#ThumbnailColumn",
      "2": "#TitleColumn",
      "3": "#UploadedDateTimeColumn",
      "4": "#ViewCountColumn",
      "5": "#CommentCountColumn",
      "6": "#MylistCountColumn"
    };
    this._columnClassNames = {
      "0": ".CheckBoxColumn",
      "1": ".ThumbnailWrapper",
      "2": ".TitleColumn",
      "3": ".UploadedDateTimeColumn",
      "4": ".ViewCountColumn",
      "5": ".CommentCountColumn",
      "6": ".MylistCountColumn"
    };
    this._separatorIDs = {
      "0": "#CheckBoxColumnSeparator",
      "1": "#ThumbnailColumnSeparator",
      "2": "#TitleColumnSeparator",
      "3": "#UploadedDateTimeColumnSeparator",
      "4": "#ViewCountColumnSeparator",
      "5": "#CommentCountColumnSeparator",
      "6": "#MylistCountColumnSeparator"
    };
  }
  //#endregion
  Initialize() {
    const sepResult = this._elmHandler.GetAll(ElementIDs.Separator);
    if (!sepResult.IsSucceeded || sepResult.Data === null)
      return;
    sepResult.Data.forEach((elm) => {
      if (!(elm instanceof HTMLElement))
        return;
      const indexS = elm.dataset.index;
      if (indexS == void 0)
        return;
      elm.addEventListener("mousedown", (_) => this.OnMouseDown(indexS));
    });
    const pageResult = this._elmHandler.Get(ElementIDs.PageContent);
    if (!pageResult.IsSucceeded || pageResult.Data === null || !(pageResult.Data instanceof HTMLElement))
      return;
    pageResult.Data.addEventListener("mouseup", (_) => this.OnMouseUp());
    const headerResult = this._elmHandler.Get(ElementIDs.VideoListHeader);
    if (!headerResult.IsSucceeded || headerResult.Data === null || !(headerResult.Data instanceof HTMLElement))
      return;
    headerResult.Data.addEventListener("mousemove", (e) => this.OnMouseMove(e));
  }
  //#region private
  OnMouseDown(index) {
    this._isResizing = true;
    this._resizingIndex = index;
  }
  OnMouseUp() {
    this._isResizing = false;
    this._resizingIndex = null;
  }
  OnMouseMove(e) {
    if (!this._isResizing || this._resizingIndex === null)
      return;
    const nextIndex = Number(this._resizingIndex) + 1;
    const headerResult = this._elmHandler.Get(this._headerColumnIDs[this._resizingIndex]);
    const nextHeaderResult = this._elmHandler.Get(this._headerColumnIDs[`${nextIndex}`]);
    const headerWrapperResult = this._elmHandler.Get(ElementIDs.VideoListHeader);
    const columnResult = this._elmHandler.GetAll(this._columnClassNames[this._resizingIndex]);
    const nextColumnResult = this._elmHandler.GetAll(this._columnClassNames[`${nextIndex}`]);
    const sepResult = this._elmHandler.Get(this._separatorIDs[this._resizingIndex]);
    if (!headerResult.IsSucceeded || headerResult.Data === null || !columnResult.IsSucceeded || columnResult.Data === null || !sepResult.IsSucceeded || sepResult.Data === null || !headerWrapperResult.IsSucceeded || headerWrapperResult.Data === null || !nextHeaderResult.IsSucceeded || nextHeaderResult.Data === null || !nextColumnResult.IsSucceeded || nextColumnResult.Data === null)
      return;
    if (!(headerResult.Data instanceof HTMLElement))
      return;
    if (!(nextHeaderResult.Data instanceof HTMLElement))
      return;
    const headerRect = headerResult.Data.getBoundingClientRect();
    const headerWrapperRect = headerWrapperResult.Data.getBoundingClientRect();
    const width = e.clientX - headerRect.left;
    const deltaWidth = width - headerResult.Data.offsetWidth;
    const nextWidth = nextHeaderResult.Data.offsetWidth - deltaWidth;
    headerResult.Data.style.width = `${width}px`;
    nextHeaderResult.Data.style.width = `${nextWidth}px`;
    columnResult.Data.forEach((elm) => {
      if (!(elm instanceof HTMLElement))
        return;
      elm.style.width = `${width}px`;
    });
    nextColumnResult.Data.forEach((elm) => {
      if (!(elm instanceof HTMLElement))
        return;
      elm.style.width = `${nextWidth}px`;
    });
    if (!(headerWrapperResult.Data instanceof HTMLElement))
      return;
    if (!(sepResult.Data instanceof HTMLElement))
      return;
    const left = headerRect.left - headerWrapperRect.left + width - 10;
    sepResult.Data.style.left = `${left}px`;
  }
  //#endregion
};

// src/videoList/main.ts
function main() {
  console.log("Hello World!!");
  const elmHandler = new ElementHandlerImpl();
  const widthHandler = new WidthHandlerImpl(elmHandler);
  widthHandler.Initialize();
}
export {
  main
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0F0dGVtcHRSZXN1bHQudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0VsZW1lbnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3ZpZGVvTGlzdC93aWR0aEhhbmRsZXIvRWxlbWVudElkcy50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3Qvd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvbWFpbi50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiZXhwb3J0IGludGVyZmFjZSBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IGV4dGVuZHMgQXR0ZW1wdFJlc3VsdCB7XHJcbiAgICBcclxuICAgIC8qKlxyXG4gICAgICogXHUzMEM3XHUzMEZDXHUzMEJGXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG59XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEF0dGVtcHRSZXN1bHQge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU2MjEwXHU1MjlGXHUzMEQ1XHUzMEU5XHUzMEIwXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHUzMEUxXHUzMEMzXHUzMEJCXHUzMEZDXHUzMEI4XHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcbn1cclxuXHJcblxyXG5leHBvcnQgY2xhc3MgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGw8VD4gaW1wbGVtZW50cyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihpc1N1Y2NlZWRlZDogYm9vbGVhbiwgZGF0YTogVCB8IG51bGwsIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgICAgICB0aGlzLk1lc3NhZ2UgPSBtZXNzYWdlO1xyXG4gICAgfVxyXG5cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG5cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBTdWNjZWVkZWQ8VD4oZGF0YTogVCB8IG51bGwpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKHRydWUsIGRhdGEsIG51bGwpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgRmFpbDxUPihtZXNzYWdlOiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKGZhbHNlLCBudWxsLCBtZXNzYWdlKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIEF0dGVtcHRSZXN1bHRJbXBsIGltcGxlbWVudHMgQXR0ZW1wdFJlc3VsdCB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoaXNTdWNjZWVkZWQ6IGJvb2xlYW4sIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5NZXNzYWdlID0gbWVzc2FnZTtcclxuICAgIH1cclxuXHJcbiAgICByZWFkb25seSBJc1N1Y2NlZWRlZDogYm9vbGVhbjtcclxuXHJcbiAgICByZWFkb25seSBNZXNzYWdlOiBzdHJpbmcgfCBudWxsO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgU3VjY2VlZGVkKCk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwodHJ1ZSwgbnVsbCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBGYWlsKG1lc3NhZ2U6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwoZmFsc2UsIG1lc3NhZ2UpO1xyXG4gICAgfVxyXG59XHJcblxyXG4iLCAiaW1wb3J0IHsgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSwgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwgfSBmcm9tIFwiLi9BdHRlbXB0UmVzdWx0XCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEVsZW1lbnRIYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXQocXVlcnk6c3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PjtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODkwN1x1NjU3MFx1MzA2RVx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj47XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBFbGVtZW50SGFuZGxlckltcGwgaW1wbGVtZW50cyBFbGVtZW50SGFuZGxlciB7XHJcblxyXG4gICAgcHVibGljIEdldChxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiB7XHJcblxyXG4gICAgICAgIGxldCByZXN1bHQ6IEVsZW1lbnQgfCBudWxsO1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gcmVzdWx0ID09IG51bGwgPyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5GYWlsKFwiXHU2MzA3XHU1QjlBXHUzMDU1XHUzMDhDXHUzMDVGXHU4OTgxXHU3RDIwXHUzMDRDXHU4OThCXHUzMDY0XHUzMDRCXHUzMDhBXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDAyXCIpIDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIHB1YmxpYyBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj4ge1xyXG5cclxuICAgICAgICBsZXQgcmVzdWx0OiBOb2RlTGlzdE9mPEVsZW1lbnQ+O1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG59IiwgImV4cG9ydCBjbGFzcyBFbGVtZW50SURzIHtcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFBhZ2VDb250ZW50ID0gJy5QYWdlQ29udGVudCc7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBWaWRlb0xpc3RIZWFkZXIgPSAnI1ZpZGVvTGlzdEhlYWRlcic7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBTZXBhcmF0b3IgPSAnLlNlcGFyYXRvcic7XHJcblxyXG59IiwgImltcG9ydCB7IEF0dGVtcHRSZXN1bHRXaWR0aERhdGEgfSBmcm9tICcuLi8uLi9zaGFyZWQvQXR0ZW1wdFJlc3VsdCc7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyJztcclxuaW1wb3J0IHsgRWxlbWVudElEcyB9IGZyb20gJy4vRWxlbWVudElkcyc7XHJcbmltcG9ydCB7IERpY3Rpb25hcnkgfSBmcm9tICcuLi8uLi9zaGFyZWQvQ29sbGVjdGlvbi9kaWN0aW9uYXJ5JztcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgV2lkdGhIYW5kbGVyIHtcclxuICAgIEluaXRpYWxpemUoKTogdm9pZDtcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFdpZHRoSGFuZGxlckltcGwgaW1wbGVtZW50cyBXaWR0aEhhbmRsZXIge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGVsZW1lbnRIYW5kbGVyOiBFbGVtZW50SGFuZGxlcikge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbGVtZW50SGFuZGxlcjtcclxuICAgICAgICB0aGlzLl9oZWFkZXJDb2x1bW5JRHMgPSB7XHJcbiAgICAgICAgICAgICcwJzogJyNDaGVja0JveENvbHVtbicsXHJcbiAgICAgICAgICAgICcxJzogJyNUaHVtYm5haWxDb2x1bW4nLFxyXG4gICAgICAgICAgICAnMic6ICcjVGl0bGVDb2x1bW4nLFxyXG4gICAgICAgICAgICAnMyc6ICcjVXBsb2FkZWREYXRlVGltZUNvbHVtbicsXHJcbiAgICAgICAgICAgICc0JzogJyNWaWV3Q291bnRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNSc6ICcjQ29tbWVudENvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzYnOiAnI015bGlzdENvdW50Q29sdW1uJyxcclxuICAgICAgICB9O1xyXG4gICAgICAgIHRoaXMuX2NvbHVtbkNsYXNzTmFtZXMgPSB7XHJcbiAgICAgICAgICAgICcwJzogJy5DaGVja0JveENvbHVtbicsXHJcbiAgICAgICAgICAgICcxJzogJy5UaHVtYm5haWxXcmFwcGVyJyxcclxuICAgICAgICAgICAgJzInOiAnLlRpdGxlQ29sdW1uJyxcclxuICAgICAgICAgICAgJzMnOiAnLlVwbG9hZGVkRGF0ZVRpbWVDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNCc6ICcuVmlld0NvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzUnOiAnLkNvbW1lbnRDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc2JzogJy5NeWxpc3RDb3VudENvbHVtbicsXHJcbiAgICAgICAgfTtcclxuICAgICAgICB0aGlzLl9zZXBhcmF0b3JJRHMgPSB7XHJcbiAgICAgICAgICAgICcwJzogJyNDaGVja0JveENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICcxJzogJyNUaHVtYm5haWxDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMic6ICcjVGl0bGVDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMyc6ICcjVXBsb2FkZWREYXRlVGltZUNvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc0JzogJyNWaWV3Q291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNSc6ICcjQ29tbWVudENvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzYnOiAnI015bGlzdENvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICB9O1xyXG4gICAgfVxyXG5cclxuICAgIC8vI3JlZ2lvbiAgZmllbGRcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9lbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9oZWFkZXJDb2x1bW5JRHM6IERpY3Rpb25hcnk8c3RyaW5nPjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9zZXBhcmF0b3JJRHM6IERpY3Rpb25hcnk8c3RyaW5nPjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9jb2x1bW5DbGFzc05hbWVzOiBEaWN0aW9uYXJ5PHN0cmluZz47XHJcblxyXG4gICAgcHJpdmF0ZSBfaXNSZXNpemluZyA9IGZhbHNlO1xyXG5cclxuICAgIHByaXZhdGUgX3Jlc2l6aW5nSW5kZXg6IG51bGwgfCBzdHJpbmc7XHJcblxyXG4gICAgLy8jZW5kcmVnaW9uXHJcblxyXG4gICAgcHVibGljIEluaXRpYWxpemUoKTogdm9pZCB7XHJcblxyXG4gICAgICAgIGNvbnN0IHNlcFJlc3VsdDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxOb2RlTGlzdE9mPEVsZW1lbnQ+PiA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKEVsZW1lbnRJRHMuU2VwYXJhdG9yKTtcclxuICAgICAgICBpZiAoIXNlcFJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzZXBSZXN1bHQuRGF0YSA9PT0gbnVsbCkgcmV0dXJuO1xyXG5cclxuICAgICAgICBzZXBSZXN1bHQuRGF0YS5mb3JFYWNoKGVsbSA9PiB7XHJcbiAgICAgICAgICAgIGlmICghKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICAgICAgY29uc3QgaW5kZXhTID0gZWxtLmRhdGFzZXQuaW5kZXg7XHJcblxyXG4gICAgICAgICAgICBpZiAoaW5kZXhTID09IHVuZGVmaW5lZCkgcmV0dXJuO1xyXG5cclxuICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNlZG93bicsIF8gPT4gdGhpcy5Pbk1vdXNlRG93bihpbmRleFMpKTtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgY29uc3QgcGFnZVJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KEVsZW1lbnRJRHMuUGFnZUNvbnRlbnQpO1xyXG4gICAgICAgIGlmICghcGFnZVJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBwYWdlUmVzdWx0LkRhdGEgPT09IG51bGwgfHwgIShwYWdlUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBwYWdlUmVzdWx0LkRhdGEuYWRkRXZlbnRMaXN0ZW5lcignbW91c2V1cCcsIF8gPT4gdGhpcy5Pbk1vdXNlVXAoKSk7XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KEVsZW1lbnRJRHMuVmlkZW9MaXN0SGVhZGVyKTtcclxuICAgICAgICBpZiAoIWhlYWRlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBoZWFkZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCB8fCAhKGhlYWRlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgaGVhZGVyUmVzdWx0LkRhdGEuYWRkRXZlbnRMaXN0ZW5lcignbW91c2Vtb3ZlJywgZSA9PiB0aGlzLk9uTW91c2VNb3ZlKGUpKTtcclxuXHJcbiAgICB9XHJcblxyXG4gICAgLy8jcmVnaW9uIHByaXZhdGVcclxuXHJcbiAgICBwcml2YXRlIE9uTW91c2VEb3duKGluZGV4OiBzdHJpbmcpOiB2b2lkIHtcclxuICAgICAgICB0aGlzLl9pc1Jlc2l6aW5nID0gdHJ1ZTtcclxuICAgICAgICB0aGlzLl9yZXNpemluZ0luZGV4ID0gaW5kZXg7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vdXNlVXAoKTogdm9pZCB7XHJcbiAgICAgICAgdGhpcy5faXNSZXNpemluZyA9IGZhbHNlO1xyXG4gICAgICAgIHRoaXMuX3Jlc2l6aW5nSW5kZXggPSBudWxsO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgT25Nb3VzZU1vdmUoZTogTW91c2VFdmVudCk6IHZvaWQge1xyXG4gICAgICAgIGlmICghdGhpcy5faXNSZXNpemluZyB8fCB0aGlzLl9yZXNpemluZ0luZGV4ID09PSBudWxsKSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IG5leHRJbmRleCA9IE51bWJlcih0aGlzLl9yZXNpemluZ0luZGV4KSArIDE7XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KHRoaXMuX2hlYWRlckNvbHVtbklEc1t0aGlzLl9yZXNpemluZ0luZGV4XSk7XHJcbiAgICAgICAgY29uc3QgbmV4dEhlYWRlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KHRoaXMuX2hlYWRlckNvbHVtbklEc1tgJHtuZXh0SW5kZXh9YF0pO1xyXG4gICAgICAgIGNvbnN0IGhlYWRlcldyYXBwZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChFbGVtZW50SURzLlZpZGVvTGlzdEhlYWRlcik7XHJcbiAgICAgICAgY29uc3QgY29sdW1uUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwodGhpcy5fY29sdW1uQ2xhc3NOYW1lc1t0aGlzLl9yZXNpemluZ0luZGV4XSk7XHJcbiAgICAgICAgY29uc3QgbmV4dENvbHVtblJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKHRoaXMuX2NvbHVtbkNsYXNzTmFtZXNbYCR7bmV4dEluZGV4fWBdKTtcclxuICAgICAgICBjb25zdCBzZXBSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldCh0aGlzLl9zZXBhcmF0b3JJRHNbdGhpcy5fcmVzaXppbmdJbmRleF0pO1xyXG5cclxuICAgICAgICBpZiAoIWhlYWRlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBoZWFkZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCB8fCAhY29sdW1uUmVzdWx0LklzU3VjY2VlZGVkIHx8IGNvbHVtblJlc3VsdC5EYXRhID09PSBudWxsIHx8ICFzZXBSZXN1bHQuSXNTdWNjZWVkZWQgfHwgc2VwUmVzdWx0LkRhdGEgPT09IG51bGwgfHwgIWhlYWRlcldyYXBwZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhID09PSBudWxsIHx8ICFuZXh0SGVhZGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IG5leHRIZWFkZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCB8fCAhbmV4dENvbHVtblJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBuZXh0Q29sdW1uUmVzdWx0LkRhdGEgPT09IG51bGwpIHJldHVybjtcclxuXHJcbiAgICAgICAgaWYgKCEoaGVhZGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBpZiAoIShuZXh0SGVhZGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgY29uc3QgaGVhZGVyUmVjdDogRE9NUmVjdCA9IGhlYWRlclJlc3VsdC5EYXRhLmdldEJvdW5kaW5nQ2xpZW50UmVjdCgpO1xyXG4gICAgICAgIGNvbnN0IGhlYWRlcldyYXBwZXJSZWN0OiBET01SZWN0ID0gaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhLmdldEJvdW5kaW5nQ2xpZW50UmVjdCgpO1xyXG5cclxuXHJcbiAgICAgICAgY29uc3Qgd2lkdGggPSBlLmNsaWVudFggLSBoZWFkZXJSZWN0LmxlZnQ7XHJcbiAgICAgICAgY29uc3QgZGVsdGFXaWR0aCA9IHdpZHRoIC0gaGVhZGVyUmVzdWx0LkRhdGEub2Zmc2V0V2lkdGg7XHJcbiAgICAgICAgY29uc3QgbmV4dFdpZHRoID0gbmV4dEhlYWRlclJlc3VsdC5EYXRhLm9mZnNldFdpZHRoIC0gZGVsdGFXaWR0aDtcclxuXHJcbiAgICAgICAgaGVhZGVyUmVzdWx0LkRhdGEuc3R5bGUud2lkdGggPSBgJHt3aWR0aH1weGA7XHJcbiAgICAgICAgbmV4dEhlYWRlclJlc3VsdC5EYXRhLnN0eWxlLndpZHRoID0gYCR7bmV4dFdpZHRofXB4YDtcclxuXHJcbiAgICAgICAgY29sdW1uUmVzdWx0LkRhdGEuZm9yRWFjaChlbG0gPT4ge1xyXG4gICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgIGVsbS5zdHlsZS53aWR0aCA9IGAke3dpZHRofXB4YDtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgbmV4dENvbHVtblJlc3VsdC5EYXRhLmZvckVhY2goZWxtID0+IHtcclxuICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgICAgICBlbG0uc3R5bGUud2lkdGggPSBgJHtuZXh0V2lkdGh9cHhgO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBpZiAoIShoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBpZiAoIShzZXBSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICBjb25zdCBsZWZ0ID0gaGVhZGVyUmVjdC5sZWZ0IC0gaGVhZGVyV3JhcHBlclJlY3QubGVmdCArIHdpZHRoIC0gMTA7XHJcbiAgICAgICAgc2VwUmVzdWx0LkRhdGEuc3R5bGUubGVmdCA9IGAke2xlZnR9cHhgO1xyXG4gICAgfVxyXG5cclxuICAgIC8vI2VuZHJlZ2lvblxyXG59XHJcblxyXG5pbnRlcmZhY2UgQ2xhc3NOYW1lc0RpY3Qge1xyXG4gICAgW2luZGV4OiBudW1iZXJdOiBzdHJpbmc7XHJcbn0iLCAiaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIsIEVsZW1lbnRIYW5kbGVySW1wbCB9IGZyb20gXCIuLi9zaGFyZWQvRWxlbWVudEhhbmRsZXJcIjtcclxuaW1wb3J0IHsgV2lkdGhIYW5kbGVyLCBXaWR0aEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlclwiO1xyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIG1haW4oKSB7XHJcbiAgICBjb25zb2xlLmxvZyhcIkhlbGxvIFdvcmxkISFcIik7XHJcblxyXG4gICAgY29uc3QgZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXIgPSBuZXcgRWxlbWVudEhhbmRsZXJJbXBsKCk7XHJcbiAgICBjb25zdCB3aWR0aEhhbmRsZXI6IFdpZHRoSGFuZGxlciA9IG5ldyBXaWR0aEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIpO1xyXG5cclxuICAgIHdpZHRoSGFuZGxlci5Jbml0aWFsaXplKCk7XHJcblxyXG59Il0sCiAgIm1hcHBpbmdzIjogIjtBQXNCTyxJQUFNLDZCQUFOLE1BQXlFO0FBQUEsRUFFNUUsWUFBWSxhQUFzQixNQUFnQixTQUF3QjtBQUN0RSxTQUFLLGNBQWM7QUFDbkIsU0FBSyxPQUFPO0FBQ1osU0FBSyxVQUFVO0FBQUEsRUFDbkI7QUFBQSxFQVFBLE9BQWMsVUFBYSxNQUEyQztBQUNsRSxXQUFPLElBQUksMkJBQTJCLE1BQU0sTUFBTSxJQUFJO0FBQUEsRUFDMUQ7QUFBQSxFQUVBLE9BQWMsS0FBUSxTQUE0QztBQUM5RCxXQUFPLElBQUksMkJBQTJCLE9BQU8sTUFB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
