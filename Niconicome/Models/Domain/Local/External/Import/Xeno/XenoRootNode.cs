using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.External.Import.Xeno
{
    public interface IXenoRootNode
    {
        int Layer { get; }
        string? Title { get; }
        string? ListPath { get; }
        string? FolderPath { get; }
    }

    public class XenoRootNode : IXenoRootNode
    {
        public XenoRootNode(string nodeText)
        {
            foreach (var item in nodeText.Split("\t").Select((content, index) => new { content, index }))
            {
                if (item.index == 0)
                {
                    var result = int.TryParse(item.content, out int layer);
                    if (!result) throw new FormatException($"レイヤーの解析に失敗しました。(content:{item.content})");
                    this.Layer = layer;
                }
                else if (item.index == 3)
                {
                    this.Title = item.content;
                }
                else if (item.index == 4)
                {
                    this.ListPath = item.content;
                }
                else if (item.index == 5)
                {
                    this.FolderPath = item.content;
                }
            }
        }

        public int Layer { get; init; }

        public string? Title { get; init; }

        public string? ListPath { get; init; }

        public string? FolderPath { get; init; }
    }
}
