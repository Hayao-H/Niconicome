import { DataUtils } from "../utils/DataUtils.ts";
import {
  CommentBaseParam,
  commentLinesBySize,
  commentOption,
  commentPosition,
  commentSizeString,
  fontSize,
} from "./types.ts";
import defaultConfig from "./config.ts";

//util
const datautl = new DataUtils();

//コメントオブジェクト
export class CommentBase {
  private ctx: CanvasRenderingContext2D; //canvas
  private customAttr: Map<string, string | number | boolean>; //カスタム属性
  private duration: number; //表示時間
  private canvasSize: Size; //canvasサイズ
  private canvasWidthFlash: number; //canvas横幅
  width: number; //幅
  private text: Array<string>; //本文
  textForRender: Array<string>; //描写用
  originalText: string; //整形前のコメント
  color: string; //色
  borderColor: string; //縁路
  commentNumber: number; //コメ番
  lines: number; //同種のコメントの行数
  selfLines: number; //コメントの行数
  life: number = 0; //残りコマ数
  delta: number = 0; //1コマ当たりのX座標の変量
  left: number = 0; //X座標
  top: number = 0; //Y座標(原点は左上)
  reveal: number = 0; //コメントが画面右から完全に露出するまでのコマ数
  touch: number = 0; //コメントの左端が画面左に到達するまでのコマ数
  fontSize: number; //フォントサイズ
  fontName: string; //フォント名
  overallSize: number; //オブジェクト高さ
  fontSizeString: commentSizeString; //small/big/mediumのいずれか
  type: commentPosition; //タイプ
  alive: boolean; //フラグ
  fixed: boolean = false; //固定フラグ
  vpos: number; //vpos
  opacity: number; //透過度
  full: boolean;
  textLength: number; //コメントの長さ
  maxLengthIndex: number; //一番長いコメントが格納されているインデックス
  onDisposed: () => void;

  constructor(param: CommentBaseParam) {
    this.type = param.option.mode; //タイプ
    this.customAttr = param.option.customAttr; //カスタム属性
    this.originalText = param.text;
    this.text = this._formatComment(param.text, this.type); //本文
    this.textForRender = this._formatRenderComment(this.text);
    this.textLength = Math.max(...this.text.map((text) => text.length));
    this.maxLengthIndex = this.text.map((comment) => comment.length).indexOf(
      this.textLength,
    );
    this.commentNumber = param.commentNumber;
    this.color = param.option.color; //色
    this.borderColor = param.option.borderColor;
    this.lines = this._getLines(param.option.commentSize, param.lines);
    this.selfLines = this.text.length; //行数
    this.ctx = param.ctx;
    this.fontName = param.option.fontName; //フォント名
    this.opacity = param.option.opacity; //透過度
    this.full = param.option.full; //臨界点リサイズ
    this.canvasSize = param.canvasSize;
    this.canvasWidthFlash = param.canvasWidthFlash;

    //固定フラグ
    if (this.text.length >= this.lines) {
      this.fixed = true;
    }

    //vpos・表示時間
    this.vpos = param.option.vpos;
    this.duration = param.option.duration || defaultConfig.duration;

    //フォント関係
    this.fontSizeString = param.option.commentSize;
    [this.fontSize, this.width] = this._getFont(
      this.text[this.maxLengthIndex],
      this.canvasSize,
      this.canvasWidthFlash,
      param.option.commentSize,
      param.fontSize,
      param.fixedFontSize,
      this.type,
    );
    this.fontSizeString = param.option.commentSize;
    this.overallSize = this.fontSize * this.selfLines;

    this.alive = true; //生存フラグ
    this.onDisposed = param.onDisposed; //コールバック

    this._set(); //セット実行
  }

  //属性取得
  getProp<T>(key: string): T {
    return this.customAttr.get(key) as T;
  }

  /**
   * 更新処理
   */
  tick(vpos: number): void {
    //コメントの累計表示時間が既定の2倍以上であった場合、コメントを削除する
    if (vpos >= this.vpos + this.duration * 2) {
      this.alive = false;
      return;
    }

    this.life--;

    //nakaコメントの場合
    if (this.type === "naka") {
      this.left -= this.delta;
      this.reveal--;
      this.touch--;
    }

    if (this.life < 0) {
      this.kill();
    }
  }

