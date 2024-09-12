import React, {
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../../state/videoState.ts";

export const TimeControler = () => {
  const { state, dispatch } = useContext(VideoStateContext);
  const [currentTime, setCurrentTime] = useState("00:00");
  const ref = useRef<HTMLInputElement>(null);

  const duration = useMemo<string>(() => {
    if (state.video === undefined) return "00:00";
    const duration = state.video.duration;
    if (isNaN(duration)) return "00:00";
    if (duration > 3600) {
      const hour = String(Math.floor(duration / 3600)).padStart(2, "0");
      const minute = String(Math.floor(duration % 3600 / 60)).padStart(2, "0");
      const second = String(Math.floor(duration % 60)).padStart(2, "0");
      return `${hour}:${minute}:${second}`;
    } else {
      const minute = String(Math.floor(duration / 60)).padStart(2, "0");
      const second = String(Math.floor(duration % 60)).padStart(2, "0");
      return `${minute}:${second}`;
    }
  }, [state.video?.duration]);

  useEffect(() => {
    if (state.video === undefined) return;
    const video = state.video;
    const onTimeUpdate = () => {
      if (video === undefined) return;
      const currentTime = video.currentTime;

      if (video.duration > 3600) {
        const hour = String(Math.floor(currentTime / 3600)).padStart(2, "0");
        const minute = String(Math.floor(currentTime % 3600 / 60)).padStart(
          2,
          "0",
        );
        const second = String(Math.floor(currentTime % 60)).padStart(2, "0");
        setCurrentTime(`${hour}:${minute}:${second}`);
      } else {
        const minute = String(Math.floor(currentTime / 60)).padStart(2, "0");
        const second = String(Math.floor(currentTime % 60)).padStart(2, "0");
        setCurrentTime(`${minute}:${second}`);
      }
    };
    video.on("timeupdate", onTimeUpdate);
    return () => {
      video.off("timeupdate", onTimeUpdate);
    };
  }, [state.video]);

  function handleSkip(isForward: boolean) {
    if (state.video === undefined) return;
    const video = state.video;
    const currentTime = video.currentTime;
    const skipTime = 10;
    if (isForward) {
      video.currentTime = currentTime + skipTime;
    } else {
      video.currentTime = currentTime - skipTime;
    }
  }

  return (
    <div className="timeControler">
      <p onClick={() => handleSkip(false)}>
        <i className="fa-solid fa-arrow-rotate-left fa-lg"></i>
      </p>
      <p className="time">{currentTime} / {duration}</p>
      <p onClick={() => handleSkip(true)}>
        <i className="fa-solid fa-arrow-rotate-right fa-lg"></i>
      </p>
    </div>
  );
};
