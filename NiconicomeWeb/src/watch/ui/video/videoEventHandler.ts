import React from "https://esm.sh/react@18.2.0";
import { VideoState } from "../state/videoState.ts";

export function handleEvent(
  e: React.KeyboardEvent<HTMLDivElement>,
  param: VideoState,
): void {
  const { state, dispatch } = param;

  if (state.video === undefined) return;
  console.log(e.key);

  if (e.key === " " || e.key === "k") {
    e.preventDefault();
    state.video.paused ? state.video.play() : state.video.pause();
  } else if (e.key === "c") {
    e.preventDefault();
    dispatch({ type: "isCommentVisible", payload: !state.isCommentVisible });
  } else if (e.key === "ArrowRight" || e.key === "l") {
    e.preventDefault();
    if (state.video.currentTime + 10 < state.video.duration) {
      state.video.currentTime += 10;
    } else {
      state.video.currentTime = state.video.duration;
    }
  } else if (e.key === "ArrowLeft" || e.key === "j") {
    e.preventDefault();
    if (state.video.currentTime - 10 > 0) {
      state.video.currentTime -= 10;
    } else {
      state.video.currentTime = 0;
    }
  } else if (e.key === "Home") {
    e.preventDefault();
    state.video.currentTime = 0;
  }
}
