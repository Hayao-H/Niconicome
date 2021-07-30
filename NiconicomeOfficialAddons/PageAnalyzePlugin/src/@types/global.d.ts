import { Response } from "./net/http/fetch/Response";

declare global {

    /**
     * APIのルート
     */
    const application: application;
    
    /**
     * fetch API（現状GETのみ）
     * @param url 取得したいページのURL
     */
    function fetch(url: string): Promise<Response>;
}


interface application {
    output: output;
}

interface output {
    /**
     * メインページの出力に書き込む
     */
    write(message: string): void;

    /**
     * メインページの出力に書き込む
     */
    write(message: unknown): void;
}