export declare class Response{

    /**
     * リクエストの成功状態
     */
    get ok():boolean;

    /**
     * レスポンスを文字列として取得
     */
    text():Promise<string>;

}