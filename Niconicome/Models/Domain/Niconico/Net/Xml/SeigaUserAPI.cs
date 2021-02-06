using System.Xml.Serialization;

namespace Niconicome.Models.Domain.Niconico.Net.Xml
{

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "response")]
    public partial class Response
    {

        private ResponseUser? userField;

        [XmlElement(ElementName = "user")]
        public ResponseUser User
        {
            get
            {
                return this.userField ?? new ResponseUser();
            }
            set
            {
                this.userField = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ResponseUser
    {

        private uint idField;

        private string? nicknameField;

        [XmlElement(ElementName = "id")]
        public uint Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        [XmlElement(ElementName = "nickname")]
        public string Nickname
        {
            get
            {
                return this.nicknameField ?? string.Empty;
            }
            set
            {
                this.nicknameField = value;
            }
        }
    }


}