  /**
   * コメントの生存フラグを強制的に降ろします
   */
  kill(): void {
    this.alive = false;
    this.onDisposed();
  }

  //セット
  private _set() {
    this.life = 0; //残りコマ数
    this.left = 0; //X座標
    this.top = 0; //Y座標(原点は左上)
    this.delta = 0; //1コマ当たりのX座標の変量
    this.reveal = 0; //コメントが画面右から完全に露出するまでのコマ数
    this.touch = 0; //コメントの左端が画面左に到達するまでのコマ数

    switch (this.type) {
      case "naka":
        this._setNaka();
        break;
      case "shita":
      case "ue":
        this._setShitaUe();
    }
  }

  //nakaコメント設定
  private _setNaka() {
    this.life = defaultConfig.fps * this.duration;
    this.left = this.canvasSize.width;
    this.delta = (this.canvasSize.width + this.width) / this.life;
    this.reveal = this.width / this.delta;
    this.touch = this.canvasSize.width / this.delta;
  }

  /**
   * shit・ueコメントを設定する
   * @param width canvasの横幅
   * @param duration コメント表示時間
   */
  private _setShitaUe() {
    this.life = defaultConfig.fps * this.duration;
    if (this.fixed) {
      this.left = (this.canvasSize.width - this.width) / 2;
      //this.left = this.canvasSize.width / 2;
    } else {
      this.left = this.canvasSize.width / 2;
    }
  }

  //fontを決定する
  private _getFont(
    text: string,
    canvasSize: Size,
    flashWidth: number,
    commentSize: commentSizeString,
    fontSize: fontSize,
    fixedFontSize: fontSize,
    type: commentPosition,
  ): [number, number] {
    let originalFont: number = this._getSize(commentSize, fontSize);
    let font: number = originalFont;

    //・改行リサイズ
    //overallSizeに相当する高さが、描写領域の1/3を上回る場合に、
    //リサイズを行う。
    if (originalFont * this.selfLines > canvasSize.height / 3) {
      font = this._getSize(commentSize, fixedFontSize);
    }

    this.ctx.font = `${font}px "Yu Gothic"`;
    let comWidth: number = this.ctx.measureText(text).width;

    //・臨界幅リサイズ
    //コメントの最大幅が描写領域を上回る場合に、描写領域に収まるようにリサイズする
    const widthOverflow: boolean = comWidth > this.canvasSize.width &&
      type !== "naka";
    if (widthOverflow && !this.full) {
      font = this._modSize(originalFont, comWidth, this.canvasWidthFlash);
      this.ctx.font = `${font}px "Yu Gothic"`;
      comWidth = this.ctx.measureText(text).width;
    } else if (widthOverflow && this.full) {
      font = this._modSize(originalFont, comWidth, this.canvasSize.width);
      this.ctx.font = `${font}px "Yu Gothic"`;
      comWidth = this.ctx.measureText(text).width;
    }

    return [font, comWidth];
  }

  //フォントサイズ修正
  private _modSize(font: number, width: number, canvasWidth: number): number {
    return font * canvasWidth / width;
  }

  //fontSize取得
  private _getSize(commentSize: commentSizeString, fontSize: fontSize): number {
    switch (commentSize) {
      case "big":
        return fontSize.big;
      case "small":
        return fontSize.small;
      default:
        return fontSize.medium;
    }
  }

  /**
   * 行数を求める
   * @param size big/small/mediumのいずれか
   * @param allLines commentLines型のオブジェクト(それぞれのサイズについて行数を表す)
   */
  private _getLines(
    size: commentSizeString,
    allLines: commentLinesBySize,
  ): number {
    switch (size) {
      case "big":
        return allLines.big;
      case "medium":
        return allLines.medium;
      case "small":
        return allLines.small;
    }
  }

  /**
   * コメントを整形します
   * @param origin コメント
   * @param commentPos コメントのタイプ
   */
  private _formatComment(
    origin: string,
    commentPos: commentPosition,
  ): Array<string> {
    let formated: Array<string> = datautl.splitter(origin, "\n");
    formated = this._deleteBlank(formated);
    formated = this._deleteFirstAndLastBlank(formated);
    formated = this._sortByType(formated, commentPos);
    return formated;
  }

  /**
   * 描写用コメントを整形します
   * @param origin コメント
   * @param commentPos コメントのタイプ
   */
  private _formatRenderComment(origin: Array<string>): Array<string> {
    let formated = [...origin];
    formated = this._deleteBlankLineFromEnd(origin);
    return formated;
  }

