import { DateExtension } from "./dateExtension.ts";

declare global {
  export interface Date extends DateExtension {}
}
