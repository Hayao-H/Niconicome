import { ArrayEX } from "./arrayExtension.ts";
import { DateExtension } from "./dateExtension.ts";

declare global {
  export interface Date extends DateExtension {}

  export interface Array<T> extends ArrayEX<T> {}
}
