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