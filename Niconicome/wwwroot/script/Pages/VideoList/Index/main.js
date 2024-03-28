// src/shared/AttemptResult.ts
var AttemptResultWidthDataImpl = class _AttemptResultWidthDataImpl {
  constructor(isSucceeded, data, message) {
    this.IsSucceeded = isSucceeded;
    this.Data = data;
    this.Message = message;
  }
  IsSucceeded;
  Data;
  Message;
  static Succeeded(data) {
    return new _AttemptResultWidthDataImpl(true, data, null);
  }
  static Fail(message) {
    return new _AttemptResultWidthDataImpl(false, null, message);
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
  _elmHandler;
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
  _elmHandler;
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

// src/videoList/dropHandler/drophandler.ts
var DropHandlerImpl = class {
  constructor(dotnet) {
    this._dotnet = dotnet;
  }
  //#region  field
  _dotnet;
  //#endregion
  Initialize() {
    window.addEventListener("dragover", (e) => e.preventDefault());
    window.addEventListener("drop", (e) => {
      e.preventDefault();
      if (e.dataTransfer === null) {
        return;
      }
      const targetList = [];
      e.dataTransfer.types.forEach((t) => {
        if (t === "text/plain") {
          const data = e.dataTransfer.getData("text/plain");
          if (data === "")
            return;
          targetList.push(data);
        }
      });
      if (e.dataTransfer.types.includes("Files")) {
        for (let i = 0; i < e.dataTransfer.files.length; i++) {
          const file = e.dataTransfer.files.item(i);
          if (file === null)
            continue;
          if (file.name === "")
            continue;
          if (file.name.endsWith(".url"))
            continue;
          targetList.push(file.name);
        }
      }
      const conevrted = targetList.map((t) => t.match(/(sm|so|nm)?[0-9]+/)?.[0] ?? "").filter((t) => t !== "");
      const distinct = [...new Set(conevrted)];
      if (distinct.length === 0) {
        return;
      }
      const result = distinct.join(" ");
      this._dotnet.invokeMethodAsync("OnDrop", result);
    });
  }
};

// src/videoList/sortHandler/elementIDs.ts
var ElementIDs = class {
  static VideoListRow = ".VideoListRow";
  static VideoListRowClassName = "VideoListRow";
  static VideoListBodyClassName = "VideoListBody";
  static DropTargetClassName = "DropTarget";
};

// src/videoList/sortHandler/sortHandler.ts
var SortHandlerImpl = class {
  constructor(elmHandler, dotnetHelper) {
    this._elmHandler = elmHandler;
    this._dotnetHelper = dotnetHelper;
  }
  _elmHandler;
  _dotnetHelper;
  _sourceNiconicoID = null;
  _sourceID = null;
  _lastOverElement = null;
  initialize(registeredList2) {
    const rowResult = this._elmHandler.GetAll(ElementIDs.VideoListRow);
    if (!rowResult.IsSucceeded || rowResult.Data === null) {
      return;
    }
    rowResult.Data.forEach((elm) => {
      if (elm instanceof HTMLElement) {
        const niconicoID = elm.dataset["niconicoid"];
        const playlistID = elm.dataset["playlistid"];
        if (niconicoID === void 0 || playlistID === void 0) {
          return;
        }
        const key = `${niconicoID}-${playlistID}`;
        if (registeredList2.includes(key)) {
          return;
        } else {
          registeredList2.push(key);
        }
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
  static PageContent = ".PageContent";
  static VideoListHeader = "#VideoListHeader";
  static Separator = ".Separator";
};

// src/videoList/widthHandler/widthHandler.ts
var WidthHandlerImpl = class {
  constructor(elementHandler, styleHandler, dotnetHelper) {
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
  //#region  field
  _elmHandler;
  _styleHandler;
  _columnIDs;
  _separatorIDs;
  _dotnetHelper;
  _isResizing = false;
  _resizingIndex;
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
async function initialize(blazorView, isFirstRender) {
  const elmHandler = new ElementHandlerImpl();
  const styleHandler = new StyleHandlerImpl(elmHandler);
  const widthHandler = new WidthHandlerImpl(elmHandler, styleHandler, blazorView);
  const sortHandler = new SortHandlerImpl(elmHandler, blazorView);
  const dropHandler = new DropHandlerImpl(blazorView);
  if (isFirstRender) {
    await widthHandler.initialize();
    dropHandler.Initialize();
  }
  sortHandler.initialize(registeredList);
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
var registeredList = [];
export {
  getSelectedIOfInput,
  initialize,
  setWidth
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0F0dGVtcHRSZXN1bHQudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0VsZW1lbnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3NoYXJlZC9TdHlsZUhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L1NlbGVjdGlvbkhhbmRsZXIvc2VsZWN0aW9uSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvZHJvcEhhbmRsZXIvZHJvcGhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL2VsZW1lbnRJRHMudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL3NvcnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3ZpZGVvTGlzdC93aWR0aEhhbmRsZXIvRWxlbWVudElkcy50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3Qvd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvbWFpbi50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiZXhwb3J0IGludGVyZmFjZSBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IGV4dGVuZHMgQXR0ZW1wdFJlc3VsdCB7XHJcbiAgICBcclxuICAgIC8qKlxyXG4gICAgICogXHUzMEM3XHUzMEZDXHUzMEJGXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG59XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEF0dGVtcHRSZXN1bHQge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU2MjEwXHU1MjlGXHUzMEQ1XHUzMEU5XHUzMEIwXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHUzMEUxXHUzMEMzXHUzMEJCXHUzMEZDXHUzMEI4XHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcbn1cclxuXHJcblxyXG5leHBvcnQgY2xhc3MgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGw8VD4gaW1wbGVtZW50cyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihpc1N1Y2NlZWRlZDogYm9vbGVhbiwgZGF0YTogVCB8IG51bGwsIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgICAgICB0aGlzLk1lc3NhZ2UgPSBtZXNzYWdlO1xyXG4gICAgfVxyXG5cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG5cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBTdWNjZWVkZWQ8VD4oZGF0YTogVCB8IG51bGwpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKHRydWUsIGRhdGEsIG51bGwpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgRmFpbDxUPihtZXNzYWdlOiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKGZhbHNlLCBudWxsLCBtZXNzYWdlKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIEF0dGVtcHRSZXN1bHRJbXBsIGltcGxlbWVudHMgQXR0ZW1wdFJlc3VsdCB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoaXNTdWNjZWVkZWQ6IGJvb2xlYW4sIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5NZXNzYWdlID0gbWVzc2FnZTtcclxuICAgIH1cclxuXHJcbiAgICByZWFkb25seSBJc1N1Y2NlZWRlZDogYm9vbGVhbjtcclxuXHJcbiAgICByZWFkb25seSBNZXNzYWdlOiBzdHJpbmcgfCBudWxsO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgU3VjY2VlZGVkKCk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwodHJ1ZSwgbnVsbCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBGYWlsKG1lc3NhZ2U6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwoZmFsc2UsIG1lc3NhZ2UpO1xyXG4gICAgfVxyXG59XHJcblxyXG4iLCAiaW1wb3J0IHsgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSwgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwgfSBmcm9tIFwiLi9BdHRlbXB0UmVzdWx0LnRzXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEVsZW1lbnRIYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXQocXVlcnk6c3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PjtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODkwN1x1NjU3MFx1MzA2RVx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj47XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBFbGVtZW50SGFuZGxlckltcGwgaW1wbGVtZW50cyBFbGVtZW50SGFuZGxlciB7XHJcblxyXG4gICAgcHVibGljIEdldChxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiB7XHJcblxyXG4gICAgICAgIGxldCByZXN1bHQ6IEVsZW1lbnQgfCBudWxsO1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gcmVzdWx0ID09IG51bGwgPyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5GYWlsKFwiXHU2MzA3XHU1QjlBXHUzMDU1XHUzMDhDXHUzMDVGXHU4OTgxXHU3RDIwXHUzMDRDXHU4OThCXHUzMDY0XHUzMDRCXHUzMDhBXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDAyXCIpIDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIHB1YmxpYyBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj4ge1xyXG5cclxuICAgICAgICBsZXQgcmVzdWx0OiBOb2RlTGlzdE9mPEVsZW1lbnQ+O1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG59IiwgImltcG9ydCB7IEF0dGVtcHRSZXN1bHRXaWR0aERhdGEsIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsIH0gZnJvbSBcIi4vQXR0ZW1wdFJlc3VsdFwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gXCIuL0VsZW1lbnRIYW5kbGVyXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFN0eWxlSGFuZGxlciB7XHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA2RVx1MzBCOVx1MzBCRlx1MzBBNFx1MzBFQlx1MzA5Mlx1NTNENlx1NUY5N1xyXG4gICAgICovXHJcbiAgICBHZXRDb21wdXRlZFN0eWxlKHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPENTU1N0eWxlRGVjbGFyYXRpb24+O1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU3R5bGVIYW5kbGVySW1wbCBpbXBsZW1lbnRzIFN0eWxlSGFuZGxlciB7XHJcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50SGFuZGxlcjogRWxlbWVudEhhbmRsZXIpIHtcclxuICAgICAgICB0aGlzLl9lbG1IYW5kbGVyID0gZWxlbWVudEhhbmRsZXI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHVibGljIEdldENvbXB1dGVkU3R5bGUocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Q1NTU3R5bGVEZWNsYXJhdGlvbj4ge1xyXG5cclxuICAgICAgICBjb25zdCByZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldChxdWVyeSk7XHJcbiAgICAgICAgaWYgKCFyZXN1bHQuSXNTdWNjZWVkZWQgfHwgcmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwocmVzdWx0Lk1lc3NhZ2UgPz8gXCJcIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICBjb25zdCBzdHlsZSA9IHdpbmRvdy5nZXRDb21wdXRlZFN0eWxlKHJlc3VsdC5EYXRhKTtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLlN1Y2NlZWRlZChzdHlsZSk7XHJcbiAgICAgICAgfSBjYXRjaCAoZXgpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoXCJcdTMwQjlcdTMwQkZcdTMwQTRcdTMwRUJcdTMwOTJcdTUzRDZcdTVGOTdcdTMwNjdcdTMwNERcdTMwN0VcdTMwNUJcdTMwOTNcdTMwNjdcdTMwNTdcdTMwNUZcdTMwMDJcIik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwgImltcG9ydCB7IEVsZW1lbnRIYW5kbGVyIH0gZnJvbSBcIi4uLy4uL3NoYXJlZC9FbGVtZW50SGFuZGxlci50c1wiO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBTZWxlY3Rpb25IYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1NTE2NVx1NTI5Qlx1NkIwNFx1MzA2N1x1OTA3OFx1NjI5RVx1MzA1NVx1MzA4Q1x1MzA2Nlx1MzA0NFx1MzA4Qlx1NjU4N1x1NUI1N1x1NTIxN1x1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICovXHJcbiAgICBnZXRTZWxlY3RlZCgpOiBzdHJpbmc7XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBTZWxlY3Rpb25IYW5kbGVySW1wbCBpbXBsZW1lbnRzIFNlbGVjdGlvbkhhbmRsZXIge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyKSB7XHJcbiAgICAgICAgdGhpcy5fZWxtSGFuZGxlciA9IGVsbUhhbmRsZXI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHVibGljIGdldFNlbGVjdGVkKCk6IHN0cmluZyB7XHJcblxyXG4gICAgICAgIGNvbnN0IGVsbVJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KFwiI0lucHV0Qm94XCIpO1xyXG4gICAgICAgIGlmICghZWxtUmVzdWx0LklzU3VjY2VlZGVkIHx8IGVsbVJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBcIlwiO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgZWxtOiBFbGVtZW50ID0gZWxtUmVzdWx0LkRhdGE7XHJcbiAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTElucHV0RWxlbWVudCkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIFwiXCI7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoIXRoaXMuSXNWYWxpZEluZGV4KGVsbS52YWx1ZSwgZWxtLnNlbGVjdGlvblN0YXJ0KSkge1xyXG4gICAgICAgICAgICByZXR1cm4gXCJcIjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICghdGhpcy5Jc1ZhbGlkSW5kZXgoZWxtLnZhbHVlLCBlbG0uc2VsZWN0aW9uRW5kLCB0cnVlKSkge1xyXG4gICAgICAgICAgICByZXR1cm4gXCJcIjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBlbG0udmFsdWUuc3Vic3RyaW5nKGVsbS5zZWxlY3Rpb25TdGFydCEsIGVsbS5zZWxlY3Rpb25FbmQhKVxyXG5cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIElzVmFsaWRJbmRleCh2YWx1ZTogc3RyaW5nLCBpbmRleDogbnVtYmVyIHwgbnVsbCwgaXNFbmQ6IGJvb2xlYW4gPSBmYWxzZSk6IGJvb2xlYW4ge1xyXG4gICAgICAgIGlmIChpbmRleCA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoaXNFbmQgJiYgaW5kZXggPiB2YWx1ZS5sZW5ndGgpIHtcclxuICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKCFpc0VuZCAmJiBpbmRleCA+IHZhbHVlLmxlbmd0aCAtIDEpIHtcclxuICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIHRydWU7XHJcbiAgICB9XHJcbn0iLCAiaW1wb3J0IHsgRG90TmV0T2JqZWN0UmVmZXJlbmNlIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0RvdE5ldE9iamVjdFJlZmVyZW5jZSc7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyJztcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgRHJvcEhhbmRsZXIge1xyXG4gICAgSW5pdGlhbGl6ZSgpOiB2b2lkO1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgRHJvcEhhbmRsZXJJbXBsIGltcGxlbWVudHMgRHJvcEhhbmRsZXIge1xyXG4gICAgY29uc3RydWN0b3IoZG90bmV0OiBEb3ROZXRPYmplY3RSZWZlcmVuY2UpIHtcclxuICAgICAgICB0aGlzLl9kb3RuZXQgPSBkb3RuZXQ7XHJcbiAgICB9XHJcblxyXG4gICAgLy8jcmVnaW9uICBmaWVsZFxyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2RvdG5ldDogRG90TmV0T2JqZWN0UmVmZXJlbmNlO1xyXG5cclxuICAgIC8vI2VuZHJlZ2lvblxyXG4gICAgcHVibGljIEluaXRpYWxpemUoKTogdm9pZCB7XHJcblxyXG4gICAgICAgIHdpbmRvdy5hZGRFdmVudExpc3RlbmVyKCdkcmFnb3ZlcicsIGUgPT4gZS5wcmV2ZW50RGVmYXVsdCgpKTtcclxuICAgICAgICB3aW5kb3cuYWRkRXZlbnRMaXN0ZW5lcignZHJvcCcsIGUgPT4ge1xyXG4gICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcblxyXG4gICAgICAgICAgICBpZiAoZS5kYXRhVHJhbnNmZXIgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgY29uc3QgdGFyZ2V0TGlzdDogc3RyaW5nW10gPSBbXTtcclxuXHJcbiAgICAgICAgICAgIGUuZGF0YVRyYW5zZmVyLnR5cGVzLmZvckVhY2godCA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAodCA9PT0gJ3RleHQvcGxhaW4nKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgZGF0YSA9IGUuZGF0YVRyYW5zZmVyIS5nZXREYXRhKCd0ZXh0L3BsYWluJyk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGRhdGEgPT09ICcnKSByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgdGFyZ2V0TGlzdC5wdXNoKGRhdGEpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGlmIChlLmRhdGFUcmFuc2Zlci50eXBlcy5pbmNsdWRlcygnRmlsZXMnKSkge1xyXG4gICAgICAgICAgICAgICAgZm9yIChsZXQgaSA9IDA7IGkgPCBlLmRhdGFUcmFuc2Zlci5maWxlcy5sZW5ndGg7IGkrKykge1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IGZpbGUgPSBlLmRhdGFUcmFuc2Zlci5maWxlcy5pdGVtKGkpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChmaWxlID09PSBudWxsKSBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoZmlsZS5uYW1lID09PSAnJykgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGZpbGUubmFtZS5lbmRzV2l0aCgnLnVybCcpKSBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgICAgICB0YXJnZXRMaXN0LnB1c2goZmlsZS5uYW1lKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgY29uc3QgY29uZXZydGVkID0gdGFyZ2V0TGlzdC5tYXAodCA9PiB0Lm1hdGNoKC8oc218c298bm0pP1swLTldKy8pPy5bMF0gPz8gJycpLmZpbHRlcih0ID0+IHQgIT09ICcnKTtcclxuICAgICAgICAgICAgY29uc3QgZGlzdGluY3QgPSBbLi4uKG5ldyBTZXQoY29uZXZydGVkKSldO1xyXG5cclxuICAgICAgICAgICAgaWYgKGRpc3RpbmN0Lmxlbmd0aCA9PT0gMCkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIFxyXG4gICAgICAgICAgICBjb25zdCByZXN1bHQgPSBkaXN0aW5jdC5qb2luKCcgJylcclxuXHJcbiAgICAgICAgICAgIHRoaXMuX2RvdG5ldC5pbnZva2VNZXRob2RBc3luYygnT25Ecm9wJywgcmVzdWx0KTtcclxuICAgICAgICB9KTtcclxuICAgIH1cclxufSIsICJleHBvcnQgY2xhc3MgRWxlbWVudElEcyB7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBWaWRlb0xpc3RSb3cgPSAnLlZpZGVvTGlzdFJvdyc7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBWaWRlb0xpc3RSb3dDbGFzc05hbWUgPSAnVmlkZW9MaXN0Um93JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdEJvZHlDbGFzc05hbWUgPSAnVmlkZW9MaXN0Qm9keSc7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBEcm9wVGFyZ2V0Q2xhc3NOYW1lID0gJ0Ryb3BUYXJnZXQnO1xyXG59IiwgImltcG9ydCB7IERvdE5ldE9iamVjdFJlZmVyZW5jZSB9IGZyb20gXCIuLi8uLi9zaGFyZWQvRG90TmV0T2JqZWN0UmVmZXJlbmNlXCI7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyLCBFbGVtZW50SGFuZGxlckltcGwgfSBmcm9tIFwiLi4vLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyXCI7XHJcbmltcG9ydCB7IEVsZW1lbnRJRHMgfSBmcm9tIFwiLi9lbGVtZW50SURzXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFNvcnRIYW5kbGVyIHtcclxuICAgIGluaXRpYWxpemUocmVnaXN0ZXJlZExpc3Q6IHN0cmluZ1tdKTogdm9pZDtcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFNvcnRIYW5kbGVySW1wbCBpbXBsZW1lbnRzIFNvcnRIYW5kbGVyIHtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlciwgZG90bmV0SGVscGVyOiBEb3ROZXRPYmplY3RSZWZlcmVuY2UpIHtcclxuICAgICAgICB0aGlzLl9lbG1IYW5kbGVyID0gZWxtSGFuZGxlcjtcclxuICAgICAgICB0aGlzLl9kb3RuZXRIZWxwZXIgPSBkb3RuZXRIZWxwZXI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZG90bmV0SGVscGVyOiBEb3ROZXRPYmplY3RSZWZlcmVuY2U7XHJcblxyXG4gICAgcHJpdmF0ZSBfc291cmNlTmljb25pY29JRDogc3RyaW5nIHwgbnVsbCA9IG51bGw7XHJcblxyXG4gICAgcHJpdmF0ZSBfc291cmNlSUQ6IHN0cmluZyB8IG51bGwgPSBudWxsO1xyXG5cclxuICAgIHByaXZhdGUgX2xhc3RPdmVyRWxlbWVudDogSFRNTEVsZW1lbnQgfCBudWxsID0gbnVsbDtcclxuXHJcbiAgICBwdWJsaWMgaW5pdGlhbGl6ZShyZWdpc3RlcmVkTGlzdDogc3RyaW5nW10pOiB2b2lkIHtcclxuICAgICAgICBjb25zdCByb3dSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldEFsbChFbGVtZW50SURzLlZpZGVvTGlzdFJvdyk7XHJcbiAgICAgICAgaWYgKCFyb3dSZXN1bHQuSXNTdWNjZWVkZWQgfHwgcm93UmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcm93UmVzdWx0LkRhdGEuZm9yRWFjaChlbG0gPT4ge1xyXG5cclxuICAgICAgICAgICAgaWYgKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcblxyXG4gICAgICAgICAgICAgICAgY29uc3Qgbmljb25pY29JRCA9IGVsbS5kYXRhc2V0WyduaWNvbmljb2lkJ107XHJcbiAgICAgICAgICAgICAgICBjb25zdCBwbGF5bGlzdElEID0gZWxtLmRhdGFzZXRbJ3BsYXlsaXN0aWQnXTtcclxuXHJcbiAgICAgICAgICAgICAgICBpZiAobmljb25pY29JRCA9PT0gdW5kZWZpbmVkIHx8IHBsYXlsaXN0SUQgPT09IHVuZGVmaW5lZCkge1xyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBjb25zdCBrZXk6IHN0cmluZyA9IGAke25pY29uaWNvSUR9LSR7cGxheWxpc3RJRH1gO1xyXG4gICAgICAgICAgICAgICAgaWYgKHJlZ2lzdGVyZWRMaXN0LmluY2x1ZGVzKGtleSkpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgIHJlZ2lzdGVyZWRMaXN0LnB1c2goa2V5KTtcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignZHJhZ3N0YXJ0JywgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCEoZS50YXJnZXQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgcm93OiBIVE1MRWxlbWVudCB8IG51bGwgPSB0aGlzLkdldFBhcmVudEJ5Q2xhc3NOYW1lKGUudGFyZ2V0LCBFbGVtZW50SURzLlZpZGVvTGlzdFJvd0NsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJvdyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9zb3VyY2VOaWNvbmljb0lEID0gcm93LmRhdGFzZXRbJ25pY29uaWNvaWQnXSA/PyBudWxsO1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuX3NvdXJjZUlEID0gcm93LmlkO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2RyYWdvdmVyJywgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHJvdzogSFRNTEVsZW1lbnQgfCBudWxsID0gdGhpcy5HZXRQYXJlbnRCeUNsYXNzTmFtZShlLnRhcmdldCwgRWxlbWVudElEcy5WaWRlb0xpc3RSb3dDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFyb3cuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNsYXNzTGlzdC5hZGQoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5fbGFzdE92ZXJFbGVtZW50ID0gcm93O1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2RyYWdsZWF2ZScsIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIShlLnRhcmdldCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByb3c6IEhUTUxFbGVtZW50IHwgbnVsbCA9IHRoaXMuR2V0UGFyZW50QnlDbGFzc05hbWUoZS50YXJnZXQsIEVsZW1lbnRJRHMuVmlkZW9MaXN0Um93Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocm93ID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcm93LmNsYXNzTGlzdC5yZW1vdmUoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignZHJvcCcsIGFzeW5jIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuX3NvdXJjZU5pY29uaWNvSUQgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgc291cmNlUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke3RoaXMuX3NvdXJjZUlEfWApO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghc291cmNlUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNvdXJjZVJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxldCBwYXJlbnQ6IFBhcmVudE5vZGUgfCBudWxsID0gZS50YXJnZXQucGFyZW50Tm9kZTtcclxuICAgICAgICAgICAgICAgICAgICBsZXQgZHJvcFRhcmdldDogSFRNTEVsZW1lbnQgPSBlLnRhcmdldDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgd2hpbGUgKHBhcmVudCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoIShwYXJlbnQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKCFwYXJlbnQuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuVmlkZW9MaXN0Qm9keUNsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRyb3BUYXJnZXQgPSBwYXJlbnQ7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQgPSBwYXJlbnQucGFyZW50Tm9kZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQuaW5zZXJ0QmVmb3JlKHNvdXJjZVJlc3VsdC5EYXRhLCBkcm9wVGFyZ2V0KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jKFwiTW92ZVZpZGVvXCIsIHRoaXMuX3NvdXJjZU5pY29uaWNvSUQsIGRyb3BUYXJnZXQuZGF0YXNldFsnbmljb25pY29pZCddISk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhcmVudCA9IG51bGw7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuX2xhc3RPdmVyRWxlbWVudCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5fbGFzdE92ZXJFbGVtZW50LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aGlzLl9sYXN0T3ZlckVsZW1lbnQuY2xhc3NMaXN0LnJlbW92ZShFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIEdldFBhcmVudEJ5Q2xhc3NOYW1lKGN1cnJlbnRFbGVtZW50OiBIVE1MRWxlbWVudCwgY2xhc3NOYW1lOiBzdHJpbmcpOiBIVE1MRWxlbWVudCB8IG51bGwge1xyXG4gICAgICAgIGxldCBwYXJlbnQ6IFBhcmVudE5vZGUgfCBudWxsID0gY3VycmVudEVsZW1lbnQ7XHJcblxyXG4gICAgICAgIHdoaWxlIChwYXJlbnQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgaWYgKCEocGFyZW50IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgaWYgKCFwYXJlbnQuY2xhc3NMaXN0LmNvbnRhaW5zKGNsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgIHBhcmVudCA9IHBhcmVudC5wYXJlbnROb2RlO1xyXG4gICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHJldHVybiBwYXJlbnQ7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgIH1cclxufSIsICJleHBvcnQgY2xhc3MgRWxlbWVudElEcyB7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBQYWdlQ29udGVudCA9ICcuUGFnZUNvbnRlbnQnO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgVmlkZW9MaXN0SGVhZGVyID0gJyNWaWRlb0xpc3RIZWFkZXInO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgU2VwYXJhdG9yID0gJy5TZXBhcmF0b3InO1xyXG5cclxufSIsICJpbXBvcnQgeyBBdHRlbXB0UmVzdWx0LCBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0F0dGVtcHRSZXN1bHQnO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gJy4uLy4uL3NoYXJlZC9FbGVtZW50SGFuZGxlcic7XHJcbmltcG9ydCB7IEVsZW1lbnRJRHMgfSBmcm9tICcuL0VsZW1lbnRJZHMnO1xyXG5pbXBvcnQgeyBEaWN0aW9uYXJ5IH0gZnJvbSAnLi4vLi4vc2hhcmVkL0NvbGxlY3Rpb24vZGljdGlvbmFyeSc7XHJcbmltcG9ydCB7IFN0eWxlSGFuZGxlciB9IGZyb20gJy4uLy4uL3NoYXJlZC9TdHlsZUhhbmRsZXInO1xyXG5pbXBvcnQgeyBEb3ROZXRPYmplY3RSZWZlcmVuY2UgfSBmcm9tICcuLi8uLi9zaGFyZWQvRG90TmV0T2JqZWN0UmVmZXJlbmNlJztcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgV2lkdGhIYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1NTIxRFx1NjcxRlx1NTMxNlxyXG4gICAgICovXHJcbiAgICBpbml0aWFsaXplKCk6IFByb21pc2U8dm9pZD47XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBcdTVFNDVcdTMwOTJcdTUxOERcdThBMkRcdTVCOUFcclxuICAgICAqL1xyXG4gICAgc2V0V2lkdGgoKTogUHJvbWlzZTx2b2lkPjtcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFdpZHRoSGFuZGxlckltcGwgaW1wbGVtZW50cyBXaWR0aEhhbmRsZXIge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGVsZW1lbnRIYW5kbGVyOiBFbGVtZW50SGFuZGxlciwgc3R5bGVIYW5kbGVyOiBTdHlsZUhhbmRsZXIsIGRvdG5ldEhlbHBlcjogRG90TmV0T2JqZWN0UmVmZXJlbmNlKSB7XHJcbiAgICAgICAgdGhpcy5fZWxtSGFuZGxlciA9IGVsZW1lbnRIYW5kbGVyO1xyXG4gICAgICAgIHRoaXMuX3N0eWxlSGFuZGxlciA9IHN0eWxlSGFuZGxlcjtcclxuICAgICAgICB0aGlzLl9kb3RuZXRIZWxwZXIgPSBkb3RuZXRIZWxwZXI7XHJcbiAgICAgICAgdGhpcy5fY29sdW1uSURzID0ge1xyXG4gICAgICAgICAgICAnMCc6ICdDaGVja0JveENvbHVtbicsXHJcbiAgICAgICAgICAgICcxJzogJ1RodW1ibmFpbENvbHVtbicsXHJcbiAgICAgICAgICAgICcyJzogJ1RpdGxlQ29sdW1uJyxcclxuICAgICAgICAgICAgJzMnOiAnVXBsb2FkZWREYXRlVGltZUNvbHVtbicsXHJcbiAgICAgICAgICAgICc0JzogJ0lzRG93bmxvYWRlZENvbHVtbicsXHJcbiAgICAgICAgICAgICc1JzogJ1ZpZXdDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc2JzogJ0NvbW1lbnRDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc3JzogJ015bGlzdENvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzgnOiAnTGlrZUNvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzknOiAnTWVzc2FnZUNvbHVtbicsXHJcbiAgICAgICAgfTtcclxuICAgICAgICB0aGlzLl9zZXBhcmF0b3JJRHMgPSB7XHJcbiAgICAgICAgICAgICcwJzogJyNDaGVja0JveENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICcxJzogJyNUaHVtYm5haWxDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMic6ICcjVGl0bGVDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMyc6ICcjVXBsb2FkZWREYXRlVGltZUNvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc0JzogJyNJc0Rvd25sb2FkZWRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNSc6ICcjVmlld0NvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzYnOiAnI0NvbW1lbnRDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc3JzogJyNNeWxpc3RDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc4JzogJyNMaWtlQ291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgIH07XHJcbiAgICB9XHJcblxyXG4gICAgLy8jcmVnaW9uICBmaWVsZFxyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2VsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyO1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX3N0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyO1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2NvbHVtbklEczogRGljdGlvbmFyeTxzdHJpbmc+O1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX3NlcGFyYXRvcklEczogRGljdGlvbmFyeTxzdHJpbmc+O1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2RvdG5ldEhlbHBlcjogRG90TmV0T2JqZWN0UmVmZXJlbmNlO1xyXG5cclxuICAgIHByaXZhdGUgX2lzUmVzaXppbmcgPSBmYWxzZTtcclxuXHJcbiAgICBwcml2YXRlIF9yZXNpemluZ0luZGV4OiBudWxsIHwgc3RyaW5nO1xyXG5cclxuICAgIC8vI2VuZHJlZ2lvblxyXG5cclxuICAgIHB1YmxpYyBhc3luYyBpbml0aWFsaXplKCk6IFByb21pc2U8dm9pZD4ge1xyXG5cclxuICAgICAgICBmb3IgKGNvbnN0IGtleSBpbiB0aGlzLl9zZXBhcmF0b3JJRHMpIHtcclxuXHJcblxyXG4gICAgICAgICAgICBjb25zdCBzZXBSZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldCh0aGlzLl9zZXBhcmF0b3JJRHNba2V5XSk7XHJcbiAgICAgICAgICAgIGlmICghc2VwUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNlcFJlc3VsdC5EYXRhID09PSBudWxsKSBjb250aW51ZTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGVsbTogRWxlbWVudCA9IHNlcFJlc3VsdC5EYXRhO1xyXG4gICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgY29uc3QgaW5kZXhTID0gZWxtLmRhdGFzZXQuaW5kZXg7XHJcblxyXG4gICAgICAgICAgICBpZiAoaW5kZXhTID09IHVuZGVmaW5lZCkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignbW91c2Vkb3duJywgXyA9PiB0aGlzLk9uTW91c2VEb3duKGluZGV4UykpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgYXdhaXQgdGhpcy5zZXRXaWR0aCgpO1xyXG5cclxuICAgICAgICBjb25zdCBwYWdlUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoRWxlbWVudElEcy5QYWdlQ29udGVudCk7XHJcbiAgICAgICAgaWYgKCFwYWdlUmVzdWx0LklzU3VjY2VlZGVkIHx8IHBhZ2VSZXN1bHQuRGF0YSA9PT0gbnVsbCB8fCAhKHBhZ2VSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIHBhZ2VSZXN1bHQuRGF0YS5hZGRFdmVudExpc3RlbmVyKCdtb3VzZXVwJywgXyA9PiB0aGlzLk9uTW91c2VVcCgpKTtcclxuXHJcbiAgICAgICAgY29uc3QgaGVhZGVyV3JhcHBlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KEVsZW1lbnRJRHMuVmlkZW9MaXN0SGVhZGVyKTtcclxuICAgICAgICBpZiAoIWhlYWRlcldyYXBwZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhID09PSBudWxsIHx8ICEoaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNlbW92ZScsIGUgPT4gdGhpcy5Pbk1vdXNlTW92ZShlKSk7XHJcblxyXG4gICAgfVxyXG4gICAgcHVibGljIGFzeW5jIHNldFdpZHRoKCk6IFByb21pc2U8dm9pZD4ge1xyXG4gICAgICAgIGxldCBsZWZ0ID0gMDtcclxuICAgICAgICBmb3IgKGNvbnN0IGtleSBpbiB0aGlzLl9jb2x1bW5JRHMpIHtcclxuXHJcbiAgICAgICAgICAgIGxldCBlbG06IEVsZW1lbnQgfCBudWxsID0gbnVsbDtcclxuXHJcbiAgICAgICAgICAgIGlmIChrZXkgaW4gdGhpcy5fc2VwYXJhdG9ySURzKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBzZXBSZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldCh0aGlzLl9zZXBhcmF0b3JJRHNba2V5XSk7XHJcbiAgICAgICAgICAgICAgICBpZiAoIXNlcFJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzZXBSZXN1bHQuRGF0YSA9PT0gbnVsbCkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtID0gc2VwUmVzdWx0LkRhdGE7XHJcbiAgICAgICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBjb25zdCBzdHlsZVJlc3VsdCA9IHRoaXMuX3N0eWxlSGFuZGxlci5HZXRDb21wdXRlZFN0eWxlKGAjJHt0aGlzLl9jb2x1bW5JRHNba2V5XX1gKTtcclxuICAgICAgICAgICAgaWYgKHN0eWxlUmVzdWx0LklzU3VjY2VlZGVkICYmIHN0eWxlUmVzdWx0LkRhdGEgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0IHN0eWxlOiBDU1NTdHlsZURlY2xhcmF0aW9uID0gc3R5bGVSZXN1bHQuRGF0YTtcclxuXHJcbiAgICAgICAgICAgICAgICBjb25zdCByYXdSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldEFsbChgLiR7dGhpcy5fY29sdW1uSURzW2tleV19YCk7XHJcbiAgICAgICAgICAgICAgICBpZiAoIXJhd1Jlc3VsdC5Jc1N1Y2NlZWRlZCB8fCByYXdSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGlmIChzdHlsZS5kaXNwbGF5ID09PSBcIm5vbmVcIikge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChlbG0gIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgLy9cdTMwRDhcdTMwQzNcdTMwQzBcdTMwRkNcdTMwNENcdTk3NUVcdTg4NjhcdTc5M0FcdTMwNkFcdTMwODlcdTMwQkJcdTMwRDFcdTMwRUNcdTMwRkNcdTMwQkZcdTMwRkNcdTMwODJcdTk3NUVcdTg4NjhcdTc5M0FcclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxtLnN0eWxlLmRpc3BsYXkgPSBcIm5vbmVcIjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vXHUzMEVBXHUzMEI5XHUzMEM4XHU1MDc0XHUzMDgyXHU5NzVFXHU4ODY4XHU3OTNBXHUzMDZCXHUzMDU5XHUzMDhCXHJcbiAgICAgICAgICAgICAgICAgICAgcmF3UmVzdWx0LkRhdGEuZm9yRWFjaChyYXcgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocmF3IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJhdy5zdHlsZS5kaXNwbGF5ID0gXCJub25lXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByZXN0b3JlV2lkdGggPSBhd2FpdCB0aGlzLl9kb3RuZXRIZWxwZXIuaW52b2tlTWV0aG9kQXN5bmM8bnVtYmVyPignR2V0V2lkdGgnLCB0aGlzLl9jb2x1bW5JRHNba2V5XSk7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgc2hvdWxkUmVzdG9yZVdpZHRoOiBib29sZWFuID0gcmVzdG9yZVdpZHRoID4gMDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgd2lkdGg6IG51bWJlciA9IHNob3VsZFJlc3RvcmVXaWR0aCA/IHJlc3RvcmVXaWR0aCA6IE51bWJlcihzdHlsZS53aWR0aC5tYXRjaCgvXFxkKy8pKTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgbGVmdCArPSB3aWR0aDtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGVsbSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbG0uc3R5bGUubGVmdCA9IGAke2xlZnR9cHhgO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHNob3VsZFJlc3RvcmVXaWR0aCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBoZWFkZXJSZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldChgIyR7dGhpcy5fY29sdW1uSURzW2tleV19YCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChoZWFkZXJSZXN1bHQuSXNTdWNjZWVkZWQgJiYgaGVhZGVyUmVzdWx0LkRhdGEgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChoZWFkZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyUmVzdWx0LkRhdGEuc3R5bGUud2lkdGggPSBgJHt3aWR0aH1weGA7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIC8vXHUzMEVBXHUzMEI5XHUzMEM4XHU1MDc0XHUzMDZFXHU1RTQ1XHUzMDkyXHUzMEQ4XHUzMEMzXHUzMEMwXHUzMEZDXHUzMDZCXHU1NDA4XHUzMDhGXHUzMDVCXHUzMDhCXHJcbiAgICAgICAgICAgICAgICAgICAgcmF3UmVzdWx0LkRhdGEuZm9yRWFjaChyYXcgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocmF3IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJhdy5zdHlsZS53aWR0aCA9IHNob3VsZFJlc3RvcmVXaWR0aCA/IGAke3dpZHRofXB4YCA6IHN0eWxlLndpZHRoO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG5cclxuXHJcbiAgICAvLyNyZWdpb24gcHJpdmF0ZVxyXG5cclxuICAgIHByaXZhdGUgT25Nb3VzZURvd24oaW5kZXg6IHN0cmluZyk6IHZvaWQge1xyXG4gICAgICAgIHRoaXMuX2lzUmVzaXppbmcgPSB0cnVlO1xyXG4gICAgICAgIHRoaXMuX3Jlc2l6aW5nSW5kZXggPSBpbmRleDtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIE9uTW91c2VVcCgpOiB2b2lkIHtcclxuICAgICAgICB0aGlzLl9pc1Jlc2l6aW5nID0gZmFsc2U7XHJcbiAgICAgICAgdGhpcy5fcmVzaXppbmdJbmRleCA9IG51bGw7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3luYyBPbk1vdXNlTW92ZShlOiBNb3VzZUV2ZW50KTogUHJvbWlzZTx2b2lkPiB7XHJcbiAgICAgICAgaWYgKCF0aGlzLl9pc1Jlc2l6aW5nIHx8IHRoaXMuX3Jlc2l6aW5nSW5kZXggPT09IG51bGwpIHJldHVybjtcclxuXHJcbiAgICAgICAgY29uc3QgbmV4dEluZGV4ID0gTnVtYmVyKHRoaXMuX3Jlc2l6aW5nSW5kZXgpICsgMTtcclxuXHJcbiAgICAgICAgY29uc3QgcmVzaXppbmdOYW1lID0gdGhpcy5fY29sdW1uSURzW3RoaXMuX3Jlc2l6aW5nSW5kZXhdO1xyXG4gICAgICAgIGNvbnN0IG5leHROYW1lID0gdGhpcy5fY29sdW1uSURzW2Ake25leHRJbmRleH1gXTtcclxuXHJcbiAgICAgICAgY29uc3QgaGVhZGVyUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke3Jlc2l6aW5nTmFtZX1gKTtcclxuICAgICAgICBjb25zdCBuZXh0SGVhZGVyUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke25leHROYW1lfWApO1xyXG4gICAgICAgIGNvbnN0IGhlYWRlcldyYXBwZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChFbGVtZW50SURzLlZpZGVvTGlzdEhlYWRlcik7XHJcbiAgICAgICAgY29uc3QgY29sdW1uUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoYC4ke3Jlc2l6aW5nTmFtZX1gKTtcclxuICAgICAgICBjb25zdCBuZXh0Q29sdW1uUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoYC4ke25leHROYW1lfWApO1xyXG4gICAgICAgIGNvbnN0IHNlcFJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KHRoaXMuX3NlcGFyYXRvcklEc1t0aGlzLl9yZXNpemluZ0luZGV4XSk7XHJcblxyXG4gICAgICAgIC8vXHU4OTgxXHU3RDIwXHU1M0Q2XHU1Rjk3XHUzMDZCXHU1OTMxXHU2NTU3XHUzMDU3XHUzMDVGXHUzMDg5cmV0dXJuXHJcbiAgICAgICAgaWYgKCFoZWFkZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgaGVhZGVyUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIWNvbHVtblJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBjb2x1bW5SZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghc2VwUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNlcFJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKCFoZWFkZXJXcmFwcGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghbmV4dEhlYWRlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBuZXh0SGVhZGVyUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuXHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghbmV4dENvbHVtblJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBuZXh0Q29sdW1uUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKCEoaGVhZGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBpZiAoIShuZXh0SGVhZGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgY29uc3QgaGVhZGVyUmVjdDogRE9NUmVjdCA9IGhlYWRlclJlc3VsdC5EYXRhLmdldEJvdW5kaW5nQ2xpZW50UmVjdCgpO1xyXG4gICAgICAgIGNvbnN0IGhlYWRlcldyYXBwZXJSZWN0OiBET01SZWN0ID0gaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhLmdldEJvdW5kaW5nQ2xpZW50UmVjdCgpO1xyXG5cclxuXHJcbiAgICAgICAgY29uc3Qgd2lkdGggPSBlLmNsaWVudFggLSBoZWFkZXJSZWN0LmxlZnQ7XHJcbiAgICAgICAgY29uc3QgZGVsdGFXaWR0aCA9IHdpZHRoIC0gaGVhZGVyUmVzdWx0LkRhdGEub2Zmc2V0V2lkdGg7XHJcbiAgICAgICAgY29uc3QgbmV4dFdpZHRoID0gbmV4dEhlYWRlclJlc3VsdC5EYXRhLm9mZnNldFdpZHRoIC0gZGVsdGFXaWR0aDtcclxuXHJcbiAgICAgICAgaGVhZGVyUmVzdWx0LkRhdGEuc3R5bGUud2lkdGggPSBgJHt3aWR0aH1weGA7XHJcbiAgICAgICAgbmV4dEhlYWRlclJlc3VsdC5EYXRhLnN0eWxlLndpZHRoID0gYCR7bmV4dFdpZHRofXB4YDtcclxuXHJcbiAgICAgICAgY29sdW1uUmVzdWx0LkRhdGEuZm9yRWFjaChlbG0gPT4ge1xyXG4gICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgIGVsbS5zdHlsZS53aWR0aCA9IGAke3dpZHRofXB4YDtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgbmV4dENvbHVtblJlc3VsdC5EYXRhLmZvckVhY2goZWxtID0+IHtcclxuICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgICAgICBlbG0uc3R5bGUud2lkdGggPSBgJHtuZXh0V2lkdGh9cHhgO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBhd2FpdCB0aGlzLl9kb3RuZXRIZWxwZXIuaW52b2tlTWV0aG9kQXN5bmMoJ1NldFdpZHRoJywgYCR7d2lkdGh9YCwgcmVzaXppbmdOYW1lKTtcclxuICAgICAgICBhd2FpdCB0aGlzLl9kb3RuZXRIZWxwZXIuaW52b2tlTWV0aG9kQXN5bmMoJ1NldFdpZHRoJywgYCR7bmV4dFdpZHRofWAsIG5leHROYW1lKTtcclxuXHJcbiAgICAgICAgaWYgKCEoaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgaWYgKCEoc2VwUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgY29uc3QgbGVmdCA9IGhlYWRlclJlY3QubGVmdCAtIGhlYWRlcldyYXBwZXJSZWN0LmxlZnQgKyB3aWR0aCAtIDEwO1xyXG4gICAgICAgIHNlcFJlc3VsdC5EYXRhLnN0eWxlLmxlZnQgPSBgJHtsZWZ0fXB4YDtcclxuICAgIH1cclxuXHJcbiAgICAvLyNlbmRyZWdpb25cclxufVxyXG5cclxuaW50ZXJmYWNlIENsYXNzTmFtZXNEaWN0IHtcclxuICAgIFtpbmRleDogbnVtYmVyXTogc3RyaW5nO1xyXG59IiwgImltcG9ydCB7IERvdE5ldE9iamVjdFJlZmVyZW5jZSB9IGZyb20gXCIuLi9zaGFyZWQvRG90TmV0T2JqZWN0UmVmZXJlbmNlXCI7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyLCBFbGVtZW50SGFuZGxlckltcGwgfSBmcm9tIFwiLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyXCI7XHJcbmltcG9ydCB7IFN0eWxlSGFuZGxlciwgU3R5bGVIYW5kbGVySW1wbCB9IGZyb20gXCIuLi9zaGFyZWQvU3R5bGVIYW5kbGVyXCI7XHJcbmltcG9ydCB7IFNlbGVjdGlvbkhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vU2VsZWN0aW9uSGFuZGxlci9zZWxlY3Rpb25IYW5kbGVyXCI7XHJcbmltcG9ydCB7IERyb3BIYW5kbGVyLCBEcm9wSGFuZGxlckltcGwgfSBmcm9tIFwiLi9kcm9wSGFuZGxlci9kcm9waGFuZGxlclwiO1xyXG5pbXBvcnQgeyBTb3J0SGFuZGxlciwgU29ydEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vc29ydEhhbmRsZXIvc29ydEhhbmRsZXJcIjtcclxuaW1wb3J0IHsgV2lkdGhIYW5kbGVyLCBXaWR0aEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlclwiO1xyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGluaXRpYWxpemUoYmxhem9yVmlldzogRG90TmV0T2JqZWN0UmVmZXJlbmNlLCBpc0ZpcnN0UmVuZGVyOiBib29sZWFuKSB7XHJcbiAgICBjb25zdCBlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlciA9IG5ldyBFbGVtZW50SGFuZGxlckltcGwoKTtcclxuICAgIGNvbnN0IHN0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyID0gbmV3IFN0eWxlSGFuZGxlckltcGwoZWxtSGFuZGxlcik7XHJcbiAgICBjb25zdCB3aWR0aEhhbmRsZXI6IFdpZHRoSGFuZGxlciA9IG5ldyBXaWR0aEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIsIHN0eWxlSGFuZGxlciwgYmxhem9yVmlldyk7XHJcbiAgICBjb25zdCBzb3J0SGFuZGxlcjogU29ydEhhbmRsZXIgPSBuZXcgU29ydEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIsIGJsYXpvclZpZXcpXHJcbiAgICBjb25zdCBkcm9wSGFuZGxlcjogRHJvcEhhbmRsZXIgPSBuZXcgRHJvcEhhbmRsZXJJbXBsKGJsYXpvclZpZXcpO1xyXG5cclxuICAgIGlmIChpc0ZpcnN0UmVuZGVyKSB7XHJcbiAgICAgICAgYXdhaXQgd2lkdGhIYW5kbGVyLmluaXRpYWxpemUoKTtcclxuICAgICAgICBkcm9wSGFuZGxlci5Jbml0aWFsaXplKCk7XHJcbiAgICB9XHJcblxyXG4gICAgc29ydEhhbmRsZXIuaW5pdGlhbGl6ZShyZWdpc3RlcmVkTGlzdCk7XHJcbn1cclxuXHJcbmV4cG9ydCBhc3luYyBmdW5jdGlvbiBzZXRXaWR0aChibGF6b3JWaWV3OiBEb3ROZXRPYmplY3RSZWZlcmVuY2UpIHtcclxuICAgIGNvbnN0IGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyID0gbmV3IEVsZW1lbnRIYW5kbGVySW1wbCgpO1xyXG4gICAgY29uc3Qgc3R5bGVIYW5kbGVyOiBTdHlsZUhhbmRsZXIgPSBuZXcgU3R5bGVIYW5kbGVySW1wbChlbG1IYW5kbGVyKTtcclxuICAgIGNvbnN0IHdpZHRoSGFuZGxlcjogV2lkdGhIYW5kbGVyID0gbmV3IFdpZHRoSGFuZGxlckltcGwoZWxtSGFuZGxlciwgc3R5bGVIYW5kbGVyLCBibGF6b3JWaWV3KTtcclxuXHJcbiAgICBhd2FpdCB3aWR0aEhhbmRsZXIuc2V0V2lkdGgoKTtcclxufVxyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFNlbGVjdGVkSU9mSW5wdXQoKTogc3RyaW5nIHtcclxuICAgIGNvbnN0IGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyID0gbmV3IEVsZW1lbnRIYW5kbGVySW1wbCgpO1xyXG4gICAgY29uc3QgaGFuZGxlciA9IG5ldyBTZWxlY3Rpb25IYW5kbGVySW1wbChlbG1IYW5kbGVyKTtcclxuXHJcbiAgICByZXR1cm4gaGFuZGxlci5nZXRTZWxlY3RlZCgpO1xyXG59XHJcblxyXG5sZXQgcmVnaXN0ZXJlZExpc3Q6IHN0cmluZ1tdID0gW107XHJcbiJdLAogICJtYXBwaW5ncyI6ICI7QUFzQk8sSUFBTSw2QkFBTixNQUFNLDRCQUFtRTtBQUFBLEVBRTVFLFlBQVksYUFBc0IsTUFBZ0IsU0FBd0I7QUFDdEUsU0FBSyxjQUFjO0FBQ25CLFNBQUssT0FBTztBQUNaLFNBQUssVUFBVTtBQUFBLEVBQ25CO0FBQUEsRUFFUztBQUFBLEVBRUE7QUFBQSxFQUVBO0FBQUEsRUFFVCxPQUFjLFVBQWEsTUFBMkM7QUFDbEUsV0FBTyxJQUFJLDRCQUEyQixNQUFNLE1BQU0sSUFBSTtBQUFBLEVBQzFEO0FBQUEsRUFFQSxPQUFjLEtBQVEsU0FBNEM7QUFDOUQsV0FBTyxJQUFJLDRCQUEyQixPQUFPLE1BQU0sT0FBTztBQUFBLEVBQzlEO0FBQ0o7OztBQzFCTyxJQUFNLHFCQUFOLE1BQW1EO0FBQUEsRUFFL0MsSUFBSSxPQUFnRDtBQUV2RCxRQUFJO0FBRUosUUFBSTtBQUNBLGVBQVMsU0FBUyxjQUFjLEtBQUs7QUFBQSxJQUN6QyxTQUFTLEdBQVE7QUFDYixhQUFPLDJCQUEyQixLQUFLLDBHQUFxQixFQUFFLE9BQU8sR0FBRztBQUFBLElBQzVFO0FBRUEsV0FBTyxVQUFVLE9BQU8sMkJBQTJCLEtBQUssa0dBQWtCLElBQUksMkJBQTJCLFVBQVUs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LFFBQVE7QUFDaEUsY0FBVSxLQUFLLE1BQU0sT0FBTyxHQUFHLElBQUk7QUFBQSxFQUN2QztBQUFBO0FBR0o7OztBQ3RQQSxlQUFzQixXQUFXLFlBQW1DLGVBQXdCO0FBQ3hGLFFBQU0sYUFBNkIsSUFBSSxtQkFBbUI7QUFDMUQsUUFBTSxlQUE2QixJQUFJLGlCQUFpQixVQUFVO0FBQ2xFLFFBQU0sZUFBNkIsSUFBSSxpQkFBaUIsWUFBWSxjQUFjLFVBQVU7QUFDNUYsUUFBTSxjQUEyQixJQUFJLGdCQUFnQixZQUFZLFVBQVU7QUFDM0UsUUFBTSxjQUEyQixJQUFJLGdCQUFnQixVQUFVO0FBRS9ELE1BQUksZUFBZTtBQUNmLFVBQU0sYUFBYSxXQUFXO0FBQzlCLGdCQUFZLFdBQVc7QUFBQSxFQUMzQjtBQUVBLGNBQVksV0FBVyxjQUFjO0FBQ3pDO0FBRUEsZUFBc0IsU0FBUyxZQUFtQztBQUM5RCxRQUFNLGFBQTZCLElBQUksbUJBQW1CO0FBQzFELFFBQU0sZUFBNkIsSUFBSSxpQkFBaUIsVUFBVTtBQUNsRSxRQUFNLGVBQTZCLElBQUksaUJBQWlCLFlBQVksY0FBYyxVQUFVO0FBRTVGLFFBQU0sYUFBYSxTQUFTO0FBQ2hDO0FBRU8sU0FBUyxzQkFBOEI7QUFDMUMsUUFBTSxhQUE2QixJQUFJLG1CQUFtQjtBQUMxRCxRQUFNLFVBQVUsSUFBSSxxQkFBcUIsVUFBVTtBQUVuRCxTQUFPLFFBQVEsWUFBWTtBQUMvQjtBQUVBLElBQUksaUJBQTJCLENBQUM7IiwKICAibmFtZXMiOiBbInJlZ2lzdGVyZWRMaXN0IiwgIkVsZW1lbnRJRHMiLCAiRWxlbWVudElEcyJdCn0K
