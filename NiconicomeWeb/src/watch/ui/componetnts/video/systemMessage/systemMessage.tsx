import React, {
  useCallback,
  useContext,
  useEffect,
} from "https://esm.sh/react@18.2.0";
import { Logger, LogItem } from "../../../state/logger.ts";
import "../../../../../shared/Extension/dateExtension.ts";
import { VideoStateContext } from "../../../state/videoState.ts";

export const SystemMessage = () => {
  const [message, setMessage] = React.useState<LogItem[]>(Logger.messages);
  const { state, dispatch } = useContext(VideoStateContext);

  const messageHandler = useCallback((log: LogItem | null) => {
    if (log === null) {
      setMessage([]);
      return;
    }
    setMessage([...message, log]);
  }, []);

  useEffect(() => {
    Logger.addEventListner("write", messageHandler);
    Logger.addEventListner("clear", messageHandler);
    
    return () => {
      Logger.removeEventListner("write", messageHandler);
      Logger.removeEventListner("clear", messageHandler);
    };
  }, []);

  return (
    <div
      className={`systemMessage ${state.isSystemMessageVisible ? "" : "hide"}`}
      onMouseDown={(e) => e.stopPropagation()}
    >
      {message.map((m, i) => (
        <div
          key={i}
          className={`message ${m.type} ${m.border ? "messageBorder" : ""}`}
        >
          <div className="time">[{m.time.Format("yyyy/MM/dd mm:ss:SS")}]</div>
          <div className="message">{m.message}</div>
        </div>
      ))}
    </div>
  );
};
