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
  initialize() {
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
  constructor(elementHandler, styleHandler, dotnetHelper) {
    this._isResizing = false;
    this._elmHandler = elementHandler;
    this._styleHandler = styleHandler;
    this._dotnetHelper = dotnetHelper;
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
  async initialize() {
    for (const key in this._separatorIDs) {
      const sepResult = this._elmHandler.Get(this._separatorIDs[key]);
      if (!sepResult.IsSucceeded || sepResult.Data === null)
        continue;
      const elm = sepResult.Data;
      if (!(elm instanceof HTMLElement))
        continue;
      const indexS = elm.dataset.index;
      if (indexS == void 0)
        continue;
      elm.addEventListener("mousedown", (_) => this.OnMouseDown(indexS));
    }
    await this.setWidth();
    const pageResult = this._elmHandler.Get(ElementIDs2.PageContent);
    if (!pageResult.IsSucceeded || pageResult.Data === null || !(pageResult.Data instanceof HTMLElement))
      return;
    pageResult.Data.addEventListener("mouseup", (_) => this.OnMouseUp());
    const headerWrapperResult = this._elmHandler.Get(ElementIDs2.VideoListHeader);
    if (!headerWrapperResult.IsSucceeded || headerWrapperResult.Data === null || !(headerWrapperResult.Data instanceof HTMLElement))
      return;
    headerWrapperResult.Data.addEventListener("mousemove", (e) => this.OnMouseMove(e));
  }
  async setWidth() {
    let left = 0;
    for (const key in this._columnIDs) {
      let elm = null;
      if (key in this._separatorIDs) {
        const sepResult = this._elmHandler.Get(this._separatorIDs[key]);
        if (!sepResult.IsSucceeded || sepResult.Data === null)
          continue;
        elm = sepResult.Data;
        if (!(elm instanceof HTMLElement))
          continue;
      }
      const styleResult = this._styleHandler.GetComputedStyle(`#${this._columnIDs[key]}`);
      if (styleResult.IsSucceeded && styleResult.Data !== null) {
        const style = styleResult.Data;
        const rawResult = this._elmHandler.GetAll(`.${this._columnIDs[key]}`);
        if (!rawResult.IsSucceeded || rawResult.Data === null) {
          continue;
        }
        if (style.display === "none") {
          if (elm !== null) {
            elm.style.display = "none";
          }
          rawResult.Data.forEach((raw) => {
            if (raw instanceof HTMLElement) {
              raw.style.display = "none";
            }
          });
          continue;
        } else {
          const restoreWidth = await this._dotnetHelper.invokeMethodAsync("GetWidth", this._columnIDs[key]);
          const shouldRestoreWidth = restoreWidth > 0;
          const width = shouldRestoreWidth ? restoreWidth : Number(style.width.match(/\d+/));
          left += width;
          if (elm !== null) {
            elm.style.left = `${left}px`;
          }
          if (shouldRestoreWidth) {
            const headerResult = this._elmHandler.Get(`#${this._columnIDs[key]}`);
            if (headerResult.IsSucceeded && headerResult.Data !== null) {
              if (headerResult.Data instanceof HTMLElement) {
                headerResult.Data.style.width = `${width}px`;
              }
            }
          }
          rawResult.Data.forEach((raw) => {
            if (raw instanceof HTMLElement) {
              raw.style.width = shouldRestoreWidth ? `${width}px` : style.width;
            }
          });
        }
      }
    }
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
  async OnMouseMove(e) {
    if (!this._isResizing || this._resizingIndex === null)
      return;
    const nextIndex = Number(this._resizingIndex) + 1;
    const resizingName = this._columnIDs[this._resizingIndex];
    const nextName = this._columnIDs[`${nextIndex}`];
    const headerResult = this._elmHandler.Get(`#${resizingName}`);
    const nextHeaderResult = this._elmHandler.Get(`#${nextName}`);
    const headerWrapperResult = this._elmHandler.Get(ElementIDs2.VideoListHeader);
    const columnResult = this._elmHandler.GetAll(`.${resizingName}`);
    const nextColumnResult = this._elmHandler.GetAll(`.${nextName}`);
    const sepResult = this._elmHandler.Get(this._separatorIDs[this._resizingIndex]);
    if (!headerResult.IsSucceeded || headerResult.Data === null) {
      return;
    }
    if (!columnResult.IsSucceeded || columnResult.Data === null) {
      return;
    }
    if (!sepResult.IsSucceeded || sepResult.Data === null) {
      return;
    }
    if (!headerWrapperResult.IsSucceeded || headerWrapperResult.Data === null) {
      return;
    }
    if (!nextHeaderResult.IsSucceeded || nextHeaderResult.Data === null) {
      return;
    }
    if (!nextColumnResult.IsSucceeded || nextColumnResult.Data === null) {
      return;
    }
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
    await this._dotnetHelper.invokeMethodAsync("SetWidth", `${width}`, resizingName);
    await this._dotnetHelper.invokeMethodAsync("SetWidth", `${nextWidth}`, nextName);
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
async function initialize(blazorView) {
  const elmHandler = new ElementHandlerImpl();
  const styleHandler = new StyleHandlerImpl(elmHandler);
  const widthHandler = new WidthHandlerImpl(elmHandler, styleHandler, blazorView);
  const sortHandler = new SortHandlerImpl(elmHandler, blazorView);
  await widthHandler.initialize();
  sortHandler.initialize();
}
async function setWidth(blazorView) {
  const elmHandler = new ElementHandlerImpl();
  const styleHandler = new StyleHandlerImpl(elmHandler);
  const widthHandler = new WidthHandlerImpl(elmHandler, styleHandler, blazorView);
  await widthHandler.setWidth();
}
export {
  initialize,
  setWidth
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0F0dGVtcHRSZXN1bHQudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0VsZW1lbnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3NoYXJlZC9TdHlsZUhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL2VsZW1lbnRJRHMudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL3NvcnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3ZpZGVvTGlzdC93aWR0aEhhbmRsZXIvRWxlbWVudElkcy50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3Qvd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvbWFpbi50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiZXhwb3J0IGludGVyZmFjZSBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IGV4dGVuZHMgQXR0ZW1wdFJlc3VsdCB7XHJcbiAgICBcclxuICAgIC8qKlxyXG4gICAgICogXHUzMEM3XHUzMEZDXHUzMEJGXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG59XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEF0dGVtcHRSZXN1bHQge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU2MjEwXHU1MjlGXHUzMEQ1XHUzMEU5XHUzMEIwXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHUzMEUxXHUzMEMzXHUzMEJCXHUzMEZDXHUzMEI4XHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcbn1cclxuXHJcblxyXG5leHBvcnQgY2xhc3MgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGw8VD4gaW1wbGVtZW50cyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihpc1N1Y2NlZWRlZDogYm9vbGVhbiwgZGF0YTogVCB8IG51bGwsIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgICAgICB0aGlzLk1lc3NhZ2UgPSBtZXNzYWdlO1xyXG4gICAgfVxyXG5cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG5cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBTdWNjZWVkZWQ8VD4oZGF0YTogVCB8IG51bGwpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKHRydWUsIGRhdGEsIG51bGwpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgRmFpbDxUPihtZXNzYWdlOiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKGZhbHNlLCBudWxsLCBtZXNzYWdlKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIEF0dGVtcHRSZXN1bHRJbXBsIGltcGxlbWVudHMgQXR0ZW1wdFJlc3VsdCB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoaXNTdWNjZWVkZWQ6IGJvb2xlYW4sIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5NZXNzYWdlID0gbWVzc2FnZTtcclxuICAgIH1cclxuXHJcbiAgICByZWFkb25seSBJc1N1Y2NlZWRlZDogYm9vbGVhbjtcclxuXHJcbiAgICByZWFkb25seSBNZXNzYWdlOiBzdHJpbmcgfCBudWxsO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgU3VjY2VlZGVkKCk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwodHJ1ZSwgbnVsbCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBGYWlsKG1lc3NhZ2U6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwoZmFsc2UsIG1lc3NhZ2UpO1xyXG4gICAgfVxyXG59XHJcblxyXG4iLCAiaW1wb3J0IHsgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSwgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwgfSBmcm9tIFwiLi9BdHRlbXB0UmVzdWx0XCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEVsZW1lbnRIYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXQocXVlcnk6c3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PjtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODkwN1x1NjU3MFx1MzA2RVx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj47XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBFbGVtZW50SGFuZGxlckltcGwgaW1wbGVtZW50cyBFbGVtZW50SGFuZGxlciB7XHJcblxyXG4gICAgcHVibGljIEdldChxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiB7XHJcblxyXG4gICAgICAgIGxldCByZXN1bHQ6IEVsZW1lbnQgfCBudWxsO1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gcmVzdWx0ID09IG51bGwgPyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5GYWlsKFwiXHU2MzA3XHU1QjlBXHUzMDU1XHUzMDhDXHUzMDVGXHU4OTgxXHU3RDIwXHUzMDRDXHU4OThCXHUzMDY0XHUzMDRCXHUzMDhBXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDAyXCIpIDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIHB1YmxpYyBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj4ge1xyXG5cclxuICAgICAgICBsZXQgcmVzdWx0OiBOb2RlTGlzdE9mPEVsZW1lbnQ+O1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG59IiwgImltcG9ydCB7IEF0dGVtcHRSZXN1bHRXaWR0aERhdGEsIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsIH0gZnJvbSBcIi4vQXR0ZW1wdFJlc3VsdFwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gXCIuL0VsZW1lbnRIYW5kbGVyXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFN0eWxlSGFuZGxlciB7XHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA2RVx1MzBCOVx1MzBCRlx1MzBBNFx1MzBFQlx1MzA5Mlx1NTNENlx1NUY5N1xyXG4gICAgICovXHJcbiAgICBHZXRDb21wdXRlZFN0eWxlKHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPENTU1N0eWxlRGVjbGFyYXRpb24+O1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU3R5bGVIYW5kbGVySW1wbCBpbXBsZW1lbnRzIFN0eWxlSGFuZGxlciB7XHJcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50SGFuZGxlcjogRWxlbWVudEhhbmRsZXIpIHtcclxuICAgICAgICB0aGlzLl9lbG1IYW5kbGVyID0gZWxlbWVudEhhbmRsZXI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHVibGljIEdldENvbXB1dGVkU3R5bGUocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Q1NTU3R5bGVEZWNsYXJhdGlvbj4ge1xyXG5cclxuICAgICAgICBjb25zdCByZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldChxdWVyeSk7XHJcbiAgICAgICAgaWYgKCFyZXN1bHQuSXNTdWNjZWVkZWQgfHwgcmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwocmVzdWx0Lk1lc3NhZ2UgPz8gXCJcIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICBjb25zdCBzdHlsZSA9IHdpbmRvdy5nZXRDb21wdXRlZFN0eWxlKHJlc3VsdC5EYXRhKTtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLlN1Y2NlZWRlZChzdHlsZSk7XHJcbiAgICAgICAgfSBjYXRjaCAoZXgpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoXCJcdTMwQjlcdTMwQkZcdTMwQTRcdTMwRUJcdTMwOTJcdTUzRDZcdTVGOTdcdTMwNjdcdTMwNERcdTMwN0VcdTMwNUJcdTMwOTNcdTMwNjdcdTMwNTdcdTMwNUZcdTMwMDJcIik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwgImV4cG9ydCBjbGFzcyBFbGVtZW50SURzIHtcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdFJvdyA9ICcuVmlkZW9MaXN0Um93JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdFJvd0NsYXNzTmFtZSA9ICdWaWRlb0xpc3RSb3cnO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgVmlkZW9MaXN0Qm9keUNsYXNzTmFtZSA9ICdWaWRlb0xpc3RCb2R5JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IERyb3BUYXJnZXRDbGFzc05hbWUgPSAnRHJvcFRhcmdldCc7XHJcbn0iLCAiaW1wb3J0IHsgRG90TmV0T2JqZWN0UmVmZXJlbmNlIH0gZnJvbSBcIi4uLy4uL3NoYXJlZC9Eb3ROZXRPYmplY3RSZWZlcmVuY2VcIjtcclxuaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIsIEVsZW1lbnRIYW5kbGVySW1wbCB9IGZyb20gXCIuLi8uLi9zaGFyZWQvRWxlbWVudEhhbmRsZXJcIjtcclxuaW1wb3J0IHsgRWxlbWVudElEcyB9IGZyb20gXCIuL2VsZW1lbnRJRHNcIjtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgU29ydEhhbmRsZXIge1xyXG4gICAgaW5pdGlhbGl6ZSgpOiB2b2lkO1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU29ydEhhbmRsZXJJbXBsIGltcGxlbWVudHMgU29ydEhhbmRsZXIge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyLCBkb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZSkge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbG1IYW5kbGVyO1xyXG4gICAgICAgIHRoaXMuX2RvdG5ldEhlbHBlciA9IGRvdG5ldEhlbHBlcjtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9lbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9kb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZTtcclxuXHJcbiAgICBwcml2YXRlIF9zb3VyY2VOaWNvbmljb0lEOiBzdHJpbmcgfCBudWxsID0gbnVsbDtcclxuXHJcbiAgICBwcml2YXRlIF9zb3VyY2VJRDogc3RyaW5nIHwgbnVsbCA9IG51bGw7XHJcblxyXG4gICAgcHJpdmF0ZSBfbGFzdE92ZXJFbGVtZW50OiBIVE1MRWxlbWVudCB8IG51bGwgPSBudWxsO1xyXG5cclxuXHJcbiAgICBwdWJsaWMgaW5pdGlhbGl6ZSgpOiB2b2lkIHtcclxuICAgICAgICBjb25zdCByb3dSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldEFsbChFbGVtZW50SURzLlZpZGVvTGlzdFJvdyk7XHJcbiAgICAgICAgaWYgKCFyb3dSZXN1bHQuSXNTdWNjZWVkZWQgfHwgcm93UmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcm93UmVzdWx0LkRhdGEuZm9yRWFjaChlbG0gPT4ge1xyXG5cclxuICAgICAgICAgICAgaWYgKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcbiAgICAgICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignZHJhZ3N0YXJ0JywgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCEoZS50YXJnZXQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgcm93OiBIVE1MRWxlbWVudCB8IG51bGwgPSB0aGlzLkdldFBhcmVudEJ5Q2xhc3NOYW1lKGUudGFyZ2V0LCBFbGVtZW50SURzLlZpZGVvTGlzdFJvd0NsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJvdyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9zb3VyY2VOaWNvbmljb0lEID0gcm93LmRhdGFzZXRbJ25pY29uaWNvaWQnXSA/PyBudWxsO1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuX3NvdXJjZUlEID0gcm93LmlkO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2RyYWdvdmVyJywgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHJvdzogSFRNTEVsZW1lbnQgfCBudWxsID0gdGhpcy5HZXRQYXJlbnRCeUNsYXNzTmFtZShlLnRhcmdldCwgRWxlbWVudElEcy5WaWRlb0xpc3RSb3dDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFyb3cuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNsYXNzTGlzdC5hZGQoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5fbGFzdE92ZXJFbGVtZW50ID0gcm93O1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2RyYWdsZWF2ZScsIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIShlLnRhcmdldCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByb3c6IEhUTUxFbGVtZW50IHwgbnVsbCA9IHRoaXMuR2V0UGFyZW50QnlDbGFzc05hbWUoZS50YXJnZXQsIEVsZW1lbnRJRHMuVmlkZW9MaXN0Um93Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocm93ID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNsYXNzTGlzdC5yZW1vdmUoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignZHJvcCcsIGFzeW5jIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuX3NvdXJjZU5pY29uaWNvSUQgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgc291cmNlUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke3RoaXMuX3NvdXJjZUlEfWApO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghc291cmNlUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNvdXJjZVJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxldCBwYXJlbnQ6IFBhcmVudE5vZGUgfCBudWxsID0gZS50YXJnZXQucGFyZW50Tm9kZTtcclxuICAgICAgICAgICAgICAgICAgICBsZXQgZHJvcFRhcmdldDogSFRNTEVsZW1lbnQgPSBlLnRhcmdldDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgd2hpbGUgKHBhcmVudCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoIShwYXJlbnQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKCFwYXJlbnQuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuVmlkZW9MaXN0Qm9keUNsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRyb3BUYXJnZXQgPSBwYXJlbnQ7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQgPSBwYXJlbnQucGFyZW50Tm9kZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQuaW5zZXJ0QmVmb3JlKHNvdXJjZVJlc3VsdC5EYXRhLCBkcm9wVGFyZ2V0KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jKFwiTW92ZVZpZGVvXCIsIHRoaXMuX3NvdXJjZU5pY29uaWNvSUQsIGRyb3BUYXJnZXQuZGF0YXNldFsnbmljb25pY29pZCddISk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhcmVudCA9IG51bGw7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuX2xhc3RPdmVyRWxlbWVudCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5fbGFzdE92ZXJFbGVtZW50LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLl9sYXN0T3ZlckVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZShFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEdldFBhcmVudEJ5Q2xhc3NOYW1lKGN1cnJlbnRFbGVtZW50OiBIVE1MRWxlbWVudCwgY2xhc3NOYW1lOiBzdHJpbmcpOiBIVE1MRWxlbWVudCB8IG51bGwge1xyXG4gICAgICAgIGxldCBwYXJlbnQ6IFBhcmVudE5vZGUgfCBudWxsID0gY3VycmVudEVsZW1lbnQ7XHJcblxyXG4gICAgICAgIHdoaWxlIChwYXJlbnQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgaWYgKCEocGFyZW50IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgaWYgKCFwYXJlbnQuY2xhc3NMaXN0LmNvbnRhaW5zKGNsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgIHBhcmVudCA9IHBhcmVudC5wYXJlbnROb2RlO1xyXG4gICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHJldHVybiBwYXJlbnQ7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgIH1cclxufSIsICJleHBvcnQgY2xhc3MgRWxlbWVudElEcyB7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBQYWdlQ29udGVudCA9ICcuUGFnZUNvbnRlbnQnO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgVmlkZW9MaXN0SGVhZGVyID0gJyNWaWRlb0xpc3RIZWFkZXInO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgU2VwYXJhdG9yID0gJy5TZXBhcmF0b3InO1xyXG5cclxufSIsICJpbXBvcnQgeyBBdHRlbXB0UmVzdWx0LCBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0F0dGVtcHRSZXN1bHQnO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gJy4uLy4uL3NoYXJlZC9FbGVtZW50SGFuZGxlcic7XHJcbmltcG9ydCB7IEVsZW1lbnRJRHMgfSBmcm9tICcuL0VsZW1lbnRJZHMnO1xyXG5pbXBvcnQgeyBEaWN0aW9uYXJ5IH0gZnJvbSAnLi4vLi4vc2hhcmVkL0NvbGxlY3Rpb24vZGljdGlvbmFyeSc7XHJcbmltcG9ydCB7IFN0eWxlSGFuZGxlciB9IGZyb20gJy4uLy4uL3NoYXJlZC9TdHlsZUhhbmRsZXInO1xyXG5pbXBvcnQgeyBEb3ROZXRPYmplY3RSZWZlcmVuY2UgfSBmcm9tICcuLi8uLi9zaGFyZWQvRG90TmV0T2JqZWN0UmVmZXJlbmNlJztcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgV2lkdGhIYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1NTIxRFx1NjcxRlx1NTMxNlxyXG4gICAgICovXHJcbiAgICBpbml0aWFsaXplKCk6IFByb21pc2U8dm9pZD47XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBcdTVFNDVcdTMwOTJcdTUxOERcdThBMkRcdTVCOUFcclxuICAgICAqL1xyXG4gICAgc2V0V2lkdGgoKTogUHJvbWlzZTx2b2lkPjtcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFdpZHRoSGFuZGxlckltcGwgaW1wbGVtZW50cyBXaWR0aEhhbmRsZXIge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGVsZW1lbnRIYW5kbGVyOiBFbGVtZW50SGFuZGxlciwgc3R5bGVIYW5kbGVyOiBTdHlsZUhhbmRsZXIsIGRvdG5ldEhlbHBlcjogRG90TmV0T2JqZWN0UmVmZXJlbmNlKSB7XHJcbiAgICAgICAgdGhpcy5fZWxtSGFuZGxlciA9IGVsZW1lbnRIYW5kbGVyO1xyXG4gICAgICAgIHRoaXMuX3N0eWxlSGFuZGxlciA9IHN0eWxlSGFuZGxlcjtcclxuICAgICAgICB0aGlzLl9kb3RuZXRIZWxwZXIgPSBkb3RuZXRIZWxwZXI7XHJcbiAgICAgICAgdGhpcy5fY29sdW1uSURzID0ge1xyXG4gICAgICAgICAgICAnMCc6ICdDaGVja0JveENvbHVtbicsXHJcbiAgICAgICAgICAgICcxJzogJ1RodW1ibmFpbENvbHVtbicsXHJcbiAgICAgICAgICAgICcyJzogJ1RpdGxlQ29sdW1uJyxcclxuICAgICAgICAgICAgJzMnOiAnVXBsb2FkZWREYXRlVGltZUNvbHVtbicsXHJcbiAgICAgICAgICAgICc0JzogJ0lzRG93bmxvYWRlZENvbHVtbicsXHJcbiAgICAgICAgICAgICc1JzogJ1ZpZXdDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc2JzogJ0NvbW1lbnRDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc3JzogJ015bGlzdENvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzgnOiAnTGlrZUNvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzknOiAnTWVzc2FnZUNvbHVtbicsXHJcbiAgICAgICAgfTtcclxuICAgICAgICB0aGlzLl9zZXBhcmF0b3JJRHMgPSB7XHJcbiAgICAgICAgICAgICcwJzogJyNDaGVja0JveENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICcxJzogJyNUaHVtYm5haWxDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMic6ICcjVGl0bGVDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMyc6ICcjVXBsb2FkZWREYXRlVGltZUNvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc0JzogJyNJc0Rvd25sb2FkZWRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNSc6ICcjVmlld0NvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzYnOiAnI0NvbW1lbnRDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc3JzogJyNNeWxpc3RDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc4JzogJyNMaWtlQ291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgIH07XHJcbiAgICB9XHJcblxyXG4gICAgLy8jcmVnaW9uICBmaWVsZFxyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2VsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyO1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX3N0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyO1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2NvbHVtbklEczogRGljdGlvbmFyeTxzdHJpbmc+O1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX3NlcGFyYXRvcklEczogRGljdGlvbmFyeTxzdHJpbmc+O1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2RvdG5ldEhlbHBlcjogRG90TmV0T2JqZWN0UmVmZXJlbmNlO1xyXG5cclxuICAgIHByaXZhdGUgX2lzUmVzaXppbmcgPSBmYWxzZTtcclxuXHJcbiAgICBwcml2YXRlIF9yZXNpemluZ0luZGV4OiBudWxsIHwgc3RyaW5nO1xyXG5cclxuICAgIC8vI2VuZHJlZ2lvblxyXG5cclxuICAgIHB1YmxpYyBhc3luYyBpbml0aWFsaXplKCk6IFByb21pc2U8dm9pZD4ge1xyXG5cclxuICAgICAgICBmb3IgKGNvbnN0IGtleSBpbiB0aGlzLl9zZXBhcmF0b3JJRHMpIHtcclxuXHJcblxyXG4gICAgICAgICAgICBjb25zdCBzZXBSZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldCh0aGlzLl9zZXBhcmF0b3JJRHNba2V5XSk7XHJcbiAgICAgICAgICAgIGlmICghc2VwUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNlcFJlc3VsdC5EYXRhID09PSBudWxsKSBjb250aW51ZTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGVsbTogRWxlbWVudCA9IHNlcFJlc3VsdC5EYXRhO1xyXG4gICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgY29uc3QgaW5kZXhTID0gZWxtLmRhdGFzZXQuaW5kZXg7XHJcblxyXG4gICAgICAgICAgICBpZiAoaW5kZXhTID09IHVuZGVmaW5lZCkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignbW91c2Vkb3duJywgXyA9PiB0aGlzLk9uTW91c2VEb3duKGluZGV4UykpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgYXdhaXQgdGhpcy5zZXRXaWR0aCgpO1xyXG5cclxuICAgICAgICBjb25zdCBwYWdlUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoRWxlbWVudElEcy5QYWdlQ29udGVudCk7XHJcbiAgICAgICAgaWYgKCFwYWdlUmVzdWx0LklzU3VjY2VlZGVkIHx8IHBhZ2VSZXN1bHQuRGF0YSA9PT0gbnVsbCB8fCAhKHBhZ2VSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIHBhZ2VSZXN1bHQuRGF0YS5hZGRFdmVudExpc3RlbmVyKCdtb3VzZXVwJywgXyA9PiB0aGlzLk9uTW91c2VVcCgpKTtcclxuXHJcbiAgICAgICAgY29uc3QgaGVhZGVyV3JhcHBlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KEVsZW1lbnRJRHMuVmlkZW9MaXN0SGVhZGVyKTtcclxuICAgICAgICBpZiAoIWhlYWRlcldyYXBwZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhID09PSBudWxsIHx8ICEoaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNlbW92ZScsIGUgPT4gdGhpcy5Pbk1vdXNlTW92ZShlKSk7XHJcblxyXG4gICAgfVxyXG4gICAgcHVibGljIGFzeW5jIHNldFdpZHRoKCk6IFByb21pc2U8dm9pZD4ge1xyXG4gICAgICAgIGxldCBsZWZ0ID0gMDtcclxuICAgICAgICBmb3IgKGNvbnN0IGtleSBpbiB0aGlzLl9jb2x1bW5JRHMpIHtcclxuXHJcbiAgICAgICAgICAgIGxldCBlbG06IEVsZW1lbnQgfCBudWxsID0gbnVsbDtcclxuXHJcbiAgICAgICAgICAgIGlmIChrZXkgaW4gdGhpcy5fc2VwYXJhdG9ySURzKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBzZXBSZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldCh0aGlzLl9zZXBhcmF0b3JJRHNba2V5XSk7XHJcbiAgICAgICAgICAgICAgICBpZiAoIXNlcFJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzZXBSZXN1bHQuRGF0YSA9PT0gbnVsbCkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtID0gc2VwUmVzdWx0LkRhdGE7XHJcbiAgICAgICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBjb25zdCBzdHlsZVJlc3VsdCA9IHRoaXMuX3N0eWxlSGFuZGxlci5HZXRDb21wdXRlZFN0eWxlKGAjJHt0aGlzLl9jb2x1bW5JRHNba2V5XX1gKTtcclxuICAgICAgICAgICAgaWYgKHN0eWxlUmVzdWx0LklzU3VjY2VlZGVkICYmIHN0eWxlUmVzdWx0LkRhdGEgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0IHN0eWxlOiBDU1NTdHlsZURlY2xhcmF0aW9uID0gc3R5bGVSZXN1bHQuRGF0YTtcclxuXHJcbiAgICAgICAgICAgICAgICBjb25zdCByYXdSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldEFsbChgLiR7dGhpcy5fY29sdW1uSURzW2tleV19YCk7XHJcbiAgICAgICAgICAgICAgICBpZiAoIXJhd1Jlc3VsdC5Jc1N1Y2NlZWRlZCB8fCByYXdSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGlmIChzdHlsZS5kaXNwbGF5ID09PSBcIm5vbmVcIikge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChlbG0gIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgLy9cdTMwRDhcdTMwQzNcdTMwQzBcdTMwRkNcdTMwNENcdTk3NUVcdTg4NjhcdTc5M0FcdTMwNkFcdTMwODlcdTMwQkJcdTMwRDFcdTMwRUNcdTMwRkNcdTMwQkZcdTMwRkNcdTMwODJcdTk3NUVcdTg4NjhcdTc5M0FcclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxtLnN0eWxlLmRpc3BsYXkgPSBcIm5vbmVcIjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vXHUzMEVBXHUzMEI5XHUzMEM4XHU1MDc0XHUzMDgyXHU5NzVFXHU4ODY4XHU3OTNBXHUzMDZCXHUzMDU5XHUzMDhCXHJcbiAgICAgICAgICAgICAgICAgICAgcmF3UmVzdWx0LkRhdGEuZm9yRWFjaChyYXcgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocmF3IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJhdy5zdHlsZS5kaXNwbGF5ID0gXCJub25lXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByZXN0b3JlV2lkdGggPSBhd2FpdCB0aGlzLl9kb3RuZXRIZWxwZXIuaW52b2tlTWV0aG9kQXN5bmM8bnVtYmVyPignR2V0V2lkdGgnLCB0aGlzLl9jb2x1bW5JRHNba2V5XSk7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgc2hvdWxkUmVzdG9yZVdpZHRoOiBib29sZWFuID0gcmVzdG9yZVdpZHRoID4gMDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgd2lkdGg6IG51bWJlciA9IHNob3VsZFJlc3RvcmVXaWR0aCA/IHJlc3RvcmVXaWR0aCA6IE51bWJlcihzdHlsZS53aWR0aC5tYXRjaCgvXFxkKy8pKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgbGVmdCArPSB3aWR0aDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGVsbSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbG0uc3R5bGUubGVmdCA9IGAke2xlZnR9cHhgO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHNob3VsZFJlc3RvcmVXaWR0aCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBoZWFkZXJSZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldChgIyR7dGhpcy5fY29sdW1uSURzW2tleV19YCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChoZWFkZXJSZXN1bHQuSXNTdWNjZWVkZWQgJiYgaGVhZGVyUmVzdWx0LkRhdGEgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChoZWFkZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyUmVzdWx0LkRhdGEuc3R5bGUud2lkdGggPSBgJHt3aWR0aH1weGA7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vXHUzMEVBXHUzMEI5XHUzMEM4XHU1MDc0XHUzMDZFXHU1RTQ1XHUzMDkyXHUzMEQ4XHUzMEMzXHUzMEMwXHUzMEZDXHUzMDZCXHU1NDA4XHUzMDhGXHUzMDVCXHUzMDhCXHJcbiAgICAgICAgICAgICAgICAgICAgcmF3UmVzdWx0LkRhdGEuZm9yRWFjaChyYXcgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocmF3IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJhdy5zdHlsZS53aWR0aCA9IHNob3VsZFJlc3RvcmVXaWR0aCA/IGAke3dpZHRofXB4YCA6IHN0eWxlLndpZHRoO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG5cclxuXHJcbiAgICAvLyNyZWdpb24gcHJpdmF0ZVxyXG5cclxuICAgIHByaXZhdGUgT25Nb3VzZURvd24oaW5kZXg6IHN0cmluZyk6IHZvaWQge1xyXG4gICAgICAgIHRoaXMuX2lzUmVzaXppbmcgPSB0cnVlO1xyXG4gICAgICAgIHRoaXMuX3Jlc2l6aW5nSW5kZXggPSBpbmRleDtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIE9uTW91c2VVcCgpOiB2b2lkIHtcclxuICAgICAgICB0aGlzLl9pc1Jlc2l6aW5nID0gZmFsc2U7XHJcbiAgICAgICAgdGhpcy5fcmVzaXppbmdJbmRleCA9IG51bGw7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3luYyBPbk1vdXNlTW92ZShlOiBNb3VzZUV2ZW50KTogUHJvbWlzZTx2b2lkPiB7XHJcbiAgICAgICAgaWYgKCF0aGlzLl9pc1Jlc2l6aW5nIHx8IHRoaXMuX3Jlc2l6aW5nSW5kZXggPT09IG51bGwpIHJldHVybjtcclxuXHJcbiAgICAgICAgY29uc3QgbmV4dEluZGV4ID0gTnVtYmVyKHRoaXMuX3Jlc2l6aW5nSW5kZXgpICsgMTtcclxuXHJcbiAgICAgICAgY29uc3QgcmVzaXppbmdOYW1lID0gdGhpcy5fY29sdW1uSURzW3RoaXMuX3Jlc2l6aW5nSW5kZXhdO1xyXG4gICAgICAgIGNvbnN0IG5leHROYW1lID0gdGhpcy5fY29sdW1uSURzW2Ake25leHRJbmRleH1gXTtcclxuXHJcbiAgICAgICAgY29uc3QgaGVhZGVyUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke3Jlc2l6aW5nTmFtZX1gKTtcclxuICAgICAgICBjb25zdCBuZXh0SGVhZGVyUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke25leHROYW1lfWApO1xyXG4gICAgICAgIGNvbnN0IGhlYWRlcldyYXBwZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChFbGVtZW50SURzLlZpZGVvTGlzdEhlYWRlcik7XHJcbiAgICAgICAgY29uc3QgY29sdW1uUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoYC4ke3Jlc2l6aW5nTmFtZX1gKTtcclxuICAgICAgICBjb25zdCBuZXh0Q29sdW1uUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoYC4ke25leHROYW1lfWApO1xyXG4gICAgICAgIGNvbnN0IHNlcFJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KHRoaXMuX3NlcGFyYXRvcklEc1t0aGlzLl9yZXNpemluZ0luZGV4XSk7XHJcblxyXG4gICAgICAgIC8vXHU4OTgxXHU3RDIwXHU1M0Q2XHU1Rjk3XHUzMDZCXHU1OTMxXHU2NTU3XHUzMDU3XHUzMDVGXHUzMDg5cmV0dXJuXHJcbiAgICAgICAgaWYgKCFoZWFkZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgaGVhZGVyUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIWNvbHVtblJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBjb2x1bW5SZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghc2VwUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNlcFJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKCFoZWFkZXJXcmFwcGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghbmV4dEhlYWRlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBuZXh0SGVhZGVyUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghbmV4dENvbHVtblJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBuZXh0Q29sdW1uUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKCEoaGVhZGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBpZiAoIShuZXh0SGVhZGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgY29uc3QgaGVhZGVyUmVjdDogRE9NUmVjdCA9IGhlYWRlclJlc3VsdC5EYXRhLmdldEJvdW5kaW5nQ2xpZW50UmVjdCgpO1xyXG4gICAgICAgIGNvbnN0IGhlYWRlcldyYXBwZXJSZWN0OiBET01SZWN0ID0gaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhLmdldEJvdW5kaW5nQ2xpZW50UmVjdCgpO1xyXG5cclxuXHJcbiAgICAgICAgY29uc3Qgd2lkdGggPSBlLmNsaWVudFggLSBoZWFkZXJSZWN0LmxlZnQ7XHJcbiAgICAgICAgY29uc3QgZGVsdGFXaWR0aCA9IHdpZHRoIC0gaGVhZGVyUmVzdWx0LkRhdGEub2Zmc2V0V2lkdGg7XHJcbiAgICAgICAgY29uc3QgbmV4dFdpZHRoID0gbmV4dEhlYWRlclJlc3VsdC5EYXRhLm9mZnNldFdpZHRoIC0gZGVsdGFXaWR0aDtcclxuXHJcbiAgICAgICAgaGVhZGVyUmVzdWx0LkRhdGEuc3R5bGUud2lkdGggPSBgJHt3aWR0aH1weGA7XHJcbiAgICAgICAgbmV4dEhlYWRlclJlc3VsdC5EYXRhLnN0eWxlLndpZHRoID0gYCR7bmV4dFdpZHRofXB4YDtcclxuXHJcbiAgICAgICAgY29sdW1uUmVzdWx0LkRhdGEuZm9yRWFjaChlbG0gPT4ge1xyXG4gICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgIGVsbS5zdHlsZS53aWR0aCA9IGAke3dpZHRofXB4YDtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgbmV4dENvbHVtblJlc3VsdC5EYXRhLmZvckVhY2goZWxtID0+IHtcclxuICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgICAgICBlbG0uc3R5bGUud2lkdGggPSBgJHtuZXh0V2lkdGh9cHhgO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBhd2FpdCB0aGlzLl9kb3RuZXRIZWxwZXIuaW52b2tlTWV0aG9kQXN5bmMoJ1NldFdpZHRoJywgYCR7d2lkdGh9YCwgcmVzaXppbmdOYW1lKTtcclxuICAgICAgICBhd2FpdCB0aGlzLl9kb3RuZXRIZWxwZXIuaW52b2tlTWV0aG9kQXN5bmMoJ1NldFdpZHRoJywgYCR7bmV4dFdpZHRofWAsIG5leHROYW1lKTtcclxuXHJcbiAgICAgICAgaWYgKCEoaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgaWYgKCEoc2VwUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgY29uc3QgbGVmdCA9IGhlYWRlclJlY3QubGVmdCAtIGhlYWRlcldyYXBwZXJSZWN0LmxlZnQgKyB3aWR0aCAtIDEwO1xyXG4gICAgICAgIHNlcFJlc3VsdC5EYXRhLnN0eWxlLmxlZnQgPSBgJHtsZWZ0fXB4YDtcclxuICAgIH1cclxuXHJcbiAgICAvLyNlbmRyZWdpb25cclxufVxyXG5cclxuaW50ZXJmYWNlIENsYXNzTmFtZXNEaWN0IHtcclxuICAgIFtpbmRleDogbnVtYmVyXTogc3RyaW5nO1xyXG59IiwgImltcG9ydCB7IERvdE5ldE9iamVjdFJlZmVyZW5jZSB9IGZyb20gXCIuLi9zaGFyZWQvRG90TmV0T2JqZWN0UmVmZXJlbmNlXCI7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyLCBFbGVtZW50SGFuZGxlckltcGwgfSBmcm9tIFwiLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyXCI7XHJcbmltcG9ydCB7IFN0eWxlSGFuZGxlciwgU3R5bGVIYW5kbGVySW1wbCB9IGZyb20gXCIuLi9zaGFyZWQvU3R5bGVIYW5kbGVyXCI7XHJcbmltcG9ydCB7IFNvcnRIYW5kbGVyLCBTb3J0SGFuZGxlckltcGwgfSBmcm9tIFwiLi9zb3J0SGFuZGxlci9zb3J0SGFuZGxlclwiO1xyXG5pbXBvcnQgeyBXaWR0aEhhbmRsZXIsIFdpZHRoSGFuZGxlckltcGwgfSBmcm9tIFwiLi93aWR0aEhhbmRsZXIvd2lkdGhIYW5kbGVyXCI7XHJcblxyXG5leHBvcnQgYXN5bmMgZnVuY3Rpb24gaW5pdGlhbGl6ZShibGF6b3JWaWV3OiBEb3ROZXRPYmplY3RSZWZlcmVuY2UpIHtcclxuICAgIGNvbnN0IGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyID0gbmV3IEVsZW1lbnRIYW5kbGVySW1wbCgpO1xyXG4gICAgY29uc3Qgc3R5bGVIYW5kbGVyOiBTdHlsZUhhbmRsZXIgPSBuZXcgU3R5bGVIYW5kbGVySW1wbChlbG1IYW5kbGVyKTtcclxuICAgIGNvbnN0IHdpZHRoSGFuZGxlcjogV2lkdGhIYW5kbGVyID0gbmV3IFdpZHRoSGFuZGxlckltcGwoZWxtSGFuZGxlciwgc3R5bGVIYW5kbGVyLCBibGF6b3JWaWV3KTtcclxuICAgIGNvbnN0IHNvcnRIYW5kbGVyOiBTb3J0SGFuZGxlciA9IG5ldyBTb3J0SGFuZGxlckltcGwoZWxtSGFuZGxlciwgYmxhem9yVmlldylcclxuXHJcbiAgICBhd2FpdCB3aWR0aEhhbmRsZXIuaW5pdGlhbGl6ZSgpO1xyXG4gICAgc29ydEhhbmRsZXIuaW5pdGlhbGl6ZSgpO1xyXG59XHJcblxyXG5leHBvcnQgYXN5bmMgZnVuY3Rpb24gc2V0V2lkdGgoYmxhem9yVmlldzogRG90TmV0T2JqZWN0UmVmZXJlbmNlKSB7XHJcbiAgICBjb25zdCBlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlciA9IG5ldyBFbGVtZW50SGFuZGxlckltcGwoKTtcclxuICAgIGNvbnN0IHN0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyID0gbmV3IFN0eWxlSGFuZGxlckltcGwoZWxtSGFuZGxlcik7XHJcbiAgICBjb25zdCB3aWR0aEhhbmRsZXI6IFdpZHRoSGFuZGxlciA9IG5ldyBXaWR0aEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIsIHN0eWxlSGFuZGxlciwgYmxhem9yVmlldyk7XHJcblxyXG4gICAgYXdhaXQgd2lkdGhIYW5kbGVyLnNldFdpZHRoKCk7XHJcbn1cclxuIl0sCiAgIm1hcHBpbmdzIjogIjtBQXNCTyxJQUFNLDZCQUFOLE1BQXlFO0FBQUEsRUFFNUUsWUFBWSxhQUFzQixNQUFnQixTQUF3QjtBQUN0RSxTQUFLLGNBQWM7QUFDbkIsU0FBSyxPQUFPO0FBQ1osU0FBSyxVQUFVO0FBQUEsRUFDbkI7QUFBQSxFQVFBLE9BQWMsVUFBYSxNQUEyQztBQUNsRSxXQUFPLElBQUksMkJBQTJCLE1BQU0s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TyxRQUFRO0FBQ2hFLGNBQVUsS0FBSyxNQUFNLE9BQU8sR0FBRztBQUFBLEVBQ25DO0FBQUE7QUFHSjs7O0FDeFBBLGVBQXNCLFdBQVcsWUFBbUM7QUFDaEUsUUFBTSxhQUE2QixJQUFJLG1CQUFtQjtBQUMxRCxRQUFNLGVBQTZCLElBQUksaUJBQWlCLFVBQVU7QUFDbEUsUUFBTSxlQUE2QixJQUFJLGlCQUFpQixZQUFZLGNBQWMsVUFBVTtBQUM1RixRQUFNLGNBQTJCLElBQUksZ0JBQWdCLFlBQVksVUFBVTtBQUUzRSxRQUFNLGFBQWEsV0FBVztBQUM5QixjQUFZLFdBQVc7QUFDM0I7QUFFQSxlQUFzQixTQUFTLFlBQW1DO0FBQzlELFFBQU0sYUFBNkIsSUFBSSxtQkFBbUI7QUFDMUQsUUFBTSxlQUE2QixJQUFJLGlCQUFpQixVQUFVO0FBQ2xFLFFBQU0sZUFBNkIsSUFBSSxpQkFBaUIsWUFBWSxjQUFjLFVBQVU7QUFFNUYsUUFBTSxhQUFhLFNBQVM7QUFDaEM7IiwKICAibmFtZXMiOiBbIkVsZW1lbnRJRHMiLCAiRWxlbWVudElEcyJdCn0K
