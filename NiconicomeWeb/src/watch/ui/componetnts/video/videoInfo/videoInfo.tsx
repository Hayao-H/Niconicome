import { JsWatchInfo } from "../../../jsWatchInfo/videoInfo.ts";
import React, { useContext } from "https://esm.sh/react@18.2.0";
import { Title } from "./title.tsx";
import { Owner } from "./owner.tsx";
import { Tags } from "./tag.tsx";
import { Description } from "./description.tsx";
import { MajorInfo } from "./majorInfo.tsx";
import { VideoStateContext } from "../../../state/videoState.ts";

export const VideoInfo = () => {
  const {state, dispatch} = useContext(VideoStateContext);
  const jsWatchInfo = state.jsWatchInfo;

if (jsWatchInfo === undefined) return (<div className="videoInfoWrapper"></div>);

  return (
    <div className="videoInfoWrapper">
      <div className="titleAndOwner">
        <Title title={jsWatchInfo.video.title} />
        <Owner
          userID={jsWatchInfo.video.owner.id}
          userName={jsWatchInfo.video.owner.name}
        />
      </div>
      <MajorInfo uploadedAt={new Date(jsWatchInfo.video.uploadedAt)} count={jsWatchInfo.video.count} niconicoID={jsWatchInfo.video.niconicoID} />
      <Tags tags={jsWatchInfo.video.tags} />
      <Description description={jsWatchInfo.video.description} />
    </div>
  );
};
