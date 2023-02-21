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
        public string Fork { get; set; } = string.Empty;

        [XmlAttribute("no")]
        public int No { get; set; }

        [XmlAttribute("vpos")]
        public int Vpos { get; set; }

        [XmlAttribute("date")]
        public long Date { get; set; }

        [XmlAttribute("date_usec")]
        public int DatUsec { get; set; } = 0;

        [XmlAttribute("user_id")]
        public string UserId { get; set; } = string.Empty;

        [XmlAttribute("mail")]
        public string Mail { get; set; } = string.Empty;

        [XmlText]
        public string Text { get; set; } = string.Empty;

        [XmlAttribute("score")]
        public int Score { get; set; }

        [XmlAttribute("anonymity")]
        public int Anonimity { get; set; } = 1;
    }
}
