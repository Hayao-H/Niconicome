interface Logger {
  /**
   * 出力されたメッセージ
   */
  messages: LogItem[];

  /**
   * ログ
   * @param message
   * @returns
   */
  log: (message: string, border?: boolean) => void;

  /**
   * エラー
   * @param message
   * @returns
   */
  error: (message: string) => void;

  /**
   * 警告
   * @param message
   * @returns
   */
  warn: (message: string) => void;

  /**
   * デバッグ
   * @param message
   * @returns
   */
  debug: (message: string) => void;

  /**
   * ログをクリア
   */
  clear(): void;

  /**
   * イベント購読
   * @param event
   * @param listener
   */
  addEventListner(
    event: "write" | "clear",
    listener: (log: LogItem | null) => void,
  ): void;

  /**
   * イベント解除
   * @param event
   * @param listener
   */
  removeEventListner(
    event: "write" | "clear",
    listener: (log: LogItem | null) => void,
  ): void;
}

export interface LogItem {
  message: string;
  type: "log" | "error" | "warn" | "debug";
  time: Date;
  border: boolean;
}

//Loggerの実装
class LoggerImpl implements Logger {
  private listeners: { [key: string]: ((log: LogItem | null) => void)[] } = {};

  _messages: LogItem[] = [];

  get messages(): LogItem[] {
    return this._messages;
  }

  log(message: string, space?: boolean): void {
    const log = this.createLog("log", message, space);
    this.writeLog(log);
  }

  error(message: string): void {
    const log = this.createLog("error", message);
    this.writeLog(log);
  }

  warn(message: string): void {
    const log = this.createLog("warn", message);
    this.writeLog(log);
  }

  debug(message: string): void {
    if (!DEBUG_MODE) return;
    const log = this.createLog("debug", message);
    this.writeLog(log);
  }

  clear(): void {
    this._messages.splice(0);
    if (this.listeners["write"] !== undefined) {
      this.listeners["write"].forEach((listener) => listener(null));
    }
  }

  addEventListner(
    event: "write" | "clear",
    listener: (log: LogItem | null) => void,
  ): void {
    if (this.listeners[event] === undefined) {
      this.listeners[event] = [];
    }

    this.listeners[event].push(listener);
  }

  removeEventListner(
    event: "write" | "clear",
    listener: (log: LogItem | null) => void,
  ): void {
    if (this.listeners[event] === undefined) {
      return;
    }

    this.listeners[event] = this.listeners[event].filter((l) => l !== listener);
  }

  private createLog(
    type: "log" | "error" | "warn" | "debug",
    message: string,
    space?: boolean,
  ): LogItem {
    return {
      message,
      type,
      time: new Date(),
      border: space ?? false,
    };
  }

  private writeLog(log: LogItem): void {
    if (this.listeners["write"] !== undefined) {
      this.listeners["write"].forEach((listener) => listener(log));
    }
    this._messages.push(log);
  }
}

export const Logger: Logger = new LoggerImpl();
