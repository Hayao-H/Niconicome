import { useContext,  useRef } from "https://esm.sh/react@18.2.0";
import { VideoElement } from "./componetnts/video/video.tsx";
import React from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "./state/videoState.ts";
import { VideoInfo } from "./componetnts/video/videoInfo/videoInfo.tsx";
import { Controler } from "./componetnts/video/controler/controler.tsx";
import { handleEvent } from "./video/videoEventHandler.ts";

export const LeftContent = () => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const { state, dispatch } = useContext(VideoStateContext);

  return (
    <div
      className="leftContent"
      tabIndex={-1}
      onKeyDown={(e) => handleEvent(e, { state, dispatch })}
    >
      <VideoElement videoRef={videoRef} />
      <Controler />
      <VideoInfo />
    </div>
  );
};
