import { NGData, NGType } from "./ngHandler.ts";

export interface NGDataFetcher {
  /**
   * NGコンテンツを追加
   * @param type 種類
   * @param value NG値
   */
  addNG(type: NGType, value: string): Promise<void>;

  /**
   * NGコンテンツを削除
   * @param type 種類
   * @param value NG値
   */
  removeNG(type: NGType, value: string): Promise<void>;

  /**
   * NGデータを取得する
   * @returns NGデータ
   */
  getNGData(): Promise<NGData>;
}

interface ServerNGResponse {
  words: string[];
  users: string[];
  commands: string[];
}

interface ServerNGRequest {
  type: NGType;
  value: string;
}

export class NGDataFetcherImpl implements NGDataFetcher {
  constructor(baseURL: string) {
    this._baseURL = baseURL;
  }

  private readonly _baseURL: string;

  public async addNG(type: NGType, value: string): Promise<void> {
    const url = new URL(`${this._baseURL}/set`);
    const request: ServerNGRequest = {
      type: type,
      value: value,
    };
    await fetch(url.toString(), {
      method: "POST",
      body: JSON.stringify(request),
    });
  }

  public async removeNG(type: NGType, value: string): Promise<void> {
    const url = new URL(`${this._baseURL}/delete`);
    const request: ServerNGRequest = {
      type: type,
      value: value,
    };
    await fetch(url.toString(), {
      method: "POST",
      body: JSON.stringify(request),
    });
  }

  public async getNGData(): Promise<NGData> {
    const url = new URL(`${this._baseURL}/get`);
    const response = await fetch(url.toString());
    const data: ServerNGResponse = await response.json();
    return {
      words: data.words,
      users: data.users,
      commands: data.commands,
    };
  }
}
