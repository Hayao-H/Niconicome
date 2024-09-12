import { Size } from "./Comment.ts";

//コメント
export type commentPosition = "naka" | "ue" | "shita";
export type commentSizeString = "big" | "medium" | "small";

//コメントオブジェクト引数インターフェース
export interface CommentBaseParam {
  readonly text: string;
  readonly option: commentOption;
  readonly ctx: CanvasRenderingContext2D;
  readonly canvasSize: Size;
  readonly canvasWidthFlash: number;
  readonly lines: commentLinesBySize;
  readonly fontSize: fontSize;
  readonly fixedFontSize: fontSize;
  readonly commentNumber: number;
  onDisposed(): any;
}

//コメントのオプション
export interface commentOption {
  readonly mode: commentPosition;
  readonly color: string;
  readonly borderColor: string;
  readonly commentSize: commentSizeString;
  readonly duration: number;
  readonly customAttr: Map<string, string | number | boolean>;
  readonly vpos: number;
  readonly fontName: string;
  readonly opacity: number;
  readonly full: boolean;
}

//sendオプション
export interface sendoption {
  /**
   * レイヤー名
   */
  layer?: string;
  /**
   * カスタム属性(オブジェクト)
   */
  customAttr?: { [key: string]: string | number | boolean };
  /**
   * コメントの位置(shita/ue/nakaのいずれか)
   */
  type?: commentPosition;
  /**
   * コメントの大きさ(big/small/demiumのいずれか)
   */
  size?: commentSizeString;
  /**
   * コメントの描写色
   */
  color?: string;
  /**
   * VPOS
   */
  vpos?: number;
  /**
   * フォント名
   */
  fontName?: string;
  /**
   * コメントの透過度
   */
  opacity?: number;
  /**
   * ニコニコ動画flash版との互換性をもたせない
   */
  full?: boolean;
  /**
   * コメントが削除される直前に呼び出される関数
   */
  onDisposed?(): () => void;
}

//フォントサイズ
export interface fontSize {
  big: number;
  medium: number;
  small: number;
}

export interface commentLinesBySize {
  big: number;
  medium: number;
  small: number;
}
