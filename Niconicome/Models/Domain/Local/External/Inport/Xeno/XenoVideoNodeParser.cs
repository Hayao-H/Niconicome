using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Local.External.Inport.Xeno
{

    public interface IXenoVideoParseResult
    {
        int SkippedCount { get; }
        int SucceededCount { get; }
        int FailedCount { get; }
        List<IXenoVideoNode> Videos { get; }
    }

    public interface IXenoVideoNodeParser
    {
        IXenoVideoParseResult Parse(string content);
        IXenoVideoParseResult ParseFromFile(string path);
    }

    /// <summary>
    /// 解析結果
    /// </summary>
    public class XenoVideoParseResult : IXenoVideoParseResult
    {
        public int SucceededCount { get; set; }

        public int SkippedCount { get; set; }

        public int FailedCount { get; set; }

        public List<IXenoVideoNode> Videos { get; init; } = new();

    }

    /// <summary>
    /// 動画データファイルを解析する
    /// </summary>
    public class XenoVideoNodeParser : IXenoVideoNodeParser
    {
        public XenoVideoNodeParser(ILogger logger)
        {
            this.logger = logger;
        }

        private readonly ILogger logger;

        /// <summary>
        /// 文字列を解析する
        /// </summary>
        /// <param name="rawContent"></param>
        /// <returns></returns>
        public IXenoVideoParseResult Parse(string rawContent)
        {
            var result = new XenoVideoParseResult();
            foreach (var line in rawContent.Split(Environment.NewLine).Select((content, index) => new { content, index }).Where(l=>!l.content.IsNullOrEmpty()))
            {
                IXenoVideoNode node;
                try
                {
                    node = new XenoVideoNode(line.content);
                }
                catch (Exception e)
                {
                    this.logger.Error($"行の解析に失敗しました。(行:{line.index + 1}, content: {line.content})", e);
                    result.FailedCount++;
                    continue;
                }

                if (node.NiconicoId is null)
                {
                    result.SkippedCount++;
                    continue;
                }

                result.Videos.Add(node);
                result.SucceededCount++;
            }

            return result;
        }

        /// <summary>
        /// ファイルを開いて解析する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IXenoVideoParseResult ParseFromFile(string path)
        {
            try
            {
                using var fs = new StreamReader(path);
                return this.Parse(fs.ReadToEnd());
            }
            catch (Exception e)
            {
                this.logger.Error("動画情報ファイルのオープンに失敗しました。", e);
                return new XenoVideoParseResult();
            }

        }
    }
}
