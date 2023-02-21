using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Xml.Comment.V2
{
    [XmlRoot("packet")]
    public class PacketElement
    {

        [XmlElement("chat")]
        public List<ChatElement> Chat { get; set; } = new();
    }
}
