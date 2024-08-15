export interface ArrayEX<T> {
  /**
   * 配列に複数の要素を追加
   * @param source
   */
  pushRange(source: T[]): void;
}

Object.defineProperty(Array.prototype, "pushRange", {
  enumerable: false,
  configurable: true,
  writable: true,
  value: function <T>(this: Array<T>, source: T[]): void {
    source.forEach((x) => {
      this.push(x);
    });
  },
});
