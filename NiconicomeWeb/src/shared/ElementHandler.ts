/// <reference lib="dom" />

import {
  AttemptResultWidthData,
  AttemptResultWidthDataImpl,
} from "./AttemptResult.ts";

export interface ElementHandler {
  /**
   * 要素を取得する
   * @param query クエリ
   */
  Get(query: string): AttemptResultWidthData<Element>;

  /**
   * 複数の要素を取得する
   * @param query クエリ
   */
  GetAll(query: string): AttemptResultWidthData<NodeListOf<Element>>;
}

export class ElementHandlerImpl implements ElementHandler {
  public Get(query: string): AttemptResultWidthData<Element> {
    let result: Element | null;

    try {
      result = document.querySelector(query);
    } catch (e: any) {
      return AttemptResultWidthDataImpl.Fail(
        `要素を取得できませんでした。(詳細：${e.message})`,
      );
    }

    return result == null
      ? AttemptResultWidthDataImpl.Fail("指定された要素が見つかりません。")
      : AttemptResultWidthDataImpl.Succeeded(result);
  }

  public GetAll(query: string): AttemptResultWidthData<NodeListOf<Element>> {
    let result: NodeListOf<Element>;

    try {
      result = document.querySelectorAll(query);
    } catch (e: any) {
      return AttemptResultWidthDataImpl.Fail(
        `要素を取得できませんでした。(詳細：${e.message})`,
      );
    }

    return AttemptResultWidthDataImpl.Succeeded(result);
  }
}