  /**
   * コメントから3回以上連続する改行を削除します。
   * @param comments コメントのリスト
   */
  private _deleteBlank(commentList: Array<string>): Array<string> {
    let count: number = 0;
    const deleted: Array<string> = [];

    //for...inループで回す
    for (const text of commentList) {
      const isBlank: boolean = !text;
      if (isBlank) {
        count++;
      } else {
        count = 0;
      }
      if (count < 3) {
        deleted.push(text);
      }
    }

    return deleted;
  }

  /**
   * 最初と最後の空白行を削除します。
   * @param comments コメントのリスト
   */
  private _deleteFirstAndLastBlank(comments: Array<string>): Array<string> {
    const deleted = [...comments];
    for (const comment of comments) {
      if (!comment) {
        deleted.shift();
      } else {
        break;
      }
    }

    //後ろから
    const reversed = [...deleted].reverse();
    for (const comment of reversed) {
      if (!comment) {
        deleted.pop();
      } else {
        break;
      }
    }

    return deleted;
  }

  /**
   * コメントの位置によってソートします
   * @param comments コメントのリスト
   * @param type コメントの位置
   */
  private _sortByType(
    comments: Array<string>,
    type: commentPosition,
  ): Array<string> {
    return type === "shita" ? comments.reverse() : comments;
  }

  /**
   * 空行を削除します
   * @param comments コメント
   */
  private _deleteBlankLineFromEnd(comments: Array<string>): Array<string> {
    const formated = [...comments];
    const reversed = [...comments].reverse();

    for (const line of reversed) {
      const isBlank: boolean = /^\s+$/.test(line) || line.length === 0;
      if (isBlank) {
        formated.pop();
      } else {
        break;
      }
    }

    return formated;
  }
}

//座標情報
export class Point {
  x: number;
  y: number;
  constructor(pos: { x: number; y: number }) {
    this.x = pos.x;
    this.y = pos.y;
  }
}

//height:width
export class Size {
  height: number;
  width: number;
  constructor(size: { height: number; width: number }) {
    this.height = size.height;
    this.width = size.width;
  }
}

//レイヤークラス
export class Layer {
  private readonly ctx: CanvasRenderingContext2D; //context
  private readonly canvasSize: Size; //横幅
  private readonly canvasWidthFlash: number; //flash板横幅
  private readonly lines: commentLinesBySize; //行数
  private readonly fonrSize: fontSize; //fontsize
  private readonly fixedFonrSize: fontSize; //fontsize
  private readonly duration: number;
  private naka: NakaLine; //nakaコメント配列
  private shita: Array<CommentBase>; //shitaコメント配列
  private ue: Array<CommentBase>; //ueコメント配列
  private maxlines: number; //最大行数(smallコメと同じになる)
  private comments: Array<CommentBase>; //総合コメント

  //初期化
  constructor(
    ctx: CanvasRenderingContext2D,
    canvasSize: Size,
    lines: commentLinesBySize,
    commentSize: fontSize,
    commentSizeFixed: fontSize,
    duration: number,
  ) {
    this.ctx = ctx;
    this.canvasSize = canvasSize;
    this.canvasWidthFlash = canvasSize.height / 3 * 4;
    this.lines = lines;
    this.fonrSize = commentSize;
    this.fixedFonrSize = commentSizeFixed;
    this.duration = duration;
    this.comments = [];
    this.maxlines = lines.small;
    this.naka = new NakaLine(lines.small, canvasSize);
    this.shita = [];
    this.ue = [];
  }

