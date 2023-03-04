export interface DotNetObjectReference {
    invokeMethodAsync(methodName: string, ...params: string[]): Promise<void>;
}