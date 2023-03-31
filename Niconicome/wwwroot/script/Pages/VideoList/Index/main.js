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

// src/videoList/SelectionHandler/selectionHandler.ts
var SelectionHandlerImpl = class {
  constructor(elmHandler) {
    this._elmHandler = elmHandler;
  }
  getSelected() {
    const elmResult = this._elmHandler.Get("#InputBox");
    if (!elmResult.IsSucceeded || elmResult.Data === null) {
      return "";
    }
    const elm = elmResult.Data;
    if (!(elm instanceof HTMLInputElement)) {
      return "";
    }
    if (!this.IsValidIndex(elm.value, elm.selectionStart)) {
      return "";
    }
    if (!this.IsValidIndex(elm.value, elm.selectionEnd, true)) {
      return "";
    }
    return elm.value.substring(elm.selectionStart, elm.selectionEnd);
  }
  IsValidIndex(value, index, isEnd = false) {
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
function getSelectedIOfInput() {
  const elmHandler = new ElementHandlerImpl();
  const handler = new SelectionHandlerImpl(elmHandler);
  return handler.getSelected();
}
export {
  getSelectedIOfInput,
  initialize,
  setWidth
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0F0dGVtcHRSZXN1bHQudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0VsZW1lbnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3NoYXJlZC9TdHlsZUhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L1NlbGVjdGlvbkhhbmRsZXIvc2VsZWN0aW9uSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3Qvc29ydEhhbmRsZXIvZWxlbWVudElEcy50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3Qvc29ydEhhbmRsZXIvc29ydEhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3dpZHRoSGFuZGxlci9FbGVtZW50SWRzLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3ZpZGVvTGlzdC93aWR0aEhhbmRsZXIvd2lkdGhIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3ZpZGVvTGlzdC9tYWluLnRzIl0sCiAgInNvdXJjZXNDb250ZW50IjogWyJleHBvcnQgaW50ZXJmYWNlIEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8VD4gZXh0ZW5kcyBBdHRlbXB0UmVzdWx0IHtcclxuICAgIFxyXG4gICAgLyoqXHJcbiAgICAgKiBcdTMwQzdcdTMwRkNcdTMwQkZcclxuICAgICAqL1xyXG4gICAgcmVhZG9ubHkgRGF0YTogVCB8IG51bGw7XHJcbn1cclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgQXR0ZW1wdFJlc3VsdCB7XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBcdTYyMTBcdTUyOUZcdTMwRDVcdTMwRTlcdTMwQjBcclxuICAgICAqL1xyXG4gICAgcmVhZG9ubHkgSXNTdWNjZWVkZWQ6IGJvb2xlYW47XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBcdTMwRTFcdTMwQzNcdTMwQkJcdTMwRkNcdTMwQjhcclxuICAgICAqL1xyXG4gICAgcmVhZG9ubHkgTWVzc2FnZTogc3RyaW5nIHwgbnVsbDtcclxufVxyXG5cclxuXHJcbmV4cG9ydCBjbGFzcyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbDxUPiBpbXBsZW1lbnRzIEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8VD4ge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGlzU3VjY2VlZGVkOiBib29sZWFuLCBkYXRhOiBUIHwgbnVsbCwgbWVzc2FnZTogc3RyaW5nIHwgbnVsbCkge1xyXG4gICAgICAgIHRoaXMuSXNTdWNjZWVkZWQgPSBpc1N1Y2NlZWRlZDtcclxuICAgICAgICB0aGlzLkRhdGEgPSBkYXRhO1xyXG4gICAgICAgIHRoaXMuTWVzc2FnZSA9IG1lc3NhZ2U7XHJcbiAgICB9XHJcblxyXG4gICAgcmVhZG9ubHkgSXNTdWNjZWVkZWQ6IGJvb2xlYW47XHJcblxyXG4gICAgcmVhZG9ubHkgRGF0YTogVCB8IG51bGw7XHJcblxyXG4gICAgcmVhZG9ubHkgTWVzc2FnZTogc3RyaW5nIHwgbnVsbDtcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIFN1Y2NlZWRlZDxUPihkYXRhOiBUIHwgbnVsbCk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8VD4ge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwodHJ1ZSwgZGF0YSwgbnVsbCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBGYWlsPFQ+KG1lc3NhZ2U6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8VD4ge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwoZmFsc2UsIG51bGwsIG1lc3NhZ2UpO1xyXG4gICAgfVxyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgQXR0ZW1wdFJlc3VsdEltcGwgaW1wbGVtZW50cyBBdHRlbXB0UmVzdWx0IHtcclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihpc1N1Y2NlZWRlZDogYm9vbGVhbiwgbWVzc2FnZTogc3RyaW5nIHwgbnVsbCkge1xyXG4gICAgICAgIHRoaXMuSXNTdWNjZWVkZWQgPSBpc1N1Y2NlZWRlZDtcclxuICAgICAgICB0aGlzLk1lc3NhZ2UgPSBtZXNzYWdlO1xyXG4gICAgfVxyXG5cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBTdWNjZWVkZWQoKTogQXR0ZW1wdFJlc3VsdCB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBBdHRlbXB0UmVzdWx0SW1wbCh0cnVlLCBudWxsKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIEZhaWwobWVzc2FnZTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdCB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBBdHRlbXB0UmVzdWx0SW1wbChmYWxzZSwgbWVzc2FnZSk7XHJcbiAgICB9XHJcbn1cclxuXHJcbiIsICJpbXBvcnQgeyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhLCBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbCB9IGZyb20gXCIuL0F0dGVtcHRSZXN1bHRcIjtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgRWxlbWVudEhhbmRsZXIge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU4OTgxXHU3RDIwXHUzMDkyXHU1M0Q2XHU1Rjk3XHUzMDU5XHUzMDhCXHJcbiAgICAgKiBAcGFyYW0gcXVlcnkgXHUzMEFGXHUzMEE4XHUzMEVBXHJcbiAgICAgKi9cclxuICAgIEdldChxdWVyeTpzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+O1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU4OTA3XHU2NTcwXHUzMDZFXHU4OTgxXHU3RDIwXHUzMDkyXHU1M0Q2XHU1Rjk3XHUzMDU5XHUzMDhCXHJcbiAgICAgKiBAcGFyYW0gcXVlcnkgXHUzMEFGXHUzMEE4XHUzMEVBXHJcbiAgICAgKi9cclxuICAgIEdldEFsbChxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxOb2RlTGlzdE9mPEVsZW1lbnQ+PjtcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIEVsZW1lbnRIYW5kbGVySW1wbCBpbXBsZW1lbnRzIEVsZW1lbnRIYW5kbGVyIHtcclxuXHJcbiAgICBwdWJsaWMgR2V0KHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+IHtcclxuXHJcbiAgICAgICAgbGV0IHJlc3VsdDogRWxlbWVudCB8IG51bGw7XHJcblxyXG4gICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgIHJlc3VsdCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IocXVlcnkpO1xyXG4gICAgICAgIH0gY2F0Y2ggKGU6IGFueSkge1xyXG4gICAgICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuRmFpbChgXHU4OTgxXHU3RDIwXHUzMDkyXHU1M0Q2XHU1Rjk3XHUzMDY3XHUzMDREXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDY3XHUzMDU3XHUzMDVGXHUzMDAyKFx1OEE3M1x1N0QzMFx1RkYxQSR7ZS5tZXNzYWdlfSlgKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiByZXN1bHQgPT0gbnVsbCA/IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoXCJcdTYzMDdcdTVCOUFcdTMwNTVcdTMwOENcdTMwNUZcdTg5ODFcdTdEMjBcdTMwNENcdTg5OEJcdTMwNjRcdTMwNEJcdTMwOEFcdTMwN0VcdTMwNUJcdTMwOTNcdTMwMDJcIikgOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5TdWNjZWVkZWQocmVzdWx0KTtcclxuICAgIH1cclxuXHJcblxyXG4gICAgcHVibGljIEdldEFsbChxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxOb2RlTGlzdE9mPEVsZW1lbnQ+PiB7XHJcblxyXG4gICAgICAgIGxldCByZXN1bHQ6IE5vZGVMaXN0T2Y8RWxlbWVudD47XHJcblxyXG4gICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgIHJlc3VsdCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3JBbGwocXVlcnkpO1xyXG4gICAgICAgIH0gY2F0Y2ggKGU6IGFueSkge1xyXG4gICAgICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuRmFpbChgXHU4OTgxXHU3RDIwXHUzMDkyXHU1M0Q2XHU1Rjk3XHUzMDY3XHUzMDREXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDY3XHUzMDU3XHUzMDVGXHUzMDAyKFx1OEE3M1x1N0QzMFx1RkYxQSR7ZS5tZXNzYWdlfSlgKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5TdWNjZWVkZWQocmVzdWx0KTtcclxuICAgIH1cclxuXHJcbn0iLCAiaW1wb3J0IHsgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSwgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwgfSBmcm9tIFwiLi9BdHRlbXB0UmVzdWx0XCI7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyIH0gZnJvbSBcIi4vRWxlbWVudEhhbmRsZXJcIjtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgU3R5bGVIYW5kbGVyIHtcclxuICAgIC8qKlxyXG4gICAgICogXHU4OTgxXHU3RDIwXHUzMDZFXHUzMEI5XHUzMEJGXHUzMEE0XHUzMEVCXHUzMDkyXHU1M0Q2XHU1Rjk3XHJcbiAgICAgKi9cclxuICAgIEdldENvbXB1dGVkU3R5bGUocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Q1NTU3R5bGVEZWNsYXJhdGlvbj47XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBTdHlsZUhhbmRsZXJJbXBsIGltcGxlbWVudHMgU3R5bGVIYW5kbGVyIHtcclxuICAgIGNvbnN0cnVjdG9yKGVsZW1lbnRIYW5kbGVyOiBFbGVtZW50SGFuZGxlcikge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbGVtZW50SGFuZGxlcjtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9lbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcjtcclxuXHJcbiAgICBwdWJsaWMgR2V0Q29tcHV0ZWRTdHlsZShxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxDU1NTdHlsZURlY2xhcmF0aW9uPiB7XHJcblxyXG4gICAgICAgIGNvbnN0IHJlc3VsdDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KHF1ZXJ5KTtcclxuICAgICAgICBpZiAoIXJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCByZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuRmFpbChyZXN1bHQuTWVzc2FnZSA/PyBcIlwiKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgIGNvbnN0IHN0eWxlID0gd2luZG93LmdldENvbXB1dGVkU3R5bGUocmVzdWx0LkRhdGEpO1xyXG4gICAgICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHN0eWxlKTtcclxuICAgICAgICB9IGNhdGNoIChleCkge1xyXG4gICAgICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuRmFpbChcIlx1MzBCOVx1MzBCRlx1MzBBNFx1MzBFQlx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMlwiKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCAiaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIgfSBmcm9tIFwiLi4vLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFNlbGVjdGlvbkhhbmRsZXIge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU1MTY1XHU1MjlCXHU2QjA0XHUzMDY3XHU5MDc4XHU2MjlFXHUzMDU1XHUzMDhDXHUzMDY2XHUzMDQ0XHUzMDhCXHU2NTg3XHU1QjU3XHU1MjE3XHUzMDkyXHU1M0Q2XHU1Rjk3XHUzMDU5XHUzMDhCXHJcbiAgICAgKi9cclxuICAgIGdldFNlbGVjdGVkKCk6IHN0cmluZztcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFNlbGVjdGlvbkhhbmRsZXJJbXBsIGltcGxlbWVudHMgU2VsZWN0aW9uSGFuZGxlciB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXIpIHtcclxuICAgICAgICB0aGlzLl9lbG1IYW5kbGVyID0gZWxtSGFuZGxlcjtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9lbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcjtcclxuXHJcbiAgICBwdWJsaWMgZ2V0U2VsZWN0ZWQoKTogc3RyaW5nIHtcclxuXHJcbiAgICAgICAgY29uc3QgZWxtUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoXCIjSW5wdXRCb3hcIik7XHJcbiAgICAgICAgaWYgKCFlbG1SZXN1bHQuSXNTdWNjZWVkZWQgfHwgZWxtUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuIFwiXCI7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBlbG06IEVsZW1lbnQgPSBlbG1SZXN1bHQuRGF0YTtcclxuICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MSW5wdXRFbGVtZW50KSkge1xyXG4gICAgICAgICAgICByZXR1cm4gXCJcIjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICghdGhpcy5Jc1ZhbGlkSW5kZXgoZWxtLnZhbHVlLCBlbG0uc2VsZWN0aW9uU3RhcnQpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBcIlwiO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKCF0aGlzLklzVmFsaWRJbmRleChlbG0udmFsdWUsIGVsbS5zZWxlY3Rpb25FbmQsIHRydWUpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBcIlwiO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIGVsbS52YWx1ZS5zdWJzdHJpbmcoZWxtLnNlbGVjdGlvblN0YXJ0ISwgZWxtLnNlbGVjdGlvbkVuZCEpXHJcblxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgSXNWYWxpZEluZGV4KHZhbHVlOiBzdHJpbmcsIGluZGV4OiBudW1iZXIgfCBudWxsLCBpc0VuZDogYm9vbGVhbiA9IGZhbHNlKTogYm9vbGVhbiB7XHJcbiAgICAgICAgaWYgKGluZGV4ID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChpc0VuZCAmJiBpbmRleCA+IHZhbHVlLmxlbmd0aCkge1xyXG4gICAgICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoIWlzRW5kICYmIGluZGV4ID4gdmFsdWUubGVuZ3RoIC0gMSkge1xyXG4gICAgICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gdHJ1ZTtcclxuICAgIH1cclxufSIsICJleHBvcnQgY2xhc3MgRWxlbWVudElEcyB7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBWaWRlb0xpc3RSb3cgPSAnLlZpZGVvTGlzdFJvdyc7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBWaWRlb0xpc3RSb3dDbGFzc05hbWUgPSAnVmlkZW9MaXN0Um93JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdEJvZHlDbGFzc05hbWUgPSAnVmlkZW9MaXN0Qm9keSc7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBEcm9wVGFyZ2V0Q2xhc3NOYW1lID0gJ0Ryb3BUYXJnZXQnO1xyXG59IiwgImltcG9ydCB7IERvdE5ldE9iamVjdFJlZmVyZW5jZSB9IGZyb20gXCIuLi8uLi9zaGFyZWQvRG90TmV0T2JqZWN0UmVmZXJlbmNlXCI7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyLCBFbGVtZW50SGFuZGxlckltcGwgfSBmcm9tIFwiLi4vLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyXCI7XHJcbmltcG9ydCB7IEVsZW1lbnRJRHMgfSBmcm9tIFwiLi9lbGVtZW50SURzXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFNvcnRIYW5kbGVyIHtcclxuICAgIGluaXRpYWxpemUoKTogdm9pZDtcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFNvcnRIYW5kbGVySW1wbCBpbXBsZW1lbnRzIFNvcnRIYW5kbGVyIHtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlciwgZG90bmV0SGVscGVyOiBEb3ROZXRPYmplY3RSZWZlcmVuY2UpIHtcclxuICAgICAgICB0aGlzLl9lbG1IYW5kbGVyID0gZWxtSGFuZGxlcjtcclxuICAgICAgICB0aGlzLl9kb3RuZXRIZWxwZXIgPSBkb3RuZXRIZWxwZXI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZG90bmV0SGVscGVyOiBEb3ROZXRPYmplY3RSZWZlcmVuY2U7XHJcblxyXG4gICAgcHJpdmF0ZSBfc291cmNlTmljb25pY29JRDogc3RyaW5nIHwgbnVsbCA9IG51bGw7XHJcblxyXG4gICAgcHJpdmF0ZSBfc291cmNlSUQ6IHN0cmluZyB8IG51bGwgPSBudWxsO1xyXG5cclxuICAgIHByaXZhdGUgX2xhc3RPdmVyRWxlbWVudDogSFRNTEVsZW1lbnQgfCBudWxsID0gbnVsbDtcclxuXHJcblxyXG4gICAgcHVibGljIGluaXRpYWxpemUoKTogdm9pZCB7XHJcbiAgICAgICAgY29uc3Qgcm93UmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoRWxlbWVudElEcy5WaWRlb0xpc3RSb3cpO1xyXG4gICAgICAgIGlmICghcm93UmVzdWx0LklzU3VjY2VlZGVkIHx8IHJvd1Jlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJvd1Jlc3VsdC5EYXRhLmZvckVhY2goZWxtID0+IHtcclxuXHJcbiAgICAgICAgICAgIGlmIChlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkge1xyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2RyYWdzdGFydCcsIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHJvdzogSFRNTEVsZW1lbnQgfCBudWxsID0gdGhpcy5HZXRQYXJlbnRCeUNsYXNzTmFtZShlLnRhcmdldCwgRWxlbWVudElEcy5WaWRlb0xpc3RSb3dDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5fc291cmNlTmljb25pY29JRCA9IHJvdy5kYXRhc2V0WyduaWNvbmljb2lkJ10gPz8gbnVsbDtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9zb3VyY2VJRCA9IHJvdy5pZDtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgICAgIGVsbS5hZGRFdmVudExpc3RlbmVyKCdkcmFnb3ZlcicsIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIShlLnRhcmdldCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByb3c6IEhUTUxFbGVtZW50IHwgbnVsbCA9IHRoaXMuR2V0UGFyZW50QnlDbGFzc05hbWUoZS50YXJnZXQsIEVsZW1lbnRJRHMuVmlkZW9MaXN0Um93Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocm93ID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghcm93LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJvdy5jbGFzc0xpc3QuYWRkKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuX2xhc3RPdmVyRWxlbWVudCA9IHJvdztcclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgICAgIGVsbS5hZGRFdmVudExpc3RlbmVyKCdkcmFnbGVhdmUnLCBlID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCEoZS50YXJnZXQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgcm93OiBIVE1MRWxlbWVudCB8IG51bGwgPSB0aGlzLkdldFBhcmVudEJ5Q2xhc3NOYW1lKGUudGFyZ2V0LCBFbGVtZW50SURzLlZpZGVvTGlzdFJvd0NsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJvdyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAocm93LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJvdy5jbGFzc0xpc3QucmVtb3ZlKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2Ryb3AnLCBhc3luYyBlID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLl9zb3VyY2VOaWNvbmljb0lEID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHNvdXJjZVJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KGAjJHt0aGlzLl9zb3VyY2VJRH1gKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIXNvdXJjZVJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzb3VyY2VSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoIShlLnRhcmdldCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBsZXQgcGFyZW50OiBQYXJlbnROb2RlIHwgbnVsbCA9IGUudGFyZ2V0LnBhcmVudE5vZGU7XHJcbiAgICAgICAgICAgICAgICAgICAgbGV0IGRyb3BUYXJnZXQ6IEhUTUxFbGVtZW50ID0gZS50YXJnZXQ7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHdoaWxlIChwYXJlbnQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKCEocGFyZW50IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICghcGFyZW50LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLlZpZGVvTGlzdEJvZHlDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBkcm9wVGFyZ2V0ID0gcGFyZW50O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcGFyZW50ID0gcGFyZW50LnBhcmVudE5vZGU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFyZW50Lmluc2VydEJlZm9yZShzb3VyY2VSZXN1bHQuRGF0YSwgZHJvcFRhcmdldCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGF3YWl0IHRoaXMuX2RvdG5ldEhlbHBlci5pbnZva2VNZXRob2RBc3luYyhcIk1vdmVWaWRlb1wiLCB0aGlzLl9zb3VyY2VOaWNvbmljb0lELCBkcm9wVGFyZ2V0LmRhdGFzZXRbJ25pY29uaWNvaWQnXSEpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQgPSBudWxsO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLl9sYXN0T3ZlckVsZW1lbnQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuX2xhc3RPdmVyRWxlbWVudC5jbGFzc0xpc3QuY29udGFpbnMoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5fbGFzdE92ZXJFbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBHZXRQYXJlbnRCeUNsYXNzTmFtZShjdXJyZW50RWxlbWVudDogSFRNTEVsZW1lbnQsIGNsYXNzTmFtZTogc3RyaW5nKTogSFRNTEVsZW1lbnQgfCBudWxsIHtcclxuICAgICAgICBsZXQgcGFyZW50OiBQYXJlbnROb2RlIHwgbnVsbCA9IGN1cnJlbnRFbGVtZW50O1xyXG5cclxuICAgICAgICB3aGlsZSAocGFyZW50ICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgIGlmICghKHBhcmVudCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIG51bGw7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGlmICghcGFyZW50LmNsYXNzTGlzdC5jb250YWlucyhjbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICBwYXJlbnQgPSBwYXJlbnQucGFyZW50Tm9kZTtcclxuICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gcGFyZW50O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIG51bGw7XHJcbiAgICB9XHJcbn0iLCAiZXhwb3J0IGNsYXNzIEVsZW1lbnRJRHMge1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgUGFnZUNvbnRlbnQgPSAnLlBhZ2VDb250ZW50JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdEhlYWRlciA9ICcjVmlkZW9MaXN0SGVhZGVyJztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFNlcGFyYXRvciA9ICcuU2VwYXJhdG9yJztcclxuXHJcbn0iLCAiaW1wb3J0IHsgQXR0ZW1wdFJlc3VsdCwgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSB9IGZyb20gJy4uLy4uL3NoYXJlZC9BdHRlbXB0UmVzdWx0JztcclxuaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIgfSBmcm9tICcuLi8uLi9zaGFyZWQvRWxlbWVudEhhbmRsZXInO1xyXG5pbXBvcnQgeyBFbGVtZW50SURzIH0gZnJvbSAnLi9FbGVtZW50SWRzJztcclxuaW1wb3J0IHsgRGljdGlvbmFyeSB9IGZyb20gJy4uLy4uL3NoYXJlZC9Db2xsZWN0aW9uL2RpY3Rpb25hcnknO1xyXG5pbXBvcnQgeyBTdHlsZUhhbmRsZXIgfSBmcm9tICcuLi8uLi9zaGFyZWQvU3R5bGVIYW5kbGVyJztcclxuaW1wb3J0IHsgRG90TmV0T2JqZWN0UmVmZXJlbmNlIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0RvdE5ldE9iamVjdFJlZmVyZW5jZSc7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFdpZHRoSGFuZGxlciB7XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBcdTUyMURcdTY3MUZcdTUzMTZcclxuICAgICAqL1xyXG4gICAgaW5pdGlhbGl6ZSgpOiBQcm9taXNlPHZvaWQ+O1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU1RTQ1XHUzMDkyXHU1MThEXHU4QTJEXHU1QjlBXHJcbiAgICAgKi9cclxuICAgIHNldFdpZHRoKCk6IFByb21pc2U8dm9pZD47XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBXaWR0aEhhbmRsZXJJbXBsIGltcGxlbWVudHMgV2lkdGhIYW5kbGVyIHtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50SGFuZGxlcjogRWxlbWVudEhhbmRsZXIsIHN0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyLCBkb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZSkge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbGVtZW50SGFuZGxlcjtcclxuICAgICAgICB0aGlzLl9zdHlsZUhhbmRsZXIgPSBzdHlsZUhhbmRsZXI7XHJcbiAgICAgICAgdGhpcy5fZG90bmV0SGVscGVyID0gZG90bmV0SGVscGVyO1xyXG4gICAgICAgIHRoaXMuX2NvbHVtbklEcyA9IHtcclxuICAgICAgICAgICAgJzAnOiAnQ2hlY2tCb3hDb2x1bW4nLFxyXG4gICAgICAgICAgICAnMSc6ICdUaHVtYm5haWxDb2x1bW4nLFxyXG4gICAgICAgICAgICAnMic6ICdUaXRsZUNvbHVtbicsXHJcbiAgICAgICAgICAgICczJzogJ1VwbG9hZGVkRGF0ZVRpbWVDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNCc6ICdJc0Rvd25sb2FkZWRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNSc6ICdWaWV3Q291bnRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNic6ICdDb21tZW50Q291bnRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNyc6ICdNeWxpc3RDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc4JzogJ0xpa2VDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc5JzogJ01lc3NhZ2VDb2x1bW4nLFxyXG4gICAgICAgIH07XHJcbiAgICAgICAgdGhpcy5fc2VwYXJhdG9ySURzID0ge1xyXG4gICAgICAgICAgICAnMCc6ICcjQ2hlY2tCb3hDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMSc6ICcjVGh1bWJuYWlsQ29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzInOiAnI1RpdGxlQ29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzMnOiAnI1VwbG9hZGVkRGF0ZVRpbWVDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNCc6ICcjSXNEb3dubG9hZGVkQ29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzUnOiAnI1ZpZXdDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc2JzogJyNDb21tZW50Q291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNyc6ICcjTXlsaXN0Q291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnOCc6ICcjTGlrZUNvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICB9O1xyXG4gICAgfVxyXG5cclxuICAgIC8vI3JlZ2lvbiAgZmllbGRcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9lbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9zdHlsZUhhbmRsZXI6IFN0eWxlSGFuZGxlcjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9jb2x1bW5JRHM6IERpY3Rpb25hcnk8c3RyaW5nPjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9zZXBhcmF0b3JJRHM6IERpY3Rpb25hcnk8c3RyaW5nPjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9kb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZTtcclxuXHJcbiAgICBwcml2YXRlIF9pc1Jlc2l6aW5nID0gZmFsc2U7XHJcblxyXG4gICAgcHJpdmF0ZSBfcmVzaXppbmdJbmRleDogbnVsbCB8IHN0cmluZztcclxuXHJcbiAgICAvLyNlbmRyZWdpb25cclxuXHJcbiAgICBwdWJsaWMgYXN5bmMgaW5pdGlhbGl6ZSgpOiBQcm9taXNlPHZvaWQ+IHtcclxuXHJcbiAgICAgICAgZm9yIChjb25zdCBrZXkgaW4gdGhpcy5fc2VwYXJhdG9ySURzKSB7XHJcblxyXG5cclxuICAgICAgICAgICAgY29uc3Qgc2VwUmVzdWx0OiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQodGhpcy5fc2VwYXJhdG9ySURzW2tleV0pO1xyXG4gICAgICAgICAgICBpZiAoIXNlcFJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzZXBSZXN1bHQuRGF0YSA9PT0gbnVsbCkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICBjb25zdCBlbG06IEVsZW1lbnQgPSBzZXBSZXN1bHQuRGF0YTtcclxuICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSBjb250aW51ZTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGluZGV4UyA9IGVsbS5kYXRhc2V0LmluZGV4O1xyXG5cclxuICAgICAgICAgICAgaWYgKGluZGV4UyA9PSB1bmRlZmluZWQpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNlZG93bicsIF8gPT4gdGhpcy5Pbk1vdXNlRG93bihpbmRleFMpKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGF3YWl0IHRoaXMuc2V0V2lkdGgoKTtcclxuXHJcbiAgICAgICAgY29uc3QgcGFnZVJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KEVsZW1lbnRJRHMuUGFnZUNvbnRlbnQpO1xyXG4gICAgICAgIGlmICghcGFnZVJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBwYWdlUmVzdWx0LkRhdGEgPT09IG51bGwgfHwgIShwYWdlUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBwYWdlUmVzdWx0LkRhdGEuYWRkRXZlbnRMaXN0ZW5lcignbW91c2V1cCcsIF8gPT4gdGhpcy5Pbk1vdXNlVXAoKSk7XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRlcldyYXBwZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChFbGVtZW50SURzLlZpZGVvTGlzdEhlYWRlcik7XHJcbiAgICAgICAgaWYgKCFoZWFkZXJXcmFwcGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCB8fCAhKGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YS5hZGRFdmVudExpc3RlbmVyKCdtb3VzZW1vdmUnLCBlID0+IHRoaXMuT25Nb3VzZU1vdmUoZSkpO1xyXG5cclxuICAgIH1cclxuICAgIHB1YmxpYyBhc3luYyBzZXRXaWR0aCgpOiBQcm9taXNlPHZvaWQ+IHtcclxuICAgICAgICBsZXQgbGVmdCA9IDA7XHJcbiAgICAgICAgZm9yIChjb25zdCBrZXkgaW4gdGhpcy5fY29sdW1uSURzKSB7XHJcblxyXG4gICAgICAgICAgICBsZXQgZWxtOiBFbGVtZW50IHwgbnVsbCA9IG51bGw7XHJcblxyXG4gICAgICAgICAgICBpZiAoa2V5IGluIHRoaXMuX3NlcGFyYXRvcklEcykge1xyXG4gICAgICAgICAgICAgICAgY29uc3Qgc2VwUmVzdWx0OiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQodGhpcy5fc2VwYXJhdG9ySURzW2tleV0pO1xyXG4gICAgICAgICAgICAgICAgaWYgKCFzZXBSZXN1bHQuSXNTdWNjZWVkZWQgfHwgc2VwUmVzdWx0LkRhdGEgPT09IG51bGwpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgICAgIGVsbSA9IHNlcFJlc3VsdC5EYXRhO1xyXG4gICAgICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSBjb250aW51ZTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgY29uc3Qgc3R5bGVSZXN1bHQgPSB0aGlzLl9zdHlsZUhhbmRsZXIuR2V0Q29tcHV0ZWRTdHlsZShgIyR7dGhpcy5fY29sdW1uSURzW2tleV19YCk7XHJcbiAgICAgICAgICAgIGlmIChzdHlsZVJlc3VsdC5Jc1N1Y2NlZWRlZCAmJiBzdHlsZVJlc3VsdC5EYXRhICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBzdHlsZTogQ1NTU3R5bGVEZWNsYXJhdGlvbiA9IHN0eWxlUmVzdWx0LkRhdGE7XHJcblxyXG4gICAgICAgICAgICAgICAgY29uc3QgcmF3UmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoYC4ke3RoaXMuX2NvbHVtbklEc1trZXldfWApO1xyXG4gICAgICAgICAgICAgICAgaWYgKCFyYXdSZXN1bHQuSXNTdWNjZWVkZWQgfHwgcmF3UmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBpZiAoc3R5bGUuZGlzcGxheSA9PT0gXCJub25lXCIpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoZWxtICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vXHUzMEQ4XHUzMEMzXHUzMEMwXHUzMEZDXHUzMDRDXHU5NzVFXHU4ODY4XHU3OTNBXHUzMDZBXHUzMDg5XHUzMEJCXHUzMEQxXHUzMEVDXHUzMEZDXHUzMEJGXHUzMEZDXHUzMDgyXHU5NzVFXHU4ODY4XHU3OTNBXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsbS5zdHlsZS5kaXNwbGF5ID0gXCJub25lXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAvL1x1MzBFQVx1MzBCOVx1MzBDOFx1NTA3NFx1MzA4Mlx1OTc1RVx1ODg2OFx1NzkzQVx1MzA2Qlx1MzA1OVx1MzA4QlxyXG4gICAgICAgICAgICAgICAgICAgIHJhd1Jlc3VsdC5EYXRhLmZvckVhY2gocmF3ID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHJhdyBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByYXcuc3R5bGUuZGlzcGxheSA9IFwibm9uZVwiO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgcmVzdG9yZVdpZHRoID0gYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jPG51bWJlcj4oJ0dldFdpZHRoJywgdGhpcy5fY29sdW1uSURzW2tleV0pO1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHNob3VsZFJlc3RvcmVXaWR0aDogYm9vbGVhbiA9IHJlc3RvcmVXaWR0aCA+IDA7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHdpZHRoOiBudW1iZXIgPSBzaG91bGRSZXN0b3JlV2lkdGggPyByZXN0b3JlV2lkdGggOiBOdW1iZXIoc3R5bGUud2lkdGgubWF0Y2goL1xcZCsvKSk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxlZnQgKz0gd2lkdGg7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChlbG0gIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxtLnN0eWxlLmxlZnQgPSBgJHtsZWZ0fXB4YDtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChzaG91bGRSZXN0b3JlV2lkdGgpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgaGVhZGVyUmVzdWx0OiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke3RoaXMuX2NvbHVtbklEc1trZXldfWApO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoaGVhZGVyUmVzdWx0LklzU3VjY2VlZGVkICYmIGhlYWRlclJlc3VsdC5EYXRhICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoaGVhZGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlclJlc3VsdC5EYXRhLnN0eWxlLndpZHRoID0gYCR7d2lkdGh9cHhgO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAvL1x1MzBFQVx1MzBCOVx1MzBDOFx1NTA3NFx1MzA2RVx1NUU0NVx1MzA5Mlx1MzBEOFx1MzBDM1x1MzBDMFx1MzBGQ1x1MzA2Qlx1NTQwOFx1MzA4Rlx1MzA1Qlx1MzA4QlxyXG4gICAgICAgICAgICAgICAgICAgIHJhd1Jlc3VsdC5EYXRhLmZvckVhY2gocmF3ID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHJhdyBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByYXcuc3R5bGUud2lkdGggPSBzaG91bGRSZXN0b3JlV2lkdGggPyBgJHt3aWR0aH1weGAgOiBzdHlsZS53aWR0aDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuXHJcblxyXG4gICAgLy8jcmVnaW9uIHByaXZhdGVcclxuXHJcbiAgICBwcml2YXRlIE9uTW91c2VEb3duKGluZGV4OiBzdHJpbmcpOiB2b2lkIHtcclxuICAgICAgICB0aGlzLl9pc1Jlc2l6aW5nID0gdHJ1ZTtcclxuICAgICAgICB0aGlzLl9yZXNpemluZ0luZGV4ID0gaW5kZXg7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vdXNlVXAoKTogdm9pZCB7XHJcbiAgICAgICAgdGhpcy5faXNSZXNpemluZyA9IGZhbHNlO1xyXG4gICAgICAgIHRoaXMuX3Jlc2l6aW5nSW5kZXggPSBudWxsO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgYXN5bmMgT25Nb3VzZU1vdmUoZTogTW91c2VFdmVudCk6IFByb21pc2U8dm9pZD4ge1xyXG4gICAgICAgIGlmICghdGhpcy5faXNSZXNpemluZyB8fCB0aGlzLl9yZXNpemluZ0luZGV4ID09PSBudWxsKSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IG5leHRJbmRleCA9IE51bWJlcih0aGlzLl9yZXNpemluZ0luZGV4KSArIDE7XHJcblxyXG4gICAgICAgIGNvbnN0IHJlc2l6aW5nTmFtZSA9IHRoaXMuX2NvbHVtbklEc1t0aGlzLl9yZXNpemluZ0luZGV4XTtcclxuICAgICAgICBjb25zdCBuZXh0TmFtZSA9IHRoaXMuX2NvbHVtbklEc1tgJHtuZXh0SW5kZXh9YF07XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KGAjJHtyZXNpemluZ05hbWV9YCk7XHJcbiAgICAgICAgY29uc3QgbmV4dEhlYWRlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KGAjJHtuZXh0TmFtZX1gKTtcclxuICAgICAgICBjb25zdCBoZWFkZXJXcmFwcGVyUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoRWxlbWVudElEcy5WaWRlb0xpc3RIZWFkZXIpO1xyXG4gICAgICAgIGNvbnN0IGNvbHVtblJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKGAuJHtyZXNpemluZ05hbWV9YCk7XHJcbiAgICAgICAgY29uc3QgbmV4dENvbHVtblJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKGAuJHtuZXh0TmFtZX1gKTtcclxuICAgICAgICBjb25zdCBzZXBSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldCh0aGlzLl9zZXBhcmF0b3JJRHNbdGhpcy5fcmVzaXppbmdJbmRleF0pO1xyXG5cclxuICAgICAgICAvL1x1ODk4MVx1N0QyMFx1NTNENlx1NUY5N1x1MzA2Qlx1NTkzMVx1NjU1N1x1MzA1N1x1MzA1Rlx1MzA4OXJldHVyblxyXG4gICAgICAgIGlmICghaGVhZGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IGhlYWRlclJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKCFjb2x1bW5SZXN1bHQuSXNTdWNjZWVkZWQgfHwgY29sdW1uUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIXNlcFJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzZXBSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghaGVhZGVyV3JhcHBlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIW5leHRIZWFkZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgbmV4dEhlYWRlclJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVyblxyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIW5leHRDb2x1bW5SZXN1bHQuSXNTdWNjZWVkZWQgfHwgbmV4dENvbHVtblJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICghKGhlYWRlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgaWYgKCEobmV4dEhlYWRlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRlclJlY3Q6IERPTVJlY3QgPSBoZWFkZXJSZXN1bHQuRGF0YS5nZXRCb3VuZGluZ0NsaWVudFJlY3QoKTtcclxuICAgICAgICBjb25zdCBoZWFkZXJXcmFwcGVyUmVjdDogRE9NUmVjdCA9IGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YS5nZXRCb3VuZGluZ0NsaWVudFJlY3QoKTtcclxuXHJcblxyXG4gICAgICAgIGNvbnN0IHdpZHRoID0gZS5jbGllbnRYIC0gaGVhZGVyUmVjdC5sZWZ0O1xyXG4gICAgICAgIGNvbnN0IGRlbHRhV2lkdGggPSB3aWR0aCAtIGhlYWRlclJlc3VsdC5EYXRhLm9mZnNldFdpZHRoO1xyXG4gICAgICAgIGNvbnN0IG5leHRXaWR0aCA9IG5leHRIZWFkZXJSZXN1bHQuRGF0YS5vZmZzZXRXaWR0aCAtIGRlbHRhV2lkdGg7XHJcblxyXG4gICAgICAgIGhlYWRlclJlc3VsdC5EYXRhLnN0eWxlLndpZHRoID0gYCR7d2lkdGh9cHhgO1xyXG4gICAgICAgIG5leHRIZWFkZXJSZXN1bHQuRGF0YS5zdHlsZS53aWR0aCA9IGAke25leHRXaWR0aH1weGA7XHJcblxyXG4gICAgICAgIGNvbHVtblJlc3VsdC5EYXRhLmZvckVhY2goZWxtID0+IHtcclxuICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgICAgICBlbG0uc3R5bGUud2lkdGggPSBgJHt3aWR0aH1weGA7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIG5leHRDb2x1bW5SZXN1bHQuRGF0YS5mb3JFYWNoKGVsbSA9PiB7XHJcbiAgICAgICAgICAgIGlmICghKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICAgICAgZWxtLnN0eWxlLndpZHRoID0gYCR7bmV4dFdpZHRofXB4YDtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jKCdTZXRXaWR0aCcsIGAke3dpZHRofWAsIHJlc2l6aW5nTmFtZSk7XHJcbiAgICAgICAgYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jKCdTZXRXaWR0aCcsIGAke25leHRXaWR0aH1gLCBuZXh0TmFtZSk7XHJcblxyXG4gICAgICAgIGlmICghKGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIGlmICghKHNlcFJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IGxlZnQgPSBoZWFkZXJSZWN0LmxlZnQgLSBoZWFkZXJXcmFwcGVyUmVjdC5sZWZ0ICsgd2lkdGggLSAxMDtcclxuICAgICAgICBzZXBSZXN1bHQuRGF0YS5zdHlsZS5sZWZ0ID0gYCR7bGVmdH1weGA7XHJcbiAgICB9XHJcblxyXG4gICAgLy8jZW5kcmVnaW9uXHJcbn1cclxuXHJcbmludGVyZmFjZSBDbGFzc05hbWVzRGljdCB7XHJcbiAgICBbaW5kZXg6IG51bWJlcl06IHN0cmluZztcclxufSIsICJpbXBvcnQgeyBEb3ROZXRPYmplY3RSZWZlcmVuY2UgfSBmcm9tIFwiLi4vc2hhcmVkL0RvdE5ldE9iamVjdFJlZmVyZW5jZVwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciwgRWxlbWVudEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4uL3NoYXJlZC9FbGVtZW50SGFuZGxlclwiO1xyXG5pbXBvcnQgeyBTdHlsZUhhbmRsZXIsIFN0eWxlSGFuZGxlckltcGwgfSBmcm9tIFwiLi4vc2hhcmVkL1N0eWxlSGFuZGxlclwiO1xyXG5pbXBvcnQgeyBTZWxlY3Rpb25IYW5kbGVySW1wbCB9IGZyb20gXCIuL1NlbGVjdGlvbkhhbmRsZXIvc2VsZWN0aW9uSGFuZGxlclwiO1xyXG5pbXBvcnQgeyBTb3J0SGFuZGxlciwgU29ydEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vc29ydEhhbmRsZXIvc29ydEhhbmRsZXJcIjtcclxuaW1wb3J0IHsgV2lkdGhIYW5kbGVyLCBXaWR0aEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlclwiO1xyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGluaXRpYWxpemUoYmxhem9yVmlldzogRG90TmV0T2JqZWN0UmVmZXJlbmNlKSB7XHJcbiAgICBjb25zdCBlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlciA9IG5ldyBFbGVtZW50SGFuZGxlckltcGwoKTtcclxuICAgIGNvbnN0IHN0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyID0gbmV3IFN0eWxlSGFuZGxlckltcGwoZWxtSGFuZGxlcik7XHJcbiAgICBjb25zdCB3aWR0aEhhbmRsZXI6IFdpZHRoSGFuZGxlciA9IG5ldyBXaWR0aEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIsIHN0eWxlSGFuZGxlciwgYmxhem9yVmlldyk7XHJcbiAgICBjb25zdCBzb3J0SGFuZGxlcjogU29ydEhhbmRsZXIgPSBuZXcgU29ydEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIsIGJsYXpvclZpZXcpXHJcblxyXG4gICAgYXdhaXQgd2lkdGhIYW5kbGVyLmluaXRpYWxpemUoKTtcclxuICAgIHNvcnRIYW5kbGVyLmluaXRpYWxpemUoKTtcclxufVxyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIHNldFdpZHRoKGJsYXpvclZpZXc6IERvdE5ldE9iamVjdFJlZmVyZW5jZSkge1xyXG4gICAgY29uc3QgZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXIgPSBuZXcgRWxlbWVudEhhbmRsZXJJbXBsKCk7XHJcbiAgICBjb25zdCBzdHlsZUhhbmRsZXI6IFN0eWxlSGFuZGxlciA9IG5ldyBTdHlsZUhhbmRsZXJJbXBsKGVsbUhhbmRsZXIpO1xyXG4gICAgY29uc3Qgd2lkdGhIYW5kbGVyOiBXaWR0aEhhbmRsZXIgPSBuZXcgV2lkdGhIYW5kbGVySW1wbChlbG1IYW5kbGVyLCBzdHlsZUhhbmRsZXIsIGJsYXpvclZpZXcpO1xyXG5cclxuICAgIGF3YWl0IHdpZHRoSGFuZGxlci5zZXRXaWR0aCgpO1xyXG59XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gZ2V0U2VsZWN0ZWRJT2ZJbnB1dCgpOiBzdHJpbmcge1xyXG4gICAgY29uc3QgZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXIgPSBuZXcgRWxlbWVudEhhbmRsZXJJbXBsKCk7XHJcbiAgICBjb25zdCBoYW5kbGVyID0gbmV3IFNlbGVjdGlvbkhhbmRsZXJJbXBsKGVsbUhhbmRsZXIpO1xyXG5cclxuICAgIHJldHVybiBoYW5kbGVyLmdldFNlbGVjdGVkKCk7XHJcbn1cclxuIl0sCiAgIm1hcHBpbmdzIjogIjtBQXNCTyxJQUFNLDZCQUFOLE1BQXlFO0FBQUEsRUFFNUUsWUFBWSxhQUFzQixNQUFnQixTQUF3QjtBQUN0RSxTQUFLLGNBQWM7QUFDbkIsU0FBSyxPQUFPO0FBQ1osU0FBSyxVQUFVO0FBQUEsRUFDbkI7QUFBQSxFQVFBLE9BQWMsVUFBYSxNQUEyQztBQUNsRSxXQUFPLElBQUksMkJBQTJCLE1BQU0sTUFBTSxJQUFJO0FBQUEsRUFDMUQ7QUFBQSxFQUVBLE9BQWMsS0FBUSxTQUE0QztBQUM5RCxXQUFPLElBQUksMkJBQTJCLE9BQU8sTUFBTSxPQUFPO0FBQUEsRUFDOUQ7QUFDSjs7O0FDMUJPLElBQU0scUJBQU4sTUFBbUQ7QUFBQSxFQUUvQyxJQUFJLE9BQWdEO0FBRXZELFFBQUk7QUFFSixRQUFJO0FBQ0EsZUFBUyxTQUFTLGNBQWMsS0FBSztBQUFBLElBQ3pDLFNBQVMsR0FBUDtBQUNFLGFBQU8sMkJBQTJCLEtBQUssMEdBQXFCLEVBQUUsVUFBVTtBQUFBLElBQzVFO0FBRUEsV0FBTyxVQUFVLE9BQU8sMkJBQTJCLEtBQUssa0dBQWtCLElBQUksMkJBQTJCLFVBQVUsTUFBTTtBQUFBLEVBQzdIO0FBQUEsRUFHTyxPQUFPLE9BQTREO0FBRXRFLFFBQUk7QUFFSixRQUFJO0FBQ0EsZUFBUyxTQUFTLGlCQUFpQixLQUFLO0FBQUEsSUFDNUMsU0FBUyxHQUFQO0FBQ0UsYUFBTywyQkFBMkIsS0FBSywwR0FBcUIsRUFBRSxVQUFVO0FBQUEsSUFDNUU7QUFFQSxXQUFPLDJCQUEyQixVQUFVLE1BQU07QUFBQSxFQUN0RDtBQUVKOzs7QUNwQ08sSUFBTSxtQkFBTixNQUErQztBQUFBLEVBQ2xELFlBQVksZ0JBQWdDO0FBQ3hDLFNBQUssY0FBYztBQUFBLEVBQ3ZCO0FBQUEsRUFJTyxpQkFBaUIsT0FBNEQ7QUFFaEYsVUFBTSxTQUEwQyxLQUFLLFlBQVksSUFBSSxLQUFLO0FBQzFFLFFBQUksQ0FBQyxPQUFPLGVBQWUsT0FBTyxTQUFTLE1BQU07QUFDN0MsYUFBTywyQkFBMkIsS0FBSyxPQUFPLFdBQVcsRUFBRTtBQUFBLElBQy9EO0FBRUEsUUFBSTtBQUNBLFlBQU0sUUFBUSxPQUFPLGlCQUFpQixPQUFPLElBQUk7QUFDakQsYUFBTywyQkFBMkIsVUFBVSxLQUFLO0FBQUEsSUFDckQsU0FBUyxJQUFQO0FBQ0UsYUFBTywyQkFBMkIsS0FBSyxrR0FBa0I7QUFBQSxJQUM3RDtBQUFBLEVBQ0o7QUFDSjs7O0FDckJPLElBQU0sdUJBQU4sTUFBdUQ7QUFBQSxFQUUxRCxZQUFZLFlBQTRCO0FBQ3BDLFNBQUssY0FBYztBQUFBLEVBQ3ZCO0FBQUEsRUFJTyxjQUFzQjtBQUV6QixVQUFNLFlBQVksS0FBSyxZQUFZLElBQUksV0FBVztBQUNsRCxRQUFJLENBQUMsVUFBVSxlQUFlLFVBQVUsU0FBUyxNQUFNO0FBQ25ELGFBQU87QUFBQSxJQUNYO0FBRUEsVUFBTSxNQUFlLFVBQVU7QUFDL0IsUUFBSSxFQUFFLGVBQWUsbUJBQW1CO0FBQ3BDLGFBQU87QUFBQSxJQUNYO0FBRUEsUUFBSSxDQUFDLEtBQUssYUFBYSxJQUFJLE9BQU8sSUFBSSxjQUFjLEdBQUc7QUFDbkQsYUFBTztBQUFBLElBQ1g7QUFFQSxRQUFJLENBQUMsS0FBSyxhQUFhLElBQUksT0FBTyxJQUFJLGNBQWMsSUFBSSxHQUFHO0FBQ3ZELGFBQU87QUFBQSxJQUNYO0FBRUEsV0FBTyxJQUFJLE1BQU0sVUFBVSxJQUFJLGdCQUFpQixJQUFJLFlBQWE7QUFBQSxFQUVyRTtBQUFBLEVBRVEsYUFBYSxPQUFlLE9BQXNCLFFBQWlCLE9BQWdCO0FBQ3ZGLFFBQUksVUFBVSxNQUFNO0FBQ2hCLGFBQU87QUFBQSxJQUNYO0FBRUEsUUFBSSxTQUFTLFFBQVEsTUFBTSxRQUFRO0FBQy9CLGFBQU87QUFBQSxJQUNYO0FBRUEsUUFBSSxDQUFDLFNBQVMsUUFBUSxNQUFNLFNBQVMsR0FBRztBQUNwQyxhQUFPO0FBQUEsSUFDWDtBQUVBLFdBQU87QUFBQSxFQUNYO0FBQ0o7OztBQ3pETyxJQUFNLGFBQU4sTUFBaUI7QUFTeEI7QUFUYSxXQUVjLGVBQWU7QUFGN0IsV0FJYyx3QkFBd0I7QUFKdEMsV0FNYyx5QkFBeUI7QUFOdkMsV0FRYyxzQkFBc0I7OztBQ0ExQyxJQUFNLGtCQUFOLE1BQTZDO0FBQUEsRUFFaEQsWUFBWSxZQUE0QixjQUFxQztBQVM3RSxTQUFRLG9CQUFtQztBQUUzQyxTQUFRLFlBQTJCO0FBRW5DLFNBQVEsbUJBQXVDO0FBWjNDLFNBQUssY0FBYztBQUNuQixTQUFLLGdCQUFnQjtBQUFBLEVBQ3pCO0FBQUEsRUFhTyxhQUFtQjtBQUN0QixVQUFNLFlBQVksS0FBSyxZQUFZLE9BQU8sV0FBVyxZQUFZO0FBQ2pFLFFBQUksQ0FBQyxVQUFVLGVBQWUsVUFBVSxTQUFTLE1BQU07QUFDbkQ7QUFBQSxJQUNKO0FBRUEsY0FBVSxLQUFLLFFBQVEsU0FBTztBQUUxQixVQUFJLGVBQWUsYUFBYTtBQUM1QixZQUFJLGlCQUFpQixhQUFhLE9BQUs7QUFDbkMsY0FBSSxFQUFFLEVBQUUsa0JBQWtCLGNBQWM7QUFDcEM7QUFBQSxVQUNKO0FBRUEsZ0JBQU0sTUFBMEIsS0FBSyxxQkFBcUIsRUFBRSxRQUFRLFdBQVcscUJBQXFCO0FBQ3BHLGNBQUksUUFBUSxNQUFNO0FBQ2Q7QUFBQSxVQUNKO0FBRUEsZUFBSyxvQkFBb0IsSUFBSSxRQUFRLFlBQVksS0FBSztBQUN0RCxlQUFLLFlBQVksSUFBSTtBQUFBLFFBQ3pCLENBQUM7QUFFRCxZQUFJLGlCQUFpQixZQUFZLE9BQUs7QUFDbEMsWUFBRSxlQUFlO0FBQ2pCLGNBQUksRUFBRSxFQUFFLGtCQUFrQixjQUFjO0FBQ3BDO0FBQUEsVUFDSjtBQUVBLGdCQUFNLE1BQTBCLEtBQUsscUJBQXFCLEVBQUUsUUFBUSxXQUFXLHFCQUFxQjtBQUNwRyxjQUFJLFFBQVEsTUFBTTtBQUNkO0FBQUEsVUFDSjtBQUVBLGNBQUksQ0FBQyxJQUFJLFVBQVUsU0FBUyxXQUFXLG1CQUFtQixHQUFHO0FBQ3pELGdCQUFJLFVBQVUsSUFBSSxXQUFXLG1CQUFtQjtBQUFBLFVBQ3BEO0FBQ0EsZUFBSyxtQkFBbUI7QUFBQSxRQUM1QixDQUFDO0FBRUQsWUFBSSxpQkFBaUIsYUFBYSxPQUFLO0FBQ25DLFlBQUUsZUFBZTtBQUNqQixjQUFJLEVBQUUsRUFBRSxrQkFBa0IsY0FBYztBQUNwQztBQUFBLFVBQ0o7QUFFQSxnQkFBTSxNQUEwQixLQUFLLHFCQUFxQixFQUFFLFFBQVEsV0FBVyxxQkFBcUI7QUFDcEcsY0FBSSxRQUFRLE1BQU07QUFDZDtBQUFBLFVBQ0o7QUFFQSxjQUFJLElBQUksVUFBVSxTQUFTLFdBQVcsbUJBQW1CLEdBQUc7QUFDeEQsZ0JBQUksVUFBVSxPQUFPLFdBQVcsbUJBQW1CO0FBQUEsVUFDdkQ7QUFBQSxRQUNKLENBQUM7QUFFRCxZQUFJLGlCQUFpQixRQUFRLE9BQU0sTUFBSztBQUNwQyxZQUFFLGVBQWU7QUFFakIsY0FBSSxLQUFLLHNCQUFzQixNQUFNO0FBQ2pDO0FBQUEsVUFDSjtBQUVBLGdCQUFNLGVBQWUsS0FBSyxZQUFZLElBQUksSUFBSSxLQUFLLFdBQVc7QUFDOUQsY0FBSSxDQUFDLGFBQWEsZUFBZSxhQUFhLFNBQVMsTUFBTTtBQUN6RDtBQUFBLFVBQ0o7QUFFQSxjQUFJLEVBQUUsRUFBRSxrQkFBa0IsY0FBYztBQUNwQztBQUFBLFVBQ0o7QUFFQSxjQUFJLFNBQTRCLEVBQUUsT0FBTztBQUN6QyxjQUFJLGFBQTBCLEVBQUU7QUFFaEMsaUJBQU8sV0FBVyxNQUFNO0FBQ3BCLGdCQUFJLEVBQUUsa0JBQWtCLGNBQWM7QUFDbEM7QUFBQSxZQUNKO0FBRUEsZ0JBQUksQ0FBQyxPQUFPLFVBQVUsU0FBUyxXQUFXLHNCQUFzQixHQUFHO0FBQy9ELDJCQUFhO0FBQ2IsdUJBQVMsT0FBTztBQUNoQjtBQUFBLFlBQ0o7QUFFQSxtQkFBTyxhQUFhLGFBQWEsTUFBTSxVQUFVO0FBQ2pELGtCQUFNLEtBQUssY0FBYyxrQkFBa0IsYUFBYSxLQUFLLG1CQUFtQixXQUFXLFFBQVEsWUFBWSxDQUFFO0FBQ2pILHFCQUFTO0FBQUEsVUFDYjtBQUdBLGNBQUksS0FBSyxxQkFBcUIsTUFBTTtBQUNoQyxnQkFBSSxLQUFLLGlCQUFpQixVQUFVLFNBQVMsV0FBVyxtQkFBbUIsR0FBRztBQUMxRSxtQkFBSyxpQkFBaUIsVUFBVSxPQUFPLFdBQVcsbUJBQW1CO0FBQUEsWUFDekU7QUFBQSxVQUNKO0FBQUEsUUFDSixDQUFDO0FBQUEsTUFDTDtBQUFBLElBQ0osQ0FBQztBQUFBLEVBQ0w7QUFBQSxFQUVRLHFCQUFxQixnQkFBNkIsV0FBdUM7QUFDN0YsUUFBSSxTQUE0QjtBQUVoQyxXQUFPLFdBQVcsTUFBTTtBQUNwQixVQUFJLEVBQUUsa0JBQWtCLGNBQWM7QUFDbEMsZUFBTztBQUFBLE1BQ1g7QUFFQSxVQUFJLENBQUMsT0FBTyxVQUFVLFNBQVMsU0FBUyxHQUFHO0FBQ3ZDLGlCQUFTLE9BQU87QUFDaEI7QUFBQSxNQUNKO0FBRUEsYUFBTztBQUFBLElBQ1g7QUFFQSxXQUFPO0FBQUEsRUFDWDtBQUNKOzs7QUNsSk8sSUFBTUEsY0FBTixNQUFpQjtBQVF4QjtBQVJhQSxZQUVjLGNBQWM7QUFGNUJBLFlBSWMsa0JBQWtCO0FBSmhDQSxZQU1jLFlBQVk7OztBQ2NoQyxJQUFNLG1CQUFOLE1BQStDO0FBQUEsRUFFbEQsWUFBWSxnQkFBZ0MsY0FBNEIsY0FBcUM7QUF5QzdHLFNBQVEsY0FBYztBQXhDbEIsU0FBSyxjQUFjO0FBQ25CLFNBQUssZ0JBQWdCO0FBQ3JCLFNBQUssZ0JBQWdCO0FBQ3JCLFNBQUssYUFBYTtBQUFBLE1BQ2QsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLE1BQ0wsS0FBSztBQUFBLElBQ1Q7QUFDQSxTQUFLLGdCQUFnQjtBQUFBLE1BQ2pCLEtBQUs7QUFBQSxNQUNMLEtBQUs7QUFBQSxNQUNMLEtBQUs7QUFBQSxNQUNMLEtBQUs7QUFBQSxNQUNMLEtBQUs7QUFBQSxNQUNMLEtBQUs7QUFBQSxNQUNMLEtBQUs7QUFBQSxNQUNMLEtBQUs7QUFBQSxNQUNMLEtBQUs7QUFBQSxJQUNUO0FBQUEsRUFDSjtBQUFBO0FBQUEsRUFvQkEsTUFBYSxhQUE0QjtBQUVyQyxlQUFXLE9BQU8sS0FBSyxlQUFlO0FBR2xDLFlBQU0sWUFBNkMsS0FBSyxZQUFZLElBQUksS0FBSyxjQUFjLEdBQUcsQ0FBQztBQUMvRixVQUFJLENBQUMsVUFBVSxlQUFlLFVBQVUsU0FBUztBQUFNO0FBRXZELFlBQU0sTUFBZSxVQUFVO0FBQy9CLFVBQUksRUFBRSxlQUFlO0FBQWM7QUFFbkMsWUFBTSxTQUFTLElBQUksUUFBUTtBQUUzQixVQUFJLFVBQVU7QUFBVztBQUV6QixVQUFJLGlCQUFpQixhQUFhLE9BQUssS0FBSyxZQUFZLE1BQU0sQ0FBQztBQUFBLElBQ25FO0FBRUEsVUFBTSxLQUFLLFNBQVM7QUFFcEIsVUFBTSxhQUFhLEtBQUssWUFBWSxJQUFJQyxZQUFXLFdBQVc7QUFDOUQsUUFBSSxDQUFDLFdBQVcsZUFBZSxXQUFXLFNBQVMsUUFBUSxFQUFFLFdBQVcsZ0JBQWdCO0FBQWM7QUFDdEcsZUFBVyxLQUFLLGlCQUFpQixXQUFXLE9BQUssS0FBSyxVQUFVLENBQUM7QUFFakUsVUFBTSxzQkFBc0IsS0FBSyxZQUFZLElBQUlBLFlBQVcsZUFBZTtBQUMzRSxRQUFJLENBQUMsb0JBQW9CLGVBQWUsb0JBQW9CLFNBQVMsUUFBUSxFQUFFLG9CQUFvQixnQkFBZ0I7QUFBYztBQUNqSSx3QkFBb0IsS0FBSyxpQkFBaUIsYUFBYSxPQUFLLEtBQUssWUFBWSxDQUFDLENBQUM7QUFBQSxFQUVuRjtBQUFBLEVBQ0EsTUFBYSxXQUEwQjtBQUNuQyxRQUFJLE9BQU87QUFDWCxlQUFXLE9BQU8sS0FBSyxZQUFZO0FBRS9CLFVBQUksTUFBc0I7QUFFMUIsVUFBSSxPQUFPLEtBQUssZUFBZTtBQUMzQixjQUFNLFlBQTZDLEtBQUssWUFBWSxJQUFJLEtBQUssY0FBYyxHQUFHLENBQUM7QUFDL0YsWUFBSSxDQUFDLFVBQVUsZUFBZSxVQUFVLFNBQVM7QUFBTTtBQUV2RCxjQUFNLFVBQVU7QUFDaEIsWUFBSSxFQUFFLGVBQWU7QUFBYztBQUFBLE1BQ3ZDO0FBRUEsWUFBTSxjQUFjLEtBQUssY0FBYyxpQkFBaUIsSUFBSSxLQUFLLFdBQVcsR0FBRyxHQUFHO0FBQ2xGLFVBQUksWUFBWSxlQUFlLFlBQVksU0FBUyxNQUFNO0FBQ3RELGNBQU0sUUFBNkIsWUFBWTtBQUUvQyxjQUFNLFlBQVksS0FBSyxZQUFZLE9BQU8sSUFBSSxLQUFLLFdBQVcsR0FBRyxHQUFHO0FBQ3BFLFlBQUksQ0FBQyxVQUFVLGVBQWUsVUFBVSxTQUFTLE1BQU07QUFDbkQ7QUFBQSxRQUNKO0FBRUEsWUFBSSxNQUFNLFlBQVksUUFBUTtBQUMxQixjQUFJLFFBQVEsTUFBTTtBQUVkLGdCQUFJLE1BQU0sVUFBVTtBQUFBLFVBQ3hCO0FBR0Esb0JBQVUsS0FBSyxRQUFRLFNBQU87QUFDMUIsZ0JBQUksZUFBZSxhQUFhO0FBQzVCLGtCQUFJLE1BQU0sVUFBVTtBQUFBLFlBQ3hCO0FBQUEsVUFDSixDQUFDO0FBRUQ7QUFBQSxRQUNKLE9BQU87QUFFSCxnQkFBTSxlQUFlLE1BQU0sS0FBSyxjQUFjLGtCQUEwQixZQUFZLEtBQUssV0FBVyxHQUFHLENBQUM7QUFDeEcsZ0JBQU0scUJBQThCLGVBQWU7QUFFbkQsZ0JBQU0sUUFBZ0IscUJBQXFCLGVBQWUsT0FBTyxNQUFNLE1BQU0sTUFBTSxLQUFLLENBQUM7QUFFekYsa0JBQVE7QUFFUixjQUFJLFFBQVEsTUFBTTtBQUNkLGdCQUFJLE1BQU0sT0FBTyxHQUFHO0FBQUEsVUFDeEI7QUFFQSxjQUFJLG9CQUFvQjtBQUNwQixrQkFBTSxlQUFnRCxLQUFLLFlBQVksSUFBSSxJQUFJLEtBQUssV0FBVyxHQUFHLEdBQUc7QUFDckcsZ0JBQUksYUFBYSxlQUFlLGFBQWEsU0FBUyxNQUFNO0FBQ3hELGtCQUFJLGFBQWEsZ0JBQWdCLGFBQWE7QUFDMUMsNkJBQWEsS0FBSyxNQUFNLFFBQVEsR0FBRztBQUFBLGNBQ3ZDO0FBQUEsWUFDSjtBQUFBLFVBQ0o7QUFHQSxvQkFBVSxLQUFLLFFBQVEsU0FBTztBQUMxQixnQkFBSSxlQUFlLGFBQWE7QUFDNUIsa0JBQUksTUFBTSxRQUFRLHFCQUFxQixHQUFHLFlBQVksTUFBTTtBQUFBLFlBQ2hFO0FBQUEsVUFDSixDQUFDO0FBQUEsUUFDTDtBQUFBLE1BQ0o7QUFBQSxJQUNKO0FBQUEsRUFDSjtBQUFBO0FBQUEsRUFNUSxZQUFZLE9BQXFCO0FBQ3JDLFNBQUssY0FBYztBQUNuQixTQUFLLGlCQUFpQjtBQUFBLEVBQzFCO0FBQUEsRUFFUSxZQUFrQjtBQUN0QixTQUFLLGNBQWM7QUFDbkIsU0FBSyxpQkFBaUI7QUFBQSxFQUMxQjtBQUFBLEVBRUEsTUFBYyxZQUFZLEdBQThCO0FBQ3BELFFBQUksQ0FBQyxLQUFLLGVBQWUsS0FBSyxtQkFBbUI7QUFBTTtBQUV2RCxVQUFNLFlBQVksT0FBTyxLQUFLLGNBQWMsSUFBSTtBQUVoRCxVQUFNLGVBQWUsS0FBSyxXQUFXLEtBQUssY0FBYztBQUN4RCxVQUFNLFdBQVcsS0FBSyxXQUFXLEdBQUcsV0FBVztBQUUvQyxVQUFNLGVBQWUsS0FBSyxZQUFZLElBQUksSUFBSSxjQUFjO0FBQzVELFVBQU0sbUJBQW1CLEtBQUssWUFBWSxJQUFJLElBQUksVUFBVTtBQUM1RCxVQUFNLHNCQUFzQixLQUFLLFlBQVksSUFBSUEsWUFBVyxlQUFlO0FBQzNFLFVBQU0sZUFBZSxLQUFLLFlBQVksT0FBTyxJQUFJLGNBQWM7QUFDL0QsVUFBTSxtQkFBbUIsS0FBSyxZQUFZLE9BQU8sSUFBSSxVQUFVO0FBQy9ELFVBQU0sWUFBWSxLQUFLLFlBQVksSUFBSSxLQUFLLGNBQWMsS0FBSyxjQUFjLENBQUM7QUFHOUUsUUFBSSxDQUFDLGFBQWEsZUFBZSxhQUFhLFNBQVMsTUFBTTtBQUN6RDtBQUFBLElBQ0o7QUFDQSxRQUFJLENBQUMsYUFBYSxlQUFlLGFBQWEsU0FBUyxNQUFNO0FBQ3pEO0FBQUEsSUFDSjtBQUNBLFFBQUksQ0FBQyxVQUFVLGVBQWUsVUFBVSxTQUFTLE1BQU07QUFDbkQ7QUFBQSxJQUNKO0FBQ0EsUUFBSSxDQUFDLG9CQUFvQixlQUFlLG9CQUFvQixTQUFTLE1BQU07QUFDdkU7QUFBQSxJQUNKO0FBQ0EsUUFBSSxDQUFDLGlCQUFpQixlQUFlLGlCQUFpQixTQUFTLE1BQU07QUFDakU7QUFBQSxJQUNKO0FBQ0EsUUFBSSxDQUFDLGlCQUFpQixlQUFlLGlCQUFpQixTQUFTLE1BQU07QUFDakU7QUFBQSxJQUNKO0FBRUEsUUFBSSxFQUFFLGFBQWEsZ0JBQWdCO0FBQWM7QUFDakQsUUFBSSxFQUFFLGlCQUFpQixnQkFBZ0I7QUFBYztBQUVyRCxVQUFNLGFBQXNCLGFBQWEsS0FBSyxzQkFBc0I7QUFDcEUsVUFBTSxvQkFBNkIsb0JBQW9CLEtBQUssc0JBQXNCO0FBR2xGLFVBQU0sUUFBUSxFQUFFLFVBQVUsV0FBVztBQUNyQyxVQUFNLGFBQWEsUUFBUSxhQUFhLEtBQUs7QUFDN0MsVUFBTSxZQUFZLGlCQUFpQixLQUFLLGNBQWM7QUFFdEQsaUJBQWEsS0FBSyxNQUFNLFFBQVEsR0FBRztBQUNuQyxxQkFBaUIsS0FBSyxNQUFNLFFBQVEsR0FBRztBQUV2QyxpQkFBYSxLQUFLLFFBQVEsU0FBTztBQUM3QixVQUFJLEVBQUUsZUFBZTtBQUFjO0FBRW5DLFVBQUksTUFBTSxRQUFRLEdBQUc7QUFBQSxJQUN6QixDQUFDO0FBRUQscUJBQWlCLEtBQUssUUFBUSxTQUFPO0FBQ2pDLFVBQUksRUFBRSxlQUFlO0FBQWM7QUFFbkMsVUFBSSxNQUFNLFFBQVEsR0FBRztBQUFBLElBQ3pCLENBQUM7QUFFRCxVQUFNLEtBQUssY0FBYyxrQkFBa0IsWUFBWSxHQUFHLFNBQVMsWUFBWTtBQUMvRSxVQUFNLEtBQUssY0FBYyxrQkFBa0IsWUFBWSxHQUFHLGFBQWEsUUFBUTtBQUUvRSxRQUFJLEVBQUUsb0JBQW9CLGdCQUFnQjtBQUFjO0FBQ3hELFFBQUksRUFBRSxVQUFVLGdCQUFnQjtBQUFjO0FBRTlDLFVBQU0sT0FBTyxXQUFXLE9BQU8sa0JBQWtCLE9BQU8sUUFBUTtBQUNoRSxjQUFVLEtBQUssTUFBTSxPQUFPLEdBQUc7QUFBQSxFQUNuQztBQUFBO0FBR0o7OztBQ3ZQQSxlQUFzQixXQUFXLFlBQW1DO0FBQ2hFLFFBQU0sYUFBNkIsSUFBSSxtQkFBbUI7QUFDMUQsUUFBTSxlQUE2QixJQUFJLGlCQUFpQixVQUFVO0FBQ2xFLFFBQU0sZUFBNkIsSUFBSSxpQkFBaUIsWUFBWSxjQUFjLFVBQVU7QUFDNUYsUUFBTSxjQUEyQixJQUFJLGdCQUFnQixZQUFZLFVBQVU7QUFFM0UsUUFBTSxhQUFhLFdBQVc7QUFDOUIsY0FBWSxXQUFXO0FBQzNCO0FBRUEsZUFBc0IsU0FBUyxZQUFtQztBQUM5RCxRQUFNLGFBQTZCLElBQUksbUJBQW1CO0FBQzFELFFBQU0sZUFBNkIsSUFBSSxpQkFBaUIsVUFBVTtBQUNsRSxRQUFNLGVBQTZCLElBQUksaUJBQWlCLFlBQVksY0FBYyxVQUFVO0FBRTVGLFFBQU0sYUFBYSxTQUFTO0FBQ2hDO0FBRU8sU0FBUyxzQkFBOEI7QUFDMUMsUUFBTSxhQUE2QixJQUFJLG1CQUFtQjtBQUMxRCxRQUFNLFVBQVUsSUFBSSxxQkFBcUIsVUFBVTtBQUVuRCxTQUFPLFFBQVEsWUFBWTtBQUMvQjsiLAogICJuYW1lcyI6IFsiRWxlbWVudElEcyIsICJFbGVtZW50SURzIl0KfQo=
