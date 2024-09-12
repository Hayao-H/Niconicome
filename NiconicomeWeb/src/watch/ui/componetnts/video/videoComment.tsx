import React, {
  useContext,
  useEffect,
  useRef,
} from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../state/videoState.ts";
import ComDraw from "../../comment/drawer/commentDrawer.ts";
import { CommentManagerImpl } from "../../comment/manager/commentManager.ts";
import { CommentFetcherObj } from "../../comment/fetch/commentFetcher.ts";
import { useResizeHandled } from "../../hooks/useResize.ts";

export const VideoComment = () => {
  const { state, dispatch } = useContext(VideoStateContext);
  const ref = useRef<HTMLDivElement>(null);
  const fetching = useRef(false);
  const rendered = useRef(false);
  const [resizeHandled, setResizeHandled] = useResizeHandled();

  function sourceChanged(): boolean {
    if (state.jsWatchInfo === undefined) return false;
    if (state.comments === undefined) return true;
    return state.comments.niconicoID !== state.jsWatchInfo.video.niconicoID;
  }

  useEffect(() => {
    if (!sourceChanged() && resizeHandled && rendered.current) return;

    setResizeHandled(true);

    if (state.jsWatchInfo === undefined) return;
    if (state.commentManager !== undefined) {
      state.commentManager.dispose();
    }

    const jsWatchInfo = state.jsWatchInfo;

    if (fetching.current) return;

    if (sourceChanged()) {
      rendered.current = false;
      fetching.current = true;
      const fetcher = CommentFetcherObj;
      fetcher.getComments(jsWatchInfo.comment.contentUrl).then(
        (result) => {
          fetching.current = false;
          if (result.IsSucceeded && result.Data !== null) {
            const filtered = state.ngHandler!.filterNG(result.Data).then(
              (comments) => {
                dispatch({
                  "type": "comments",
                  "payload": {
                    comments: comments,
                    niconicoID: jsWatchInfo.video.niconicoID,
                  },
                });
              },
            );
          } else {
            dispatch({
              "type": "comments",
              "payload": {
                comments: [],
                niconicoID: jsWatchInfo.video.niconicoID,
              },
            });
          }
        },
      );
    }

    if (state.video === undefined || state.comments === undefined) return;
    if (state.comments.niconicoID !== state.jsWatchInfo.video.niconicoID) {
      return;
    }

    if (ref.current === null) return;
    const drawer = new ComDraw(
      "commentCanvas",
      ref.current.clientWidth,
      ref.current.clientHeight,
      {
        autoTickDisabled: true,
      },
    );

    if (state.comments.comments.length > 0) {
      const manager = new CommentManagerImpl(state.video, drawer);
      dispatch({
        type: "commentManager",
        payload: manager,
      });
      manager.load(
        state.comments.comments,
        state.video.duration,
        state.comments.niconicoID,
      );
      if (!state.video.paused) {
        manager.start();
      }
      rendered.current = true;
    }
  });

  if (state.comments === undefined || state.comments.comments.length === 0) {
    return <div className="commentWrapper"></div>;
  }

  return (
    <div className="commentWrapper" ref={ref}>
      <canvas
        id="commentCanvas"
        className={`commentCanvas ${state.isCommentVisible ? "visible" : ""}`}
      >
      </canvas>
    </div>
  );
};
