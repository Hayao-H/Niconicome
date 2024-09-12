import React from "https://esm.sh/react@18.2.0";
import { PlaylistVideo } from "../../jsWatchInfo/videoInfo.ts";
import { PlaylistItem } from "./playlistItem.tsx";

export const Playlist = (
  { videos, baseURL }: { videos: PlaylistVideo[]; baseURL: string },
) => {
  return (
    <div className="playlist">
      {videos.map((video) => <PlaylistItem video={video} baseURL={baseURL} />)}
    </div>
  );
};
