using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Xml.API.Obsoleted
{
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", ElementName = "nicovideo_thumb_response", IsNullable = false)]
    public class GetThumbInfoApiResponse
    {
        [XmlAttribute("status")]
        public string Status { get; set; } = "ok";

        [XmlElement(ElementName = "thumb")]
        public Thumb Thumb { get; set; } = new();
    }

    [XmlRoot(ElementName = "tag")]
    public class Tag
    {
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "tags")]
    public class Tags
    {
        [XmlElement(ElementName = "tag")]
        public List<Tag> Tag { get; set; } = new();
    }

    [XmlRoot(ElementName = "thumb")]
    public class Thumb
    {

        [XmlElement(ElementName = "video_id")]
        public string VideoId { get; set; } = string.Empty;

        [XmlElement(ElementName = "title")]
        public string Title { get; set; } = string.Empty;

        [XmlElement(ElementName = "description")]
        public string Description { get; set; } = string.Empty;

        [XmlElement(ElementName = "thumbnail_url")]
        public string ThumbnailUrl { get; set; } = string.Empty;

        [XmlElement(ElementName = "first_retrieve")]
        public string FirstRetrieve { get; set; } = string.Empty;

        [XmlElement(ElementName = "length")]
        public string Length { get; set; } = string.Empty;

        [XmlElement(ElementName = "view_counter")]
        public long ViewCounter { get; set; }

        [XmlElement(ElementName = "comment_num")]
        public long CommentNum { get; set; }

        [XmlElement(ElementName ="like_counter")]
        public long LikeCounter { get; set; }

        [XmlElement(ElementName = "mylist_counter")]
        public long MylistCounter { get; set; }

        [XmlElement(ElementName = "watch_url")]
        public string WatchUrl { get; set; } = string.Empty;

        [XmlElement(ElementName = "tags")]
        public Tags Tags { get; set; } = new();

        [XmlElement(ElementName = "user_id", IsNullable =false)]
        public string? UserId { get; set; }

        [XmlElement(ElementName = "user_nickname", IsNullable = false)]
        public string? UserNickname { get; set; }

        [XmlElement(ElementName = "ch_id", IsNullable = false)]
        public string? ChId { get; set; }

        [XmlElement(ElementName = "ch_name", IsNullable = false)]
        public string? ChName { get; set; }
    }

}



