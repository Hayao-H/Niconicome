using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Local.External.Inport.Xeno
{
    public interface IXenoParseResult
    {
        int SkippedCount { get; }
        int SucceededCount { get; }
        int FailedCount { get; }
        List<IXenoPlaylist> Playlists { get; }
    }
    public interface IXenoRootParser
    {
        IXenoParseResult ParseText(string text);
    }

    public class XenoParseResult : IXenoParseResult
    {
        public int SucceededCount { get; set; }

        public int SkippedCount { get; set; }

        public int FailedCount { get; set; }

        public List<IXenoPlaylist> Playlists { get; init; } = new();

    }

    public class XenoRootParser : IXenoRootParser
    {
        public XenoRootParser(ILogger logger, IXenoVideoNodeParser videoNodeParser)
        {
            this.logger = logger;
            this.videoNodeParser = videoNodeParser;
        }

        private readonly ILogger logger;

        private readonly IXenoVideoNodeParser videoNodeParser;


        /// <summary>
        /// 一覧を解析する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IXenoParseResult ParseText(string text)
        {
            var result = new XenoParseResult();
            var currentParent = new XenoPlaylist("Root");

            foreach (var line in text.Split(Environment.NewLine).Select((content, index) => new { content, index }))
            {
                IXenoRootNode node;
                try
                {
                    node = new XenoRootNode(line.content);
                }
                catch (Exception e)
                {
                    result.FailedCount++;
                    this.logger.Error($"行の解析に失敗しました。(行:{line.index + 1}, content: {line.content})", e);
                    continue;
                }

                var playlist = this.ConvertToXenoPlaylist(node, currentParent, out bool cResult);
                if (cResult)
                {
                    result.Playlists.Add(playlist);
                    result.SucceededCount++;
                }
                else
                {
                    result.FailedCount++;
                }

            }

            return result;
        }

        /// <summary>
        /// ノード情報をプレイリスト情報に変換する
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private IXenoPlaylist ConvertToXenoPlaylist(IXenoRootNode node, IXenoPlaylist parent, out bool result)
        {
            if (node.Title is null)
            {
                result = false;
                return new XenoPlaylist("Failed");
            }

            if (node.ListPath is not null)
            {
                var listPath = node.ListPath;
                if (Uri.IsWellFormedUriString(listPath, UriKind.Absolute))
                {
                    if (node.ListPath.Contains("ch.nicovideo.jp"))
                    {
                        result = true;
                        var playlist = new XenoPlaylist(node.Title, listPath[(listPath.LastIndexOf("/") + 1)..], parent);
                        return playlist;
                    }
                    else if (File.Exists(node.ListPath))
                    {
                        var vResult = this.videoNodeParser.ParseFromFile(node.ListPath);
                        var playlist = new XenoPlaylist(node.Title, parent);
                        result = true;
                        playlist.Videos.AddRange(vResult.Videos.Select(v => v.NiconicoId!));
                        return playlist;
                    }
                }
            }

            result = true;
            return new XenoPlaylist(node.Title,parent);
        }
    }
}
