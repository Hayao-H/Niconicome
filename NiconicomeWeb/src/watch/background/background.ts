import {
  CreateCompleted,
  MessageBase,
  StartCreate,
} from "../shared/message/message.ts";

if (self.onmessage === null) {
  self.onmessage = async (e: MessageEvent<string>) => {
    if (e.data === "") {
      return;
    }

    const message = JSON.parse(e.data) as MessageBase;

    if (message.messageType === "startCreate") {
      const url = (message as StartCreate).url;

      const response = await fetch(url, {
        signal: AbortSignal.timeout(30 * 1000),
      });

      if (response.ok) {
        const result: CreateCompleted = {
          isSucceeded: true,
          message: undefined,
          messageType: "createCompleted",
        };
        postMessage(JSON.stringify(result));
      } else {
        const result: CreateCompleted = {
          isSucceeded: false,
          message: await response.text(),
          messageType: "createCompleted",
        };
        postMessage(JSON.stringify(result));
      }
    }
  };
}
