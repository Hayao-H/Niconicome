using Niconicome.Models.Domain.Local.External.Inport.Xeno;


namespace NiconicomeTest.Stabs.Models.Domain.Local.Extarnal.Inport.Xeno
{
    class XenoVideoNodeParserStab : IXenoVideoNodeParser
    {

        public IXenoVideoParseResult Parse(string content)
        {
            return new XenoVideoParseResult();
        }

        public IXenoVideoParseResult ParseFromFile(string path)
        {
            return new XenoVideoParseResult();
        }
    }

    class XenoVideoNodeStab : IXenoVideoNode
    {
        public XenoVideoNodeStab(string id)
        {
            this.NiconicoId = id;
        }

        public string? NiconicoId { get; init; }
    }
}