  /**
   * レイヤーにコメントを追加する
   * @param text コメント本文
   * @param commentNumber コメ番
   * @param customAttr カスタム属性
   * @param type naka/shita/ueのいずれか
   * @param size big/medium/largeのいずれか
   * @param vpos VPOS
   */
  add(
    text: string,
    commentNumber: number,
    customAttr: Map<string, string | number | boolean>,
    type: commentPosition = "naka",
    size: commentSizeString = "medium",
    callBack: { onDispased: () => void },
    vpos: number,
  ): void {
    if (this.comments.length > 40) return;
    const options: commentOption = {
      mode: type,
      color: customAttr.get("color") as string || "#fff",
      borderColor: customAttr.get("bcolor") as string || "#000",
      duration: this.duration,
      customAttr: customAttr,
      commentSize: size,
      vpos: vpos,
      fontName: customAttr.get("fontName") as string,
      opacity: customAttr.get("opacity") as number,
      full: customAttr.get("full") as boolean,
    };

    const param: CommentBaseParam = {
      text: text,
      ctx: this.ctx,
      canvasSize: this.canvasSize,
      canvasWidthFlash: this.canvasWidthFlash,
      fontSize: this.fonrSize,
      fixedFontSize: this.fixedFonrSize,
      lines: this.lines,
      option: options,
      commentNumber: commentNumber,
      onDisposed: callBack.onDispased,
    };

    const commentObj = new CommentBase(param);

    switch (commentObj.type) {
      case "naka":
        this.naka.add(commentObj);
        break;
      case "shita":
        this._appendShita(commentObj);
        break;
      case "ue":
        this._appendUe(commentObj);
        break;
    }
  }

  /**
   * 更新処理
   * @param counter ループ回数
   */
  tick(options?: { vpos?: number; render?: boolean }): void {
    const currentVpos: number = options ? options.vpos ? options.vpos : 0 : 0;
    const doRender: boolean = options ? options.render ? true : false : true;

    if (this.comments.length) {
      this.comments.forEach((comment) => {
        if (doRender) this._render(comment);
        comment.tick(currentVpos);
      });
    }

    //nakaコメ

    if (doRender) {
      this.naka.getList().forEach((comment) => {
        this._render(comment);
      });
    }
    this.naka.tick(currentVpos);

    this._clean();
  }

  /**
   * 描写処理
   * @param comment コメント
   */
  private _render(comment: CommentBase) {
    this.ctx.textBaseline = "top";
    //揃え位置
    switch (true) {
      case comment.type === "naka" || comment.fixed:
        this.ctx.textAlign = "left";
        break;
      default:
        this.ctx.textAlign = "center";
        break;
    }
    this.ctx.fillStyle = `${comment.color}`;
    this.ctx.shadowColor = comment.borderColor;
    this.ctx.shadowOffsetX = 1.5;
    this.ctx.shadowOffsetY = 1.5;
    this.ctx.globalAlpha = comment.opacity;
    this.ctx.font = `bold ${comment.fontSize}px "${comment.fontName}"`;
    //描写時に考慮する正負
    const deltaMinusOrPlus: number = comment.type === "shita" ? -1 : 1;
    const delta = deltaMinusOrPlus * comment.fontSize;
    for (let i = 0; i < comment.textForRender.length; i++) {
      this.ctx.fillText(
        comment.textForRender[i],
        comment.left,
        comment.top + delta * i,
      );
    }
  }

  /**
   * コメントを削除する
   * @param  option 最初のコメントまたは、最後のコメントのみを削除することが出来ます。
   */
  clear(option?: "first" | "last") {
    switch (option) {
      case undefined:
        this.comments.forEach((comment) => comment.kill());
        this.comments = [];
        this.naka = new NakaLine(this.lines.small, this.canvasSize);
        this.ue = [];
        this.shita = [];
        break;
      case "first":
        if (this.comments.length) this.comments[0].kill();
        break;
      case "last":
        if (this.comments.length) {
          this.comments[this.comments.length - 1].kill();
        }
    }
  }

  /**
   * コメント配列からフラグが降りているコメントを削除
   */
  private _clean(): void {
    this.comments = this.comments.filter((commet) => commet.alive);
    //nakaコメ
    this.naka.clean();
  }

  /**
   * shitaコメントを追加する
   * @param comment コメントオブジェクト
   */
  private _appendShita(comment: CommentBase) {
    //shitaコメント
    let bottom = this.canvasSize.height;
    for (let i = 0; i < 40; i++) {
      if (this.shita[i] && this.shita[i].alive && !comment.fixed) {
        bottom -= this.shita[i].overallSize;
      }

      switch (true) {
        case this.shita.length === 0:
        case this.shita.length <= i:
        case !this.shita[i].alive:
          break;
        //固定コメントであった場合
        case comment.fixed:
          break;
        //コメントが表示限界を超える場合
        case bottom - comment.overallSize < 0:
          break;
        default:
          continue;
      }

      if (comment.fixed) {
        comment.top = this.canvasSize.height - comment.fontSize;
      } else if (bottom - comment.overallSize < 0) {
        comment.top = Math.random() *
          (this.canvasSize.height - comment.overallSize);
      } else {
        comment.top = bottom - comment.overallSize;
      }
      this.shita[i] = comment;
      this.comments.push(comment);
      break;
    }
  }

