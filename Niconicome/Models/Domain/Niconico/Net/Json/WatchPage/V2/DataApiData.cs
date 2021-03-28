using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2
{


    public class Thumbnail
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("smallUrl")]
        public string? SmallUrl { get; set; }

        [JsonPropertyName("middleUrl")]
        public string? MiddleUrl { get; set; }

        [JsonPropertyName("largeUrl")]
        public string? LargeUrl { get; set; }
    }

    public class Channel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("isOfficialAnime")]
        public bool IsOfficialAnime { get; set; }

    }


    public class Thread
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("fork")]
        public int Fork { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("isDefaultPostTarget")]
        public bool IsDefaultPostTarget { get; set; }

        [JsonPropertyName("isEasyCommentPostTarget")]
        public bool IsEasyCommentPostTarget { get; set; }

        [JsonPropertyName("isLeafRequired")]
        public bool IsLeafRequired { get; set; }

        [JsonPropertyName("isOwnerThread")]
        public bool IsOwnerThread { get; set; }

        [JsonPropertyName("isThreadkeyRequired")]
        public bool IsThreadkeyRequired { get; set; }

        [JsonPropertyName("threadkey")]
        public string? Threadkey { get; set; }

        [JsonPropertyName("is184Forced")]
        public bool Is184Forced { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;
    }

    public class Keys
    {
        [JsonPropertyName("userKey")]
        public string UserKey { get; set; } = string.Empty;
    }


    public class Comment
    {

        [JsonPropertyName("threads")]
        public List<Thread> Threads { get; set; } = new();

        [JsonPropertyName("keys")]
        public Keys Keys { get; set; } = new();
    }


    public class Encryption
    {
    }

    public class AuthTypes
    {
        [JsonPropertyName("http")]
        public string Http { get; set; } = string.Empty;
    }


    public class Session
    {

        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; } = string.Empty;

        [JsonPropertyName("videos")]
        public List<string> Videos { get; set; } = new();

        [JsonPropertyName("audios")]
        public List<string> Audios { get; set; } = new();

        [JsonPropertyName("authTypes")]
        public AuthTypes AuthTypes { get; set; } = new();

        [JsonPropertyName("serviceUserId")]
        public string ServiceUserId { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;

        [JsonPropertyName("heartbeatLifetime")]
        public int HeartbeatLifetime { get; set; }

        [JsonPropertyName("contentKeyTimeout")]
        public int ContentKeyTimeout { get; set; }

        [JsonPropertyName("priority")]
        public double Priority { get; set; }

        [JsonPropertyName("transferPresets")]
        public List<string> TransferPrisets { get; set; } = new();
    }

    public class Movie
    {
        [JsonPropertyName("contentId")]
        public string ContentId { get; set; } = string.Empty;

        [JsonPropertyName("session")]
        public Session Session { get; set; } = new();
    }

    public class Delivery
    {
        [JsonPropertyName("recipeId")]
        public string RecipeId { get; set; } = string.Empty;

        [JsonPropertyName("encryption")]
        public Encryption? Encryption { get; set; }

        [JsonPropertyName("movie")]
        public Movie Movie { get; set; } = new();
    }

    public class Media
    {
        [JsonPropertyName("delivery")]
        public Delivery Delivery { get; set; } = new();
    }



    public class Item
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }


    public class Tag
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = new();
    }

    public class Count
    {
        [JsonPropertyName("view")]
        public int View { get; set; }

        [JsonPropertyName("comment")]
        public int Comment { get; set; }

        [JsonPropertyName("mylist")]
        public int Mylist { get; set; }

        [JsonPropertyName("like")]
        public int Like { get; set; }
    }


    public class Video
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public Count Count { get; set; } = new();

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("thumbnail")]
        public Thumbnail Thumbnail { get; set; } = new();

        [JsonPropertyName("registeredAt")]
        public DateTimeOffset RegisteredAt { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("isAuthenticationRequired")]
        public bool IsAuthenticationRequired { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
    public class Owner
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = string.Empty;

        [JsonPropertyName("iconUrl")]
        public string IconUrl { get; set; } = string.Empty;
    }

    public class DataApiData
    {

        [JsonPropertyName("channel")]
        public Channel? Channel { get; set; }

        [JsonPropertyName("comment")]
        public Comment Comment { get; set; } = new();

        [JsonPropertyName("media")]
        public Media? Media { get; set; }

        [JsonPropertyName("owner")]
        public Owner? Owner { get; set; }

        [JsonPropertyName("tag")]
        public Tag Tag { get; set; } = new();

        [JsonPropertyName("video")]
        public Video Video { get; set; } = new();
    }



}
