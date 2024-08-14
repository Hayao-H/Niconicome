import React, { useContext } from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../state/videoState.ts";

export const Shortcut = () => {
  const { state, dispatch } = useContext(VideoStateContext);

  function closeShortcut() {
    dispatch({ type: "shortcut", payload: false });
  }

  return (
    <div className={`shortcut ${state.isShortcutVisible ? "" : "hidden"}`}>
      <div className="header">
        <p>ショートカットキー 一覧</p>
        <p className="closeButton" onClick={closeShortcut}>
          <i className="fa-solid fa-xmark fa-lg"></i>
        </p>
      </div>
      <div className="content">
        <p className="section">動画再生</p>
        <div className="item">
          <p>再生・一時停止</p>
          <p>space / k</p>
        </div>
        <div className="item">
          <p>コメント表示・非表示</p>
          <p>c</p>
        </div>
        <div className="item">
          <p>10秒進める</p>
          <p>→</p>
        </div>
        <div className="item">
          <p>10秒戻す</p>
          <p>←</p>
        </div>
        <div className="item">
          <p>動画の最初に移動</p>
          <p>home</p>
        </div>
      </div>
    </div>
  );
};
