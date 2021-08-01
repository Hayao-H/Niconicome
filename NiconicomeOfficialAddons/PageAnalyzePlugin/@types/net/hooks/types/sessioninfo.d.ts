export interface SessionInfo {
    RecipeId: string;
    ContentId: string;
    HeartbeatLifetime: number;
    Token: string;
    Signature: string;
    AuthType: string;
    ContentKeyTimeout: number;
    ServiceUserId: string;
    PlayerId: string;
    TransferPriset: string;
    Priority: number;
    Videos: string[]
    Audios: string[]
}