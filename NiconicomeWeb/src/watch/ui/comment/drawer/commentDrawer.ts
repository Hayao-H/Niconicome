import {
  commentLinesBySize,
  commentPosition,
  commentSizeString,
  fontSize,
  sendoption,
} from "./module/types.ts";
import { CommentBase, Layer, Size } from "./module/Comment.ts";
import defaultConfig from "./module/config.ts";

/**
 * メインクラス
 */
export default class NicommentJS {
  /**
   * canvasコンテキスト
   */
  private readonly ctx: CanvasRenderingContext2D;
  /**
   * canvasサイズ
   */
  private readonly canvasSize: Size;
  /**
   * メタ情報
   */
  private readonly meta: Meta;
  /**
   * コメントのサイズごとの行数
   */
  private readonly lines: commentLinesBySize;
  /**
   * 固定コメントのサイズごとの行数
   */
  private readonly fixedLines: commentLinesBySize;
  /**
   * フォントサイズ
   */
  private readonly fonrSize: fontSize;
  /**
   * 固定コメントのフォントサイズ
   */
  private readonly fixedFonrSize: fontSize;
  /**
   * コメント表示時間
   */
  private readonly duration: number;
  /**
   * 自動更新フラグ
   */
  private readonly autoTickDisabled: boolean;
  /**
   * メインレイヤー
   */
  private mainLayerName: string;
  /**
   * 実行フラグ
   */
  private run: boolean;
  /**
   * 再生フラグ
   */
  private isPlay: boolean;
  /**
   * レイヤー
   */
  private layers: Map<string, Layer>;
  /**
   * トータルコメ数
   */
  private total: number;

  /**
   * デバッグモード
   */
  private isDebug: boolean;

  /**
   * @param id canvasのID
   * @param width canvasの幅
   * @param height canvasの高さ
   * @param option オプション
   */
  constructor(
    id: string,
    width: number,
    height: number,
    option?: NicommentJSParam,
  ) {
    //デバッグモード
    this.isDebug = option?.debug ?? false;

    //引数チェック
    this.checkArgs(id, width, height);

    //context
    this.ctx = this._getContext(id, width, height);

    //サイズ
    this.canvasSize = new Size({ height: height, width: width });

    //メタ情報
    this.meta = new Meta();

    //表示時間
    this.duration = option
      ? option.duration ? option.duration : defaultConfig.duration
      : defaultConfig.duration;

    //自動更新
    this.mainLayerName = option
      ? option.layerName ? option.layerName : defaultConfig.defaultLayer
      : defaultConfig.defaultLayer;

    //サイズ・フォント
    this.lines = {
      big: option
        ? option.bigLines ? option.bigLines : defaultConfig.bigLines
        : defaultConfig.bigLines,
      medium: option
        ? option.mediumLines ? option.mediumLines : defaultConfig.mediumLines
        : defaultConfig.mediumLines,
      small: option
        ? option.smallLines ? option.smallLines : defaultConfig.smallLines
        : defaultConfig.smallLines,
    };
    this.fixedLines = {
      big: defaultConfig.fix.big,
      medium: defaultConfig.fix.medium,
      small: defaultConfig.fix.smal,
    };

    this.fonrSize = this._getFontSize(height, this.lines); //fintSIze一覧
    this.fixedFonrSize = this._getFontSize(height, this.fixedLines);

    //レイヤー
    this.mainLayerName = option
      ? option.layerName ? option.layerName : defaultConfig.defaultLayer
      : defaultConfig.defaultLayer;
    this.layers = new Map();
    this.addLayer(this.mainLayerName);
    this.total = 0;
    this.run = true;
    this.isPlay = true;

    //自動更新フラグ
    this.autoTickDisabled = option
      ? option.autoTickDisabled ? option.autoTickDisabled : false
      : false;

    if (option?.debug) {
      Logger.write(`canvasID:${id}, width:${width}px, height:${height}px`);
      Logger.write("初期化が完了しました。");
    }

    if (!this.autoTickDisabled) {
      if (option?.debug) Logger.write("更新処理を開始します。");
      this.tick();
    }
  }

