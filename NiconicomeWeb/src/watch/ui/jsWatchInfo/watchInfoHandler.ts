import { JsWatchInfo } from "./videoInfo.ts";

export interface JsWatchInfoHandler {
  /**
   * データを取得する
   */
  getData(): JsWatchInfo | null;
}

export class JsWatchInfoHandlerImpl implements JsWatchInfoHandler {
  getData(): JsWatchInfo | null {
    const element = document.querySelector("#jsWatchInfo");
    if (element === null) return null;

    if (!(element instanceof HTMLElement)) {
      return null;
    }

    const raw = element.dataset["jsWatchInfo"];
    if (raw === "" || raw === undefined) return null;

    return JSON.parse(raw) as JsWatchInfo;
  }
}
