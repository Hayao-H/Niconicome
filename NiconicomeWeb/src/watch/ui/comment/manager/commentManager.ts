import { Video } from "../../video/videoElement.ts";
import NicommentJS from "../drawer/commentDrawer.ts";
import { ManagedComment } from "./managedComment.ts";

export interface CommentManager {
  on(event: "commentAdded", listener: (comment: ManagedComment) => void): void;

  load(comments: ManagedComment[], duration: number, niconicoID: string): void;

  start(): void;

  dispose(): void;

  readonly currentID: string;
}

export class CommentManagerImpl implements CommentManager {
  constructor(video: Video, commentDrawer: NicommentJS) {
    this._video = video;
    this._commentDrawer = commentDrawer;
    this.initialize();
  }

  private readonly _commentDrawer: NicommentJS;

  private readonly _video: Video;

  private _comments: ManagedComment[][] = [];

  private _isPlaying: boolean = false;

  private _isMainloopRunning: boolean = false;

  private _isDisposed: boolean = false;

  private _currentID: string = "";

  public get currentID(): string {
    return this._currentID;
  }

  private _commentAddedListener:
    | ((comment: ManagedComment) => void)
    | undefined;

  private initialize(): void {
    this._video.on("play", this.onPlay.bind(this));
    this._video.on("pause", this.onPause.bind(this));
    this._video.on("seeked", this.onSeek.bind(this));
  }

  public load(
    comments: ManagedComment[],
    duration: number,
    niconicoID: string,
  ): void {
    this._currentID = niconicoID;
    this._isPlaying = !this._video.paused;
    this._comments = [];
    const blockCount = Math.ceil(duration / 10) + 1;

    for (let i = 0; i < blockCount; i++) {
      this._comments.push([]);
    }

    comments.forEach((comment) => {
      const blockIndex = Math.floor(comment.vposMS / 1000 / 10);
      if (blockIndex + 1 >= blockCount) return;
      if (this._comments[blockIndex] === undefined) {
        this._comments[blockIndex] = [];
      }
      this._comments[blockIndex].push(comment);
    });
  }

  public start(): void {
    if (this._isMainloopRunning) return;
    this._isPlaying = true;
    this._isMainloopRunning = true;
    this._commentDrawer.play();
    this.mainloop();
  }

  public dispose(): void {
    this._isDisposed = true;
    this._isPlaying = false;
    this._commentDrawer.dispose();
    this._video.off("play", this.onPlay);
    this._video.off("pause", this.onPause);
    this._video.off("seeked", this.onSeek);
  }

  public on(
    event: "commentAdded",
    listener: (comment: ManagedComment) => void,
  ): void {
    switch (event) {
      case "commentAdded":
        this._commentAddedListener = listener;
        break;
    }
  }

  private mainloop(): void {
    if (this._isDisposed) return;
    if (!this._isPlaying) return;
    const currentTime = this._video.currentTime;
    const blockIndex = Math.floor(currentTime / 10);
    const vpos = currentTime * 1000;

    this._comments[blockIndex].forEach((comment) => {
      if (
        !comment.isAdded && comment.vposMS < vpos + 500 &&
        comment.vposMS > vpos - 500
      ) {
        comment.isAdded = true;
        this._commentDrawer.send(comment.body, {
          vpos: comment.vposMS,
          color: comment.color,
          type: comment.type,
        });

        if (this._commentAddedListener) {
          this._commentAddedListener(comment);
        }
      }
    });

    this._commentDrawer.tick({ vpos: vpos, render: true });

    requestAnimationFrame(() => {
      if (this._isDisposed || !this._isPlaying) return;
      this.mainloop();
    });
  }

  private onPlay(): void {
    this.start();
  }

  private onPause(): void {
    this._isPlaying = false;
    this._isMainloopRunning = false;
    this._commentDrawer.pause();
  }

  private onSeek(): void {
    this._comments.forEach((block) => {
      block.forEach((comment) => {
        comment.isAdded = false;
      });
    });
  }
}