  /**
   * 引数チェック
   * @param id id
   * @param width width
   * @param height height
   */
  private checkArgs(id: string, width: number, height: number): void {
    if (!id) throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NOT_EXIST.ID);
    if (!width) {
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NOT_EXIST.WIDTH);
    }
    if (!height) {
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NOT_EXIST.HEIGHT);
    }

    if (typeof width !== "number") {
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NaN.WIDTH(width));
    }
    if (typeof height !== "number") {
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NaN.HEIGHT(height));
    }
  }

  /**
   * コメントを追加する
   * @param text 表示するコメント
   * @param option オプション。;
   */
  send(text: string, option?: sendoption) {
    let customAttr = new Map<
      string,
      string | number | boolean
    >();
    const layer: string = option
      ? option.layer ? option.layer : this.mainLayerName
      : this.mainLayerName;
    const comType: commentPosition = option
      ? option.type ? option.type : "naka"
      : "naka";
    const comSize: commentSizeString = option
      ? option.size ? option.size : "medium"
      : "medium";
    const color: string = option
      ? option.color ? option.color : "#fff"
      : "#fff";
    const bcolor: string = this.getBcolor(color);
    const vpos: number = option ? option.vpos ? option.vpos : 0 : 0;
    const fontName: string = option
      ? option.fontName ? option.fontName : defaultConfig.defaultFont
      : defaultConfig.defaultFont;
    const opacity: number = option
      ? option.opacity ? option.opacity : defaultConfig.opacity
      : defaultConfig.opacity;
    const full: boolean = option ? option.full ? option.full : false : false;
    const onDisposed: () => void = option
      ? option.onDisposed ? option.onDisposed : () => {}
      : () => {};

    this.total++;
    if (option !== undefined) {
      if (option.customAttr !== undefined) {
        customAttr = this._getAttr(option.customAttr);
      }
    }
    customAttr.set("color", color);
    customAttr.set("bcolor", bcolor);
    customAttr.set("fontName", fontName);
    customAttr.set("opacity", opacity);
    customAttr.set("full", full);

    const layerObj = this.layers.get(layer);
    if (layerObj !== undefined) {
      layerObj.add(text, this.total, customAttr, comType, comSize, {
        onDispased: onDisposed,
      }, vpos);
    } else {
      throw new Error(NicoExceptions.LAYER.LAYER_DOES_NOT_EXIST(layer));
    }

    if (this.isDebug){
      Logger.write(`コメントを追加しました。(${text})`);
    }
  }

  /**
   * 一時停止
   */
  pause(): void {
    if (this.isDebug){
      Logger.write("一時停止");
    }
    this.isPlay = false;
  }

  /**
   * 再生
   */
  play(): void {
    if(this.isDebug){
      Logger.write("再生");
    }
    this.isPlay = true;
  }

  /**
   * コメントを削除します
   * @param layer レイヤー
   */
  clear(layer?: string): void {
    if (layer) {
      const layerObj = this.layers.get(layer);
      if (layerObj !== undefined) {
        layerObj.clear();
      } else {
        throw new Error(NicoExceptions.LAYER.LAYER_DOES_NOT_EXIST(layer));
      }
    } else {
      this.layers.forEach((layer) => {
        layer.clear();
      });
    }
  }

  /**
   * 全ての処理を終了します
   */
  dispose(): void {
    this.run = false;
    this.isPlay = false;
    this.layers.forEach((layer) => {
      layer.clear();
    });
    this.ctx.clearRect(0, 0, this.canvasSize.width, this.canvasSize.height);
    this.layers.clear();
    if (this.isDebug){
      Logger.write("全ての処理を終了しました。");
    }
  }

  /**
   * 属性を取得します
   * @param customAttr カスタム属性
   */
  private _getAttr(customAttr: { [key: string]: any }): Map<string, any> {
    const mapobj: Map<string, any> = new Map();
    for (const [key, value] of Object.entries(customAttr)) {
      mapobj.set(key, value);
    }
    return mapobj;
  }

  private _getFontSize(height: number, lines: commentLinesBySize): fontSize {
    const big: number = height / lines.big;
    const medium: number = height / lines.medium;
    const small: number = height / lines.small;

    return { big: big, medium: medium, small: small };
  }

  /**
   * canvasコンテキストを取得します
   * @param id ID
   * @param width 横幅
   * @param height 高さ
   */
  private _getContext(
    id: string,
    width: number,
    height: number,
  ): CanvasRenderingContext2D {
    const elm: HTMLCanvasElement = document.getElementById(
      id,
    ) as HTMLCanvasElement;
    if (!elm) {
      throw new Error(NicoExceptions.__INIT__.ELEMENT.NOT_EXIST(id));
    } else {
      elm.height = height;
      elm.width = width;
      elm.style.width = `${width}px`;
      elm.style.height = `${height}px`;
      const ctx = elm.getContext("2d");
      if (ctx !== null) {
        return ctx;
      } else {
        throw new Error();
      }
    }
  }

  /**
   * メインループ
   */
  tick(options?: { vpos?: number; render?: boolean }): void {
    if (this.isPlay) {
      const doRender: boolean = options ? options.render ? true : false : true;
      const currentVpos: number = options ? options.vpos ? options.vpos : 0 : 0;

      if (doRender) {
        this.ctx.clearRect(0, 0, this.canvasSize.width, this.canvasSize.height);
      }

      this.layers.forEach((layer) => {
        layer.tick({
          vpos: currentVpos,
          render: doRender,
        });
      });

      this.meta.loop(); //インクリメント
    }

    if (this.run && !this.autoTickDisabled) {
      requestAnimationFrame(() => {
        this.tick(options);
      });
    }
  }

  /**
   * 縁取り色を取得します
   * @param color 色
   */
  private getBcolor(color: string): string {
    switch (color) {
      case "black":
      case "#000":
      case "#000000":
        return "#fff";
      default:
        return "#000";
    }
  }

  /**
   * 指定した位置に存在するコメントを取得します
   * @param x X座標
   * @param y Y座標
   */
  get(x: number, y: number): Array<CommentBase> {
    const comments: Array<CommentBase> = [];
    this.layers.forEach((layer) => {
      comments.push(...layer.get(x, y));
    });

    return comments;
  }

  /**
   * レイヤーを追加します
   * @param layerName レイヤー名
   */
  addLayer(layerName: string): void {
    if (this.layers.has(layerName)) {
      throw new Error(NicoExceptions.LAYER.DUPLICATION(layerName));
    } else {
      this.layers.set(
        layerName,
        new Layer(
          this.ctx,
          this.canvasSize,
          this.lines,
          this.fonrSize,
          this.fixedFonrSize,
          this.duration,
        ),
      );
    }

    if (this.isDebug) {
      Logger.write(`レイヤー "${layerName}" を追加しました。`);
    }
  }

  /**
   * レイヤーを削除します
   * @param layerName レイヤー名
   */
  removeLayer(layerName: string): void {
    if (this.layers.has(layerName)) {
      throw new Error(NicoExceptions.LAYER.DUPLICATION(layerName));
    } else {
      this.layers.delete(layerName);
    }

    if (this.isDebug) {
      Logger.write(`レイヤー "${layerName}" を削除しました。`);
    }
  }
}

