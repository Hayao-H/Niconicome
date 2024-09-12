export interface JsWatchInfo {
  meta: Meta;
  api: API;
  media: Media;
  comment: Comment;
  thumbnail: Thumbnail;
  video: Video;
  playlistVideos: PlaylistVideo[];
}

export interface Meta {
  status: number;
  message: string;
}

export interface Media {
  isDownloaded: boolean;
  isDMS: boolean;
  contentUrl: string;
  createUrl: string;
}

export interface Comment {
  contentUrl: string;
  commentNGAPIBaseUrl: string;
}

export interface Thumbnail {
  contentUrl: string;
}

export interface Video extends PlaylistVideo {
  tags: Tag[];
  owner: Owner;
  description: string;
  count: Count;
}

export interface Tag {
  name: string;
  isNicodicExists: boolean;
}

export interface Owner {
  name: string;
  id: string;
}

export interface PlaylistVideo {
  title: string;
  uploadedAt: string;
  niconicoID: string;
  thumbnailURL: string;
  duration: number;
  viewCount: number;
}

export interface Count {
  view: number;
  comment: number;
  mylist: number;
  like: number;
}

export interface API {
  baseUrl: string;
}
