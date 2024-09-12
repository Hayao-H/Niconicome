import Hls from "https://esm.sh/hls.js@1.5.8";
import { JsWatchInfo } from "../jsWatchInfo/videoInfo.ts";
import { AttemptResult } from "../../../shared/AttemptResult.ts";
import { AttemptResultImpl } from "../../../shared/AttemptResult.ts";
import { Logger } from "../state/logger.ts";

export interface Video {
  /**
   * 再生時間
   */
  duration: number;

  /**
   * 現在の再生時間
   */
  currentTime: number;

  /**
   * 再生フラグ
   */
  paused: boolean;

  /**
   * バッファー
   */
  buffered: TimeRanges;

  /**
   * リピートフラグ
   */
  repeat: boolean;

  /**
   * 初期化
   */
  Initialize(
    source: JsWatchInfo,
    videoElement?: HTMLVideoElement | undefined,
  ): Promise<AttemptResult>;

  /**
   * イベントリスナー登録
   * @param event
   * @param listener
   */
  on<K extends keyof HTMLVideoElementEventMap>(
    event: K,
    listener: (ev: HTMLVideoElementEventMap[K]) => void,
  ): void;

  /**
   * イベントリスナーを解除
   * @param listener
   */
  off<K extends keyof HTMLVideoElementEventMap>(
    event: K,
    listener: () => void,
  ): void;

  /**
   * 購読状況を確認
   * @param ev
   */
  listened<K extends keyof HTMLVideoElementEventMap>(
    ev: K,
    listener: (ev: HTMLVideoElementEventMap[K]) => void,
  ): boolean;

  /**
   * 再生
   */
  play(): void;

  /**
   * 一時停止
   */
  pause(): void;
}

//Videoの実装
export class VideoImpl implements Video {
  private _videoElement: HTMLVideoElement;

  private _hls: Hls | undefined;

  private _source: JsWatchInfo | undefined;

  private _listners = new Map<string, object[]>();

  constructor(videoElement: HTMLVideoElement) {
    this._videoElement = videoElement;
  }

  get duration() {
    return this._videoElement.duration;
  }

  get currentTime() {
    return this._videoElement.currentTime;
  }

  set currentTime(time: number) {
    this._videoElement.currentTime = time;
  }

  get paused() {
    return this._videoElement.paused;
  }

  get buffered() {
    return this._videoElement.buffered;
  }

  get repeat() {
    return this._videoElement.loop;
  }

  set repeat(repeat: boolean) {
    this._videoElement.loop = repeat;
  }

  public async Initialize(
    source: JsWatchInfo,
    videoElement?: HTMLVideoElement | undefined,
  ): Promise<AttemptResult> {
    if (!Hls.isSupported()) {
      return AttemptResultImpl.Fail("HLS is not supported");
    }

    if (videoElement) {
      this._videoElement = videoElement;
    }

    this._source = source;
    if (this._hls) {
      this._hls.destroy();
    }
    this._hls = new Hls();

    if (!this._source.media.isDMS) {
      const res = await fetch(source.media.createUrl);
      if (!res.ok) {
        return AttemptResultImpl.Fail("Failed to fetch DMS URL");
      }

      Logger.log("HLSメディアファイルを生成しました。");
      this._hls!.loadSource(source.media.contentUrl);
      this._hls!.attachMedia(this._videoElement);
    } else {
      this._hls.loadSource(source.media.contentUrl);
      this._hls.attachMedia(this._videoElement);
    }

    return AttemptResultImpl.Succeeded();
  }

  public on<K extends keyof HTMLVideoElementEventMap>(
    event: K,
    listener: (ev: HTMLVideoElementEventMap[K]) => void,
  ): void {
    if (!this._listners.has(event)) this._listners.set(event, []);
    this._listners.get(event)!.push(listener);
    this._videoElement.addEventListener(event, listener);
  }

  public off<K extends keyof HTMLVideoElementEventMap>(
    event: K,
    listener: () => void,
  ): void {
    if (this._listners.has(event)) {
      this._listners.set(
        event,
        this._listners.get(event)!.filter((l) => l !== listener),
      );
    }
    this._videoElement.removeEventListener(event, listener);
  }

  public play(): void {
    this._videoElement.play();
  }

  public pause(): void {
    this._videoElement.pause();
  }

  public listened<K extends keyof HTMLVideoElementEventMap>(
    ev: K,
    listener: (ev: HTMLVideoElementEventMap[K]) => void,
  ): boolean {
    const listeners = this._listners.get(ev);
    if (!listeners) return false;
    return listeners.filter((l) => l === listener).length > 0;
  }
}
