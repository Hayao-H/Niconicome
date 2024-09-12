import React, { useState } from "https://esm.sh/react@18.2.0";

export const Description = ({ description }: { description: string }) => {
  const [isExpanded, setIsExpanded] = useState(false);

  if (!isExpanded){
    description = description.replace(/<.+?>/g,"");
  }

  return (
    <div className="descriptionWrapper">
      <div
        className={`description ${!isExpanded ? "collapsed" : ""}`}
        onClick={() => setIsExpanded(true)}
        dangerouslySetInnerHTML={{ __html: description }}
      >
      </div>
      <span
        className={`expandButton ${!isExpanded ? "collapsed" : ""}`}
        onClick={() => setIsExpanded(!isExpanded)}
      >
        {isExpanded ? "▲閉じる" : "▼続きを読む"}
      </span>
    </div>
  );
};
