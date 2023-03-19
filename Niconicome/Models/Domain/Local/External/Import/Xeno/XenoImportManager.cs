using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;

namespace Niconicome.Models.Domain.Local.External.Import.Xeno
{
    public interface IXenoImportResult
    {
        int FailedCount { get; set; }
        ITreePlaylistInfo PlaylistInfo { get; set; }
        int SucceededCount { get; set; }
    }

    public interface IXenoImportManager
    {
        IXenoImportResult ImportData(string path);
        bool TryImportData(string path, out IXenoImportResult? result);
    }

    /// <summary>
    /// インポート結果
    /// </summary>
    public class XenoImportResult : IXenoImportResult
    {
        public XenoImportResult(ITreePlaylistInfo playlist)
        {
            this.PlaylistInfo = playlist;
        }

        public int SucceededCount { get; set; }

        public int FailedCount { get; set; }

        public ITreePlaylistInfo PlaylistInfo { get; set; }
    }

    /// <summary>
    /// インポートマネージャー(Modelから直接触る)
    /// </summary>
    public class XenoImportManager : IXenoImportManager
    {
        public XenoImportManager(IXenoRootParser parser, IXenoPlaylistConverter converter, ILogger logger)
        {
            this.parser = parser;
            this.converter = converter;
            this.logger = logger;
        }

        private readonly ILogger logger;

        private readonly IXenoRootParser parser;

        private readonly IXenoPlaylistConverter converter;

        /// <summary>
        /// データをインポートする
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IXenoImportResult ImportData(string path)
        {
            using var fs = new StreamReader(path);
            var parsed = this.parser.ParseText(fs.ReadToEnd());
            var converted = this.converter.ConvertToTreePlaylistInfo(parsed.RootPlaylist);

            return new XenoImportResult(converted) { SucceededCount = parsed.SucceededCount, FailedCount = parsed.FailedCount };
        }

        /// <summary>
        /// データのインポートを試行する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryImportData(string path, out IXenoImportResult? result)
        {
            try
            {
                result = this.ImportData(path);
            }
            catch (Exception e)
            {
                this.logger.Error("Xenoからのインポートに失敗しました。", e);
                result = null;
                return false;
            }

            return true;
        }
    }
}
