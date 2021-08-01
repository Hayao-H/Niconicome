import { SessionInfo } from "./sessioninfo";
import { Thread } from "./thread";
import { Thumbinfo } from "./thumbinfo";

export interface DmcInfo {
    Title: string;
    Id: string;
    Owner: string;
    OwnerID: number;
    Userkey: string;
    UserId: string;
    ChannelID: string;
    ChannelName: string;
    Description: string;
    ViewCount: number;
    CommentCount: number;
    MylistCount: number;
    LikeCount: number;
    Duration: number;
    Tags: string[];
    IsDownloadsble: boolean;
    IsEncrypted: boolean;
    IsOfficial: boolean;
    UploadedOn: Date;
    DownloadStartedOn: Date;
    SessionInfo: SessionInfo;
    CommentThreads: Thread[];
    ThumbInfo: Thumbinfo;
}