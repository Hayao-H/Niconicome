import React, {
  RefObject,
  useContext,
  useEffect,
  useRef,
  useState,
} from "https://esm.sh/react@18.2.0";
import { JsWatchInfo } from "../../jsWatchInfo/videoInfo.ts";
import { VideoStateContext } from "../../state/videoState.ts";
import { VideoImpl } from "../../video/videoElement.ts";
import { initialize } from "../../../../videoList/main.ts";
import { VideoComment } from "./videoComment.tsx";
import { VideoOverflow } from "./systemMessage/videoOverflow.tsx";
import { Logger } from "../../state/logger.ts";

export const VideoElement = (
  { videoRef }: {
    videoRef: RefObject<HTMLVideoElement>;
  },
) => {
  const initialized = useRef(false);
  const currentNiconioID = useRef<string>("");
  const { state, dispatch } = useContext(VideoStateContext);

  //非DMSの場合HLSを生成する
  useEffect(() => {
    if (state.jsWatchInfo === undefined) return;
    if (
      currentNiconioID.current === state.jsWatchInfo.video.niconicoID &&
      initialized.current
    ) return;

    let changeSource = false;
    if (
      currentNiconioID.current !== "" &&
      currentNiconioID.current !== state.jsWatchInfo.video.niconicoID
    ) {
      changeSource = true;
    }

    initialized.current = true;
    currentNiconioID.current = state.jsWatchInfo.video.niconicoID;

    if (!changeSource && videoRef.current !== null) {
      const video = new VideoImpl(
        videoRef.current,
      );
      video.Initialize(state.jsWatchInfo)
      .then((result) => {
      if (!result.IsSucceeded) {
        Logger.error(result.Message ?? "動画の読み込みに失敗しました。");
      } else {
        Logger.log(
          `動画を読み込みました。(${state.jsWatchInfo!.media.contentUrl})`,
        );
        video.on("pause", () => {
          dispatch({
            type: "isPlaying",
            payload: false,
          });
        });

        video.on("play", () => {
          dispatch({
            type: "isPlaying",
            payload: true,
          });
        });

        dispatch({
          type: "video",
          payload: video,
        });
      }});
    } else {
      const video = state.video;
      if (video === undefined) return;
      if (videoRef.current === null) return;
      video.Initialize(state.jsWatchInfo, videoRef.current)
      .then((result) => {

      if (!result.IsSucceeded) {
        Logger.error(result.Message ?? "動画の読み込みに失敗しました。");
      } else {
        Logger.log(
          `動画を読み込みました。(${state.jsWatchInfo!.media.contentUrl})`,
        );
      }});
    }
  });

  return (
    <div className="videoWrapper">
      <video
        ref={videoRef}
        id="player"
        className="videoElm"
        poster={state.jsWatchInfo?.thumbnail.contentUrl}
      />
      <VideoComment />
      <VideoOverflow />
    </div>
  );
};
