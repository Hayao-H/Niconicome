import { DmcInfo } from "./types/dmcinfo";

export interface Hooks {
    registerPageAnalyzeFunction(func: (page: string) => DmcInfo): void;
}