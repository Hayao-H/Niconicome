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

// src/videoList/dropHandler/drophandler.ts
var DropHandlerImpl = class {
  constructor(dotnet) {
    this._dotnet = dotnet;
  }
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
      console.log("Something dropped!!");
      console.log(distinct);
      this._dotnet.invokeMethodAsync("OnDrop", result);
    });
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
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0F0dGVtcHRSZXN1bHQudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvc2hhcmVkL0VsZW1lbnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3NoYXJlZC9TdHlsZUhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L1NlbGVjdGlvbkhhbmRsZXIvc2VsZWN0aW9uSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvZHJvcEhhbmRsZXIvZHJvcGhhbmRsZXIudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL2VsZW1lbnRJRHMudHMiLCAiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvdmlkZW9MaXN0L3NvcnRIYW5kbGVyL3NvcnRIYW5kbGVyLnRzIiwgIi4uLy4uLy4uLy4uLy4uLy4uL05pY29uaWNvbWVXZWIvc3JjL3ZpZGVvTGlzdC93aWR0aEhhbmRsZXIvRWxlbWVudElkcy50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3Qvd2lkdGhIYW5kbGVyL3dpZHRoSGFuZGxlci50cyIsICIuLi8uLi8uLi8uLi8uLi8uLi9OaWNvbmljb21lV2ViL3NyYy92aWRlb0xpc3QvbWFpbi50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiZXhwb3J0IGludGVyZmFjZSBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IGV4dGVuZHMgQXR0ZW1wdFJlc3VsdCB7XHJcbiAgICBcclxuICAgIC8qKlxyXG4gICAgICogXHUzMEM3XHUzMEZDXHUzMEJGXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG59XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEF0dGVtcHRSZXN1bHQge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU2MjEwXHU1MjlGXHUzMEQ1XHUzMEU5XHUzMEIwXHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHUzMEUxXHUzMEMzXHUzMEJCXHUzMEZDXHUzMEI4XHJcbiAgICAgKi9cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcbn1cclxuXHJcblxyXG5leHBvcnQgY2xhc3MgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGw8VD4gaW1wbGVtZW50cyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihpc1N1Y2NlZWRlZDogYm9vbGVhbiwgZGF0YTogVCB8IG51bGwsIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5EYXRhID0gZGF0YTtcclxuICAgICAgICB0aGlzLk1lc3NhZ2UgPSBtZXNzYWdlO1xyXG4gICAgfVxyXG5cclxuICAgIHJlYWRvbmx5IElzU3VjY2VlZGVkOiBib29sZWFuO1xyXG5cclxuICAgIHJlYWRvbmx5IERhdGE6IFQgfCBudWxsO1xyXG5cclxuICAgIHJlYWRvbmx5IE1lc3NhZ2U6IHN0cmluZyB8IG51bGw7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBTdWNjZWVkZWQ8VD4oZGF0YTogVCB8IG51bGwpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKHRydWUsIGRhdGEsIG51bGwpO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgRmFpbDxUPihtZXNzYWdlOiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPFQ+IHtcclxuICAgICAgICByZXR1cm4gbmV3IEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsKGZhbHNlLCBudWxsLCBtZXNzYWdlKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGNsYXNzIEF0dGVtcHRSZXN1bHRJbXBsIGltcGxlbWVudHMgQXR0ZW1wdFJlc3VsdCB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoaXNTdWNjZWVkZWQ6IGJvb2xlYW4sIG1lc3NhZ2U6IHN0cmluZyB8IG51bGwpIHtcclxuICAgICAgICB0aGlzLklzU3VjY2VlZGVkID0gaXNTdWNjZWVkZWQ7XHJcbiAgICAgICAgdGhpcy5NZXNzYWdlID0gbWVzc2FnZTtcclxuICAgIH1cclxuXHJcbiAgICByZWFkb25seSBJc1N1Y2NlZWRlZDogYm9vbGVhbjtcclxuXHJcbiAgICByZWFkb25seSBNZXNzYWdlOiBzdHJpbmcgfCBudWxsO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgU3VjY2VlZGVkKCk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwodHJ1ZSwgbnVsbCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyBGYWlsKG1lc3NhZ2U6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHQge1xyXG4gICAgICAgIHJldHVybiBuZXcgQXR0ZW1wdFJlc3VsdEltcGwoZmFsc2UsIG1lc3NhZ2UpO1xyXG4gICAgfVxyXG59XHJcblxyXG4iLCAiaW1wb3J0IHsgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YSwgQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwgfSBmcm9tIFwiLi9BdHRlbXB0UmVzdWx0XCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIEVsZW1lbnRIYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXQocXVlcnk6c3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PjtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1ODkwN1x1NjU3MFx1MzA2RVx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICogQHBhcmFtIHF1ZXJ5IFx1MzBBRlx1MzBBOFx1MzBFQVxyXG4gICAgICovXHJcbiAgICBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj47XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBFbGVtZW50SGFuZGxlckltcGwgaW1wbGVtZW50cyBFbGVtZW50SGFuZGxlciB7XHJcblxyXG4gICAgcHVibGljIEdldChxdWVyeTogc3RyaW5nKTogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiB7XHJcblxyXG4gICAgICAgIGxldCByZXN1bHQ6IEVsZW1lbnQgfCBudWxsO1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gcmVzdWx0ID09IG51bGwgPyBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhSW1wbC5GYWlsKFwiXHU2MzA3XHU1QjlBXHUzMDU1XHUzMDhDXHUzMDVGXHU4OTgxXHU3RDIwXHUzMDRDXHU4OThCXHUzMDY0XHUzMDRCXHUzMDhBXHUzMDdFXHUzMDVCXHUzMDkzXHUzMDAyXCIpIDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIHB1YmxpYyBHZXRBbGwocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Tm9kZUxpc3RPZjxFbGVtZW50Pj4ge1xyXG5cclxuICAgICAgICBsZXQgcmVzdWx0OiBOb2RlTGlzdE9mPEVsZW1lbnQ+O1xyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICByZXN1bHQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yQWxsKHF1ZXJ5KTtcclxuICAgICAgICB9IGNhdGNoIChlOiBhbnkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoYFx1ODk4MVx1N0QyMFx1MzA5Mlx1NTNENlx1NUY5N1x1MzA2N1x1MzA0RFx1MzA3RVx1MzA1Qlx1MzA5M1x1MzA2N1x1MzA1N1x1MzA1Rlx1MzAwMihcdThBNzNcdTdEMzBcdUZGMUEke2UubWVzc2FnZX0pYCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXR1cm4gQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YUltcGwuU3VjY2VlZGVkKHJlc3VsdCk7XHJcbiAgICB9XHJcblxyXG59IiwgImltcG9ydCB7IEF0dGVtcHRSZXN1bHRXaWR0aERhdGEsIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsIH0gZnJvbSBcIi4vQXR0ZW1wdFJlc3VsdFwiO1xyXG5pbXBvcnQgeyBFbGVtZW50SGFuZGxlciB9IGZyb20gXCIuL0VsZW1lbnRIYW5kbGVyXCI7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIFN0eWxlSGFuZGxlciB7XHJcbiAgICAvKipcclxuICAgICAqIFx1ODk4MVx1N0QyMFx1MzA2RVx1MzBCOVx1MzBCRlx1MzBBNFx1MzBFQlx1MzA5Mlx1NTNENlx1NUY5N1xyXG4gICAgICovXHJcbiAgICBHZXRDb21wdXRlZFN0eWxlKHF1ZXJ5OiBzdHJpbmcpOiBBdHRlbXB0UmVzdWx0V2lkdGhEYXRhPENTU1N0eWxlRGVjbGFyYXRpb24+O1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU3R5bGVIYW5kbGVySW1wbCBpbXBsZW1lbnRzIFN0eWxlSGFuZGxlciB7XHJcbiAgICBjb25zdHJ1Y3RvcihlbGVtZW50SGFuZGxlcjogRWxlbWVudEhhbmRsZXIpIHtcclxuICAgICAgICB0aGlzLl9lbG1IYW5kbGVyID0gZWxlbWVudEhhbmRsZXI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHVibGljIEdldENvbXB1dGVkU3R5bGUocXVlcnk6IHN0cmluZyk6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8Q1NTU3R5bGVEZWNsYXJhdGlvbj4ge1xyXG5cclxuICAgICAgICBjb25zdCByZXN1bHQ6IEF0dGVtcHRSZXN1bHRXaWR0aERhdGE8RWxlbWVudD4gPSB0aGlzLl9lbG1IYW5kbGVyLkdldChxdWVyeSk7XHJcbiAgICAgICAgaWYgKCFyZXN1bHQuSXNTdWNjZWVkZWQgfHwgcmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwocmVzdWx0Lk1lc3NhZ2UgPz8gXCJcIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICBjb25zdCBzdHlsZSA9IHdpbmRvdy5nZXRDb21wdXRlZFN0eWxlKHJlc3VsdC5EYXRhKTtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLlN1Y2NlZWRlZChzdHlsZSk7XHJcbiAgICAgICAgfSBjYXRjaCAoZXgpIHtcclxuICAgICAgICAgICAgcmV0dXJuIEF0dGVtcHRSZXN1bHRXaWR0aERhdGFJbXBsLkZhaWwoXCJcdTMwQjlcdTMwQkZcdTMwQTRcdTMwRUJcdTMwOTJcdTUzRDZcdTVGOTdcdTMwNjdcdTMwNERcdTMwN0VcdTMwNUJcdTMwOTNcdTMwNjdcdTMwNTdcdTMwNUZcdTMwMDJcIik7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59IiwgImltcG9ydCB7IEVsZW1lbnRIYW5kbGVyIH0gZnJvbSBcIi4uLy4uL3NoYXJlZC9FbGVtZW50SGFuZGxlclwiO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBTZWxlY3Rpb25IYW5kbGVyIHtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1NTE2NVx1NTI5Qlx1NkIwNFx1MzA2N1x1OTA3OFx1NjI5RVx1MzA1NVx1MzA4Q1x1MzA2Nlx1MzA0NFx1MzA4Qlx1NjU4N1x1NUI1N1x1NTIxN1x1MzA5Mlx1NTNENlx1NUY5N1x1MzA1OVx1MzA4QlxyXG4gICAgICovXHJcbiAgICBnZXRTZWxlY3RlZCgpOiBzdHJpbmc7XHJcbn1cclxuXHJcbmV4cG9ydCBjbGFzcyBTZWxlY3Rpb25IYW5kbGVySW1wbCBpbXBsZW1lbnRzIFNlbGVjdGlvbkhhbmRsZXIge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyKSB7XHJcbiAgICAgICAgdGhpcy5fZWxtSGFuZGxlciA9IGVsbUhhbmRsZXI7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHVibGljIGdldFNlbGVjdGVkKCk6IHN0cmluZyB7XHJcblxyXG4gICAgICAgIGNvbnN0IGVsbVJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KFwiI0lucHV0Qm94XCIpO1xyXG4gICAgICAgIGlmICghZWxtUmVzdWx0LklzU3VjY2VlZGVkIHx8IGVsbVJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybiBcIlwiO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgZWxtOiBFbGVtZW50ID0gZWxtUmVzdWx0LkRhdGE7XHJcbiAgICAgICAgaWYgKCEoZWxtIGluc3RhbmNlb2YgSFRNTElucHV0RWxlbWVudCkpIHtcclxuICAgICAgICAgICAgcmV0dXJuIFwiXCI7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoIXRoaXMuSXNWYWxpZEluZGV4KGVsbS52YWx1ZSwgZWxtLnNlbGVjdGlvblN0YXJ0KSkge1xyXG4gICAgICAgICAgICByZXR1cm4gXCJcIjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICghdGhpcy5Jc1ZhbGlkSW5kZXgoZWxtLnZhbHVlLCBlbG0uc2VsZWN0aW9uRW5kLCB0cnVlKSkge1xyXG4gICAgICAgICAgICByZXR1cm4gXCJcIjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBlbG0udmFsdWUuc3Vic3RyaW5nKGVsbS5zZWxlY3Rpb25TdGFydCEsIGVsbS5zZWxlY3Rpb25FbmQhKVxyXG5cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIElzVmFsaWRJbmRleCh2YWx1ZTogc3RyaW5nLCBpbmRleDogbnVtYmVyIHwgbnVsbCwgaXNFbmQ6IGJvb2xlYW4gPSBmYWxzZSk6IGJvb2xlYW4ge1xyXG4gICAgICAgIGlmIChpbmRleCA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoaXNFbmQgJiYgaW5kZXggPiB2YWx1ZS5sZW5ndGgpIHtcclxuICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKCFpc0VuZCAmJiBpbmRleCA+IHZhbHVlLmxlbmd0aCAtIDEpIHtcclxuICAgICAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcmV0dXJuIHRydWU7XHJcbiAgICB9XHJcbn0iLCAiaW1wb3J0IHsgRG90TmV0T2JqZWN0UmVmZXJlbmNlIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0RvdE5ldE9iamVjdFJlZmVyZW5jZSc7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyJztcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgRHJvcEhhbmRsZXIge1xyXG4gICAgSW5pdGlhbGl6ZSgpOiB2b2lkO1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgRHJvcEhhbmRsZXJJbXBsIGltcGxlbWVudHMgRHJvcEhhbmRsZXIge1xyXG4gICAgY29uc3RydWN0b3IoZG90bmV0OiBEb3ROZXRPYmplY3RSZWZlcmVuY2UpIHtcclxuICAgICAgICB0aGlzLl9kb3RuZXQgPSBkb3RuZXQ7XHJcbiAgICB9XHJcblxyXG4gICAgLy8jcmVnaW9uICBmaWVsZFxyXG5cclxuICAgIHByaXZhdGUgcmVhZG9ubHkgX2RvdG5ldDogRG90TmV0T2JqZWN0UmVmZXJlbmNlO1xyXG5cclxuICAgIC8vI2VuZHJlZ2lvblxyXG4gICAgcHVibGljIEluaXRpYWxpemUoKTogdm9pZCB7XHJcblxyXG4gICAgICAgIHdpbmRvdy5hZGRFdmVudExpc3RlbmVyKCdkcmFnb3ZlcicsIGUgPT4gZS5wcmV2ZW50RGVmYXVsdCgpKTtcclxuICAgICAgICB3aW5kb3cuYWRkRXZlbnRMaXN0ZW5lcignZHJvcCcsIGUgPT4ge1xyXG4gICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcblxyXG4gICAgICAgICAgICBpZiAoZS5kYXRhVHJhbnNmZXIgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgY29uc3QgdGFyZ2V0TGlzdDogc3RyaW5nW10gPSBbXTtcclxuXHJcbiAgICAgICAgICAgIGUuZGF0YVRyYW5zZmVyLnR5cGVzLmZvckVhY2godCA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAodCA9PT0gJ3RleHQvcGxhaW4nKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3QgZGF0YSA9IGUuZGF0YVRyYW5zZmVyIS5nZXREYXRhKCd0ZXh0L3BsYWluJyk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGRhdGEgPT09ICcnKSByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgdGFyZ2V0TGlzdC5wdXNoKGRhdGEpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGlmIChlLmRhdGFUcmFuc2Zlci50eXBlcy5pbmNsdWRlcygnRmlsZXMnKSkge1xyXG4gICAgICAgICAgICAgICAgZm9yIChsZXQgaSA9IDA7IGkgPCBlLmRhdGFUcmFuc2Zlci5maWxlcy5sZW5ndGg7IGkrKykge1xyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IGZpbGUgPSBlLmRhdGFUcmFuc2Zlci5maWxlcy5pdGVtKGkpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChmaWxlID09PSBudWxsKSBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoZmlsZS5uYW1lID09PSAnJykgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGZpbGUubmFtZS5lbmRzV2l0aCgnLnVybCcpKSBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgICAgICB0YXJnZXRMaXN0LnB1c2goZmlsZS5uYW1lKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgY29uc3QgY29uZXZydGVkID0gdGFyZ2V0TGlzdC5tYXAodCA9PiB0Lm1hdGNoKC8oc218c298bm0pP1swLTldKy8pPy5bMF0gPz8gJycpLmZpbHRlcih0ID0+IHQgIT09ICcnKTtcclxuICAgICAgICAgICAgY29uc3QgZGlzdGluY3QgPSBbLi4uKG5ldyBTZXQoY29uZXZydGVkKSldO1xyXG5cclxuICAgICAgICAgICAgaWYgKGRpc3RpbmN0Lmxlbmd0aCA9PT0gMCkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIFxyXG4gICAgICAgICAgICBjb25zdCByZXN1bHQgPSBkaXN0aW5jdC5qb2luKCcgJylcclxuXHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKCdTb21ldGhpbmcgZHJvcHBlZCEhJyk7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKGRpc3RpbmN0KTtcclxuICAgICAgICAgICAgdGhpcy5fZG90bmV0Lmludm9rZU1ldGhvZEFzeW5jKCdPbkRyb3AnLCByZXN1bHQpO1xyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG59IiwgImV4cG9ydCBjbGFzcyBFbGVtZW50SURzIHtcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdFJvdyA9ICcuVmlkZW9MaXN0Um93JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFZpZGVvTGlzdFJvd0NsYXNzTmFtZSA9ICdWaWRlb0xpc3RSb3cnO1xyXG5cclxuICAgIHB1YmxpYyBzdGF0aWMgcmVhZG9ubHkgVmlkZW9MaXN0Qm9keUNsYXNzTmFtZSA9ICdWaWRlb0xpc3RCb2R5JztcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IERyb3BUYXJnZXRDbGFzc05hbWUgPSAnRHJvcFRhcmdldCc7XHJcbn0iLCAiaW1wb3J0IHsgRG90TmV0T2JqZWN0UmVmZXJlbmNlIH0gZnJvbSBcIi4uLy4uL3NoYXJlZC9Eb3ROZXRPYmplY3RSZWZlcmVuY2VcIjtcclxuaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIsIEVsZW1lbnRIYW5kbGVySW1wbCB9IGZyb20gXCIuLi8uLi9zaGFyZWQvRWxlbWVudEhhbmRsZXJcIjtcclxuaW1wb3J0IHsgRWxlbWVudElEcyB9IGZyb20gXCIuL2VsZW1lbnRJRHNcIjtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgU29ydEhhbmRsZXIge1xyXG4gICAgaW5pdGlhbGl6ZShyZWdpc3RlcmVkTGlzdDogc3RyaW5nW10pOiB2b2lkO1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgU29ydEhhbmRsZXJJbXBsIGltcGxlbWVudHMgU29ydEhhbmRsZXIge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyLCBkb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZSkge1xyXG4gICAgICAgIHRoaXMuX2VsbUhhbmRsZXIgPSBlbG1IYW5kbGVyO1xyXG4gICAgICAgIHRoaXMuX2RvdG5ldEhlbHBlciA9IGRvdG5ldEhlbHBlcjtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9lbG1IYW5kbGVyOiBFbGVtZW50SGFuZGxlcjtcclxuXHJcbiAgICBwcml2YXRlIHJlYWRvbmx5IF9kb3RuZXRIZWxwZXI6IERvdE5ldE9iamVjdFJlZmVyZW5jZTtcclxuXHJcbiAgICBwcml2YXRlIF9zb3VyY2VOaWNvbmljb0lEOiBzdHJpbmcgfCBudWxsID0gbnVsbDtcclxuXHJcbiAgICBwcml2YXRlIF9zb3VyY2VJRDogc3RyaW5nIHwgbnVsbCA9IG51bGw7XHJcblxyXG4gICAgcHJpdmF0ZSBfbGFzdE92ZXJFbGVtZW50OiBIVE1MRWxlbWVudCB8IG51bGwgPSBudWxsO1xyXG5cclxuICAgIHB1YmxpYyBpbml0aWFsaXplKHJlZ2lzdGVyZWRMaXN0OiBzdHJpbmdbXSk6IHZvaWQge1xyXG4gICAgICAgIGNvbnN0IHJvd1Jlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKEVsZW1lbnRJRHMuVmlkZW9MaXN0Um93KTtcclxuICAgICAgICBpZiAoIXJvd1Jlc3VsdC5Jc1N1Y2NlZWRlZCB8fCByb3dSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByb3dSZXN1bHQuRGF0YS5mb3JFYWNoKGVsbSA9PiB7XHJcblxyXG4gICAgICAgICAgICBpZiAoZWxtIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpIHtcclxuXHJcbiAgICAgICAgICAgICAgICBjb25zdCBuaWNvbmljb0lEID0gZWxtLmRhdGFzZXRbJ25pY29uaWNvaWQnXTtcclxuICAgICAgICAgICAgICAgIGNvbnN0IHBsYXlsaXN0SUQgPSBlbG0uZGF0YXNldFsncGxheWxpc3RpZCddO1xyXG5cclxuICAgICAgICAgICAgICAgIGlmIChuaWNvbmljb0lEID09PSB1bmRlZmluZWQgfHwgcGxheWxpc3RJRCA9PT0gdW5kZWZpbmVkKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGNvbnN0IGtleTogc3RyaW5nID0gYCR7bmljb25pY29JRH0tJHtwbGF5bGlzdElEfWA7XHJcbiAgICAgICAgICAgICAgICBpZiAocmVnaXN0ZXJlZExpc3QuaW5jbHVkZXMoa2V5KSkge1xyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmVnaXN0ZXJlZExpc3QucHVzaChrZXkpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIGVsbS5hZGRFdmVudExpc3RlbmVyKCdkcmFnc3RhcnQnLCBlID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIShlLnRhcmdldCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCByb3c6IEhUTUxFbGVtZW50IHwgbnVsbCA9IHRoaXMuR2V0UGFyZW50QnlDbGFzc05hbWUoZS50YXJnZXQsIEVsZW1lbnRJRHMuVmlkZW9MaXN0Um93Q2xhc3NOYW1lKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocm93ID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuX3NvdXJjZU5pY29uaWNvSUQgPSByb3cuZGF0YXNldFsnbmljb25pY29pZCddID8/IG51bGw7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5fc291cmNlSUQgPSByb3cuaWQ7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignZHJhZ292ZXInLCBlID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCEoZS50YXJnZXQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgY29uc3Qgcm93OiBIVE1MRWxlbWVudCB8IG51bGwgPSB0aGlzLkdldFBhcmVudEJ5Q2xhc3NOYW1lKGUudGFyZ2V0LCBFbGVtZW50SURzLlZpZGVvTGlzdFJvd0NsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJvdyA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoIXJvdy5jbGFzc0xpc3QuY29udGFpbnMoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByb3cuY2xhc3NMaXN0LmFkZChFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB0aGlzLl9sYXN0T3ZlckVsZW1lbnQgPSByb3c7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICBlbG0uYWRkRXZlbnRMaXN0ZW5lcignZHJhZ2xlYXZlJywgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmICghKGUudGFyZ2V0IGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHJvdzogSFRNTEVsZW1lbnQgfCBudWxsID0gdGhpcy5HZXRQYXJlbnRCeUNsYXNzTmFtZShlLnRhcmdldCwgRWxlbWVudElEcy5WaWRlb0xpc3RSb3dDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChyb3cgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKHJvdy5jbGFzc0xpc3QuY29udGFpbnMoRWxlbWVudElEcy5Ecm9wVGFyZ2V0Q2xhc3NOYW1lKSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByb3cuY2xhc3NMaXN0LnJlbW92ZShFbGVtZW50SURzLkRyb3BUYXJnZXRDbGFzc05hbWUpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgICAgIGVsbS5hZGRFdmVudExpc3RlbmVyKCdkcm9wJywgYXN5bmMgZSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5fc291cmNlTmljb25pY29JRCA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBzb3VyY2VSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChgIyR7dGhpcy5fc291cmNlSUR9YCk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFzb3VyY2VSZXN1bHQuSXNTdWNjZWVkZWQgfHwgc291cmNlUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCEoZS50YXJnZXQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgbGV0IHBhcmVudDogUGFyZW50Tm9kZSB8IG51bGwgPSBlLnRhcmdldC5wYXJlbnROb2RlO1xyXG4gICAgICAgICAgICAgICAgICAgIGxldCBkcm9wVGFyZ2V0OiBIVE1MRWxlbWVudCA9IGUudGFyZ2V0O1xyXG5cclxuICAgICAgICAgICAgICAgICAgICB3aGlsZSAocGFyZW50ICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICghKHBhcmVudCBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoIXBhcmVudC5jbGFzc0xpc3QuY29udGFpbnMoRWxlbWVudElEcy5WaWRlb0xpc3RCb2R5Q2xhc3NOYW1lKSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZHJvcFRhcmdldCA9IHBhcmVudDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhcmVudCA9IHBhcmVudC5wYXJlbnROb2RlO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhcmVudC5pbnNlcnRCZWZvcmUoc291cmNlUmVzdWx0LkRhdGEsIGRyb3BUYXJnZXQpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhd2FpdCB0aGlzLl9kb3RuZXRIZWxwZXIuaW52b2tlTWV0aG9kQXN5bmMoXCJNb3ZlVmlkZW9cIiwgdGhpcy5fc291cmNlTmljb25pY29JRCwgZHJvcFRhcmdldC5kYXRhc2V0WyduaWNvbmljb2lkJ10hKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFyZW50ID0gbnVsbDtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5fbGFzdE92ZXJFbGVtZW50ICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLl9sYXN0T3ZlckVsZW1lbnQuY2xhc3NMaXN0LmNvbnRhaW5zKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRoaXMuX2xhc3RPdmVyRWxlbWVudC5jbGFzc0xpc3QucmVtb3ZlKEVsZW1lbnRJRHMuRHJvcFRhcmdldENsYXNzTmFtZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgR2V0UGFyZW50QnlDbGFzc05hbWUoY3VycmVudEVsZW1lbnQ6IEhUTUxFbGVtZW50LCBjbGFzc05hbWU6IHN0cmluZyk6IEhUTUxFbGVtZW50IHwgbnVsbCB7XHJcbiAgICAgICAgbGV0IHBhcmVudDogUGFyZW50Tm9kZSB8IG51bGwgPSBjdXJyZW50RWxlbWVudDtcclxuXHJcbiAgICAgICAgd2hpbGUgKHBhcmVudCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICBpZiAoIShwYXJlbnQgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybiBudWxsO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICBpZiAoIXBhcmVudC5jbGFzc0xpc3QuY29udGFpbnMoY2xhc3NOYW1lKSkge1xyXG4gICAgICAgICAgICAgICAgcGFyZW50ID0gcGFyZW50LnBhcmVudE5vZGU7XHJcbiAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgcmV0dXJuIHBhcmVudDtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiBudWxsO1xyXG4gICAgfVxyXG59IiwgImV4cG9ydCBjbGFzcyBFbGVtZW50SURzIHtcclxuXHJcbiAgICBwdWJsaWMgc3RhdGljIHJlYWRvbmx5IFBhZ2VDb250ZW50ID0gJy5QYWdlQ29udGVudCc7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBWaWRlb0xpc3RIZWFkZXIgPSAnI1ZpZGVvTGlzdEhlYWRlcic7XHJcblxyXG4gICAgcHVibGljIHN0YXRpYyByZWFkb25seSBTZXBhcmF0b3IgPSAnLlNlcGFyYXRvcic7XHJcblxyXG59IiwgImltcG9ydCB7IEF0dGVtcHRSZXN1bHQsIEF0dGVtcHRSZXN1bHRXaWR0aERhdGEgfSBmcm9tICcuLi8uLi9zaGFyZWQvQXR0ZW1wdFJlc3VsdCc7XHJcbmltcG9ydCB7IEVsZW1lbnRIYW5kbGVyIH0gZnJvbSAnLi4vLi4vc2hhcmVkL0VsZW1lbnRIYW5kbGVyJztcclxuaW1wb3J0IHsgRWxlbWVudElEcyB9IGZyb20gJy4vRWxlbWVudElkcyc7XHJcbmltcG9ydCB7IERpY3Rpb25hcnkgfSBmcm9tICcuLi8uLi9zaGFyZWQvQ29sbGVjdGlvbi9kaWN0aW9uYXJ5JztcclxuaW1wb3J0IHsgU3R5bGVIYW5kbGVyIH0gZnJvbSAnLi4vLi4vc2hhcmVkL1N0eWxlSGFuZGxlcic7XHJcbmltcG9ydCB7IERvdE5ldE9iamVjdFJlZmVyZW5jZSB9IGZyb20gJy4uLy4uL3NoYXJlZC9Eb3ROZXRPYmplY3RSZWZlcmVuY2UnO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBXaWR0aEhhbmRsZXIge1xyXG5cclxuICAgIC8qKlxyXG4gICAgICogXHU1MjFEXHU2NzFGXHU1MzE2XHJcbiAgICAgKi9cclxuICAgIGluaXRpYWxpemUoKTogUHJvbWlzZTx2b2lkPjtcclxuXHJcbiAgICAvKipcclxuICAgICAqIFx1NUU0NVx1MzA5Mlx1NTE4RFx1OEEyRFx1NUI5QVxyXG4gICAgICovXHJcbiAgICBzZXRXaWR0aCgpOiBQcm9taXNlPHZvaWQ+O1xyXG59XHJcblxyXG5leHBvcnQgY2xhc3MgV2lkdGhIYW5kbGVySW1wbCBpbXBsZW1lbnRzIFdpZHRoSGFuZGxlciB7XHJcblxyXG4gICAgY29uc3RydWN0b3IoZWxlbWVudEhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyLCBzdHlsZUhhbmRsZXI6IFN0eWxlSGFuZGxlciwgZG90bmV0SGVscGVyOiBEb3ROZXRPYmplY3RSZWZlcmVuY2UpIHtcclxuICAgICAgICB0aGlzLl9lbG1IYW5kbGVyID0gZWxlbWVudEhhbmRsZXI7XHJcbiAgICAgICAgdGhpcy5fc3R5bGVIYW5kbGVyID0gc3R5bGVIYW5kbGVyO1xyXG4gICAgICAgIHRoaXMuX2RvdG5ldEhlbHBlciA9IGRvdG5ldEhlbHBlcjtcclxuICAgICAgICB0aGlzLl9jb2x1bW5JRHMgPSB7XHJcbiAgICAgICAgICAgICcwJzogJ0NoZWNrQm94Q29sdW1uJyxcclxuICAgICAgICAgICAgJzEnOiAnVGh1bWJuYWlsQ29sdW1uJyxcclxuICAgICAgICAgICAgJzInOiAnVGl0bGVDb2x1bW4nLFxyXG4gICAgICAgICAgICAnMyc6ICdVcGxvYWRlZERhdGVUaW1lQ29sdW1uJyxcclxuICAgICAgICAgICAgJzQnOiAnSXNEb3dubG9hZGVkQ29sdW1uJyxcclxuICAgICAgICAgICAgJzUnOiAnVmlld0NvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzYnOiAnQ29tbWVudENvdW50Q29sdW1uJyxcclxuICAgICAgICAgICAgJzcnOiAnTXlsaXN0Q291bnRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnOCc6ICdMaWtlQ291bnRDb2x1bW4nLFxyXG4gICAgICAgICAgICAnOSc6ICdNZXNzYWdlQ29sdW1uJyxcclxuICAgICAgICB9O1xyXG4gICAgICAgIHRoaXMuX3NlcGFyYXRvcklEcyA9IHtcclxuICAgICAgICAgICAgJzAnOiAnI0NoZWNrQm94Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzEnOiAnI1RodW1ibmFpbENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICcyJzogJyNUaXRsZUNvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICczJzogJyNVcGxvYWRlZERhdGVUaW1lQ29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzQnOiAnI0lzRG93bmxvYWRlZENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgICAgICc1JzogJyNWaWV3Q291bnRDb2x1bW5TZXBhcmF0b3InLFxyXG4gICAgICAgICAgICAnNic6ICcjQ29tbWVudENvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzcnOiAnI015bGlzdENvdW50Q29sdW1uU2VwYXJhdG9yJyxcclxuICAgICAgICAgICAgJzgnOiAnI0xpa2VDb3VudENvbHVtblNlcGFyYXRvcicsXHJcbiAgICAgICAgfTtcclxuICAgIH1cclxuXHJcbiAgICAvLyNyZWdpb24gIGZpZWxkXHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXI7XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfc3R5bGVIYW5kbGVyOiBTdHlsZUhhbmRsZXI7XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfY29sdW1uSURzOiBEaWN0aW9uYXJ5PHN0cmluZz47XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfc2VwYXJhdG9ySURzOiBEaWN0aW9uYXJ5PHN0cmluZz47XHJcblxyXG4gICAgcHJpdmF0ZSByZWFkb25seSBfZG90bmV0SGVscGVyOiBEb3ROZXRPYmplY3RSZWZlcmVuY2U7XHJcblxyXG4gICAgcHJpdmF0ZSBfaXNSZXNpemluZyA9IGZhbHNlO1xyXG5cclxuICAgIHByaXZhdGUgX3Jlc2l6aW5nSW5kZXg6IG51bGwgfCBzdHJpbmc7XHJcblxyXG4gICAgLy8jZW5kcmVnaW9uXHJcblxyXG4gICAgcHVibGljIGFzeW5jIGluaXRpYWxpemUoKTogUHJvbWlzZTx2b2lkPiB7XHJcblxyXG4gICAgICAgIGZvciAoY29uc3Qga2V5IGluIHRoaXMuX3NlcGFyYXRvcklEcykge1xyXG5cclxuXHJcbiAgICAgICAgICAgIGNvbnN0IHNlcFJlc3VsdDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KHRoaXMuX3NlcGFyYXRvcklEc1trZXldKTtcclxuICAgICAgICAgICAgaWYgKCFzZXBSZXN1bHQuSXNTdWNjZWVkZWQgfHwgc2VwUmVzdWx0LkRhdGEgPT09IG51bGwpIGNvbnRpbnVlO1xyXG5cclxuICAgICAgICAgICAgY29uc3QgZWxtOiBFbGVtZW50ID0gc2VwUmVzdWx0LkRhdGE7XHJcbiAgICAgICAgICAgIGlmICghKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgY29udGludWU7XHJcblxyXG4gICAgICAgICAgICBjb25zdCBpbmRleFMgPSBlbG0uZGF0YXNldC5pbmRleDtcclxuXHJcbiAgICAgICAgICAgIGlmIChpbmRleFMgPT0gdW5kZWZpbmVkKSBjb250aW51ZTtcclxuXHJcbiAgICAgICAgICAgIGVsbS5hZGRFdmVudExpc3RlbmVyKCdtb3VzZWRvd24nLCBfID0+IHRoaXMuT25Nb3VzZURvd24oaW5kZXhTKSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBhd2FpdCB0aGlzLnNldFdpZHRoKCk7XHJcblxyXG4gICAgICAgIGNvbnN0IHBhZ2VSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChFbGVtZW50SURzLlBhZ2VDb250ZW50KTtcclxuICAgICAgICBpZiAoIXBhZ2VSZXN1bHQuSXNTdWNjZWVkZWQgfHwgcGFnZVJlc3VsdC5EYXRhID09PSBudWxsIHx8ICEocGFnZVJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpKSByZXR1cm47XHJcbiAgICAgICAgcGFnZVJlc3VsdC5EYXRhLmFkZEV2ZW50TGlzdGVuZXIoJ21vdXNldXAnLCBfID0+IHRoaXMuT25Nb3VzZVVwKCkpO1xyXG5cclxuICAgICAgICBjb25zdCBoZWFkZXJXcmFwcGVyUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQoRWxlbWVudElEcy5WaWRlb0xpc3RIZWFkZXIpO1xyXG4gICAgICAgIGlmICghaGVhZGVyV3JhcHBlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEgPT09IG51bGwgfHwgIShoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEuYWRkRXZlbnRMaXN0ZW5lcignbW91c2Vtb3ZlJywgZSA9PiB0aGlzLk9uTW91c2VNb3ZlKGUpKTtcclxuXHJcbiAgICB9XHJcbiAgICBwdWJsaWMgYXN5bmMgc2V0V2lkdGgoKTogUHJvbWlzZTx2b2lkPiB7XHJcbiAgICAgICAgbGV0IGxlZnQgPSAwO1xyXG4gICAgICAgIGZvciAoY29uc3Qga2V5IGluIHRoaXMuX2NvbHVtbklEcykge1xyXG5cclxuICAgICAgICAgICAgbGV0IGVsbTogRWxlbWVudCB8IG51bGwgPSBudWxsO1xyXG5cclxuICAgICAgICAgICAgaWYgKGtleSBpbiB0aGlzLl9zZXBhcmF0b3JJRHMpIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0IHNlcFJlc3VsdDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KHRoaXMuX3NlcGFyYXRvcklEc1trZXldKTtcclxuICAgICAgICAgICAgICAgIGlmICghc2VwUmVzdWx0LklzU3VjY2VlZGVkIHx8IHNlcFJlc3VsdC5EYXRhID09PSBudWxsKSBjb250aW51ZTtcclxuXHJcbiAgICAgICAgICAgICAgICBlbG0gPSBzZXBSZXN1bHQuRGF0YTtcclxuICAgICAgICAgICAgICAgIGlmICghKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgY29udGludWU7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIGNvbnN0IHN0eWxlUmVzdWx0ID0gdGhpcy5fc3R5bGVIYW5kbGVyLkdldENvbXB1dGVkU3R5bGUoYCMke3RoaXMuX2NvbHVtbklEc1trZXldfWApO1xyXG4gICAgICAgICAgICBpZiAoc3R5bGVSZXN1bHQuSXNTdWNjZWVkZWQgJiYgc3R5bGVSZXN1bHQuRGF0YSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgY29uc3Qgc3R5bGU6IENTU1N0eWxlRGVjbGFyYXRpb24gPSBzdHlsZVJlc3VsdC5EYXRhO1xyXG5cclxuICAgICAgICAgICAgICAgIGNvbnN0IHJhd1Jlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0QWxsKGAuJHt0aGlzLl9jb2x1bW5JRHNba2V5XX1gKTtcclxuICAgICAgICAgICAgICAgIGlmICghcmF3UmVzdWx0LklzU3VjY2VlZGVkIHx8IHJhd1Jlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgaWYgKHN0eWxlLmRpc3BsYXkgPT09IFwibm9uZVwiKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKGVsbSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAvL1x1MzBEOFx1MzBDM1x1MzBDMFx1MzBGQ1x1MzA0Q1x1OTc1RVx1ODg2OFx1NzkzQVx1MzA2QVx1MzA4OVx1MzBCQlx1MzBEMVx1MzBFQ1x1MzBGQ1x1MzBCRlx1MzBGQ1x1MzA4Mlx1OTc1RVx1ODg2OFx1NzkzQVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbG0uc3R5bGUuZGlzcGxheSA9IFwibm9uZVwiO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgLy9cdTMwRUFcdTMwQjlcdTMwQzhcdTUwNzRcdTMwODJcdTk3NUVcdTg4NjhcdTc5M0FcdTMwNkJcdTMwNTlcdTMwOEJcclxuICAgICAgICAgICAgICAgICAgICByYXdSZXN1bHQuRGF0YS5mb3JFYWNoKHJhdyA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChyYXcgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmF3LnN0eWxlLmRpc3BsYXkgPSBcIm5vbmVcIjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIGNvbnN0IHJlc3RvcmVXaWR0aCA9IGF3YWl0IHRoaXMuX2RvdG5ldEhlbHBlci5pbnZva2VNZXRob2RBc3luYzxudW1iZXI+KCdHZXRXaWR0aCcsIHRoaXMuX2NvbHVtbklEc1trZXldKTtcclxuICAgICAgICAgICAgICAgICAgICBjb25zdCBzaG91bGRSZXN0b3JlV2lkdGg6IGJvb2xlYW4gPSByZXN0b3JlV2lkdGggPiAwO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBjb25zdCB3aWR0aDogbnVtYmVyID0gc2hvdWxkUmVzdG9yZVdpZHRoID8gcmVzdG9yZVdpZHRoIDogTnVtYmVyKHN0eWxlLndpZHRoLm1hdGNoKC9cXGQrLykpO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBsZWZ0ICs9IHdpZHRoO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoZWxtICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsbS5zdHlsZS5sZWZ0ID0gYCR7bGVmdH1weGA7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICBpZiAoc2hvdWxkUmVzdG9yZVdpZHRoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IGhlYWRlclJlc3VsdDogQXR0ZW1wdFJlc3VsdFdpZHRoRGF0YTxFbGVtZW50PiA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KGAjJHt0aGlzLl9jb2x1bW5JRHNba2V5XX1gKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGhlYWRlclJlc3VsdC5Jc1N1Y2NlZWRlZCAmJiBoZWFkZXJSZXN1bHQuRGF0YSAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGhlYWRlclJlc3VsdC5EYXRhIGluc3RhbmNlb2YgSFRNTEVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJSZXN1bHQuRGF0YS5zdHlsZS53aWR0aCA9IGAke3dpZHRofXB4YDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAgICAgLy9cdTMwRUFcdTMwQjlcdTMwQzhcdTUwNzRcdTMwNkVcdTVFNDVcdTMwOTJcdTMwRDhcdTMwQzNcdTMwQzBcdTMwRkNcdTMwNkJcdTU0MDhcdTMwOEZcdTMwNUJcdTMwOEJcclxuICAgICAgICAgICAgICAgICAgICByYXdSZXN1bHQuRGF0YS5mb3JFYWNoKHJhdyA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChyYXcgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmF3LnN0eWxlLndpZHRoID0gc2hvdWxkUmVzdG9yZVdpZHRoID8gYCR7d2lkdGh9cHhgIDogc3R5bGUud2lkdGg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcblxyXG5cclxuICAgIC8vI3JlZ2lvbiBwcml2YXRlXHJcblxyXG4gICAgcHJpdmF0ZSBPbk1vdXNlRG93bihpbmRleDogc3RyaW5nKTogdm9pZCB7XHJcbiAgICAgICAgdGhpcy5faXNSZXNpemluZyA9IHRydWU7XHJcbiAgICAgICAgdGhpcy5fcmVzaXppbmdJbmRleCA9IGluZGV4O1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgT25Nb3VzZVVwKCk6IHZvaWQge1xyXG4gICAgICAgIHRoaXMuX2lzUmVzaXppbmcgPSBmYWxzZTtcclxuICAgICAgICB0aGlzLl9yZXNpemluZ0luZGV4ID0gbnVsbDtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGFzeW5jIE9uTW91c2VNb3ZlKGU6IE1vdXNlRXZlbnQpOiBQcm9taXNlPHZvaWQ+IHtcclxuICAgICAgICBpZiAoIXRoaXMuX2lzUmVzaXppbmcgfHwgdGhpcy5fcmVzaXppbmdJbmRleCA9PT0gbnVsbCkgcmV0dXJuO1xyXG5cclxuICAgICAgICBjb25zdCBuZXh0SW5kZXggPSBOdW1iZXIodGhpcy5fcmVzaXppbmdJbmRleCkgKyAxO1xyXG5cclxuICAgICAgICBjb25zdCByZXNpemluZ05hbWUgPSB0aGlzLl9jb2x1bW5JRHNbdGhpcy5fcmVzaXppbmdJbmRleF07XHJcbiAgICAgICAgY29uc3QgbmV4dE5hbWUgPSB0aGlzLl9jb2x1bW5JRHNbYCR7bmV4dEluZGV4fWBdO1xyXG5cclxuICAgICAgICBjb25zdCBoZWFkZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChgIyR7cmVzaXppbmdOYW1lfWApO1xyXG4gICAgICAgIGNvbnN0IG5leHRIZWFkZXJSZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldChgIyR7bmV4dE5hbWV9YCk7XHJcbiAgICAgICAgY29uc3QgaGVhZGVyV3JhcHBlclJlc3VsdCA9IHRoaXMuX2VsbUhhbmRsZXIuR2V0KEVsZW1lbnRJRHMuVmlkZW9MaXN0SGVhZGVyKTtcclxuICAgICAgICBjb25zdCBjb2x1bW5SZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldEFsbChgLiR7cmVzaXppbmdOYW1lfWApO1xyXG4gICAgICAgIGNvbnN0IG5leHRDb2x1bW5SZXN1bHQgPSB0aGlzLl9lbG1IYW5kbGVyLkdldEFsbChgLiR7bmV4dE5hbWV9YCk7XHJcbiAgICAgICAgY29uc3Qgc2VwUmVzdWx0ID0gdGhpcy5fZWxtSGFuZGxlci5HZXQodGhpcy5fc2VwYXJhdG9ySURzW3RoaXMuX3Jlc2l6aW5nSW5kZXhdKTtcclxuXHJcbiAgICAgICAgLy9cdTg5ODFcdTdEMjBcdTUzRDZcdTVGOTdcdTMwNkJcdTU5MzFcdTY1NTdcdTMwNTdcdTMwNUZcdTMwODlyZXR1cm5cclxuICAgICAgICBpZiAoIWhlYWRlclJlc3VsdC5Jc1N1Y2NlZWRlZCB8fCBoZWFkZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGlmICghY29sdW1uUmVzdWx0LklzU3VjY2VlZGVkIHx8IGNvbHVtblJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKCFzZXBSZXN1bHQuSXNTdWNjZWVkZWQgfHwgc2VwUmVzdWx0LkRhdGEgPT09IG51bGwpIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuICAgICAgICBpZiAoIWhlYWRlcldyYXBwZXJSZXN1bHQuSXNTdWNjZWVkZWQgfHwgaGVhZGVyV3JhcHBlclJlc3VsdC5EYXRhID09PSBudWxsKSB7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKCFuZXh0SGVhZGVyUmVzdWx0LklzU3VjY2VlZGVkIHx8IG5leHRIZWFkZXJSZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm5cclxuICAgICAgICB9XHJcbiAgICAgICAgaWYgKCFuZXh0Q29sdW1uUmVzdWx0LklzU3VjY2VlZGVkIHx8IG5leHRDb2x1bW5SZXN1bHQuRGF0YSA9PT0gbnVsbCkge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAoIShoZWFkZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG4gICAgICAgIGlmICghKG5leHRIZWFkZXJSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICBjb25zdCBoZWFkZXJSZWN0OiBET01SZWN0ID0gaGVhZGVyUmVzdWx0LkRhdGEuZ2V0Qm91bmRpbmdDbGllbnRSZWN0KCk7XHJcbiAgICAgICAgY29uc3QgaGVhZGVyV3JhcHBlclJlY3Q6IERPTVJlY3QgPSBoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEuZ2V0Qm91bmRpbmdDbGllbnRSZWN0KCk7XHJcblxyXG5cclxuICAgICAgICBjb25zdCB3aWR0aCA9IGUuY2xpZW50WCAtIGhlYWRlclJlY3QubGVmdDtcclxuICAgICAgICBjb25zdCBkZWx0YVdpZHRoID0gd2lkdGggLSBoZWFkZXJSZXN1bHQuRGF0YS5vZmZzZXRXaWR0aDtcclxuICAgICAgICBjb25zdCBuZXh0V2lkdGggPSBuZXh0SGVhZGVyUmVzdWx0LkRhdGEub2Zmc2V0V2lkdGggLSBkZWx0YVdpZHRoO1xyXG5cclxuICAgICAgICBoZWFkZXJSZXN1bHQuRGF0YS5zdHlsZS53aWR0aCA9IGAke3dpZHRofXB4YDtcclxuICAgICAgICBuZXh0SGVhZGVyUmVzdWx0LkRhdGEuc3R5bGUud2lkdGggPSBgJHtuZXh0V2lkdGh9cHhgO1xyXG5cclxuICAgICAgICBjb2x1bW5SZXN1bHQuRGF0YS5mb3JFYWNoKGVsbSA9PiB7XHJcbiAgICAgICAgICAgIGlmICghKGVsbSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICAgICAgZWxtLnN0eWxlLndpZHRoID0gYCR7d2lkdGh9cHhgO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBuZXh0Q29sdW1uUmVzdWx0LkRhdGEuZm9yRWFjaChlbG0gPT4ge1xyXG4gICAgICAgICAgICBpZiAoIShlbG0gaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgIGVsbS5zdHlsZS53aWR0aCA9IGAke25leHRXaWR0aH1weGA7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIGF3YWl0IHRoaXMuX2RvdG5ldEhlbHBlci5pbnZva2VNZXRob2RBc3luYygnU2V0V2lkdGgnLCBgJHt3aWR0aH1gLCByZXNpemluZ05hbWUpO1xyXG4gICAgICAgIGF3YWl0IHRoaXMuX2RvdG5ldEhlbHBlci5pbnZva2VNZXRob2RBc3luYygnU2V0V2lkdGgnLCBgJHtuZXh0V2lkdGh9YCwgbmV4dE5hbWUpO1xyXG5cclxuICAgICAgICBpZiAoIShoZWFkZXJXcmFwcGVyUmVzdWx0LkRhdGEgaW5zdGFuY2VvZiBIVE1MRWxlbWVudCkpIHJldHVybjtcclxuICAgICAgICBpZiAoIShzZXBSZXN1bHQuRGF0YSBpbnN0YW5jZW9mIEhUTUxFbGVtZW50KSkgcmV0dXJuO1xyXG5cclxuICAgICAgICBjb25zdCBsZWZ0ID0gaGVhZGVyUmVjdC5sZWZ0IC0gaGVhZGVyV3JhcHBlclJlY3QubGVmdCArIHdpZHRoIC0gMTA7XHJcbiAgICAgICAgc2VwUmVzdWx0LkRhdGEuc3R5bGUubGVmdCA9IGAke2xlZnR9cHhgO1xyXG4gICAgfVxyXG5cclxuICAgIC8vI2VuZHJlZ2lvblxyXG59XHJcblxyXG5pbnRlcmZhY2UgQ2xhc3NOYW1lc0RpY3Qge1xyXG4gICAgW2luZGV4OiBudW1iZXJdOiBzdHJpbmc7XHJcbn0iLCAiaW1wb3J0IHsgRG90TmV0T2JqZWN0UmVmZXJlbmNlIH0gZnJvbSBcIi4uL3NoYXJlZC9Eb3ROZXRPYmplY3RSZWZlcmVuY2VcIjtcclxuaW1wb3J0IHsgRWxlbWVudEhhbmRsZXIsIEVsZW1lbnRIYW5kbGVySW1wbCB9IGZyb20gXCIuLi9zaGFyZWQvRWxlbWVudEhhbmRsZXJcIjtcclxuaW1wb3J0IHsgU3R5bGVIYW5kbGVyLCBTdHlsZUhhbmRsZXJJbXBsIH0gZnJvbSBcIi4uL3NoYXJlZC9TdHlsZUhhbmRsZXJcIjtcclxuaW1wb3J0IHsgU2VsZWN0aW9uSGFuZGxlckltcGwgfSBmcm9tIFwiLi9TZWxlY3Rpb25IYW5kbGVyL3NlbGVjdGlvbkhhbmRsZXJcIjtcclxuaW1wb3J0IHsgRHJvcEhhbmRsZXIsIERyb3BIYW5kbGVySW1wbCB9IGZyb20gXCIuL2Ryb3BIYW5kbGVyL2Ryb3BoYW5kbGVyXCI7XHJcbmltcG9ydCB7IFNvcnRIYW5kbGVyLCBTb3J0SGFuZGxlckltcGwgfSBmcm9tIFwiLi9zb3J0SGFuZGxlci9zb3J0SGFuZGxlclwiO1xyXG5pbXBvcnQgeyBXaWR0aEhhbmRsZXIsIFdpZHRoSGFuZGxlckltcGwgfSBmcm9tIFwiLi93aWR0aEhhbmRsZXIvd2lkdGhIYW5kbGVyXCI7XHJcblxyXG5leHBvcnQgYXN5bmMgZnVuY3Rpb24gaW5pdGlhbGl6ZShibGF6b3JWaWV3OiBEb3ROZXRPYmplY3RSZWZlcmVuY2UsIGlzRmlyc3RSZW5kZXI6IGJvb2xlYW4pIHtcclxuICAgIGNvbnN0IGVsbUhhbmRsZXI6IEVsZW1lbnRIYW5kbGVyID0gbmV3IEVsZW1lbnRIYW5kbGVySW1wbCgpO1xyXG4gICAgY29uc3Qgc3R5bGVIYW5kbGVyOiBTdHlsZUhhbmRsZXIgPSBuZXcgU3R5bGVIYW5kbGVySW1wbChlbG1IYW5kbGVyKTtcclxuICAgIGNvbnN0IHdpZHRoSGFuZGxlcjogV2lkdGhIYW5kbGVyID0gbmV3IFdpZHRoSGFuZGxlckltcGwoZWxtSGFuZGxlciwgc3R5bGVIYW5kbGVyLCBibGF6b3JWaWV3KTtcclxuICAgIGNvbnN0IHNvcnRIYW5kbGVyOiBTb3J0SGFuZGxlciA9IG5ldyBTb3J0SGFuZGxlckltcGwoZWxtSGFuZGxlciwgYmxhem9yVmlldylcclxuICAgIGNvbnN0IGRyb3BIYW5kbGVyOiBEcm9wSGFuZGxlciA9IG5ldyBEcm9wSGFuZGxlckltcGwoYmxhem9yVmlldyk7XHJcblxyXG4gICAgaWYgKGlzRmlyc3RSZW5kZXIpIHtcclxuICAgICAgICBhd2FpdCB3aWR0aEhhbmRsZXIuaW5pdGlhbGl6ZSgpO1xyXG4gICAgICAgIGRyb3BIYW5kbGVyLkluaXRpYWxpemUoKTtcclxuICAgIH1cclxuXHJcbiAgICBzb3J0SGFuZGxlci5pbml0aWFsaXplKHJlZ2lzdGVyZWRMaXN0KTtcclxufVxyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIHNldFdpZHRoKGJsYXpvclZpZXc6IERvdE5ldE9iamVjdFJlZmVyZW5jZSkge1xyXG4gICAgY29uc3QgZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXIgPSBuZXcgRWxlbWVudEhhbmRsZXJJbXBsKCk7XHJcbiAgICBjb25zdCBzdHlsZUhhbmRsZXI6IFN0eWxlSGFuZGxlciA9IG5ldyBTdHlsZUhhbmRsZXJJbXBsKGVsbUhhbmRsZXIpO1xyXG4gICAgY29uc3Qgd2lkdGhIYW5kbGVyOiBXaWR0aEhhbmRsZXIgPSBuZXcgV2lkdGhIYW5kbGVySW1wbChlbG1IYW5kbGVyLCBzdHlsZUhhbmRsZXIsIGJsYXpvclZpZXcpO1xyXG5cclxuICAgIGF3YWl0IHdpZHRoSGFuZGxlci5zZXRXaWR0aCgpO1xyXG59XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gZ2V0U2VsZWN0ZWRJT2ZJbnB1dCgpOiBzdHJpbmcge1xyXG4gICAgY29uc3QgZWxtSGFuZGxlcjogRWxlbWVudEhhbmRsZXIgPSBuZXcgRWxlbWVudEhhbmRsZXJJbXBsKCk7XHJcbiAgICBjb25zdCBoYW5kbGVyID0gbmV3IFNlbGVjdGlvbkhhbmRsZXJJbXBsKGVsbUhhbmRsZXIpO1xyXG5cclxuICAgIHJldHVybiBoYW5kbGVyLmdldFNlbGVjdGVkKCk7XHJcbn1cclxuXHJcbmxldCByZWdpc3RlcmVkTGlzdDogc3RyaW5nW10gPSBbXTtcclxuIl0sCiAgIm1hcHBpbmdzIjogIjtBQXNCTyxJQUFNLDZCQUFOLE1BQXlFO0FBQUEsRUFFNUUsWUFBWSxhQUFzQixNQUFnQixTQUF3QjtBQUN0RSxTQUFLLGNBQWM7QUFDbkIsU0FBSyxPQUFPO0FBQ1osU0FBSyxVQUFVO0FBQUEsRUFDbkI7QUFBQSxFQVFBLE9BQWMsVUFBYSxNQUEyQztBQUNsRSxXQUFPLElBQUksMkJBQTJCLE1BQU0sTUFBTSxJQUFJO0FBQUEsRUFDMUQ7QUFBQSxFQUVBLE9BQWMsS0FBUSxTQUE0QztBQUM5RCxXQUFPLElBQUksMkJBQTJCLE9BQU8sTUFBTSxPQUFPO0FBQUEsRUFDOUQ7QUFDSjs7O0FDMUJPLElBQU0scUJBQU4sTUFBbUQ7QUFBQSxFQUUvQyxJQUFJLE9BQWdEO0FBRXZELFFBQUk7QUFFSixRQUFJO0FBQ0EsZUFBUyxTQUFTLGNBQWMsS0FBSztBQUFBLElBQ3pDLFNBQVMsR0FBUDtBQUNFLGFBQU8sMkJBQTJCLEtBQUssMEdBQXFCLEVBQUUsVUFBVTtBQUFBLElBQzVFO0FBRUEsV0FBTyxVQUFV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QixZQUFZLGNBQWMsVUFBVTtBQUM1RixRQUFNLGNBQTJCLElBQUksZ0JBQWdCLFlBQVksVUFBVTtBQUMzRSxRQUFNLGNBQTJCLElBQUksZ0JBQWdCLFVBQVU7QUFFL0QsTUFBSSxlQUFlO0FBQ2YsVUFBTSxhQUFhLFdBQVc7QUFDOUIsZ0JBQVksV0FBVztBQUFBLEVBQzNCO0FBRUEsY0FBWSxXQUFXLGNBQWM7QUFDekM7QUFFQSxlQUFzQixTQUFTLFlBQW1DO0FBQzlELFFBQU0sYUFBNkIsSUFBSSxtQkFBbUI7QUFDMUQsUUFBTSxlQUE2QixJQUFJLGlCQUFpQixVQUFVO0FBQ2xFLFFBQU0sZUFBNkIsSUFBSSxpQkFBaUIsWUFBWSxjQUFjLFVBQVU7QUFFNUYsUUFBTSxhQUFhLFNBQVM7QUFDaEM7QUFFTyxTQUFTLHNCQUE4QjtBQUMxQyxRQUFNLGFBQTZCLElBQUksbUJBQW1CO0FBQzFELFFBQU0sVUFBVSxJQUFJLHFCQUFxQixVQUFVO0FBRW5ELFNBQU8sUUFBUSxZQUFZO0FBQy9CO0FBRUEsSUFBSSxpQkFBMkIsQ0FBQzsiLAogICJuYW1lcyI6IFsicmVnaXN0ZXJlZExpc3QiLCAiRWxlbWVudElEcyIsICJFbGVtZW50SURzIl0KfQo=
