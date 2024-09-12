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
      return AttemptResultWidthDataImpl.Fail(
        `\u8981\u7D20\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002(\u8A73\u7D30\uFF1A${e.message})`
      );
    }
    return result == null ? AttemptResultWidthDataImpl.Fail("\u6307\u5B9A\u3055\u308C\u305F\u8981\u7D20\u304C\u898B\u3064\u304B\u308A\u307E\u305B\u3093\u3002") : AttemptResultWidthDataImpl.Succeeded(result);
  }
  GetAll(query) {
    let result;
    try {
      result = document.querySelectorAll(query);
    } catch (e) {
      return AttemptResultWidthDataImpl.Fail(
        `\u8981\u7D20\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002(\u8A73\u7D30\uFF1A${e.message})`
      );
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
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0F0dGVtcHRSZXN1bHQudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0VsZW1lbnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3NoYXJlZC9TdHlsZUhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L1NlbGVjdGlvbkhhbmRsZXIvc2VsZWN0aW9uSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvZHJvcEhhbmRsZXIvZHJvcGhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL2VsZW1lbnRJRHMudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL3NvcnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3ZpZGVvTGlzdC93aWR0aEhhbmRsZXIvRWxlbWVudElkcy50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3Qvd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvbWFpbi50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiZXhwb3J0IGludGVyZmFjZSBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IGV4dGVuZHMgQXR0ZW1wdFJlc3VsdCB7XHJcbiAgICBcclxuICAgIC8qKlxyXG4gICAgICogXHUzMEM3XHUzMEZDXHUzMEJGXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG59XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEF0dGVtcHRSZXN1bHQge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU2MjEwXHU1MjlGXHUzMEQ1XHUzMEU5XHUzMEIwXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHUzMEUxXHUzMEMzXHUzMEJCXHUzMEZDXHUzMEI4XHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcbn1cclxuXHJcblxyXG5leHBvcnQgY2xhc3MgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGw8VD4gaW1wbGVtZW50cyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihpc1N1Y2NlZWRlZDogYm9vbGVhbiwgZGF0YTogVCB8IG51bGwsIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgICAgICB0aGlzLk1lc3NhZ2UgPSBtZXNzYWdlO1xyXG4gICAgfVxyXG5cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG5cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBTdWNjZWVkZWQ8VD4oZGF0YTogVCB8IG51bGwpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKHRydWUsIGRhdGEsIG51bGwpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgRmFpbDxUPihtZXNzYWdlOiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKGZhbHNlLCBudWxsLCBtZXNzYWdlKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIEF0dGVtcHRSZXN1bHRJbXBsIGltcGxlbWVudHMgQXR0ZW1wdFJlc3VsdCB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoaXNTdWNjZWVkZWQ6IGJvb2xlYW4sIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5NZXNzYWdlID0gbWVzc2FnZTtcclxuICAgIH1cclxuXHJcbiAgICByZWFkb25seSBJc1N1Y2NlZWRlZDogYm9vbGVhbjtcclxuXHJcbiAgICByZWFkb25seSBNZXNzYWdlOiBzdHJpbmcgfCBudWxsO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgU3VjY2VlZGVkKCk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwodHJ1ZSwgbnVsbCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBGYWlsKG1lc3NhZ2U6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwoZmFsc2UsIG1lc3NhZ2UpO1xyXG4gICAgfVxyXG59XHJcblxyXG4iLCAiLy8vIDxyZWZlcmVuY2UgbGliPVwiZG9tXCIgLz5cclxuXHJcbmltcG9ydCB7XHJcbiAgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSxcclxuICBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbCxcclxufSBmcm9tIFwiLi9BdHRlbXB0UmVzdWx0LnRzXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEVsZW1lbnRIYW5kbGVyIHtcclxuICAvKipcclxuICAgKiBcdTg5ODFcdTdEMjBcdTMwOTJcdTUzRDZcdTVGOTdcdTMwNTlcdTMwOEJcclxuICAgKiBAcGFyYW0gcXVlcnkgXHUzMEFGXHUzMEE4XHUzMEVBXHJcbiAgICovXHJcbiAgR2V0KHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+O1xyXG5cclxuICAvKipcclxuICAgKiBcdTg5MDdcdTY1NzBcdTMwNkVcdTg5ODFcdTdEMjBcdTMwOTJcdTUzRDZcdTVGOTdcdTMwNTlcdTMwOEJcclxuICAgKiBAcGFyYW0gcXVlcnkgXHUzMEFGXHUzMEE4XHUzMEVBXHJcbiAgICovXHJcbiAgR2V0QWxsKHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPE5vZGVMaXN0T2Y8RWxlbWVudD4+O1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgRWxlbWVudEhhbmRsZXJJbXBsIGltcGxlbWVudHMgRWxlbWVudEhhbmRsZXIge1xyXG4gIHB1YmxpYyBHZXQocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4ge1xyXG4gICAgbGV0IHJlc3VsdDogRWxlbWVudCB8IG51bGw7XHJcblxyXG4gICAgdHJ5IHtcclxuICAgICAgcmVzdWx0ID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcihxdWVyeSk7XHJcbiAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoXHJcbiAgICAgICAgYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCxcclxuICAgICAgKTtcclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4gcmVzdWx0ID09IG51bGxcclxuICAgICAgPyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5GYWlsKFwiXHU2MzA3XHU1QjlBXHUzMDU1XHUzMDhDXHUzMDVGXHU4OTgxXHU3RDIwXHUzMDRDXHU4OThCXHUzMDY0XHUzMDRCXHUzMDhBXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDAyXCIpXHJcbiAgICAgIDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgfVxyXG5cclxuICBwdWJsaWMgR2V0QWxsKHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPE5vZGVMaXN0T2Y8RWxlbWVudD4+IHtcclxuICAgIGxldCByZXN1bHQ6IE5vZGVMaXN0T2Y8RWxlbWVudD47XHJcblxyXG4gICAgdHJ5IHtcclxuICAgICAgcmVzdWx0ID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvckFsbChxdWVyeSk7XHJcbiAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoXHJcbiAgICAgICAgYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCxcclxuICAgICAgKTtcclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgfVxyXG59XHJcbiIsICJpbXBvcnQgeyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhLCBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbCB9IGZyb20gXCIuL0F0dGVtcHRSZXN1bHRcIjtcclxuaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIgfSBmcm9tIFwiLi9FbGVtZW50SGFuZGxlclwiO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBTdHlsZUhhbmRsZXIge1xyXG4gICAgLyoqXHJcbiAgICAgKiBcdTg5ODFcdTdEMjBcdTMwNkVcdTMwQjlcdTMwQkZcdTMwQTRcdTMwRUJcdTMwOTJcdTUzRDZcdTVGOTdcclxuICAgICAqL1xyXG4gICAgR2V0Q29tcHV0ZWRTdHlsZShxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxDU1NTdHlsZURlY2xhcmF0aW9uPjtcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIFN0eWxlSGFuZGxlckltcGwgaW1wbGVtZW50cyBTdHlsZUhhbmRsZXIge1xyXG4gICAgY29uc3RydWN0b3IoZWxlbWVudEhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyKSB7XHJcbiAgICAgICAgdGhpcy5fZWxtSGFuZGxlciA9IGVsZW1lbnRIYW5kbGVyO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2VsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyO1xyXG5cclxuICAgIHB1YmxpYyBHZXRDb21wdXRlZFN0eWxlKHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPENTU1N0eWxlRGVjbGFyYXRpb24+IHtcclxuXHJcbiAgICAgICAgY29uc3QgcmVzdWx0OiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQocXVlcnkpO1xyXG4gICAgICAgIGlmICghcmVzdWx0LklzU3VjY2VlZGVkIHx8IHJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5GYWlsKHJlc3VsdC5NZXNzYWdlID8/IFwiXCIpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdHJ5IHtcclxuICAgICAgICAgICAgY29uc3Qgc3R5bGUgPSB3aW5kb3cuZ2V0Q29tcHV0ZWRTdHlsZShyZXN1bHQuRGF0YSk7XHJcbiAgICAgICAgICAgIHJldHVybiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5TdWNjZWVkZWQoc3R5bGUpO1xyXG4gICAgICAgIH0gY2F0Y2ggKGV4KSB7XHJcbiAgICAgICAgICAgIHJldHVybiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5GYWlsKFwiXHUzMEI5XHUzMEJGXHUzMEE0XHUzMEVCXHUzMDkyXHU1M0Q2XHU1Rjk3XHUzMDY3XHUzMDREXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDY3XHUzMDU3XHUzMDVGXHUzMDAyXCIpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxufSIsICJpbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gXCIuLi8uLi9zaGFyZWQvRWxlbWVudEhhbmRsZXIudHNcIjtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgU2VsZWN0aW9uSGFuZGxlciB7XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBcdTUxNjVcdTUyOUJcdTZCMDRcdTMwNjdcdTkwNzhcdTYyOUVcdTMwNTVcdTMwOENcdTMwNjZcdTMwNDRcdTMwOEJcdTY1ODdcdTVCNTdcdTUyMTdcdTMwOTJcdTUzRDZcdTVGOTdcdTMwNTlcdTMwOEJcclxuICAgICAqL1xyXG4gICAgZ2V0U2VsZWN0ZWQoKTogc3RyaW5nO1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU2VsZWN0aW9uSGFuZGxlckltcGwgaW1wbGVtZW50cyBTZWxlY3Rpb25IYW5kbGVyIHtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcikge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbG1IYW5kbGVyO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2VsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyO1xyXG5cclxuICAgIHB1YmxpYyBnZXRTZWxlY3RlZCgpOiBzdHJpbmcge1xyXG5cclxuICAgICAgICBjb25zdCBlbG1SZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChcIiNJbnB1dEJveFwiKTtcclxuICAgICAgICBpZiAoIWVsbVJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBlbG1SZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm4gXCJcIjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGNvbnN0IGVsbTogRWxlbWVudCA9IGVsbVJlc3VsdC5EYXRhO1xyXG4gICAgICAgIGlmICghKGVsbSBpbnN0YW5jZW9mIEhUTUxJbnB1dEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBcIlwiO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKCF0aGlzLklzVmFsaWRJbmRleChlbG0udmFsdWUsIGVsbS5zZWxlY3Rpb25TdGFydCkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIFwiXCI7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoIXRoaXMuSXNWYWxpZEluZGV4KGVsbS52YWx1ZSwgZWxtLnNlbGVjdGlvbkVuZCwgdHJ1ZSkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIFwiXCI7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gZWxtLnZhbHVlLnN1YnN0cmluZyhlbG0uc2VsZWN0aW9uU3RhcnQhLCBlbG0uc2VsZWN0aW9uRW5kISlcclxuXHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBJc1ZhbGlkSW5kZXgodmFsdWU6IHN0cmluZywgaW5kZXg6IG51bWJlciB8IG51bGwsIGlzRW5kOiBib29sZWFuID0gZmFsc2UpOiBib29sZWFuIHtcclxuICAgICAgICBpZiAoaW5kZXggPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKGlzRW5kICYmIGluZGV4ID4gdmFsdWUubGVuZ3RoKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICghaXNFbmQgJiYgaW5kZXggPiB2YWx1ZS5sZW5ndGggLSAxKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiB0cnVlO1xyXG4gICAgfVxyXG59IiwgImltcG9ydCB7IERvdE5ldE9iamVjdFJlZmVyZW5jZSB9IGZyb20gJy4uLy4uL3NoYXJlZC9Eb3ROZXRPYmplY3RSZWZlcmVuY2UnO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gJy4uLy4uL3NoYXJlZC9FbGVtZW50SGFuZGxlcic7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIERyb3BIYW5kbGVyIHtcclxuICAgIEluaXRpYWxpemUoKTogdm9pZDtcclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIERyb3BIYW5kbGVySW1wbCBpbXBsZW1lbnRzIERyb3BIYW5kbGVyIHtcclxuICAgIGNvbnN0cnVjdG9yKGRvdG5ldDogRG90TmV0T2JqZWN0UmVmZXJlbmNlKSB7XHJcbiAgICAgICAgdGhpcy5fZG90bmV0ID0gZG90bmV0O1xyXG4gICAgfVxyXG5cclxuICAgIC8vI3JlZ2lvbiAgZmllbGRcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9kb3RuZXQ6IERvdE5ldE9iamVjdFJlZmVyZW5jZTtcclxuXHJcbiAgICAvLyNlbmRyZWdpb25cclxuICAgIHB1YmxpYyBJbml0aWFsaXplKCk6IHZvaWQge1xyXG5cclxuICAgICAgICB3aW5kb3cuYWRkRXZlbnRMaXN0ZW5lcignZHJhZ292ZXInLCBlID0+IGUucHJldmVudERlZmF1bHQoKSk7XHJcbiAgICAgICAgd2luZG93LmFkZEV2ZW50TGlzdGVuZXIoJ2Ryb3AnLCBlID0+IHtcclxuICAgICAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG5cclxuICAgICAgICAgICAgaWYgKGUuZGF0YVRyYW5zZmVyID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGNvbnN0IHRhcmdldExpc3Q6IHN0cmluZ1tdID0gW107XHJcblxyXG4gICAgICAgICAgICBlLmRhdGFUcmFuc2Zlci50eXBlcy5mb3JFYWNoKHQgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKHQgPT09ICd0ZXh0L3BsYWluJykge1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IGRhdGEgPSBlLmRhdGFUcmFuc2ZlciEuZ2V0RGF0YSgndGV4dC9wbGFpbicpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChkYXRhID09PSAnJykgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIHRhcmdldExpc3QucHVzaChkYXRhKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICBpZiAoZS5kYXRhVHJhbnNmZXIudHlwZXMuaW5jbHVkZXMoJ0ZpbGVzJykpIHtcclxuICAgICAgICAgICAgICAgIGZvciAobGV0IGkgPSAwOyBpIDwgZS5kYXRhVHJhbnNmZXIuZmlsZXMubGVuZ3RoOyBpKyspIHtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBmaWxlID0gZS5kYXRhVHJhbnNmZXIuZmlsZXMuaXRlbShpKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoZmlsZSA9PT0gbnVsbCkgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGZpbGUubmFtZSA9PT0gJycpIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChmaWxlLm5hbWUuZW5kc1dpdGgoJy51cmwnKSkgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICAgICAgdGFyZ2V0TGlzdC5wdXNoKGZpbGUubmFtZSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGNvbmV2cnRlZCA9IHRhcmdldExpc3QubWFwKHQgPT4gdC5tYXRjaCgvKHNtfHNvfG5tKT9bMC05XSsvKT8uWzBdID8/ICcnKS5maWx0ZXIodCA9PiB0ICE9PSAnJyk7XHJcbiAgICAgICAgICAgIGNvbnN0IGRpc3RpbmN0ID0gWy4uLihuZXcgU2V0KGNvbmV2cnRlZCkpXTtcclxuXHJcbiAgICAgICAgICAgIGlmIChkaXN0aW5jdC5sZW5ndGggPT09IDApIHtcclxuICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICBcclxuICAgICAgICAgICAgY29uc3QgcmVzdWx0ID0gZGlzdGluY3Quam9pbignICcpXHJcblxyXG4gICAgICAgICAgICB0aGlzLl9kb3RuZXQuaW52b2tlTWV0aG9kQXN5bmMoJ09uRHJvcCcsIHJlc3VsdCk7XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcbn0iLCAiZXhwb3J0IGNsYXNzIEVsZW1lbnRJRHMge1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgVmlkZW9MaXN0Um93ID0gJy5WaWRlb0xpc3RSb3cnO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgVmlkZW9MaXN0Um93Q2xhc3NOYW1lID0gJ1ZpZGVvTGlzdFJvdyc7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBWaWRlb0xpc3RCb2R5Q2xhc3NOYW1lID0gJ1ZpZGVvTGlzdEJvZHknO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgRHJvcFRhcmdldENsYXNzTmFtZSA9ICdEcm9wVGFyZ2V0JztcclxufSIsICJpbXBvcnQgeyBEb3ROZXRPYmplY3RSZWZlcmVuY2UgfSBmcm9tIFwiLi4vLi4vc2hhcmVkL0RvdE5ldE9iamVjdFJlZmVyZW5jZVwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciwgRWxlbWVudEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4uLy4uL3NoYXJlZC9FbGVtZW50SGFuZGxlclwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SURzIH0gZnJvbSBcIi4vZWxlbWVudElEc1wiO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBTb3J0SGFuZGxlciB7XHJcbiAgICBpbml0aWFsaXplKHJlZ2lzdGVyZWRMaXN0OiBzdHJpbmdbXSk6IHZvaWQ7XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBTb3J0SGFuZGxlckltcGwgaW1wbGVtZW50cyBTb3J0SGFuZGxlciB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXIsIGRvdG5ldEhlbHBlcjogRG90TmV0T2JqZWN0UmVmZXJlbmNlKSB7XHJcbiAgICAgICAgdGhpcy5fZWxtSGFuZGxlciA9IGVsbUhhbmRsZXI7XHJcbiAgICAgICAgdGhpcy5fZG90bmV0SGVscGVyID0gZG90bmV0SGVscGVyO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2VsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyO1xyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2RvdG5ldEhlbHBlcjogRG90TmV0T2JqZWN0UmVmZXJlbmNlO1xyXG5cclxuICAgIHByaXZhdGUgX3NvdXJjZU5pY29uaWNvSUQ6IHN0cmluZyB8IG51bGwgPSBudWxsO1xyXG5cclxuICAgIHByaXZhdGUgX3NvdXJjZUlEOiBzdHJpbmcgfCBudWxsID0gbnVsbDtcclxuXHJcbiAgICBwcml2YXRlIF9sYXN0T3ZlckVsZW1lbnQ6IEhUTUxFbGVtZW50IHwgbnVsbCA9IG51bGw7XHJcblxyXG4gICAgcHVibGljIGluaXRpYWxpemUocmVnaXN0ZXJlZExpc3Q6IHN0cmluZ1tdKTogdm9pZCB7XHJcbiAgICAgICAgY29uc3Qgcm93UmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoRWxlbWVudElEcy5WaWRlb0xpc3RSb3cpO1xyXG4gICAgICAgIGlmICghcm93UmVzdWx0LklzU3VjY2VlZGVkIHx8IHJvd1Jlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJvd1Jlc3VsdC5EYXRhLmZvckVhY2goZWxtID0+IHtcclxuXHJcbiAgICAgICAgICAgIGlmIChlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkge1xyXG5cclxuICAgICAgICAgICAgICAgIGNvbnN0IG5pY29uaWNvSUQgPSBlbG0uZGF0YXNldFsnbmljb25pY29pZCddO1xyXG4gICAgICAgICAgICAgICAgY29uc3QgcGxheWxpc3RJRCA9IGVsbS5kYXRhc2V0WydwbGF5bGlzdGlkJ107XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKG5pY29uaWNvSUQgPT09IHVuZGVmaW5lZCB8fCBwbGF5bGlzdElEID09PSB1bmRlZmluZWQpIHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgY29uc3Qga2V5OiBzdHJpbmcgPSBgJHtuaWNvbmljb0lEfS0ke3BsYXlsaXN0SUR9YDtcclxuICAgICAgICAgICAgICAgIGlmIChyZWdpc3RlcmVkTGlzdC5pbmNsdWRlcyhrZXkpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICByZWdpc3RlcmVkTGlzdC5wdXNoKGtleSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2RyYWdzdGFydCcsIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHJvdzogSFRNTEVsZW1lbnQgfCBudWxsID0gdGhpcy5HZXRQYXJlbnRCeUNsYXNzTmFtZShlLnRhcmdldCwgRWxlbWVudElEcy5WaWRlb0xpc3RSb3dDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5fc291cmNlTmljb25pY29JRCA9IHJvdy5kYXRhc2V0WyduaWNvbmljb2lkJ10gPz8gbnVsbDtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9zb3VyY2VJRCA9IHJvdy5pZDtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgICAgIGVsbS5hZGRFdmVudExpc3RlbmVyKCdkcmFnb3ZlcicsIGUgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIShlLnRhcmdldCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByb3c6IEhUTUxFbGVtZW50IHwgbnVsbCA9IHRoaXMuR2V0UGFyZW50QnlDbGFzc05hbWUoZS50YXJnZXQsIEVsZW1lbnRJRHMuVmlkZW9MaXN0Um93Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocm93ID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICghcm93LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJvdy5jbGFzc0xpc3QuYWRkKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuX2xhc3RPdmVyRWxlbWVudCA9IHJvdztcclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgICAgIGVsbS5hZGRFdmVudExpc3RlbmVyKCdkcmFnbGVhdmUnLCBlID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCEoZS50YXJnZXQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgcm93OiBIVE1MRWxlbWVudCB8IG51bGwgPSB0aGlzLkdldFBhcmVudEJ5Q2xhc3NOYW1lKGUudGFyZ2V0LCBFbGVtZW50SURzLlZpZGVvTGlzdFJvd0NsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJvdyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAocm93LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJvdy5jbGFzc0xpc3QucmVtb3ZlKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ2Ryb3AnLCBhc3luYyBlID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLl9zb3VyY2VOaWNvbmljb0lEID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHNvdXJjZVJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KGAjJHt0aGlzLl9zb3VyY2VJRH1gKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIXNvdXJjZVJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzb3VyY2VSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoIShlLnRhcmdldCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBsZXQgcGFyZW50OiBQYXJlbnROb2RlIHwgbnVsbCA9IGUudGFyZ2V0LnBhcmVudE5vZGU7XHJcbiAgICAgICAgICAgICAgICAgICAgbGV0IGRyb3BUYXJnZXQ6IEhUTUxFbGVtZW50ID0gZS50YXJnZXQ7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHdoaWxlIChwYXJlbnQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKCEocGFyZW50IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICghcGFyZW50LmNsYXNzTGlzdC5jb250YWlucyhFbGVtZW50SURzLlZpZGVvTGlzdEJvZHlDbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBkcm9wVGFyZ2V0ID0gcGFyZW50O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcGFyZW50ID0gcGFyZW50LnBhcmVudE5vZGU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFyZW50Lmluc2VydEJlZm9yZShzb3VyY2VSZXN1bHQuRGF0YSwgZHJvcFRhcmdldCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGF3YWl0IHRoaXMuX2RvdG5ldEhlbHBlci5pbnZva2VNZXRob2RBc3luYyhcIk1vdmVWaWRlb1wiLCB0aGlzLl9zb3VyY2VOaWNvbmljb0lELCBkcm9wVGFyZ2V0LmRhdGFzZXRbJ25pY29uaWNvaWQnXSEpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYXJlbnQgPSBudWxsO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLl9sYXN0T3ZlckVsZW1lbnQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuX2xhc3RPdmVyRWxlbWVudC5jbGFzc0xpc3QuY29udGFpbnMoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5fbGFzdE92ZXJFbGVtZW50LmNsYXNzTGlzdC5yZW1vdmUoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBHZXRQYXJlbnRCeUNsYXNzTmFtZShjdXJyZW50RWxlbWVudDogSFRNTEVsZW1lbnQsIGNsYXNzTmFtZTogc3RyaW5nKTogSFRNTEVsZW1lbnQgfCBudWxsIHtcclxuICAgICAgICBsZXQgcGFyZW50OiBQYXJlbnROb2RlIHwgbnVsbCA9IGN1cnJlbnRFbGVtZW50O1xyXG5cclxuICAgICAgICB3aGlsZSAocGFyZW50ICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgIGlmICghKHBhcmVudCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIG51bGw7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGlmICghcGFyZW50LmNsYXNzTGlzdC5jb250YWlucyhjbGFzc05hbWUpKSB7XHJcbiAgICAgICAgICAgICAgICBwYXJlbnQgPSBwYXJlbnQucGFyZW50Tm9kZTtcclxuICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gcGFyZW50O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIG51bGw7XHJcbiAgICB9XHJcbn0iLCAiZXhwb3J0IGNsYXNzIEVsZW1lbnRJRHMge1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgUGFnZUNvbnRlbnQgPSAnLlBhZ2VDb250ZW50JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdEhlYWRlciA9ICcjVmlkZW9MaXN0SGVhZGVyJztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFNlcGFyYXRvciA9ICcuU2VwYXJhdG9yJztcclxuXHJcbn0iLCAiaW1wb3J0IHsgQXR0ZW1wdFJlc3VsdCwgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSB9IGZyb20gJy4uLy4uL3NoYXJlZC9BdHRlbXB0UmVzdWx0JztcclxuaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIgfSBmcm9tICcuLi8uLi9zaGFyZWQvRWxlbWVudEhhbmRsZXInO1xyXG5pbXBvcnQgeyBFbGVtZW50SURzIH0gZnJvbSAnLi9FbGVtZW50SWRzJztcclxuaW1wb3J0IHsgRGljdGlvbmFyeSB9IGZyb20gJy4uLy4uL3NoYXJlZC9Db2xsZWN0aW9uL2RpY3Rpb25hcnknO1xyXG5pbXBvcnQgeyBTdHlsZUhhbmRsZXIgfSBmcm9tICcuLi8uLi9zaGFyZWQvU3R5bGVIYW5kbGVyJztcclxuaW1wb3J0IHsgRG90TmV0T2JqZWN0UmVmZXJlbmNlIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0RvdE5ldE9iamVjdFJlZmVyZW5jZSc7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFdpZHRoSGFuZGxlciB7XHJcblxyXG4gICAgLyoqXHJcbiAgICAgKiBcdTUyMURcdTY3MUZcdTUzMTZcclxuICAgICAqL1xyXG4gICAgaW5pdGlhbGl6ZSgpOiBQcm9taXNlPHZvaWQ+O1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU1RTQ1XHUzMDkyXHU1MThEXHU4QTJEXHU1QjlBXHJcbiAgICAgKi9cclxuICAgIHNldFdpZHRoKCk6IFByb21pc2U8dm9pZD47XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBXaWR0aEhhbmRsZXJJbXBsIGltcGxlbWVudHMgV2lkdGhIYW5kbGVyIHtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50SGFuZGxlcjogRWxlbWVudEhhbmRsZXIsIHN0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyLCBkb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZSkge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbGVtZW50SGFuZGxlcjtcclxuICAgICAgICB0aGlzLl9zdHlsZUhhbmRsZXIgPSBzdHlsZUhhbmRsZXI7XHJcbiAgICAgICAgdGhpcy5fZG90bmV0SGVscGVyID0gZG90bmV0SGVscGVyO1xyXG4gICAgICAgIHRoaXMuX2NvbHVtbklEcyA9IHtcclxuICAgICAgICAgICAgJzAnOiAnQ2hlY2tCb3hDb2x1bW4nLFxyXG4gICAgICAgICAgICAnMSc6ICdUaHVtYm5haWxDb2x1bW4nLFxyXG4gICAgICAgICAgICAnMic6ICdUaXRsZUNvbHVtbicsXHJcbiAgICAgICAgICAgICczJzogJ1VwbG9hZGVkRGF0ZVRpbWVDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNCc6ICdJc0Rvd25sb2FkZWRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNSc6ICdWaWV3Q291bnRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNic6ICdDb21tZW50Q291bnRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnNyc6ICdNeWxpc3RDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc4JzogJ0xpa2VDb3VudENvbHVtbicsXHJcbiAgICAgICAgICAgICc5JzogJ01lc3NhZ2VDb2x1bW4nLFxyXG4gICAgICAgIH07XHJcbiAgICAgICAgdGhpcy5fc2VwYXJhdG9ySURzID0ge1xyXG4gICAgICAgICAgICAnMCc6ICcjQ2hlY2tCb3hDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnMSc6ICcjVGh1bWJuYWlsQ29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzInOiAnI1RpdGxlQ29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzMnOiAnI1VwbG9hZGVkRGF0ZVRpbWVDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNCc6ICcjSXNEb3dubG9hZGVkQ29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzUnOiAnI1ZpZXdDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc2JzogJyNDb21tZW50Q291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNyc6ICcjTXlsaXN0Q291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnOCc6ICcjTGlrZUNvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICB9O1xyXG4gICAgfVxyXG5cclxuICAgIC8vI3JlZ2lvbiAgZmllbGRcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9lbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9zdHlsZUhhbmRsZXI6IFN0eWxlSGFuZGxlcjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9jb2x1bW5JRHM6IERpY3Rpb25hcnk8c3RyaW5nPjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9zZXBhcmF0b3JJRHM6IERpY3Rpb25hcnk8c3RyaW5nPjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9kb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZTtcclxuXHJcbiAgICBwcml2YXRlIF9pc1Jlc2l6aW5nID0gZmFsc2U7XHJcblxyXG4gICAgcHJpdmF0ZSBfcmVzaXppbmdJbmRleDogbnVsbCB8IHN0cmluZztcclxuXHJcbiAgICAvLyNlbmRyZWdpb25cclxuXHJcbiAgICBwdWJsaWMgYXN5bmMgaW5pdGlhbGl6ZSgpOiBQcm9taXNlPHZvaWQ+IHtcclxuXHJcbiAgICAgICAgZm9yIChjb25zdCBrZXkgaW4gdGhpcy5fc2VwYXJhdG9ySURzKSB7XHJcblxyXG5cclxuICAgICAgICAgICAgY29uc3Qgc2VwUmVzdWx0OiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQodGhpcy5fc2VwYXJhdG9ySURzW2tleV0pO1xyXG4gICAgICAgICAgICBpZiAoIXNlcFJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzZXBSZXN1bHQuRGF0YSA9PT0gbnVsbCkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICBjb25zdCBlbG06IEVsZW1lbnQgPSBzZXBSZXN1bHQuRGF0YTtcclxuICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSBjb250aW51ZTtcclxuXHJcbiAgICAgICAgICAgIGNvbnN0IGluZGV4UyA9IGVsbS5kYXRhc2V0LmluZGV4O1xyXG5cclxuICAgICAgICAgICAgaWYgKGluZGV4UyA9PSB1bmRlZmluZWQpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgZWxtLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNlZG93bicsIF8gPT4gdGhpcy5Pbk1vdXNlRG93bihpbmRleFMpKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGF3YWl0IHRoaXMuc2V0V2lkdGgoKTtcclxuXHJcbiAgICAgICAgY29uc3QgcGFnZVJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KEVsZW1lbnRJRHMuUGFnZUNvbnRlbnQpO1xyXG4gICAgICAgIGlmICghcGFnZVJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBwYWdlUmVzdWx0LkRhdGEgPT09IG51bGwgfHwgIShwYWdlUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBwYWdlUmVzdWx0LkRhdGEuYWRkRXZlbnRMaXN0ZW5lcignbW91c2V1cCcsIF8gPT4gdGhpcy5Pbk1vdXNlVXAoKSk7XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRlcldyYXBwZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChFbGVtZW50SURzLlZpZGVvTGlzdEhlYWRlcik7XHJcbiAgICAgICAgaWYgKCFoZWFkZXJXcmFwcGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCB8fCAhKGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YS5hZGRFdmVudExpc3RlbmVyKCdtb3VzZW1vdmUnLCBlID0+IHRoaXMuT25Nb3VzZU1vdmUoZSkpO1xyXG5cclxuICAgIH1cclxuICAgIHB1YmxpYyBhc3luYyBzZXRXaWR0aCgpOiBQcm9taXNlPHZvaWQ+IHtcclxuICAgICAgICBsZXQgbGVmdCA9IDA7XHJcbiAgICAgICAgZm9yIChjb25zdCBrZXkgaW4gdGhpcy5fY29sdW1uSURzKSB7XHJcblxyXG4gICAgICAgICAgICBsZXQgZWxtOiBFbGVtZW50IHwgbnVsbCA9IG51bGw7XHJcblxyXG4gICAgICAgICAgICBpZiAoa2V5IGluIHRoaXMuX3NlcGFyYXRvcklEcykge1xyXG4gICAgICAgICAgICAgICAgY29uc3Qgc2VwUmVzdWx0OiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQodGhpcy5fc2VwYXJhdG9ySURzW2tleV0pO1xyXG4gICAgICAgICAgICAgICAgaWYgKCFzZXBSZXN1bHQuSXNTdWNjZWVkZWQgfHwgc2VwUmVzdWx0LkRhdGEgPT09IG51bGwpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgICAgIGVsbSA9IHNlcFJlc3VsdC5EYXRhO1xyXG4gICAgICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSBjb250aW51ZTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgY29uc3Qgc3R5bGVSZXN1bHQgPSB0aGlzLl9zdHlsZUhhbmRsZXIuR2V0Q29tcHV0ZWRTdHlsZShgIyR7dGhpcy5fY29sdW1uSURzW2tleV19YCk7XHJcbiAgICAgICAgICAgIGlmIChzdHlsZVJlc3VsdC5Jc1N1Y2NlZWRlZCAmJiBzdHlsZVJlc3VsdC5EYXRhICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBzdHlsZTogQ1NTU3R5bGVEZWNsYXJhdGlvbiA9IHN0eWxlUmVzdWx0LkRhdGE7XHJcblxyXG4gICAgICAgICAgICAgICAgY29uc3QgcmF3UmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXRBbGwoYC4ke3RoaXMuX2NvbHVtbklEc1trZXldfWApO1xyXG4gICAgICAgICAgICAgICAgaWYgKCFyYXdSZXN1bHQuSXNTdWNjZWVkZWQgfHwgcmF3UmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICBpZiAoc3R5bGUuZGlzcGxheSA9PT0gXCJub25lXCIpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoZWxtICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vXHUzMEQ4XHUzMEMzXHUzMEMwXHUzMEZDXHUzMDRDXHU5NzVFXHU4ODY4XHU3OTNBXHUzMDZBXHUzMDg5XHUzMEJCXHUzMEQxXHUzMEVDXHUzMEZDXHUzMEJGXHUzMEZDXHUzMDgyXHU5NzVFXHU4ODY4XHU3OTNBXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsbS5zdHlsZS5kaXNwbGF5ID0gXCJub25lXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAvL1x1MzBFQVx1MzBCOVx1MzBDOFx1NTA3NFx1MzA4Mlx1OTc1RVx1ODg2OFx1NzkzQVx1MzA2Qlx1MzA1OVx1MzA4QlxyXG4gICAgICAgICAgICAgICAgICAgIHJhd1Jlc3VsdC5EYXRhLmZvckVhY2gocmF3ID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHJhdyBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByYXcuc3R5bGUuZGlzcGxheSA9IFwibm9uZVwiO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgcmVzdG9yZVdpZHRoID0gYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jPG51bWJlcj4oJ0dldFdpZHRoJywgdGhpcy5fY29sdW1uSURzW2tleV0pO1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHNob3VsZFJlc3RvcmVXaWR0aDogYm9vbGVhbiA9IHJlc3RvcmVXaWR0aCA+IDA7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHdpZHRoOiBudW1iZXIgPSBzaG91bGRSZXN0b3JlV2lkdGggPyByZXN0b3JlV2lkdGggOiBOdW1iZXIoc3R5bGUud2lkdGgubWF0Y2goL1xcZCsvKSk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGxlZnQgKz0gd2lkdGg7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChlbG0gIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxtLnN0eWxlLmxlZnQgPSBgJHtsZWZ0fXB4YDtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGlmIChzaG91bGRSZXN0b3JlV2lkdGgpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgY29uc3QgaGVhZGVyUmVzdWx0OiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPEVsZW1lbnQ+ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoYCMke3RoaXMuX2NvbHVtbklEc1trZXldfWApO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoaGVhZGVyUmVzdWx0LklzU3VjY2VlZGVkICYmIGhlYWRlclJlc3VsdC5EYXRhICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoaGVhZGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlclJlc3VsdC5EYXRhLnN0eWxlLndpZHRoID0gYCR7d2lkdGh9cHhgO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAvL1x1MzBFQVx1MzBCOVx1MzBDOFx1NTA3NFx1MzA2RVx1NUU0NVx1MzA5Mlx1MzBEOFx1MzBDM1x1MzBDMFx1MzBGQ1x1MzA2Qlx1NTQwOFx1MzA4Rlx1MzA1Qlx1MzA4QlxyXG4gICAgICAgICAgICAgICAgICAgIHJhd1Jlc3VsdC5EYXRhLmZvckVhY2gocmF3ID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHJhdyBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByYXcuc3R5bGUud2lkdGggPSBzaG91bGRSZXN0b3JlV2lkdGggPyBgJHt3aWR0aH1weGAgOiBzdHlsZS53aWR0aDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuXHJcblxyXG4gICAgLy8jcmVnaW9uIHByaXZhdGVcclxuXHJcbiAgICBwcml2YXRlIE9uTW91c2VEb3duKGluZGV4OiBzdHJpbmcpOiB2b2lkIHtcclxuICAgICAgICB0aGlzLl9pc1Jlc2l6aW5nID0gdHJ1ZTtcclxuICAgICAgICB0aGlzLl9yZXNpemluZ0luZGV4ID0gaW5kZXg7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vdXNlVXAoKTogdm9pZCB7XHJcbiAgICAgICAgdGhpcy5faXNSZXNpemluZyA9IGZhbHNlO1xyXG4gICAgICAgIHRoaXMuX3Jlc2l6aW5nSW5kZXggPSBudWxsO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgYXN5bmMgT25Nb3VzZU1vdmUoZTogTW91c2VFdmVudCk6IFByb21pc2U8dm9pZD4ge1xyXG4gICAgICAgIGlmICghdGhpcy5faXNSZXNpemluZyB8fCB0aGlzLl9yZXNpemluZ0luZGV4ID09PSBudWxsKSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IG5leHRJbmRleCA9IE51bWJlcih0aGlzLl9yZXNpemluZ0luZGV4KSArIDE7XHJcblxyXG4gICAgICAgIGNvbnN0IHJlc2l6aW5nTmFtZSA9IHRoaXMuX2NvbHVtbklEc1t0aGlzLl9yZXNpemluZ0luZGV4XTtcclxuICAgICAgICBjb25zdCBuZXh0TmFtZSA9IHRoaXMuX2NvbHVtbklEc1tgJHtuZXh0SW5kZXh9YF07XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KGAjJHtyZXNpemluZ05hbWV9YCk7XHJcbiAgICAgICAgY29uc3QgbmV4dEhlYWRlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KGAjJHtuZXh0TmFtZX1gKTtcclxuICAgICAgICBjb25zdCBoZWFkZXJXcmFwcGVyUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoRWxlbWVudElEcy5WaWRlb0xpc3RIZWFkZXIpO1xyXG4gICAgICAgIGNvbnN0IGNvbHVtblJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKGAuJHtyZXNpemluZ05hbWV9YCk7XHJcbiAgICAgICAgY29uc3QgbmV4dENvbHVtblJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKGAuJHtuZXh0TmFtZX1gKTtcclxuICAgICAgICBjb25zdCBzZXBSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldCh0aGlzLl9zZXBhcmF0b3JJRHNbdGhpcy5fcmVzaXppbmdJbmRleF0pO1xyXG5cclxuICAgICAgICAvL1x1ODk4MVx1N0QyMFx1NTNENlx1NUY5N1x1MzA2Qlx1NTkzMVx1NjU1N1x1MzA1N1x1MzA1Rlx1MzA4OXJldHVyblxyXG4gICAgICAgIGlmICghaGVhZGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IGhlYWRlclJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKCFjb2x1bW5SZXN1bHQuSXNTdWNjZWVkZWQgfHwgY29sdW1uUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIXNlcFJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBzZXBSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghaGVhZGVyV3JhcHBlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIW5leHRIZWFkZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgbmV4dEhlYWRlclJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVyblxyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIW5leHRDb2x1bW5SZXN1bHQuSXNTdWNjZWVkZWQgfHwgbmV4dENvbHVtblJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICghKGhlYWRlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgaWYgKCEobmV4dEhlYWRlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRlclJlY3Q6IERPTVJlY3QgPSBoZWFkZXJSZXN1bHQuRGF0YS5nZXRCb3VuZGluZ0NsaWVudFJlY3QoKTtcclxuICAgICAgICBjb25zdCBoZWFkZXJXcmFwcGVyUmVjdDogRE9NUmVjdCA9IGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YS5nZXRCb3VuZGluZ0NsaWVudFJlY3QoKTtcclxuXHJcblxyXG4gICAgICAgIGNvbnN0IHdpZHRoID0gZS5jbGllbnRYIC0gaGVhZGVyUmVjdC5sZWZ0O1xyXG4gICAgICAgIGNvbnN0IGRlbHRhV2lkdGggPSB3aWR0aCAtIGhlYWRlclJlc3VsdC5EYXRhLm9mZnNldFdpZHRoO1xyXG4gICAgICAgIGNvbnN0IG5leHRXaWR0aCA9IG5leHRIZWFkZXJSZXN1bHQuRGF0YS5vZmZzZXRXaWR0aCAtIGRlbHRhV2lkdGg7XHJcblxyXG4gICAgICAgIGhlYWRlclJlc3VsdC5EYXRhLnN0eWxlLndpZHRoID0gYCR7d2lkdGh9cHhgO1xyXG4gICAgICAgIG5leHRIZWFkZXJSZXN1bHQuRGF0YS5zdHlsZS53aWR0aCA9IGAke25leHRXaWR0aH1weGA7XHJcblxyXG4gICAgICAgIGNvbHVtblJlc3VsdC5EYXRhLmZvckVhY2goZWxtID0+IHtcclxuICAgICAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgICAgICBlbG0uc3R5bGUud2lkdGggPSBgJHt3aWR0aH1weGA7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIG5leHRDb2x1bW5SZXN1bHQuRGF0YS5mb3JFYWNoKGVsbSA9PiB7XHJcbiAgICAgICAgICAgIGlmICghKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICAgICAgZWxtLnN0eWxlLndpZHRoID0gYCR7bmV4dFdpZHRofXB4YDtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jKCdTZXRXaWR0aCcsIGAke3dpZHRofWAsIHJlc2l6aW5nTmFtZSk7XHJcbiAgICAgICAgYXdhaXQgdGhpcy5fZG90bmV0SGVscGVyLmludm9rZU1ldGhvZEFzeW5jKCdTZXRXaWR0aCcsIGAke25leHRXaWR0aH1gLCBuZXh0TmFtZSk7XHJcblxyXG4gICAgICAgIGlmICghKGhlYWRlcldyYXBwZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIGlmICghKHNlcFJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IGxlZnQgPSBoZWFkZXJSZWN0LmxlZnQgLSBoZWFkZXJXcmFwcGVyUmVjdC5sZWZ0ICsgd2lkdGggLSAxMDtcclxuICAgICAgICBzZXBSZXN1bHQuRGF0YS5zdHlsZS5sZWZ0ID0gYCR7bGVmdH1weGA7XHJcbiAgICB9XHJcblxyXG4gICAgLy8jZW5kcmVnaW9uXHJcbn1cclxuXHJcbmludGVyZmFjZSBDbGFzc05hbWVzRGljdCB7XHJcbiAgICBbaW5kZXg6IG51bWJlcl06IHN0cmluZztcclxufSIsICJpbXBvcnQgeyBEb3ROZXRPYmplY3RSZWZlcmVuY2UgfSBmcm9tIFwiLi4vc2hhcmVkL0RvdE5ldE9iamVjdFJlZmVyZW5jZVwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciwgRWxlbWVudEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4uL3NoYXJlZC9FbGVtZW50SGFuZGxlclwiO1xyXG5pbXBvcnQgeyBTdHlsZUhhbmRsZXIsIFN0eWxlSGFuZGxlckltcGwgfSBmcm9tIFwiLi4vc2hhcmVkL1N0eWxlSGFuZGxlclwiO1xyXG5pbXBvcnQgeyBTZWxlY3Rpb25IYW5kbGVySW1wbCB9IGZyb20gXCIuL1NlbGVjdGlvbkhhbmRsZXIvc2VsZWN0aW9uSGFuZGxlclwiO1xyXG5pbXBvcnQgeyBEcm9wSGFuZGxlciwgRHJvcEhhbmRsZXJJbXBsIH0gZnJvbSBcIi4vZHJvcEhhbmRsZXIvZHJvcGhhbmRsZXJcIjtcclxuaW1wb3J0IHsgU29ydEhhbmRsZXIsIFNvcnRIYW5kbGVySW1wbCB9IGZyb20gXCIuL3NvcnRIYW5kbGVyL3NvcnRIYW5kbGVyXCI7XHJcbmltcG9ydCB7IFdpZHRoSGFuZGxlciwgV2lkdGhIYW5kbGVySW1wbCB9IGZyb20gXCIuL3dpZHRoSGFuZGxlci93aWR0aEhhbmRsZXJcIjtcclxuXHJcbmV4cG9ydCBhc3luYyBmdW5jdGlvbiBpbml0aWFsaXplKGJsYXpvclZpZXc6IERvdE5ldE9iamVjdFJlZmVyZW5jZSwgaXNGaXJzdFJlbmRlcjogYm9vbGVhbikge1xyXG4gICAgY29uc3QgZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXIgPSBuZXcgRWxlbWVudEhhbmRsZXJJbXBsKCk7XHJcbiAgICBjb25zdCBzdHlsZUhhbmRsZXI6IFN0eWxlSGFuZGxlciA9IG5ldyBTdHlsZUhhbmRsZXJJbXBsKGVsbUhhbmRsZXIpO1xyXG4gICAgY29uc3Qgd2lkdGhIYW5kbGVyOiBXaWR0aEhhbmRsZXIgPSBuZXcgV2lkdGhIYW5kbGVySW1wbChlbG1IYW5kbGVyLCBzdHlsZUhhbmRsZXIsIGJsYXpvclZpZXcpO1xyXG4gICAgY29uc3Qgc29ydEhhbmRsZXI6IFNvcnRIYW5kbGVyID0gbmV3IFNvcnRIYW5kbGVySW1wbChlbG1IYW5kbGVyLCBibGF6b3JWaWV3KVxyXG4gICAgY29uc3QgZHJvcEhhbmRsZXI6IERyb3BIYW5kbGVyID0gbmV3IERyb3BIYW5kbGVySW1wbChibGF6b3JWaWV3KTtcclxuXHJcbiAgICBpZiAoaXNGaXJzdFJlbmRlcikge1xyXG4gICAgICAgIGF3YWl0IHdpZHRoSGFuZGxlci5pbml0aWFsaXplKCk7XHJcbiAgICAgICAgZHJvcEhhbmRsZXIuSW5pdGlhbGl6ZSgpO1xyXG4gICAgfVxyXG5cclxuICAgIHNvcnRIYW5kbGVyLmluaXRpYWxpemUocmVnaXN0ZXJlZExpc3QpO1xyXG59XHJcblxyXG5leHBvcnQgYXN5bmMgZnVuY3Rpb24gc2V0V2lkdGgoYmxhem9yVmlldzogRG90TmV0T2JqZWN0UmVmZXJlbmNlKSB7XHJcbiAgICBjb25zdCBlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlciA9IG5ldyBFbGVtZW50SGFuZGxlckltcGwoKTtcclxuICAgIGNvbnN0IHN0eWxlSGFuZGxlcjogU3R5bGVIYW5kbGVyID0gbmV3IFN0eWxlSGFuZGxlckltcGwoZWxtSGFuZGxlcik7XHJcbiAgICBjb25zdCB3aWR0aEhhbmRsZXI6IFdpZHRoSGFuZGxlciA9IG5ldyBXaWR0aEhhbmRsZXJJbXBsKGVsbUhhbmRsZXIsIHN0eWxlSGFuZGxlciwgYmxhem9yVmlldyk7XHJcblxyXG4gICAgYXdhaXQgd2lkdGhIYW5kbGVyLnNldFdpZHRoKCk7XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRTZWxlY3RlZElPZklucHV0KCk6IHN0cmluZyB7XHJcbiAgICBjb25zdCBlbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlciA9IG5ldyBFbGVtZW50SGFuZGxlckltcGwoKTtcclxuICAgIGNvbnN0IGhhbmRsZXIgPSBuZXcgU2VsZWN0aW9uSGFuZGxlckltcGwoZWxtSGFuZGxlcik7XHJcblxyXG4gICAgcmV0dXJuIGhhbmRsZXIuZ2V0U2VsZWN0ZWQoKTtcclxufVxyXG5cclxubGV0IHJlZ2lzdGVyZWRMaXN0OiBzdHJpbmdbXSA9IFtdO1xyXG4iXSwKICAibWFwcGluZ3MiOiAiO0FBc0JPLElBQU0sNkJBQU4sTUFBTSw0QkFBbUU7QUFBQSxFQUU1RSxZQUFZLGFBQXNCLE1BQWdCLFNBQXdCO0FBQ3RFLFNBQUssY0FBYztBQUNuQixTQUFLLE9BQU87QUFDWixTQUFLLFVBQVU7QUFBQSxFQUNuQjtBQUFBLEVBRVM7QUFBQSxFQUVBO0FBQUEsRUFFQTtBQUFBLEVBRVQsT0FBYyxVQUFhLE1BQTJDO0FBQ2xFLFdBQU8sSUFBSSw0QkFBMkIsTUFBTSxNQUFNLElBQUk7QUFBQSxFQUMxRDtBQUFBLEVBRUEsT0FBYyxLQUFRLFNBQTRDO0FBQzlELFdBQU8sSUFBSSw0QkFBMkIsT0FBTyxNQUFNLE9BQU87QUFBQSxFQUM5RDtBQUNKOzs7QUN0Qk8sSUFBTSxxQkFBTixNQUFtRDtBQUFBLEVBQ2pELElBQUksT0FBZ0Q7QUFDekQsUUFBSTtBQUVKLFFBQUk7QUFDRixlQUFTLFNBQVMsY0FBYyxLQUFLO0FBQUEsSUFDdkMsU0FBUyxHQUFRO0FBQ2YsYUFBTywyQkFBMkI7QUFBQSxRQUNoQywwR0FBcUIsRUFBRSxPQUFPO0FBQUEsTUFDaEM7QUFBQSxJQUNGO0FBRUEsV0FBTyxVQUFVLE9BQ2IsMkJBQTJCLEtBQUssa0dBQWtCLElB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
