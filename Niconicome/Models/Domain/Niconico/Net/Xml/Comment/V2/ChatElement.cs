using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2
{
    internal class ChatElement
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
        public int? Premium { get; set; }

        [XmlAttribute(AttributeName = "score")]
        public int Score { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public bool PremiumSpecified => Premium.HasValue;
    }
}
