import React, { useEffect} from "https://esm.sh/react@18.2.0";

export const Owner = (
  { userID, userName }: { userID: string; userName: string },
) => {
  const [userIcon, setUserIcon] = React.useState<string>(
    "https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg",
  );

  useEffect(() => {
    const userIDNum = Number.parseInt(userID);
    const userIDDivided = Math.floor(userIDNum / 10000);
    const userIconURL =
      `https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/${userIDDivided}/${userIDNum}.jpg`;

    //fetchをキャンセルするためのシグナル
    const controller = new AbortController();

    fetch(userIconURL, { signal: controller.signal })
      .then((res) => {
        if (res.ok) {
          setUserIcon(userIconURL);
        }
      });

    return () => {
      controller.abort();
    };
  },[userID]);

  return (
    <div className="ownerWrapper">
      <a href={`https://nicovideo.jp/user/${userID}`}><img src={userIcon} className="ownerIcon" /></a>
      <a href={`https://nicovideo.jp/user/${userID}/video`} className="ownerName">{userName}</a>
    </div>
  );
};
