export interface Thread {
    ID: number;
    Fork: number;
    IsActive: boolean;
    IsDefaultPostTarget: boolean;
    IsEasyCommentPostTarget: boolean;
    IsLeafRequired: boolean;
    IsOwnerThread: boolean;
    IsThreadkeyRequired: boolean;
    Threadkey: string?;
    Is184Forced: boolean;
    HasNicoscript: boolean;
    Label: string;
    PostkeyStatus: number;
    Server: string;
}