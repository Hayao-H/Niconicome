using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2
{
    internal class ThreadElement
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
}
