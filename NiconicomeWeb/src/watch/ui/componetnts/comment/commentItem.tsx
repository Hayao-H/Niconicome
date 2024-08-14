import React, {
  forwardRef,
  Ref,
  RefObject,
  useContext,
} from "https://esm.sh/react@18.2.0";
import { CommentType } from "../../comment/fetch/comment.ts";
import { VideoStateContext } from "../../state/videoState.ts";

export const CommentItem = (
  { comment }: { comment: CommentType },
) => {
  function setCurrentTime(vposMS: number) {
    if (state.video) state.video.currentTime = vposMS / 1000;
  }
  const { state, dispatch } = useContext(VideoStateContext);

  const postedAt = new Date(comment.postedAt);
  const postedAtString = `${postedAt.getFullYear()}/${
    postedAt.getMonth() + 1
  }/${postedAt.getDate()} ${postedAt.getHours()}:${postedAt.getMinutes()}`;

  const vposMinutes = Math.floor(comment.vposMS / 60000);
  const vposSeconds = Math.floor((comment.vposMS % 60000) / 1000);
  const time = `${vposMinutes.toString().padStart(2, "0")}:${
    vposSeconds.toString().padStart(2, "0")
  }`;

  return (
    <div
      className="commentItem"
      onDoubleClick={() => setCurrentTime(comment.vposMS)}
    >
      <div className="commentContent" title={comment.body}>{comment.body}</div>
      <div className="commentTime">{time}</div>
      <div className="commentNumber">{comment.number}</div>
      <div className="commentPostedAt">{postedAtString}</div>
    </div>
  );
};
