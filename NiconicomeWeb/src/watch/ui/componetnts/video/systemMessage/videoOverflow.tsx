import React, {
  useContext,
  useRef,
  useState,
} from "https://esm.sh/react@18.2.0";
import { SystemMessage } from "./systemMessage.tsx";
import { VideoStateContext } from "../../../state/videoState.ts";

type Point = {
  x: number;
  y: number;
};

export const VideoOverflow = () => {
  const [contextOpen, setContextOpen] = useState(false);
  const [position, setPosition] = useState<Point>({ x: 0, y: 0 });
  const { state, dispatch } = useContext(VideoStateContext);
  const ref = useRef<HTMLDivElement>(null);

  function onClick(e: React.MouseEvent<HTMLDivElement, MouseEvent>) {
    if (ref.current === null) return;
    if (e.button !== 2) {
      if (
        !state.isSystemMessageVisible && !state.contextMenu.open &&
        state.video !== undefined
      ) {
        if (state.video.paused) {
          state.video.play();
        } else {
          state.video.pause();
        }
      }

      dispatch({ type: "systemMessage", payload: false });
      return;
    }

    const left = e.clientX;
    const top = e.clientY;

    dispatch({ type: "contextMenu", payload: { open: true, left, top } });
  }

  return (
    <div
      className="videoOverflow"
      onMouseDown={(e) => onClick(e)}
      ref={ref}
    >
      <SystemMessage />
    </div>
  );
};
