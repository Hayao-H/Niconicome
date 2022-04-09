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
        [XmlAnyElement]
        public XmlComment VideoIDComment
        {
            get => new XmlDocument().CreateComment($"BoonSutazioData={this.VideoID}");
            set { }
        }

        [XmlElement("thread")]
        public ThreadElement Thread { get; set; } = new();

        [XmlElement("chat")]
        public List<ChatElement> Chat { get; set; } = new();

        [XmlIgnore]
        public string VideoID { get; init; } = string.Empty;
    }
}
