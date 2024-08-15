import { unique } from "../../../../shared/Collection/unique.ts";
import { ManagedComment } from "../manager/managedComment.ts";
import { NGDataFetcher } from "./ngDataFethcer.ts";
import { Logger } from "../../state/logger.ts";
import { Dictionary } from "../../../../shared/Collection/dictionary.ts";

export interface NGHandler {
  /**
   * NGコンテンツを追加
   * @param type 種類
   * @param value NG値
   */
  addNG(type: NGType, value: string): Promise<NGData>;

  /**
   * NGコンテンツを削除
   * @param type 種類
   * @param value NG値
   */
  removeNG(type: NGType, value: string): Promise<NGData>;

  /**
   * NGデータを取得する
   * @returns NGデータ
   */
  getNGData(): Promise<NGData>;

  /**
   * NGワードをフィルタリングする
   * @param comments
   */
  filterNG(comments: ManagedComment[]): Promise<ManagedComment[]>;
}

export type NGType = "word" | "user" | "command";

export interface NGData {
    
  /**
   * NGワード
   */
  words: string[];

  /**
   * NGユーザー
   */
  users: string[];

  /**
   * NGコマンド
   */
  commands: string[];
}

export class NGHandlerImpl implements NGHandler {
  private readonly _ngDataFetcher: NGDataFetcher;

  private _ngData: NGData | undefined;

  constructor(ngDataFetcher: NGDataFetcher) {
    this._ngDataFetcher = ngDataFetcher;
  }

  public async addNG(type: NGType, value: string): Promise<NGData> {
    await this._ngDataFetcher.addNG(type, value);

    if (this._ngData === undefined) {
      this._ngData = await this._ngDataFetcher.getNGData();
    }

    if (type === "word") {
      this._ngData.words = unique([...this._ngData.words, value]);
    } else if (type === "user") {
      this._ngData.users = unique([...this._ngData.users, value]);
    } else if (type === "command") {
      this._ngData.commands = unique([...this._ngData.commands, value]);
    }

    Logger.log(`NG設定を追加しました。種類:${type} 値:${value}`);

    return this._ngData;
  }

  public async removeNG(type: NGType, value: string): Promise<NGData> {
    await this._ngDataFetcher.removeNG(type, value);

    if (this._ngData === undefined) {
      this._ngData = await this._ngDataFetcher.getNGData();
    }

    if (type === "word") {
      this._ngData.words = this._ngData.words.filter((v) => v !== value);
    } else if (type === "user") {
      this._ngData.users = this._ngData.users.filter((v) => v !== value);
    } else if (type === "command") {
      this._ngData.commands = this._ngData.commands.filter((v) => v !== value);
    }

    Logger.log(`NG設定を削除しました。種類:${type} 値:${value}`);

    return this._ngData;
  }

  public async getNGData(): Promise<NGData> {
    if (this._ngData === undefined) {
      this._ngData = await this._ngDataFetcher.getNGData();
    }

    return this._ngData;
  }

  public async filterNG(comments: ManagedComment[]): Promise<ManagedComment[]> {
    if (this._ngData === undefined) {
      this._ngData = await this.getNGData();
    }

   let hit = 0;

    const filtered = comments.filter((comment) => {

      let result = true;

      this._ngData!.words.forEach((word) => {
        if (comment.body.includes(word)) {
          result = false;
          hit++;
          Logger.debug(`filtered: "${comment.body}" due to rule "${word}", type: word`);
          return;
        }
      });

      if (!result) return false;

      this._ngData!.users.forEach((user) => {
        if (comment.userID === user) {
          result = false;
          hit++;
          Logger.debug(`filtered: "${comment.body}" due to rule "${user}", type: user`);
          return;
        }
      });

      if (!result) return false;

      this._ngData!.commands.forEach((command) => {
        if (comment.mail.includes(command)) {
          result = false;
          hit++;
          Logger.debug(`filtered: "${comment.body}" due to rule "${command}", type: command`);
          return;
        }
      });

      if (!result) return false;

      return true;
    });

    Logger.log(`NGフィルタリングを実行しました。${comments.length}件中${hit}件がフィルタリングされました。`);

    return filtered;
  }
}
