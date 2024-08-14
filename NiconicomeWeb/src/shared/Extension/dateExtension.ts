export interface DateExtension {
  /**
   * 日付をフォーマットする
   * @param format フォーマット yyyy:年 MM:月 dd:日 HH:時 mm:分 ss:秒 SSS:ミリ秒 SS:ミリ秒(上2桁)
   * @returns フォーマットされた日付
   */
  Format(format: string): string;
}

Object.defineProperty(Date.prototype, "Format", {
  enumerable: false,
  configurable: true,
  writable: true,
  value: function (format: string): string {
    const year = this.getFullYear();
    const month = (this.getMonth() + 1).toString().padStart(2, "0");
    const date = this.getDate().toString().padStart(2, "0");
    const hours = this.getHours().toString().padStart(2, "0");
    const minutes = this.getMinutes().toString().padStart(2, "0");
    const seconds = this.getSeconds().toString().padStart(2, "0");
    const milliseconds = this.getMilliseconds().toString().padStart(3, "0");
    const milliseconds2 = milliseconds.slice(0, 2);

    return format.replace("yyyy", year.toString())
      .replace("MM", month)
      .replace("dd", date)
      .replace("HH", hours)
      .replace("mm", minutes)
      .replace("ss", seconds)
      .replace("SSS", milliseconds)
      .replace("SS", milliseconds2);
  },
});
