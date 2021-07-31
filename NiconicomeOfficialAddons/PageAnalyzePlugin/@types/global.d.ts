import { Response } from "./net/http/fetch/Response";

declare global {

    export var application: Application;

    /**
     * fetch API（現状GETのみ）
     * @param url 取得したいページのURL
     */
    export function fetch(url: string): Promise<Response>;
}

/**
 * グローバルスコープに公開されているapplication変数のインターフェースです
 */
export interface Application {

    /**
     * output APIです</br>
     * 使用するためにはoutput権限を取得する必要があります。
     */
    output: Output;
}

/**
 * {@link Application}直下のoutputプロパティのインターフェースです
 */
export interface Output {
    /**
     * メインページの出力に書き込みます
     */
    write(message: string): void;

    /**
     * メインページの出力に書き込みます
     */
    write(message: unknown): void;
}
