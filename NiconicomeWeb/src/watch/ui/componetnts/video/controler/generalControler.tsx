import React, { useContext, useState } from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../../state/videoState.ts";

export const GeneralControler = () => {
  const { state, dispatch } = useContext(VideoStateContext);
  const [repeat, setRepeat] = useState(state.video?.repeat ?? false);

  function toggleComment() {
    dispatch({
      type: "isCommentVisible",
      payload: !state.isCommentVisible,
    });
  }

  function toggleRepeat() {
    if (state.video === undefined) return;
    state.video.repeat = !state.video.repeat;
    setRepeat(state.video.repeat);
  }

  return (
    <div className="generalControler">
      <p
        className={`repeat ${repeat ? "enable" : ""}`}
        onClick={toggleRepeat}
      >
        <i className="fa-solid fa-repeat fa-lg"></i>
      </p>
      <p
        className={`commentVisibility ${
          state.isCommentVisible ? "visible" : ""
        }`}
        onClick={toggleComment}
      >
        <i className="fa-solid fa-message fa-lg"></i>
      </p>
    </div>
  );
};
