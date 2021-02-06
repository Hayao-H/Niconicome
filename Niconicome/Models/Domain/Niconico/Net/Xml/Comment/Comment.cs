
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Xml.Comment
{


    [XmlRoot(ElementName = "thread")]
    public class PacketThread
    {

        [XmlAttribute(AttributeName = "resultcode")]
        public long Resultcode { get; set; }

        [XmlAttribute(AttributeName = "thread")]
        public string Thread { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "server_time")]
        public long ServerTime { get; set; }

        [XmlAttribute(AttributeName = "last_res")]
        public long LastRes { get; set; }

        [XmlAttribute(AttributeName = "ticket")]
        public string? Ticket { get; set; }

        [XmlAttribute(AttributeName = "revision")]
        public long Revision { get; set; }
    }

    [XmlRoot(ElementName = "chat")]
    public class PacketChat
    {

        [XmlAttribute(AttributeName = "thread")]
        public string Thread { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "no")]
        public long No { get; set; }

        [XmlAttribute(AttributeName = "vpos")]
        public long Vpos { get; set; }

        [XmlAttribute(AttributeName = "date")]
        public long Date { get; set; }

        [XmlAttribute(AttributeName = "date_usec")]
        public int DateUsec { get; set; }

        [XmlAttribute(AttributeName = "anonymity")]
        public long Anonymity { get; set; }

        [XmlAttribute(AttributeName = "user_id")]
        public string? UserId { get; set; }

        [XmlAttribute(AttributeName = "mail")]
        public string? Mail { get; set; }

        [XmlText]
        public string? Text { get; set; }

        [XmlAttribute(AttributeName = "leaf")]
        public int Leaf { get; set; }

        [XmlAttribute(AttributeName = "premium")]
        public int Premium { get; set; }

        [XmlAttribute(AttributeName = "score")]
        public int Score { get; set; }
    }

    [XmlRoot(ElementName = "packet")]
    public class Packet
    {

        [XmlElement(ElementName = "thread")]
        public PacketThread Thread { get; set; } = new();

        [XmlElement(ElementName = "chat")]
        public List<PacketChat> Chat { get; set; } = new();
    }




}
