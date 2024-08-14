import React, { useContext } from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../state/videoState.ts";

export const ContextMenu = () => {
  const { state, dispatch } = useContext(VideoStateContext);

  function onClick(e: React.MouseEvent<HTMLDivElement, MouseEvent>) {
    dispatch({
      type: "contextMenu",
      payload: { open: false, left: 0, top: 0 },
    });
  }

  function openSystemMessage(){
    dispatch({
      type: "contextMenu",
      payload: { open: false, left: 0, top: 0 },
    });
    dispatch({ type: "systemMessage", payload: true });
  }

  function openShortcut(){
    dispatch({
      type: "contextMenu",
      payload: { open: false, left: 0, top: 0 },
    });
    dispatch({ type: "shortcut", payload: true });
  }

  return (
    <div className="contextMenuWrapper" onClick={(e) => onClick(e)}>
      <div
        className={`contextMenu ${state.contextMenu.open ? "" : "hide"}`}
        style={{
          left: `${state.contextMenu.left}px`,
          top: `${state.contextMenu.top}px`,
        }}
      >
        <div className="menuItem" onClick={openSystemMessage}>システムメッセージを開く</div>
        <div className="menuItem" onClick={openShortcut}>ショートカット一覧を開く</div>
      </div>
    </div>
  );
};
