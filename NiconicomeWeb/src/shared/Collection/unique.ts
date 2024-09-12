export function unique<T>(array: T[]): T[] {
  return array.filter((x, i, self) => self.indexOf(x) === i);
}
