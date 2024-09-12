import React, {
  useContext,
  useEffect,
  useState,
} from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../../state/videoState.ts";
import { Slider } from "./slider.tsx";
import { Handle } from "./handle.tsx";

export const SliderWrapper = () => {
  const { state, dispatch } = useContext(VideoStateContext);
  const [timePercentage, setTimePercentage] = useState(0);
  const [bufferPercentage, setBufferPercentage] = useState(0);
  const wrapperRef = React.useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (state.video === undefined || state.jsWatchInfo === undefined) return;
    const video = state.video;
    const duration = state.jsWatchInfo.video.duration;

    const update = () => {
      const time = video.currentTime;
      if (video.buffered.length === 0) return;
      const buffer = video.buffered.end(video.buffered.length - 1);
      setTimePercentage(time / duration);
      setBufferPercentage(buffer / duration);
    };

    video.on("timeupdate", update);
    video.on("progress", update);
    return () => {
      video.off("timeupdate", update);
      video.off("progress", update);
    };
  });

  function handleClicked(e: React.MouseEvent<HTMLDivElement>) {
    if (wrapperRef.current === null || state.video === undefined) return;
    const x = e.clientX - wrapperRef.current.getBoundingClientRect().left;
    const percentage = x / wrapperRef.current.clientWidth;
    const time = state.video.duration * percentage;
    state.video.currentTime = time;
  }

  return (
    <div className="sliderWrapper" ref={wrapperRef} onClick={handleClicked}>
      <Slider percentage={bufferPercentage} className="buffer" />
      <Slider percentage={timePercentage} className="time" />
      <Handle percentage={timePercentage}/>
    </div>
  );
};
