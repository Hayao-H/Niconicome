import { application } from "../../@types/global";
export class OutputHandler {
    /**
     * 使用可能であるかどうか
     */
    get canUse() {
        return application.output !== null;
    }
    /**
     * メッセージを出力する
     * @param message 書き込むメッセージ
     * @returns
     */
    write(message) {
        if (!this.canUse)
            return;
        application.output.write(message);
    }
}
