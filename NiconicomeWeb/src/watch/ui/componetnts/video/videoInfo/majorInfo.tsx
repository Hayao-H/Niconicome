import React from "https://esm.sh/react@18.2.0";
import { Count } from "../../../jsWatchInfo/videoInfo.ts";
export const MajorInfo = (
  { uploadedAt, count, niconicoID }: {
    uploadedAt: Date;
    count: Count;
    niconicoID: string;
  },
) => {
  //uploadedAtをyyyy/mm/ddの形式に変換
  const formattedDate = `${uploadedAt.getFullYear()}/${
    (uploadedAt.getMonth() + 1).toString().padStart(2, "0")
  }/${(uploadedAt.getDate()).toString().padStart(2, "0")} ${
    uploadedAt.getHours().toString().padStart(2, "0")
  }:${uploadedAt.getMinutes().toString().padStart(2, "0")}`;

  let view;
  if (count.view > 10000) {
    view = `${Math.floor(count.view / 1000) / 10}万`;
  } else {
    view = count.view.toString();
  }

  let comment;
  if (count.comment > 10000) {
    comment = `${Math.floor(count.comment / 1000) / 10}万`;
  } else {
    comment = count.comment.toString();
  }

  let mylist;
  if (count.mylist > 10000) {
    mylist = `${Math.floor(count.mylist / 1000) / 10}万`;
  } else {
    mylist = count.mylist.toString();
  }

  let like;
  if (count.like > 10000) {
    like = `${Math.floor(count.like / 1000) / 10}万`;
  } else {
    like = count.like.toString();
  }

  return (
    <div className="majorInfoWrapper">
      <div className="uploadedAt">
        <span>投稿日: {formattedDate}</span>
      </div>
      <div className="counts">
        <span>
          <i className="icon fa-solid fa-play"></i>
          {view}
        </span>
        <span>
          <i className="icon fa-solid fa-message"></i> {comment}
        </span>
        <span>
          <i className="icon fa-solid fa-folder"></i>
          {mylist}
        </span>
        <span>
          <i className="icon like fa-solid fa-heart"></i> {like}
        </span>
        <a href={`https://nico.ms/${niconicoID}`}>
          <i className="icon fa-solid fa-globe"></i>ニコニコ動画
        </a>
      </div>
    </div>
  );
};
