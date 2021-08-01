export class OutputHandler {

    /**
     * 使用可能であるかどうか
     */
    get canUse(): boolean {
        return application.output !== null;
    }

    /**
     * メッセージを出力する
     * @param message 書き込むメッセージ
     * @returns 
     */
    write(message:string):void{
        if (!this.canUse) return;
        application.output!.write(message);
    }
}