/**
 * メタ情報クラス
 */
class Meta {
  /**
   * ループ回数
   */
  private count: number;

  /**
   * 初期化
   */
  constructor() {
    this.count = 0;
  }

  /**
   * カウントを増やします
   */
  loop(): void {
    this.count++;
  }

  /**
   * カウントを取得
   */
  getCount(): number {
    return this.count;
  }
}

/**
 * ログ
 */
class Logger {
  /**
   * ログを出力する
   * @param log 本文
   */
  static write(log: string): void {
    console.log(`[NicommentJS]${log}`);
  }
}

/**
 * エラー
 */
const NicoExceptions = {
  /**
   * 初期化エラー
   */
  __INIT__: {
    /**
     * 引数エラー
     */
    ARGUMENTS: {
      /**
       * 必要な引数が存在しない
       */
      NOT_EXIST: {
        /**
         * 高さ
         */
        HEIGHT: '[ERR]argument "height" must be specified.',
        /**
         * 横幅
         */
        WIDTH: '[ERR]argument "width" must be specified.',
        /**
         * ID
         */
        ID: '[ERR]argument "id" must be specified.',
      },
      /**
       * 引数の値が不適切である
       */
      NaN: {
        /**
         * 高さが数字でない
         */
        HEIGHT: (value: string) =>
          `[ERR]${value} is not a number. "height" must be a number.`,
        /**
         * 横幅が数字でない
         */
        WIDTH: (value: string) =>
          `[ERR]${value} is not a number. "width" must be a number.`,
      },
    },
    /**
     * 要素が存在しない・canvasでない
     */
    ELEMENT: {
      /**
       * 要素が存在しない
       */
      NOT_EXIST: (id: string) => {
        return `[ERR]Canvas Element which id is "${id}" was not found.`;
      },
      /**
       * 要素がHTMLCanvasでない
       */
      NOT_A_CANVAS_ELEMENT: (id: string) => {
        return `[ERR]Element which id is "${id}" is not a canvasHTML5 Element.`;
      },
    },
  },
  /**
   * コメント追加時のエラー
   */
  SEND: {},
  /**
   * レイヤー関係のエラー
   */
  LAYER: {
    /**
     * 重複
     */
    DUPLICATION: (name: string) => {
      return `[ERR]The layer name ${name} already exists.`;
    },
    /**
     * レイヤーが存在しない
     */
    LAYER_DOES_NOT_EXIST: (name: string) => {
      return `[ERR]A layer name is ${name} does not exist.`;
    },
  },
};

/**
 * メインクラスオプション引数
 */
interface NicommentJSParam {
  /**
   * bigコメントの行数
   */
  bigLines?: number;
  /**
   * 通常コメントの行数
   */
  mediumLines?: number;
  /**
   * smallコメントの行数
   */
  smallLines?: number;
  /**
   * コメントの表示時間
   */
  duration?: number;
  /**
   * デフォルトのレイヤー名
   */
  layerName?: string;
  /**
   * 自動更新を無効にする
   */
  autoTickDisabled?: boolean;
  /**
   * デバッグモード
   */
  debug?: boolean;
}
