import React, { useContext } from "https://esm.sh/react@18.2.0";
import { PlaylistVideo } from "../../jsWatchInfo/videoInfo.ts";
import { VideoStateContext } from "../../state/videoState.ts";
import { Logger } from "../../state/logger.ts";

export const PlaylistItem = (
  { video, baseURL }: { video: PlaylistVideo; baseURL: string },
) => {
  const { state, dispatch } = useContext(VideoStateContext);

  const uploadedAt = new Date(video.uploadedAt);
  const formattedDate = `${uploadedAt.getFullYear()}/${
    (uploadedAt.getMonth() + 1).toString().padStart(2, "0")
  }/${(uploadedAt.getDate()).toString().padStart(2, "0")} ${
    uploadedAt.getHours().toString().padStart(2, "0")
  }:${uploadedAt.getMinutes().toString().padStart(2, "0")}`;

  let viewcount;
  if (video.viewCount > 10000) {
    viewcount = `${Math.floor(video.viewCount / 1000) / 10}万`;
  } else {
    viewcount = video.viewCount.toString();
  }

  function handleClick(niconicoID: string) {
    globalThis.scrollTo(0, 0);
    Logger.log(`${niconicoID}の読み込みを開始します。`, true);
    const url = `${baseURL}${niconicoID}.json`;
    fetch(url).then((res) => res.json())
      .then((data) => {
        dispatch({ "type": "jsWatchInfo", "payload": data });
        dispatch({ "type": "isPlaying", "payload": false });
      }).catch(() => {
        return;
      });
  }

  return (
    <div
      className="playlistVideo"
      onClick={() => handleClick(video.niconicoID)}
    >
      <div className="thumbnail">
        <img src={video.thumbnailURL} />
      </div>
      <div className="videoInfo">
        <p className="title">
          {video.title}
        </p>
        <p className="other">
          <span>{formattedDate}投稿・{viewcount}回再生</span>
        </p>
      </div>
    </div>
  );
};
