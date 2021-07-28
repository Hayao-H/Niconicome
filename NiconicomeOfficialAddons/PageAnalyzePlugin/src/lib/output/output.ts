export class OutputHandler {

    /**
     * 使用可能であるかどうか
     */
    get canUse(): boolean {
        return globalThis.application.output !== null;
    }

    /**
     * メッセージを出力する
     * @param message 書き込むメッセージ
     * @returns 
     */
    write(message:string):void{
        if (!this.canUse) return;
        globalThis.application.output.write(message);
    }
}