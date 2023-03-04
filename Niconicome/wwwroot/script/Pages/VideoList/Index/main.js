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

// src/shared/StyleHandler.ts
var StyleHandlerImpl = class {
  constructor(elementHandler) {
    this._elmHandler = elementHandler;
  }
  GetComputedStyle(query) {
    const result = this._elmHandler.Get(query);
    if (!result.IsSucceeded || result.Data === null) {
      return AttemptResultWidthDataImpl.Fail(result.Message ?? "");
    }
    try {
      const style = window.getComputedStyle(result.Data);
      return AttemptResultWidthDataImpl.Succeeded(style);
    } catch (ex) {
      return AttemptResultWidthDataImpl.Fail("\u30B9\u30BF\u30A4\u30EB\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002");
    }
  }
};

// src/videoList/sortHandler/elementIDs.ts
var ElementIDs = class {
};
ElementIDs.VideoListRow = ".VideoListRow";
ElementIDs.VideoListRowClassName = "VideoListRow";
ElementIDs.VideoListBodyClassName = "VideoListBody";
ElementIDs.DropTargetClassName = "DropTarget";

// src/videoList/sortHandler/sortHandler.ts
var SortHandlerImpl = class {
  constructor(elmHandler, dotnetHelper) {
    this._sourceNiconicoID = null;
    this._sourceID = null;
    this._lastOverElement = null;
    this._elmHandler = elmHandler;
    this._dotnetHelper = dotnetHelper;
  }
  Initialize() {
    const rowResult = this._elmHandler.GetAll(ElementIDs.VideoListRow);
    if (!rowResult.IsSucceeded || rowResult.Data === null) {
      return;
    }
    rowResult.Data.forEach((elm) => {
      if (elm instanceof HTMLElement) {
        elm.addEventListener("dragstart", (e) => {
          if (!(e.target instanceof HTMLElement)) {
            return;
          }
          const row = this.GetParentByClassName(e.target, ElementIDs.VideoListRowClassName);
          if (row === null) {
            return;
          }
          this._sourceNiconicoID = row.dataset["niconicoid"] ?? null;
          this._sourceID = row.id;
        });
        elm.addEventListener("dragover", (e) => {
          e.preventDefault();
          if (!(e.target instanceof HTMLElement)) {
            return;
          }
          const row = this.GetParentByClassName(e.target, ElementIDs.VideoListRowClassName);
          if (row === null) {
            return;
          }
          if (!row.classList.contains(ElementIDs.DropTargetClassName)) {
            row.classList.add(ElementIDs.DropTargetClassName);
          }
          this._lastOverElement = row;
        });
        elm.addEventListener("dragleave", (e) => {
          e.preventDefault();
          if (!(e.target instanceof HTMLElement)) {
            return;
          }
          const row = this.GetParentByClassName(e.target, ElementIDs.VideoListRowClassName);
          if (row === null) {
            return;
          }
          if (row.classList.contains(ElementIDs.DropTargetClassName)) {
            row.classList.remove(ElementIDs.DropTargetClassName);
          }
        });
        elm.addEventListener("drop", async (e) => {
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
          let parent = e.target.parentNode;
          let dropTarget = e.target;
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
            await this._dotnetHelper.invokeMethodAsync("MoveVideo", this._sourceNiconicoID, dropTarget.dataset["niconicoid"]);
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
  GetParentByClassName(currentElement, className) {
    let parent = currentElement;
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
};

// src/videoList/widthHandler/ElementIds.ts
var ElementIDs2 = class {
};
ElementIDs2.PageContent = ".PageContent";
ElementIDs2.VideoListHeader = "#VideoListHeader";
ElementIDs2.Separator = ".Separator";

// src/videoList/widthHandler/widthHandler.ts
var WidthHandlerImpl = class {
  constructor(elementHandler, styleHandler) {
    this._isResizing = false;
    this._elmHandler = elementHandler;
    this._styleHandler = styleHandler;
    this._columnIDs = {
      "0": "CheckBoxColumn",
      "1": "ThumbnailColumn",
      "2": "TitleColumn",
      "3": "UploadedDateTimeColumn",
      "4": "IsDownloadedColumn",
      "5": "ViewCountColumn",
      "6": "CommentCountColumn",
      "7": "MylistCountColumn",
      "8": "LikeCountColumn",
      "9": "MessageColumn"
    };
    this._separatorIDs = {
      "0": "#CheckBoxColumnSeparator",
      "1": "#ThumbnailColumnSeparator",
      "2": "#TitleColumnSeparator",
      "3": "#UploadedDateTimeColumnSeparator",
      "4": "#IsDownloadedColumnSeparator",
      "5": "#ViewCountColumnSeparator",
      "6": "#CommentCountColumnSeparator",
      "7": "#MylistCountColumnSeparator",
      "8": "#LikeCountColumnSeparator"
    };
  }
  //#endregion
  Initialize() {
    let left = 0;
    for (const key in this._separatorIDs) {
      const sepResult = this._elmHandler.Get(this._separatorIDs[key]);
      if (!sepResult.IsSucceeded || sepResult.Data === null)
        continue;
      const elm = sepResult.Data;
      if (!(elm instanceof HTMLElement))
        continue;
      const styleResult = this._styleHandler.GetComputedStyle(`#${this._columnIDs[key]}`);
      if (styleResult.IsSucceeded && styleResult.Data !== null) {
        const style = styleResult.Data;
        const rawResult = this._elmHandler.GetAll(`.${this._columnIDs[key]}`);
        if (!rawResult.IsSucceeded || rawResult.Data === null) {
          continue;
        }
        if (style.display === "none") {
          elm.style.display = "none";
          rawResult.Data.forEach((raw) => {
            if (raw instanceof HTMLElement) {
              raw.style.display = "none";
            }
          });
          continue;
        } else {
          left += Number(style.width.match(/\d+/));
          elm.style.left = `${left}px`;
          rawResult.Data.forEach((raw) => {
            if (raw instanceof HTMLElement) {
              raw.style.width = style.width;
            }
          });
        }
      }
      const indexS = elm.dataset.index;
      if (indexS == void 0)
        continue;
      elm.addEventListener("mousedown", (_) => this.OnMouseDown(indexS));
    }
    const pageResult = this._elmHandler.Get(ElementIDs2.PageContent);
    if (!pageResult.IsSucceeded || pageResult.Data === null || !(pageResult.Data instanceof HTMLElement))
      return;
    pageResult.Data.addEventListener("mouseup", (_) => this.OnMouseUp());
    const headerResult = this._elmHandler.Get(ElementIDs2.VideoListHeader);
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
    const headerResult = this._elmHandler.Get(`#${this._columnIDs[this._resizingIndex]}`);
    const nextHeaderResult = this._elmHandler.Get(`#${this._columnIDs[`${nextIndex}`]}`);
    const headerWrapperResult = this._elmHandler.Get(ElementIDs2.VideoListHeader);
    const columnResult = this._elmHandler.GetAll(`.${this._columnIDs[this._resizingIndex]}`);
    const nextColumnResult = this._elmHandler.GetAll(`.${this._columnIDs[`${nextIndex}`]}`);
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
function main(blazorView) {
  const elmHandler = new ElementHandlerImpl();
  const styleHandler = new StyleHandlerImpl(elmHandler);
  const widthHandler = new WidthHandlerImpl(elmHandler, styleHandler);
  const sortHandler = new SortHandlerImpl(elmHandler, blazorView);
  widthHandler.Initialize();
  sortHandler.Initialize();
}
export {
  main
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0F0dGVtcHRSZXN1bHQudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0VsZW1lbnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3NoYXJlZC9TdHlsZUhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL2VsZW1lbnRJRHMudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL3NvcnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3ZpZGVvTGlzdC93aWR0aEhhbmRsZXIvRWxlbWVudElkcy50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3Qvd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvbWFpbi50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiZXhwb3J0IGludGVyZmFjZSBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IGV4dGVuZHMgQXR0ZW1wdFJlc3VsdCB7XHJcbiAgICBcclxuICAgIC8qKlxyXG4gICAgICogXHUzMEM3XHUzMEZDXHUzMEJGXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG59XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEF0dGVtcHRSZXN1bHQge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU2MjEwXHU1MjlGXHUzMEQ1XHUzMEU5XHUzMEIwXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHUzMEUxXHUzMEMzXHUzMEJCXHUzMEZDXHUzMEI4XHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcbn1cclxuXHJcblxyXG5leHBvcnQgY2xhc3MgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGw8VD4gaW1wbGVtZW50cyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihpc1N1Y2NlZWRlZDogYm9vbGVhbiwgZGF0YTogVCB8IG51bGwsIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgICAgICB0aGlzLk1lc3NhZ2UgPSBtZXNzYWdlO1xyXG4gICAgfVxyXG5cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG5cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBTdWNjZWVkZWQ8VD4oZGF0YTogVCB8IG51bGwpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKHRydWUsIGRhdGEsIG51bGwpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgRmFpbDxUPihtZXNzYWdlOiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKGZhbHNlLCBudWxsLCBtZXNzYWdlKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIEF0dGVtcHRSZXN1bHRJbXBsIGltcGxlbWVudHMgQXR0ZW1wdFJlc3VsdCB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoaXNTdWNjZWVkZWQ6IGJvb2xlYW4sIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5NZXNzYWdlID0gbWVzc2FnZTtcclxuICAgIH1cclxuXHJcbiAgICByZWFkb25seSBJc1N1Y2NlZWRlZDogYm9vbGVhbjtcclxuXHJcbiAgICByZWFkb25seSBNZXNzYWdlOiBzdHJpbmcgfCBudWxsO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgU3VjY2VlZGVkKCk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwodHJ1ZSwgbnVsbCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBGYWlsKG1lc3NhZ2U6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwoZmFsc2UsIG1lc3NhZ2UpO1xyXG4gICAgfVxyXG59XHJcblxyXG4iLCAiaW1wb3J0IHsgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSwgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwgfSBmcm9tIFwiLi9BdHRlbXB0UmVzdWx0XCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEVsZW1lbnRIYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXQocXVlcnk6c3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PjtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODkwN1x1NjU3MFx1MzA2RVx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj47XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBFbGVtZW50SGFuZGxlckltcGwgaW1wbGVtZW50cyBFbGVtZW50SGFuZGxlciB7XHJcblxyXG4gICAgcHVibGljIEdldChxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiB7XHJcblxyXG4gICAgICAgIGxldCByZXN1bHQ6IEVsZW1lbnQgfCBudWxsO1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gcmVzdWx0ID09IG51bGwgPyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5GYWlsKFwiXHU2MzA3XHU1QjlBXHUzMDU1XHUzMDhDXHUzMDVGXHU4OTgxXHU3RDIwXHUzMDRDXHU4OThCXHUzMDY0XHUzMDRCXHUzMDhBXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDAyXCIpIDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIHB1YmxpYyBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj4ge1xyXG5cclxuICAgICAgICBsZXQgcmVzdWx0OiBOb2RlTGlzdE9mPEVsZW1lbnQ+O1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG59IiwgImltcG9ydCB7IEF0dGVtcHRSZXN1bHRXaWR0aERhdGEsIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsIH0gZnJvbSBcIi4vQXR0ZW1wdFJlc3VsdFwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gXCIuL0VsZW1lbnRIYW5kbGVyXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFN0eWxlSGFuZGxlciB7XHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA2RVx1MzBCOVx1MzBCRlx1MzBBNFx1MzBFQlx1MzA5Mlx1NTNENlx1NUY5N1xyXG4gICAgICovXHJcbiAgICBHZXRDb21wdXRlZFN0eWxlKHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPENTU1N0eWxlRGVjbGFyYXRpb24+O1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU3R5bGVIYW5kbGVySW1wbCBpbXBsZW1lbnRzIFN0eWxlSGFuZGxlciB7XHJcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50SGFuZGxlcjogRWxlbWVudEhhbmRsZXIpIHtcclxuICAgICAgICB0aGlzLl9lbG1IYW5kbGVyID0gZWxlbWVudEhhbmRsZXI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHVibGljIEdldENvbXB1dGVkU3R5bGUocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Q1NTU3R5bGVEZWNsYXJhdGlvbj4ge1xyXG5cclxuICAgICAgICBjb25zdCByZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldChxdWVyeSk7XHJcbiAgICAgICAgaWYgKCFyZXN1bHQuSXNTdWNjZWVkZWQgfHwgcmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwocmVzdWx0Lk1lc3NhZ2UgPz8gXCJcIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICBjb25zdCBzdHlsZSA9IHdpbmRvdy5nZXRDb21wdXRlZFN0eWxlKHJlc3VsdC5EYXRhKTtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLlN1Y2NlZWRlZChzdHlsZSk7XHJcbiAgICAgICAgfSBjYXRjaCAoZXgpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoXCJcdTMwQjlcdTMwQkZcdTMwQTRcdTMwRUJcdTMwOTJcdTUzRDZcdTVGOTdcdTMwNjdcdTMwNERcdTMwN0VcdTMwNUJcdTMwOTNcdTMwNjdcdTMwNTdcdTMwNUZcdTMwMDJcIik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwgImV4cG9ydCBjbGFzcyBFbGVtZW50SURzIHtcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdFJvdyA9ICcuVmlkZW9MaXN0Um93JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdFJvd0NsYXNzTmFtZSA9ICdWaWRlb0xpc3RSb3cnO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgVmlkZW9MaXN0Qm9keUNsYXNzTmFtZSA9ICdWaWRlb0xpc3RCb2R5JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IERyb3BUYXJnZXRDbGFzc05hbWUgPSAnRHJvcFRhcmdldCc7XHJcbn0iLCAiaW1wb3J0IHsgRG90TmV0T2JqZWN0UmVmZXJlbmNlIH0gZnJvbSBcIi4uLy4uL3NoYXJlZC9Eb3ROZXRPYmplY3RSZWZlcmVuY2VcIjtcclxuaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIsIEVsZW1lbnRIYW5kbGVySW1wbCB9IGZyb20gXCIuLi8uLi9zaGFyZWQvRWxlbWVudEhhbmRsZXJcIjtcclxuaW1wb3J0IHsgRWxlbWVudElEcyB9IGZyb20gXCIuL2VsZW1lbnRJRHNcIjtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgU29ydEhhbmRsZXIge1xyXG4gICAgSW5pdGlhbGl6ZSgpOiB2b2lkO1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU29ydEhhbmRsZXJJbXBsIGltcGxlbWVudHMgU29ydEhhbmRsZXIge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyLCBkb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZSkge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbG1IYW5kbGVyO1xyXG4gICAgICAgIHRoaXMuX2RvdG5ldEhlbHBlciA9IGRvdG5ldEhlbHBlcjtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9lbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9kb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZTtcclxuXHJcbiAgICBwcml2YXRlIF9zb3VyY2VOaWNvbmljb0lEOiBzdHJpbmcgfCBudWxsID0gbnVsbDtcclxuXHJcbiAgICBwcml2YXRlIF9zb3VyY2VJRDogc3RyaW5nIHwgbnVsbCA9IG51bGw7XHJcblxyXG4gICAgcHJpdmF0ZSBfbGFzdE92ZXJFbGVtZW50OiBIVE1MRWxlbWVudCB8IG51bGwgPSBudWxsO1xyXG5cclxuXHJcbiAgICBwdWJsaWMgSW5pdGlhbGl6ZSgpOiB2b2lkIHtcclxuICAgICAgICBjb25zdCByb3dSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldEFsbChFbGVtZW50SURzLlZpZGVvTGlzdFJvdyk7XHJcbiAgICAgICAgaWYgKCFyb3dSZXN1bHQuSXNTdWNjZWVkZWQgfHwgcm93UmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcm93UmVzdWx0LkRhdGEuZm9yRWFjaChlbG0gPT4ge1xyXG5cclxuICAgICAgICAgICAgaWYgKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcbiAgICAgICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignZHJhZ3N0YXJ0JywgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCEoZS50YXJnZXQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgcm93OiBIVE1MRWxlbWVudCB8IG51bGwgPSB0aGlzLkdldFBhcmVudEJ5Q2xhc3NOYW1lKGUudGFyZ2V0LCBFbGVtZW50SURzLlZpZGVvTGlzdFJvd0NsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJvdyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9zb3VyY2VOaWNvbmljb0lEID0gcm93LmRhdGFzZXRbJ25pY29uaWNvaWQnXSA/PyBudWxsO1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuX3NvdXJjZUlEID0gcm93LmlkO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2RyYWdvdmVyJywgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHJvdzogSFRNTEVsZW1lbnQgfCBudWxsID0gdGhpcy5HZXRQYXJlbnRCeUNsYXNzTmFtZShlLnRhcmdldCwgRWxlbWVudElEcy5WaWRlb0xpc3RSb3dDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFyb3cuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNsYXNzTGlzdC5hZGQoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5fbGFzdE92ZXJFbGVtZW50ID0gcm93O1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2RyYWdsZWF2ZScsIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIShlLnRhcmdldCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByb3c6IEhUTUxFbGVtZW50IHwgbnVsbCA9IHRoaXMuR2V0UGFyZW50QnlDbGFzc05hbWUoZS50YXJnZXQsIEVsZW1lbnRJRHMuVmlkZW9MaXN0Um93Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocm93ID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNsYXNzTGlzdC5yZW1vdmUoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignZHJvcCcsIGFzeW5jIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuX3NvdXJjZU5pY29uaWNvSUQgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgc291cmNlUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke3RoaXMuX3NvdXJjZUlEfWApO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghc291cmNlUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNvdXJjZVJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxldCBwYXJlbnQ6IFBhcmVudE5vZGUgfCBudWxsID0gZS50YXJnZXQucGFyZW50Tm9kZTtcclxuICAgICAgICAgICAgICAgICAgICBsZXQgZHJvcFRhcmdldDogSFRNTEVsZW1lbnQgPSBlLnRhcmdldDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgd2hpbGUgKHBhcmVudCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoIShwYXJlbnQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKCFwYXJlbnQuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuVmlkZW9MaXN0Qm9keUNsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRyb3BUYXJnZXQgPSBwYXJlbnQ7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQgPSBwYXJlbnQucGFyZW50Tm9kZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQuaW5zZXJ0QmVmb3JlKHNvdXJjZVJlc3VsdC5EYXRhLCBkcm9wVGFyZ2V0KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jKFwiTW92ZVZpZGVvXCIsIHRoaXMuX3NvdXJjZU5pY29uaWNvSUQsIGRyb3BUYXJnZXQuZGF0YXNldFsnbmljb25pY29pZCddISk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhcmVudCA9IG51bGw7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuX2xhc3RPdmVyRWxlbWVudCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5fbGFzdE92ZXJFbGVtZW50LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLl9sYXN0T3ZlckVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZShFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEdldFBhcmVudEJ5Q2xhc3NOYW1lKGN1cnJlbnRFbGVtZW50OiBIVE1MRWxlbWVudCwgY2xhc3NOYW1lOiBzdHJpbmcpOiBIVE1MRWxlbWVudCB8IG51bGwge1xyXG4gICAgICAgIGxldCBwYXJlbnQ6IFBhcmVudE5vZGUgfCBudWxsID0gY3VycmVudEVsZW1lbnQ7XHJcblxyXG4gICAgICAgIHdoaWxlIChwYXJlbnQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgaWYgKCEocGFyZW50IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgaWYgKCFwYXJlbnQuY2xhc3NMaXN0LmNvbnRhaW5zKGNsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgIHBhcmVudCA9IHBhcmVudC5wYXJlbnROb2RlO1xyXG4gICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHJldHVybiBwYXJlbnQ7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgIH1cclxufSIsICJleHBvcnQgY2xhc3MgRWxlbWVudElEcyB7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBQYWdlQ29udGVudCA9ICcuUGFnZUNvbnRlbnQnO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgVmlkZW9MaXN0SGVhZGVyID0gJyNWaWRlb0xpc3RIZWFkZXInO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgU2VwYXJhdG9yID0gJy5TZXBhcmF0b3InO1xyXG5cclxufSIsICJpbXBvcnQgeyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0F0dGVtcHRSZXN1bHQnO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gJy4uLy4uL3NoYXJlZC9FbGVtZW50SGFuZGxlcic7XHJcbmltcG9ydCB7IEVsZW1lbnRJRHMgfSBmcm9tICcuL0VsZW1lbnRJZHMnO1xyXG5pbXBvcnQgeyBEaWN0aW9uYXJ5IH0gZnJvbSAnLi4vLi4vc2hhcmVkL0NvbGxlY3Rpb24vZGljdGlvbmFyeSc7XHJcbmltcG9ydCB7IFN0eWxlSGFuZGxlciB9IGZyb20gJy4uLy4uL3NoYXJlZC9TdHlsZUhhbmRsZXInO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBXaWR0aEhhbmRsZXIge1xyXG4gICAgSW5pdGlhbGl6ZSgpOiB2b2lkO1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgV2lkdGhIYW5kbGVySW1wbCBpbXBsZW1lbnRzIFdpZHRoSGFuZGxlciB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoZWxlbWVudEhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyLCBzdHlsZUhhbmRsZXI6IFN0eWxlSGFuZGxlcikge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbGVtZW50SGFuZGxlcjtcclxuICAgICAgICB0aGlzLl9zdHlsZUhhbmRsZXIgPSBzdHlsZUhhbmRsZXI7XHJcbiAgICAgICAgdGhpcy5fY29sdW1uSURzID0ge1xyXG4gICAgICAgICAgICAnMCc6ICdDaGVja0JveENvbHVtbicsXHJcbiAgICAgICAgICAgICcxJzogJ1RodW1ibmFpbENvbHVtbicsXHJcbiAgICAgICAgICAgICcyJzogJ1RpdGxlQ29sdW1uJyxcclxuICAgICAgICAgICAgJzMnOiAnVXBsb2FkZWREYXRlVGltZUNvbHVtbicsXHJcbiAgICAgICAgICAgICc0JzogJ0lzRG93bmxvYWRlZENvbHVtbicsXHJcbiAgICAgICAgICAgICc1JzogJ1ZpZXdDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc2JzogJ0NvbW1lbnRDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc3JzogJ015bGlzdENvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzgnOiAnTGlrZUNvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzknOiAnTWVzc2FnZUNvbHVtbicsXHJcbiAgICAgICAgfTtcclxuICAgICAgICB0aGlzLl9zZXBhcmF0b3JJRHMgPSB7XHJcbiAgICAgICAgICAgICcwJzogJyNDaGVja0JveENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICcxJzogJyNUaHVtYm5haWxDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMic6ICcjVGl0bGVDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMyc6ICcjVXBsb2FkZWREYXRlVGltZUNvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc0JzogJyNJc0Rvd25sb2FkZWRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNSc6ICcjVmlld0NvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzYnOiAnI0NvbW1lbnRDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc3JzogJyNNeWxpc3RDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc4JzogJyNMaWtlQ291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgIH07XHJcbiAgICB9XHJcblxyXG4gICAgLy8jcmVnaW9uICBmaWVsZFxyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2VsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyO1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX3N0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyO1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2NvbHVtbklEczogRGljdGlvbmFyeTxzdHJpbmc+O1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX3NlcGFyYXRvcklEczogRGljdGlvbmFyeTxzdHJpbmc+O1xyXG5cclxuICAgIHByaXZhdGUgX2lzUmVzaXppbmcgPSBmYWxzZTtcclxuXHJcbiAgICBwcml2YXRlIF9yZXNpemluZ0luZGV4OiBudWxsIHwgc3RyaW5nO1xyXG5cclxuICAgIC8vI2VuZHJlZ2lvblxyXG5cclxuICAgIHB1YmxpYyBJbml0aWFsaXplKCk6IHZvaWQge1xyXG5cclxuICAgICAgICBsZXQgbGVmdCA9IDA7XHJcbiAgICAgICAgZm9yIChjb25zdCBrZXkgaW4gdGhpcy5fc2VwYXJhdG9ySURzKSB7XHJcblxyXG4gICAgICAgICAgICBjb25zdCBzZXBSZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldCh0aGlzLl9zZXBhcmF0b3JJRHNba2V5XSk7XHJcbiAgICAgICAgICAgIGlmICghc2VwUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNlcFJlc3VsdC5EYXRhID09PSBudWxsKSBjb250aW51ZTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGVsbSA9IHNlcFJlc3VsdC5EYXRhO1xyXG4gICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgY29uc3Qgc3R5bGVSZXN1bHQgPSB0aGlzLl9zdHlsZUhhbmRsZXIuR2V0Q29tcHV0ZWRTdHlsZShgIyR7dGhpcy5fY29sdW1uSURzW2tleV19YCk7XHJcbiAgICAgICAgICAgIGlmIChzdHlsZVJlc3VsdC5Jc1N1Y2NlZWRlZCAmJiBzdHlsZVJlc3VsdC5EYXRhICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBzdHlsZTogQ1NTU3R5bGVEZWNsYXJhdGlvbiA9IHN0eWxlUmVzdWx0LkRhdGE7XHJcblxyXG4gICAgICAgICAgICAgICAgY29uc3QgcmF3UmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoYC4ke3RoaXMuX2NvbHVtbklEc1trZXldfWApO1xyXG4gICAgICAgICAgICAgICAgaWYgKCFyYXdSZXN1bHQuSXNTdWNjZWVkZWQgfHwgcmF3UmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBpZiAoc3R5bGUuZGlzcGxheSA9PT0gXCJub25lXCIpIHtcclxuICAgICAgICAgICAgICAgICAgICAvL1x1MzBEOFx1MzBDM1x1MzBDMFx1MzBGQ1x1MzA0Q1x1OTc1RVx1ODg2OFx1NzkzQVx1MzA2QVx1MzA4OVx1MzBCQlx1MzBEMVx1MzBFQ1x1MzBGQ1x1MzBCRlx1MzBGQ1x1MzA4Mlx1OTc1RVx1ODg2OFx1NzkzQVxyXG4gICAgICAgICAgICAgICAgICAgIGVsbS5zdHlsZS5kaXNwbGF5ID0gXCJub25lXCI7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vXHUzMEVBXHUzMEI5XHUzMEM4XHU1MDc0XHUzMDgyXHU5NzVFXHU4ODY4XHU3OTNBXHUzMDZCXHUzMDU5XHUzMDhCXHJcbiAgICAgICAgICAgICAgICAgICAgcmF3UmVzdWx0LkRhdGEuZm9yRWFjaChyYXcgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocmF3IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJhdy5zdHlsZS5kaXNwbGF5ID0gXCJub25lXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgIGxlZnQgKz0gTnVtYmVyKHN0eWxlLndpZHRoLm1hdGNoKC9cXGQrLykpO1xyXG4gICAgICAgICAgICAgICAgICAgIGVsbS5zdHlsZS5sZWZ0ID0gYCR7bGVmdH1weGA7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vXHUzMEVBXHUzMEI5XHUzMEM4XHU1MDc0XHUzMDZFXHU1RTQ1XHUzMDkyXHUzMEQ4XHUzMEMzXHUzMEMwXHUzMEZDXHUzMDZCXHU1NDA4XHUzMDhGXHUzMDVCXHUzMDhCXHJcbiAgICAgICAgICAgICAgICAgICAgcmF3UmVzdWx0LkRhdGEuZm9yRWFjaChyYXcgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocmF3IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJhdy5zdHlsZS53aWR0aCA9IHN0eWxlLndpZHRoO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGluZGV4UyA9IGVsbS5kYXRhc2V0LmluZGV4O1xyXG5cclxuICAgICAgICAgICAgaWYgKGluZGV4UyA9PSB1bmRlZmluZWQpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNlZG93bicsIF8gPT4gdGhpcy5Pbk1vdXNlRG93bihpbmRleFMpKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGNvbnN0IHBhZ2VSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChFbGVtZW50SURzLlBhZ2VDb250ZW50KTtcclxuICAgICAgICBpZiAoIXBhZ2VSZXN1bHQuSXNTdWNjZWVkZWQgfHwgcGFnZVJlc3VsdC5EYXRhID09PSBudWxsIHx8ICEocGFnZVJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgcGFnZVJlc3VsdC5EYXRhLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNldXAnLCBfID0+IHRoaXMuT25Nb3VzZVVwKCkpO1xyXG5cclxuICAgICAgICBjb25zdCBoZWFkZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChFbGVtZW50SURzLlZpZGVvTGlzdEhlYWRlcik7XHJcbiAgICAgICAgaWYgKCFoZWFkZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgaGVhZGVyUmVzdWx0LkRhdGEgPT09IG51bGwgfHwgIShoZWFkZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIGhlYWRlclJlc3VsdC5EYXRhLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNlbW92ZScsIGUgPT4gdGhpcy5Pbk1vdXNlTW92ZShlKSk7XHJcblxyXG4gICAgfVxyXG5cclxuICAgIC8vI3JlZ2lvbiBwcml2YXRlXHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vdXNlRG93bihpbmRleDogc3RyaW5nKTogdm9pZCB7XHJcbiAgICAgICAgdGhpcy5faXNSZXNpemluZyA9IHRydWU7XHJcbiAgICAgICAgdGhpcy5fcmVzaXppbmdJbmRleCA9IGluZGV4O1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgT25Nb3VzZVVwKCk6IHZvaWQge1xyXG4gICAgICAgIHRoaXMuX2lzUmVzaXppbmcgPSBmYWxzZTtcclxuICAgICAgICB0aGlzLl9yZXNpemluZ0luZGV4ID0gbnVsbDtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIE9uTW91c2VNb3ZlKGU6IE1vdXNlRXZlbnQpOiB2b2lkIHtcclxuICAgICAgICBpZiAoIXRoaXMuX2lzUmVzaXppbmcgfHwgdGhpcy5fcmVzaXppbmdJbmRleCA9PT0gbnVsbCkgcmV0dXJuO1xyXG5cclxuICAgICAgICBjb25zdCBuZXh0SW5kZXggPSBOdW1iZXIodGhpcy5fcmVzaXppbmdJbmRleCkgKyAxO1xyXG5cclxuICAgICAgICBjb25zdCBoZWFkZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChgIyR7dGhpcy5fY29sdW1uSURzW3RoaXMuX3Jlc2l6aW5nSW5kZXhdfWApO1xyXG4gICAgICAgIGNvbnN0IG5leHRIZWFkZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChgIyR7dGhpcy5fY29sdW1uSURzW2Ake25leHRJbmRleH1gXX1gKTtcclxuICAgICAgICBjb25zdCBoZWFkZXJXcmFwcGVyUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoRWxlbWVudElEcy5WaWRlb0xpc3RIZWFkZXIpO1xyXG4gICAgICAgIGNvbnN0IGNvbHVtblJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKGAuJHt0aGlzLl9jb2x1bW5JRHNbdGhpcy5fcmVzaXppbmdJbmRleF19YCk7XHJcbiAgICAgICAgY29uc3QgbmV4dENvbHVtblJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKGAuJHt0aGlzLl9jb2x1bW5JRHNbYCR7bmV4dEluZGV4fWBdfWApO1xyXG4gICAgICAgIGNvbnN0IHNlcFJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KHRoaXMuX3NlcGFyYXRvcklEc1t0aGlzLl9yZXNpemluZ0luZGV4XSk7XHJcblxyXG4gICAgICAgIGlmICghaGVhZGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IGhlYWRlclJlc3VsdC5EYXRhID09PSBudWxsIHx8ICFjb2x1bW5SZXN1bHQuSXNTdWNjZWVkZWQgfHwgY29sdW1uUmVzdWx0LkRhdGEgPT09IG51bGwgfHwgIXNlcFJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzZXBSZXN1bHQuRGF0YSA9PT0gbnVsbCB8fCAhaGVhZGVyV3JhcHBlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEgPT09IG51bGwgfHwgIW5leHRIZWFkZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgbmV4dEhlYWRlclJlc3VsdC5EYXRhID09PSBudWxsIHx8ICFuZXh0Q29sdW1uUmVzdWx0LklzU3VjY2VlZGVkIHx8IG5leHRDb2x1bW5SZXN1bHQuRGF0YSA9PT0gbnVsbCkgcmV0dXJuO1xyXG5cclxuICAgICAgICBpZiAoIShoZWFkZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIGlmICghKG5leHRIZWFkZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICBjb25zdCBoZWFkZXJSZWN0OiBET01SZWN0ID0gaGVhZGVyUmVzdWx0LkRhdGEuZ2V0Qm91bmRpbmdDbGllbnRSZWN0KCk7XHJcbiAgICAgICAgY29uc3QgaGVhZGVyV3JhcHBlclJlY3Q6IERPTVJlY3QgPSBoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEuZ2V0Qm91bmRpbmdDbGllbnRSZWN0KCk7XHJcblxyXG5cclxuICAgICAgICBjb25zdCB3aWR0aCA9IGUuY2xpZW50WCAtIGhlYWRlclJlY3QubGVmdDtcclxuICAgICAgICBjb25zdCBkZWx0YVdpZHRoID0gd2lkdGggLSBoZWFkZXJSZXN1bHQuRGF0YS5vZmZzZXRXaWR0aDtcclxuICAgICAgICBjb25zdCBuZXh0V2lkdGggPSBuZXh0SGVhZGVyUmVzdWx0LkRhdGEub2Zmc2V0V2lkdGggLSBkZWx0YVdpZHRoO1xyXG5cclxuICAgICAgICBoZWFkZXJSZXN1bHQuRGF0YS5zdHlsZS53aWR0aCA9IGAke3dpZHRofXB4YDtcclxuICAgICAgICBuZXh0SGVhZGVyUmVzdWx0LkRhdGEuc3R5bGUud2lkdGggPSBgJHtuZXh0V2lkdGh9cHhgO1xyXG5cclxuICAgICAgICBjb2x1bW5SZXN1bHQuRGF0YS5mb3JFYWNoKGVsbSA9PiB7XHJcbiAgICAgICAgICAgIGlmICghKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICAgICAgZWxtLnN0eWxlLndpZHRoID0gYCR7d2lkdGh9cHhgO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBuZXh0Q29sdW1uUmVzdWx0LkRhdGEuZm9yRWFjaChlbG0gPT4ge1xyXG4gICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgIGVsbS5zdHlsZS53aWR0aCA9IGAke25leHRXaWR0aH1weGA7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIGlmICghKGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIGlmICghKHNlcFJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IGxlZnQgPSBoZWFkZXJSZWN0LmxlZnQgLSBoZWFkZXJXcmFwcGVyUmVjdC5sZWZ0ICsgd2lkdGggLSAxMDtcclxuICAgICAgICBzZXBSZXN1bHQuRGF0YS5zdHlsZS5sZWZ0ID0gYCR7bGVmdH1weGA7XHJcbiAgICB9XHJcblxyXG4gICAgLy8jZW5kcmVnaW9uXHJcbn1cclxuXHJcbmludGVyZmFjZSBDbGFzc05hbWVzRGljdCB7XHJcbiAgICBbaW5kZXg6IG51bWJlcl06IHN0cmluZztcclxufSIsICJpbXBvcnQgeyBEb3ROZXRPYmplY3RSZWZlcmVuY2UgfSBmcm9tIFwiLi4vc2hhcmVkL0RvdE5ldE9iamVjdFJlZmVyZW5jZVwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciwgRWxlbWVudEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4uL3NoYXJlZC9FbGVtZW50SGFuZGxlclwiO1xyXG5pbXBvcnQgeyBTdHlsZUhhbmRsZXIsIFN0eWxlSGFuZGxlckltcGwgfSBmcm9tIFwiLi4vc2hhcmVkL1N0eWxlSGFuZGxlclwiO1xyXG5pbXBvcnQgeyBTb3J0SGFuZGxlciwgU29ydEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vc29ydEhhbmRsZXIvc29ydEhhbmRsZXJcIjtcclxuaW1wb3J0IHsgV2lkdGhIYW5kbGVyLCBXaWR0aEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlclwiO1xyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIG1haW4oYmxhem9yVmlldzogRG90TmV0T2JqZWN0UmVmZXJlbmNlKSB7XHJcbiAgICBjb25zdCBlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlciA9IG5ldyBFbGVtZW50SGFuZGxlckltcGwoKTtcclxuICAgIGNvbnN0IHN0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyID0gbmV3IFN0eWxlSGFuZGxlckltcGwoZWxtSGFuZGxlcik7XHJcbiAgICBjb25zdCB3aWR0aEhhbmRsZXI6IFdpZHRoSGFuZGxlciA9IG5ldyBXaWR0aEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIsIHN0eWxlSGFuZGxlcik7XHJcbiAgICBjb25zdCBzb3J0SGFuZGxlcjogU29ydEhhbmRsZXIgPSBuZXcgU29ydEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIsIGJsYXpvclZpZXcpXHJcblxyXG4gICAgd2lkdGhIYW5kbGVyLkluaXRpYWxpemUoKTtcclxuICAgIHNvcnRIYW5kbGVyLkluaXRpYWxpemUoKTtcclxufSJdLAogICJtYXBwaW5ncyI6ICI7QUFzQk8sSUFBTSw2QkFBTixNQUF5RTtBQUFBLEVBRTVFLFlBQVksYUFBc0IsTUFBZ0IsU0FBd0I7QUFDdEUsU0FBSyxjQUFjO0FBQ25CLFNBQUssT0FBTztBQUNaLFNBQUssVUFBVTtBQUFBLEVBQ25CO0FBQUEsRUFRQSxPQUFjLFVBQWEsTUFBMkM7QUFDbEUsV0FBTyxJQUFJLDJCQUEyQixNQUFNLE1BQU0sSUFBSTtBQUFBLEVBQzFEO0FBQUEsRUFFQSxPQUFjLEtBQVEsU0FBNEM7QUFDOUQsV0FBTyxJQUFJLDJCQUEyQixPQUFPLE1BQU0sT0FBTztBQUFBLEVBQzlEO0FBQ0o7OztBQzFCTyxJQUFNLHFCQUFOLE1BQW1EO0FBQUEsRUFFL0MsSUFBSSxPQUFnRDtBQUV2RCxRQUFJO0FBRUosUUFBSTtBQUNBLGVBQVMsU0FBUyxjQUFjLEtBQUs7QUFBQSxJQUN6QyxTQUFTLEdBQVA7QUFDRSxhQUFPLDJCQUEyQixLQUFLLDBHQUFxQixFQUFFLFVBQVU7QUFBQSxJQUM1RTtBQUVBLFdBQU8sVUFBVSxPQUFPLDJCQUEyQixLQUFLLGtHQUFrQixJQUFJLDJCQUEyQixVQUFVLE1BQU07QUFBQSxFQUM3SDtBQUFBLEVBR08sT0FBTyxPQUE0RDtBQUV0RSxRQUFJO0FBRUosUUFBSTtBQUNBLGVBQVMsU0FB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
