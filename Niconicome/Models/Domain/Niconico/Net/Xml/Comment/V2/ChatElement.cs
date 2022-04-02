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
    public class ChatElement
    {
        [XmlAttribute("thread")]
        public string Thread { get; set; } = string.Empty;

        [XmlAttribute("fork")]
        public int Fork { get; set; }

        [XmlAttribute("no")]
        public int No { get; set; }

        [XmlAttribute("vpos")]
        public int Vpos { get; set; }

        [XmlAttribute("date")]
        public long Date { get; set; }

        [XmlAttribute("date_usec")]
        public long DateUsec { get; set; }

        [XmlAttribute("anonymity")]
        public int Anonymity { get; set; }

        [XmlAttribute("nicoru")]
        public int Nicoru { get; set; }

        [XmlAttribute("user_id")]
        public string? UserId { get; set; }

        [XmlAttribute("mail")]
        public string? Mail { get; set; }

        [XmlText]
        public string? Text { get; set; }

        [XmlAttribute("premium")]
        public int Premium { get; set; }

        [XmlAttribute("score")]
        public int Score { get; set; }

        [XmlAttribute("deleted")]
        public int Deleted { get; set; }
    }
}
