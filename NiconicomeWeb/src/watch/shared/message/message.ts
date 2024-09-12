export interface CreateCompleted extends MessageBase {
  isSucceeded: boolean;

  message: string | undefined;
}

export interface MessageBase {
  messageType: "startCreate" | "createCompleted";
}

export interface StartCreate extends MessageBase {
    
  messageType: "startCreate";

  /**
   * 作成URL
   */
  url: string;
}