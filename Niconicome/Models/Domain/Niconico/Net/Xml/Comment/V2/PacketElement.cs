using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2
{
    [XmlRoot("packet")]
    internal class PacketElement
    {
        [XmlElement("thread")]
        public ThreadElement Thread { get; set; } = new();

        [XmlElement("chat")]
        public List<ChatElement> Chat { get; set; } = new();
    }
}
