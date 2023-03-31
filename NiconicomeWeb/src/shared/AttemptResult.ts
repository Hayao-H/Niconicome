export interface AttemptResultWidthData<T> extends AttemptResult {
    
    /**
     * データ
     */
    readonly Data: T | null;
}

export interface AttemptResult {

    /**
     * 成功フラグ
     */
    readonly IsSucceeded: boolean;

    /**
     * メッセージ
     */
    readonly Message: string | null;
}


export class AttemptResultWidthDataImpl<T> implements AttemptResultWidthData<T> {

    constructor(isSucceeded: boolean, data: T | null, message: string | null) {
        this.IsSucceeded = isSucceeded;
        this.Data = data;
        this.Message = message;
    }

    readonly IsSucceeded: boolean;

    readonly Data: T | null;

    readonly Message: string | null;

    public static Succeeded<T>(data: T | null): AttemptResultWidthData<T> {
        return new AttemptResultWidthDataImpl(true, data, null);
    }

    public static Fail<T>(message: string): AttemptResultWidthData<T> {
        return new AttemptResultWidthDataImpl(false, null, message);
    }
}

export class AttemptResultImpl implements AttemptResult {

    constructor(isSucceeded: boolean, message: string | null) {
        this.IsSucceeded = isSucceeded;
        this.Message = message;
    }

    readonly IsSucceeded: boolean;

    readonly Message: string | null;

    public static Succeeded(): AttemptResult {
        return new AttemptResultImpl(true, null);
    }

    public static Fail(message: string): AttemptResult {
        return new AttemptResultImpl(false, message);
    }
}

