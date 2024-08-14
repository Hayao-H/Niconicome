import React, {
  useContext,
  useEffect,
  useState,
} from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../../state/videoState.ts";

export const PlayControler = () => {
  const { state, dispatch } = useContext(VideoStateContext);

  function switchPlay(){
    if (state.isPlaying) {
      state.video?.pause();
    } else {
        state.video?.play();
    }
  }

  return (
    <div className="playControler">
      <div className="playButton" onClick={() => switchPlay()}>
        <i className={`fa-solid fa-lg ${state.isPlaying ? "fa-pause" : "fa-play"}`}>
        </i>
      </div>
    </div>
  );
};
