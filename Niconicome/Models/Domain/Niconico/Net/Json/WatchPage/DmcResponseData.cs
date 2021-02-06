using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Response
{


    public partial class DmcResponseData
    {
        [JsonPropertyName("meta")]
        public Meta Meta { get; set; } = new();

        [JsonPropertyName("data")]
        public Data Data { get; set; } = new();
    }

    public partial class Data
    {
        [JsonPropertyName("session")]
        public Session Session { get; set; } = new();
    }

    public partial class Session
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("recipe_id")]
        public string? RecipeId { get; set; }

        [JsonPropertyName("content_id")]
        public string? ContentId { get; set; }

        [JsonPropertyName("content_src_id_sets")]
        public List<ContentSrcIdSet> ContentSrcIdSets { get; set; } = new();

        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }

        [JsonPropertyName("timing_constraint")]
        public string? TimingConstraint { get; set; }

        [JsonPropertyName("keep_method")]
        public KeepMethod KeepMethod { get; set; } = new();

        [JsonPropertyName("protocol")]
        public Protocol Protocol { get; set; } = new();

        [JsonPropertyName("play_seek_time")]
        public int PlaySeekTime { get; set; }

        [JsonPropertyName("play_speed")]
        public float PlaySpeed { get; set; }

        [JsonPropertyName("play_control_range")]
        public PlayControlRange PlayControlRange { get; set; } = new();

        [JsonPropertyName("content_uri")]
        public string? ContentUri { get; set; }

        [JsonPropertyName("session_operation_auth")]
        public SessionOperationAuth SessionOperationAuth { get; set; } = new();

        [JsonPropertyName("content_auth")]
        public ContentAuth ContentAuth { get; set; } = new();

        [JsonPropertyName("runtime_info")]
        public RuntimeInfo RuntimeInfo { get; set; } = new();

        [JsonPropertyName("client_info")]
        public ClientInfo ClientInfo { get; set; } = new();

        [JsonPropertyName("created_time")]
        public long CreatedTime { get; set; }

        [JsonPropertyName("modified_time")]
        public long ModifiedTime { get; set; }

        [JsonPropertyName("priority")]
        public float Priority { get; set; }

        [JsonPropertyName("content_route")]
        public int ContentRoute { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("content_status")]
        public string? ContentStatus { get; set; }
    }

    public partial class ClientInfo
    {
        [JsonPropertyName("player_id")]
        public string? PlayerId { get; set; }

        [JsonPropertyName("remote_ip")]
        public string? RemoteIp { get; set; }

        [JsonPropertyName("tracking_info")]
        public string? TrackingInfo { get; set; }
    }

    public partial class ContentAuth
    {
        [JsonPropertyName("auth_type")]
        public string? AuthType { get; set; }

        [JsonPropertyName("max_content_count")]
        public int MaxContentCount { get; set; }

        [JsonPropertyName("content_key_timeout")]
        public long ContentKeyTimeout { get; set; }

        [JsonPropertyName("service_id")]
        public string? ServiceId { get; set; }

        [JsonPropertyName("service_user_id")]
        public string? ServiceUserId { get; set; }

        [JsonPropertyName("content_auth_info")]
        public ContentAuthInfo ContentAuthInfo { get; set; } = new();
    }

    public partial class ContentAuthInfo
    {
        [JsonPropertyName("method")]
        public string? Method { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public partial class ContentSrcIdSet
    {
        [JsonPropertyName("content_src_ids")]
        public List<ContentSrcId> ContentSrcIds { get; set; } = new();

        [JsonPropertyName("allow_subset")]
        public string? AllowSubset { get; set; }
    }

    public partial class ContentSrcId
    {
        [JsonPropertyName("src_id_to_mux")]
        public SrcIdToMux SrcIdToMux { get; set; } = new();
    }

    public partial class SrcIdToMux
    {
        [JsonPropertyName("video_src_ids")]
        public List<string?> VideoSrcIds { get; set; } = new();

        [JsonPropertyName("audio_src_ids")]
        public List<string?> AudioSrcIds { get; set; } = new();
    }

    public partial class KeepMethod
    {
        [JsonPropertyName("heartbeat")]
        public Heartbeat Heartbeat { get; set; } = new();
    }

    public partial class Heartbeat
    {
        [JsonPropertyName("lifetime")]
        public int Lifetime { get; set; }

        [JsonPropertyName("onetime_token")]
        public string? OnetimeToken { get; set; }

        [JsonPropertyName("deletion_timeout_on_no_stream")]
        public int DeletionTimeoutOnNoStream { get; set; }
    }

    public partial class PlayControlRange
    {
        [JsonPropertyName("max_play_speed")]
        public float MaxPlaySpeed { get; set; }

        [JsonPropertyName("min_play_speed")]
        public float MinPlaySpeed { get; set; }
    }

    public partial class Protocol
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("parameters")]
        public ProtocolParameters Parameters { get; set; } = new();
    }

    public partial class ProtocolParameters
    {
        [JsonPropertyName("http_parameters")]
        public HttpParameters HttpParameters { get; set; } = new();
    }

    public partial class HttpParameters
    {
        [JsonPropertyName("method")]
        public string? Method { get; set; }

        [JsonPropertyName("parameters")]
        public HttpParametersParameters Parameters { get; set; } = new();
    }

    public partial class HttpParametersParameters
    {
        [JsonPropertyName("hls_parameters")]
        public HlsParameters HlsParameters { get; set; } = new();
    }

    public partial class HlsParameters
    {
        [JsonPropertyName("segment_duration")]
        public int SegmentDuration { get; set; }

        [JsonPropertyName("total_duration")]
        public int TotalDuration { get; set; }

        [JsonPropertyName("transfer_preset")]
        public string? TransferPreset { get; set; }

        [JsonPropertyName("use_ssl")]
        public string? UseSsl { get; set; }

        [JsonPropertyName("use_well_known_port")]
        public string? UseWellKnownPort { get; set; }

        [JsonPropertyName("media_segment_format")]
        public string? MediaSegmentFormat { get; set; }

        [JsonPropertyName("encryption")]
        public Encryption Encryption { get; set; } = new();

        [JsonPropertyName("separate_audio_stream")]
        public string? SeparateAudioStream { get; set; }
    }

    public partial class Encryption
    {
        [JsonPropertyName("empty")]
        public Empty Empty { get; set; } = new();
    }

    public partial class Empty
    {
    }

    public partial class RuntimeInfo
    {
        [JsonPropertyName("node_id")]
        public string? NodeId { get; set; }

        [JsonPropertyName("execution_history")]
        public List<object> ExecutionHistory { get; set; } = new();

        [JsonPropertyName("thumbnailer_state")]
        public List<object> ThumbnailerState { get; set; } = new();
    }

    public partial class SessionOperationAuth
    {
        [JsonPropertyName("session_operation_auth_by_signature")]
        public SessionOperationAuthBySignature SessionOperationAuthBySignature { get; set; } = new();
    }

    public partial class SessionOperationAuthBySignature
    {
        [JsonPropertyName("created_time")]
        public long CreatedTime { get; set; }

        [JsonPropertyName("expire_time")]
        public long ExpireTime { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("signature")]
        public string? Signature { get; set; }
    }

    public partial class Meta
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
