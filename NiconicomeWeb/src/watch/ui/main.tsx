import { createRoot } from "https://esm.sh/react-dom@18.2.0/client";
import {
  JsWatchInfoHandler,
  JsWatchInfoHandlerImpl,
} from "./jsWatchInfo/watchInfoHandler.ts";
import { JsWatchInfo } from "./jsWatchInfo/videoInfo.ts";
import { VideoElement as _VideoElement } from "./componetnts/video/video.tsx";
import React from "https://esm.sh/react@18.2.0";
import {useReducer} from "https://esm.sh/react@18.2.0";
import { reduceFunc } from "./state/videoState.ts";
import { InitialData } from "./state/videoState.ts";
import { VideoStateContext } from "./state/videoState.ts";
import { VideoInfo as _VideoInfo } from "./componetnts/video/videoInfo/videoInfo.tsx";
import { Comment } from "./componetnts/comment/comment.tsx";
import { Controler as _Controler } from "./componetnts/video/controler/controler.tsx";
import { Playlist } from "./componetnts/playlist/playlist.tsx";
import { ContextMenu } from "./componetnts/videoContextMenu/contextMenu.tsx";
import { Shortcut } from "./componetnts/shortcut/shortcut.tsx";
import { NGHandlerImpl } from "./comment/ng/ngHandler.ts";
import { NGDataFetcherImpl } from "./comment/ng/ngDataFethcer.ts";
import { handleEvent as _handleEvent } from "./video/videoEventHandler.ts";
import { LeftContent } from "./leftContent.tsx";
import { Logger } from "./state/logger.ts";

declare global {
  // deno-lint-ignore no-var
  var DEBUG_MODE: boolean;
}

export function main() {
  Logger.clear();
  if (DEBUG_MODE){
    Logger.log("DEBUG MODE");
  }

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
  const [state, dispatch] = useReducer(reduceFunc, {
    ...InitialData,
    jsWatchInfo: jsWatchInfo,
    ngHandler: new NGHandlerImpl(
      new NGDataFetcherImpl(jsWatchInfo.comment.commentNGAPIBaseUrl),
    ),
  });

  return (
    <VideoStateContext.Provider value={{ state, dispatch }}>
      {state.contextMenu.open ? <ContextMenu /> : undefined}
      <div className="watchWrapper">
        <LeftContent />
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
