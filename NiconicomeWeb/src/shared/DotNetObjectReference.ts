export interface DotNetObjectReference {
    invokeMethodAsync<T>(methodName: string, ...params: string[]): Promise<T>;
}