  /**
   * ueコメントを追加する
   * @param comment コメントオブジェクト
   */
  private _appendUe(comment: CommentBase) {
    //shitaコメント
    let top = 0;
    for (let i = 0; i < 40; i++) {
      if (this.ue[i] && this.ue[i].alive && !comment.fixed) {
        top += this.ue[i].overallSize;
      }

      switch (true) {
        case this.ue.length === 0:
        case this.ue.length <= i:
        case !this.ue[i].alive:
        case comment.fixed:
        case top + comment.overallSize > this.canvasSize.height:
          break;
        default:
          continue;
      }

      if (comment.fixed) {
        comment.top = 0;
      } else if (top + comment.overallSize > this.canvasSize.height) {
        comment.top = Math.random() *
          (this.canvasSize.height - comment.overallSize);
      } else {
        comment.top = top;
      }
      this.ue[i] = comment;
      this.comments.push(comment);
      break;
    }
  }

  /**
   * 指定した位置に存在するコメントの配列を取得します
   * @param x X座標
   * @param y Y座標
   */
  get(x: number, y: number): Array<CommentBase> {
    const comments: Array<CommentBase> = [];
    const naka = this.naka.get(x, y);
    if (naka !== undefined) {
      comments.push(naka);
    }

    const ueshita: Array<CommentBase> = this.comments.filter((comment) => {
      const half: number = comment.width / 2;
      const isX: boolean = x > comment.left - half && x < comment.left + half;
      const isY: boolean = y > comment.top &&
        y < comment.top + comment.overallSize;
      return isX && isY;
    });

    comments.push(...ueshita);

    return comments;
  }
}

/**
 * nakaコメント
 */
class NakaLine {
  private lastCommentStream: Array<CommentBase>;
  private comments: Array<CommentBase>;
  private canvasSize: Size;
  private allines: number; //全ての行数

  constructor(smallLines: number, size: Size) {
    //配列初期化
    this.comments = [];
    this.lastCommentStream = [];

    //配列長
    this.allines = smallLines;

    //canvasサイズ
    this.canvasSize = size;
  }

  /**
   * nakaコメントを追加する
   * @param comment コメント
   */
  add(comment: CommentBase): void {
    let top: number = 0;
    for (let i: number = 0; i < this.allines; i++) {
      const line = this.lastCommentStream[i];
      switch (true) {
        //i行目にコメントが無い場合
        case (line === undefined):
          break;
          //固定コメであった場合
        case (comment.fixed):
          break;
          //最終追加コメントが全て表示されていて、なおかつ追いつかない場合
        case (line.reveal < 0 && line.life < comment.touch):
          break;
          //コメントのtopがcanvasの高さを超えている
        case (top + comment.overallSize > this.canvasSize.height):
          break;

        //それ以外は次の行を確認
        default:
          top += line.overallSize;
          continue;
      }

      //座標設定
      if (comment.fixed) {
        comment.top = 0;
      } else if (top + comment.overallSize > this.canvasSize.height) {
        //弾幕モード
        comment.top = Math.random() *
          (this.canvasSize.height - comment.overallSize);
      } else {
        comment.top = top;
      }
      this.comments.push(comment);
      this.lastCommentStream[i] = comment;
      break;
    }
  }

  /**
   * 流れたコメントを削除する
   */
  clean(): void {
    this.comments = this.comments.filter((comment) => comment.alive);
  }

  /**
   * 更新処理
   */
  tick(vpos: number): void {
    this.comments.forEach((comment) => {
      comment.tick(vpos);
    });
  }

  /**
   * コメントのリストを取得する
   */
  getList(): Array<CommentBase> {
    return this.comments;
  }

  /**
   * コメントを取得する
   * @param x X座標
   * @param y Y座標
   */
  get(x: number, y: number): CommentBase | undefined {
    return this.comments.find((comment) => {
      const isX: boolean = x > comment.left && x < comment.left + comment.width;
      const isY: boolean = y > comment.top &&
        y < comment.top + comment.overallSize;
      if (isX && isY) {
        return true;
      }
    });
  }
}
