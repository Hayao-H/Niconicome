import {
  AttemptResultWidthData,
  AttemptResultWidthDataImpl,
} from "../../../../shared/AttemptResult.ts";
import { Logger } from "../../state/logger.ts";
import { ManagedComment } from "../manager/managedComment.ts";
import { Comments, CommentType } from "./comment.ts";

export interface CommentFetcher {
  /**
   * コメントを取得する
   * @param url URL
   */
  getComments(url: string): Promise<AttemptResultWidthData<ManagedComment[]>>;
}

interface Cache {
  url: string;
  comments: ManagedComment[];
}

class CommentFetcherImpl implements CommentFetcher {
  private _cache: Cache | undefined = {} as Cache;

  public async getComments(
    url: string,
  ): Promise<AttemptResultWidthData<ManagedComment[]>> {
    if (this._cache !== undefined && this._cache.url === url) {
      return AttemptResultWidthDataImpl.Succeeded<ManagedComment[]>(
        this._cache.comments,
      );
    }

    const startTime = Date.now();
    Logger.log(`コメントを取得します。(${url})`);

    let response: Response;
    try {
      response = await fetch(url);
    } catch {
      Logger.error(`ネットワークエラーによりコメントの取得に失敗しました。`);
      return AttemptResultWidthDataImpl.Fail<ManagedComment[]>(
        `コメントの取得に失敗しました。`,
      );
    }
    if (!response.ok) {
      const message = await response.text();
      Logger.error(`コメントの取得に失敗しました。 ${message}`);
      return AttemptResultWidthDataImpl.Fail<ManagedComment[]>(
        `コメントの取得に失敗しました。 ${message}`,
      );
    }

    const data: Comments = await response.json();

    Logger.log(`コメントを取得しました。(${Date.now() - startTime}ms)`);

    this._cache = {
      url: url,
      comments: data.comments.sort((a, b) => a.vposMS - b.vposMS).map((
        comment,
        i,
      ) => this.getManagedComment(comment, i)),
    };

    return AttemptResultWidthDataImpl.Succeeded<ManagedComment[]>(
      this._cache.comments,
    );
  }

  private getManagedComment(
    comment: CommentType,
    index: number,
  ): ManagedComment {
    let position: "naka" | "ue" | "shita" = "naka";
    if (comment.mail.includes("ue")) {
      position = "ue";
    } else if (comment.mail.includes("shita")) {
      position = "shita";
    }

    let color = "#fff";
    //ニコニコ動画のコメント色をHTMLカラーコードに変換
    if (comment.mail.includes("red")) {
      color = "#ff0000";
    } else if (comment.mail.includes("pink")) {
      color = "#ff8080";
    } else if (comment.mail.includes("orange")) {
      color = "#ffcc00";
    } else if (comment.mail.includes("yellow")) {
      color = "#ffff00";
    } else if (comment.mail.includes("green")) {
      color = "#00ff00";
    } else if (comment.mail.includes("cyan")) {
      color = "#00ffff";
    } else if (comment.mail.includes("blue")) {
      color = "#0000ff";
    } else if (comment.mail.includes("purple")) {
      color = "#8000ff";
    } else if (comment.mail.includes("black")) {
      color = "#000";
    }

    return {
      body: comment.body,
      isAdded: false,
      mail: comment.mail,
      number: comment.number,
      postedAt: comment.postedAt,
      type: position,
      userID: comment.userID,
      vposMS: comment.vposMS,
      color: color,
      innnerIndex: index,
    };
  }
}

export const CommentFetcherObj: CommentFetcher = new CommentFetcherImpl();
