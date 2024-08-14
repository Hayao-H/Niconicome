import { createRoot } from "https://esm.sh/react-dom@18.2.0/client";
import { useReducer, useRef } from "https://esm.sh/react@18.2.0";
import {
  JsWatchInfoHandler,
  JsWatchInfoHandlerImpl,
} from "./jsWatchInfo/watchInfoHandler.ts";
import { JsWatchInfo } from "./jsWatchInfo/videoInfo.ts";
import { VideoElement } from "./componetnts/video/video.tsx";
import React from "https://esm.sh/react@18.2.0";
import { reduceFunc } from "./state/videoState.ts";
import { InitialData } from "./state/videoState.ts";
import { VideoStateContext } from "./state/videoState.ts";
import { VideoInfo } from "./componetnts/video/videoInfo/videoInfo.tsx";
import { Comment } from "./componetnts/comment/comment.tsx";
import { Controler } from "./componetnts/video/controler/controler.tsx";
import { Playlist } from "./componetnts/playlist/playlist.tsx";
import { Logger } from "./state/logger.ts";
import { ContextMenu } from "./componetnts/videoContextMenu/contextMenu.tsx";
import { Shortcut } from "./componetnts/shortcut/shortcut.tsx";

export function main() {
  const element = document.querySelector("#watchApp");

  const handler: JsWatchInfoHandler = new JsWatchInfoHandlerImpl();
  const watchInfo = handler.getData();

  if (element !== null && watchInfo !== null) {
    createRoot(element).render(
      <Main jsWatchInfo={watchInfo} />,
    );
  }
}

const Main = (
  { jsWatchInfo }: {
    jsWatchInfo: JsWatchInfo;
  },
) => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const [state, dispatch] = useReducer(reduceFunc, {
    ...InitialData,
    jsWatchInfo: jsWatchInfo,
  });

  return (
    <VideoStateContext.Provider value={{ state, dispatch }}>
      {state.contextMenu.open ? <ContextMenu /> : undefined}
      <div className="watchWrapper">
        <div className="leftContent">
          <VideoElement videoRef={videoRef} />
          <Controler />
          <VideoInfo />
        </div>
        <div className="rightContent">
          <Comment />
          <Shortcut />
          <Playlist
            videos={jsWatchInfo.playlistVideos}
            baseURL={jsWatchInfo.api.baseUrl}
          />
        </div>
      </div>
    </VideoStateContext.Provider>
  );
};
