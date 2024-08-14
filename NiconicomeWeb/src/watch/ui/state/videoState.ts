import { createContext } from "https://esm.sh/react@18.2.0";
import { Video } from "../video/videoElement.ts";
import { JsWatchInfo } from "../jsWatchInfo/videoInfo.ts";
import { CommentManager } from "../comment/manager/commentManager.ts";
import { ManagedComment } from "../comment/manager/managedComment.ts";

interface VideoStateData {
  canPlay: boolean;
  isSystemMessageVisible: boolean;
  contextMenu: ContextMenuState;
  isPlaying: boolean;
  isCommentVisible: boolean;
  isShortcutVisible: boolean;
  video: Video | undefined;
  jsWatchInfo: JsWatchInfo | undefined;
  commentManager: CommentManager | undefined;
  comments: CommentData | undefined;
}

export interface VideoState {
  state: VideoStateData;
  dispatch: React.Dispatch<VideoStateAction>;
}

export interface CommentData {
  niconicoID: string;
  comments: ManagedComment[];
}

export interface ContextMenuState {
  open: boolean;
  left: number;
  top: number;
}

export const InitialData: VideoStateData = {
  canPlay: false,
  video: undefined,
  jsWatchInfo: undefined,
  isPlaying: false,
  isCommentVisible: true,
  commentManager: undefined,
  comments: undefined,
  isSystemMessageVisible: false,
  isShortcutVisible: false,
  contextMenu: {
    open: false,
    left: 0,
    top: 0,
  },
};

type VideoStateAction = {
  type:
    | "canPlay"
    | "video"
    | "jsWatchInfo"
    | "isPlaying"
    | "commentManager"
    | "isCommentVisible"
    | "comments"
    | "systemMessage"
    | "contextMenu"
    | "shortcut";
  payload:
    | boolean
    | Video
    | JsWatchInfo
    | CommentManager
    | CommentData
    | ContextMenuState;
};

export const VideoStateContext = createContext<VideoState>({} as VideoState);

export const reduceFunc = (
  state: VideoStateData,
  action: VideoStateAction,
): VideoStateData => {
  switch (action.type) {
    case "canPlay":
      return { ...state, canPlay: action.payload as boolean };
    case "video":
      return { ...state, video: action.payload as Video };
    case "jsWatchInfo":
      return { ...state, jsWatchInfo: action.payload as JsWatchInfo };
    case "isPlaying":
      return { ...state, isPlaying: action.payload as boolean };
    case "commentManager":
      return { ...state, commentManager: action.payload as CommentManager };
    case "isCommentVisible":
      return { ...state, isCommentVisible: action.payload as boolean };
    case "comments":
      return { ...state, comments: action.payload as CommentData };
    case "systemMessage":
      return { ...state, isSystemMessageVisible: action.payload as boolean };
    case "contextMenu":
      return {
        ...state,
        contextMenu: action.payload as ContextMenuState,
      };
    case "shortcut":
      return { ...state, isShortcutVisible: action.payload as boolean };
  }
};
