import React, {
  useContext,
  useEffect,
  useRef,
  useState,
} from "https://esm.sh/react@18.2.0";
import { VideoStateContext } from "../../state/videoState.ts";
import { CommentItem } from "./commentItem.tsx";

export const Comment = () => {
  const { state, dispatch } = useContext(VideoStateContext);
  const [isExpanded, setIsExpanded] = useState(true);
  const ref = useRef<HTMLDivElement>(null);
  const currentVideo = useRef("");
  const [isAutoScroll, setIsAutoScroll] = useState(true);
  const lastWheelTime = useRef(Date.now());

  function sourceChanged(): boolean {
    if (currentVideo.current === "") return true;
    if (state.commentManager === undefined) return false;
    return currentVideo.current !== state.commentManager.currentID;
  }

  function onWheel() {
    if (!isAutoScroll) return;

    lastWheelTime.current = Date.now();
    setIsAutoScroll(false);

    setTimeout(() => {
      if (Date.now() - lastWheelTime.current < 10000) {
        return;
      }
      setIsAutoScroll(true);
    }, 10000);
  }

  //コメントを取得
  useEffect(() => {
    if (!sourceChanged()) return;

    if (state.commentManager === undefined) return;
    state.commentManager.on("commentAdded", (comment) => {
      if (!isAutoScroll) return;
      let y = 30 * (comment.innnerIndex + 1 - 12);
      if (y < 0) {
        y = 0;
      }
      ref.current?.scrollTo(0, y);
    });

    currentVideo.current = state.jsWatchInfo?.video.niconicoID ?? "";
  });

  const elements = state.comments?.comments.map((comment) => (
    <CommentItem
      key={comment.number}
      comment={comment}
    />
  ));

  return (
    <div className="commentWrapper">
      <div className="listControler" onClick={() => setIsExpanded(!isExpanded)}>
        <p>コメントリスト</p>
        <p>
          {isExpanded ? "∧" : "∨"}
        </p>
      </div>

      <div
        className={`autoScroll form-check form-switch form-check-reverse ${
          isExpanded ? "" : "collapsed"
        }`}
      >
        <input
          className="form-check-input"
          type="checkbox"
          role="switch"
          id="commentScrollSwitch"
          checked={isAutoScroll}
          onChange={() => setIsAutoScroll(!isAutoScroll)}
        />
        <label className="form-check-label" htmlFor="commentScrollSwitch">
          自動スクロール
        </label>
      </div>

      <div
        className={`${isExpanded ? "commentList" : "commentList collapsed"}`}
        ref={ref}
        onWheel={onWheel}
      >
        <div className="commentItem commentHeader">
          <div className="commentContent">コメント</div>
          <div className="commentTime">再生時間</div>
          <div className="commentNumber">コメ番</div>
          <div className="commentPostedAt">投稿時刻</div>
        </div>
        {elements}
      </div>
    </div>
  );
};
