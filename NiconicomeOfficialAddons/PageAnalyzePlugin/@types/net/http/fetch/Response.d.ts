export declare interface Response {

    /**
     * リクエストの成功状態
     * ```typescript
     * if (!response.ok){
     *  //エラー処理
     * }
     * ```
     */
    get ok(): boolean;

    /**
     * レスポンスを文字列として取得
     * ``` typescript
     * content:string = await response.text();
     * ```
     */
    text(): Promise<string>;

}