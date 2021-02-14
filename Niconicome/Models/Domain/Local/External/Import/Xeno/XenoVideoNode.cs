namespace Niconicome.Models.Domain.Local.External.Import.Xeno
{
    public interface IXenoVideoNode
    {
        string? NiconicoId { get; init; }
    }

    public class XenoVideoNode : IXenoVideoNode
    {
        public XenoVideoNode(string stringNode)
        {
            var splitted = stringNode.Split("\t");
            if (splitted.Length >= 10)
            {
                this.NiconicoId = splitted[9];
            }
        }

        public string? NiconicoId { get; init; }
    }
}
