using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Json.WatchPage
{
    public partial class DataApiData
    {
        [JsonPropertyName("video")]
        public Video Video { get; set; } = new();

        [JsonPropertyName("commentComposite")]
        public CommentComposite CommentComposite { get; set; } = new();

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; } = new();

        [JsonPropertyName("owner")]
        public User Owner { get; set; } = new();

        [JsonPropertyName("context")]
        public Context Context { get; set; } = new();
    }

    public partial class CommentComposite
    {
        [JsonPropertyName("threads")]
        public List<CommentThread> Threads { get; set; } = new();
    }

    public partial class CommentThread
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("fork")]
        public int Fork { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("postkeyStatus")]
        public int PostkeyStatus { get; set; }

        [JsonPropertyName("isDefaultPostTarget")]
        public bool IsDefaultPostTarget { get; set; }

        [JsonPropertyName("isThreadkeyRequired")]
        public bool IsThreadkeyRequired { get; set; }

        [JsonPropertyName("isLeafRequired")]
        public bool IsLeafRequired { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("isOwnerThread")]
        public bool IsOwnerThread { get; set; }

        [JsonPropertyName("hasNicoscript")]
        public bool HasNicoscript { get; set; }
    }

    public partial class Context
    {
        [JsonPropertyName("isNeedPayment")]
        public bool IsNeedPayment { get; set; }

        [JsonPropertyName("isPremiumOnly")]
        public bool IsPremiumOnly { get; set; }

        [JsonPropertyName("csrfToken")]
        public string? CsrfToken { get; set; }

        [JsonPropertyName("userkey")]
        public string? Userkey { get; set; }

        [JsonPropertyName("isAuthenticationRequired")]
        public bool IsAuthenticationRequired { get; set; }

        [JsonPropertyName("isPeakTime")]
        public bool? IsPeakTime { get; set; }
    }

    public partial class Tag
    {

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public partial class Video
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("originalTitle")]
        public string? OriginalTitle { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("thumbnailURL")]
        public string? ThumbnailUrl { get; set; }

        [JsonPropertyName("largeThumbnailURL")]
        public string? LargeThumbnailUrl { get; set; }

        [JsonPropertyName("postedDateTime")]
        public string? PostedDateTime { get; set; }

        [JsonPropertyName("viewCount")]
        public int ViewCount { get; set; }

        [JsonPropertyName("dmcInfo")]
        public DmcInfo DmcInfo { get; set; } = new();

        [JsonPropertyName("isOfficialAnime")]
        public int? IsOfficialAnime { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        [JsonPropertyName("isOfficial")]
        public bool IsOfficial { get; set; }
    }

    public partial class DmcInfo
    {

        [JsonPropertyName("user")]
        public User User { get; set; } = new();

        [JsonPropertyName("encryption")]
        public Encryption? Encryption { get; set; }

        [JsonPropertyName("session_api")]
        public SessionApi SessionApi { get; set; } = new();
    }

    public partial class Encryption
    {
        [JsonPropertyName("hls_encryption_v1")]
        public HlsEncryptionV1 HlsEncryptionV1 { get; set; } = new();
    }

    public partial class HlsEncryptionV1
    {
    }

    public partial class SessionApi
    {
        [JsonPropertyName("recipe_id")]
        public string? RecipeId { get; set; }

        [JsonPropertyName("player_id")]
        public string? PlayerId { get; set; }

        [JsonPropertyName("videos")]
        public List<string> Videos { get; set; } = new();

        [JsonPropertyName("audios")]
        public List<string> Audios { get; set; } = new();

        [JsonPropertyName("auth_types")]
        public SessionApiAuthTypes AuthTypes { get; set; } = new();

        [JsonPropertyName("service_user_id")]
        public string? ServiceUserId { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("signature")]
        public string? Signature { get; set; }

        [JsonPropertyName("content_id")]
        public string? ContentId { get; set; }

        [JsonPropertyName("heartbeat_lifetime")]
        public int HeartbeatLifetime { get; set; }

        [JsonPropertyName("content_key_timeout")]
        public int ContentKeyTimeout { get; set; }

        [JsonPropertyName("priority")]
        public float Priority { get; set; }
    }

    public partial class SessionApiAuthTypes
    {
        [JsonPropertyName("hls")]
        public string? Hls { get; set; }

        [JsonPropertyName("http")]
        public string? Http { get; set; }
    }

    public partial class User
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }
    }

}
