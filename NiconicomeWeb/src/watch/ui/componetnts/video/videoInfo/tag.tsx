import React from "https://esm.sh/react@18.2.0";
import { Tag } from "../../../jsWatchInfo/videoInfo.ts";

//タグを描写
//引数はタグの配列
//タグの型は../../jsWatchInfo/videoInfo.tsにあるTag
export const Tags = ({ tags }: { tags: Tag[] }) => {
  return (
    <div className="tagWrapper">
      {tags.map((tag, index) => {
        const className = tag.isNicodicExists ? "nicodic exist" : "nicodic";
        const classNameIcon = tag.isNicodicExists ? "fa-solid fa-book fa-sm" : "fa-solid fa-circle-question fa-sm";

        return (
          <div key={index} className="tag">
            <a
              className="tagName"
              href={`https://nicovideo.jp/tag/${tag.name}`}
            >
              {tag.name}
            </a>
            <a href={`https://dic.nicovideo.jp/a/${tag.name}`} className={className}>
              <i className={classNameIcon}></i>
            </a>
          </div>
        );
      })}
    </div>
  );
};
