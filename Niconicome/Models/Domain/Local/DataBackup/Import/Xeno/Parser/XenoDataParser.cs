using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using Niconicome.Extensions.System.Collections.Generic;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Error;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.StringContent;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Type;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Parser
{
    public interface IXenoDataParser
    {
        /// <summary>
        /// データを解析
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        IAttemptResult<IEnumerable<IXenoPlaylist>> ParseData(string rootPath);
    }

    public class XenoDataParser : IXenoDataParser
    {
        public XenoDataParser(INiconicomeFileIO fileIO,IErrorHandler errorHandler,IStringHandler stringHandler)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        #endregion
        public IAttemptResult<IEnumerable<IXenoPlaylist>> ParseData(string rootPath)
        {
            if (!this._fileIO.Exists(rootPath))
            {
                this._errorHandler.HandleError(XenoDataParserError.FileDoesNotExist, rootPath);
                return AttemptResult<IEnumerable<IXenoPlaylist>>.Fail(this._errorHandler.GetMessageForResult(XenoDataParserError.FileDoesNotExist, rootPath));
            }

            IAttemptResult<string> readResult = this._fileIO.Read(rootPath);
            if (!readResult.IsSucceeded||readResult.Data is null)
            {
                return AttemptResult<IEnumerable<IXenoPlaylist>>.Fail(readResult.Message);
            }

            IAttemptResult<IEnumerable<XenoRootNode>> rootResult = this.ParseRoot(readResult.Data);
            if (!rootResult.IsSucceeded||rootResult.Data is null)
            {
                return AttemptResult<IEnumerable<IXenoPlaylist>>.Fail(rootResult.Message);
            }

            return this.GetPlaylistsFromRootNodes(rootResult.Data);
        }

        #region private

        /// <summary>
        /// ルート解析情報からプレイリストを取得
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private IAttemptResult<IEnumerable<IXenoPlaylist>> GetPlaylistsFromRootNodes(IEnumerable<XenoRootNode> nodes)
        {
            var data = new List<IXenoPlaylist>();
            var parents = new Dictionary<int, IXenoPlaylist>();

            int prevLayer = 0;

            foreach (var node in nodes)
            {
                if (!this.CheckWhetherValidLayer(prevLayer, node.Layer))
                {
                    this._errorHandler.HandleError(XenoDataParserError.FailedToConvertRootNodeToPlaylist, prevLayer, node.Layer);
                    return AttemptResult<IEnumerable<IXenoPlaylist>>.Fail(this._errorHandler.GetMessageForResult(XenoDataParserError.FailedToConvertRootNodeToPlaylist, prevLayer, node.Layer));
                }

                IXenoPlaylist playlist;

                if (Regex.IsMatch(node.PlaylistInfo, @"https?://ch.nicovideo.jp/.+"))
                {
                    playlist = new XenoPlaylist(node.Title, node.FolderPath, this.GetChannelID(node.PlaylistInfo));
                }
                else
                {
                    playlist = new XenoPlaylist(node.Title, node.FolderPath);
                }
                data.Add(playlist);

                if (node.Layer == 1)
                {
                    prevLayer = 1;
                    parents.Clear();
                    parents.Add(1, playlist);
                }

                if (node.Layer == prevLayer)
                {
                    parents[node.Layer - 1].Children.Add(playlist.ID);
                }

                if (node.Layer == prevLayer + 1)
                {
                    parents[prevLayer].Children.Add(playlist.ID);
                    parents.AddOrSet(node.Layer, playlist);
                    prevLayer = node.Layer;
                }

                if (node.Layer < prevLayer)
                {
                    parents[node.Layer - 1].Children.Add(playlist.ID);
                }

                //動画追加処理

                if (playlist.IsChannel)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(node.PlaylistInfo))
                {
                    continue;
                }

                IAttemptResult<IEnumerable<IXenoVideo>> vResult = this.GetVideosFromPlaylistInfo(node.PlaylistInfo);
                if (vResult.IsSucceeded && vResult.Data is not null)
                {
                    playlist.Videos.AddRange(vResult.Data);
                }
            }

            return AttemptResult<IEnumerable<IXenoPlaylist>>.Succeeded(data);
        }

        /// <summary>
        /// レイヤーチェック
        /// </summary>
        /// <param name="prevLayer"></param>
        /// <param name="currentLayer"></param>
        /// <returns></returns>
        private bool CheckWhetherValidLayer(int prevLayer, int currentLayer)
        {
            if (currentLayer == 1)
            {
                return true;
            }

            if (currentLayer == prevLayer)
            {
                return true;
            }

            if (currentLayer == prevLayer + 1)
            {
                return true;
            }

            if (currentLayer < prevLayer)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// チャンネルIDを取得
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetChannelID(string content)
        {
            if (content.EndsWith('/'))
            {
                return content.Split('/')[^2];
            }
            else
            {
                return content.Split('/').Last();
            }
        }

        /// <summary>
        /// 動画一覧を取得
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IAttemptResult<IEnumerable<IXenoVideo>> GetVideosFromPlaylistInfo(string content)
        {
            var data = new List<IXenoVideo>();

            if (!this._fileIO.Exists(content))
            {
                return AttemptResult<IEnumerable<IXenoVideo>>.Succeeded(data);
            }

            IAttemptResult<string> readResult = this._fileIO.Read(content);
            if (!readResult.IsSucceeded || readResult.Data is null)
            {
                return AttemptResult<IEnumerable<IXenoVideo>>.Fail(readResult.Message);
            }

            foreach (var line in readResult.Data.Split(Environment.NewLine))
            {
                string[] splitted = line.Split('\t');

                if (splitted.Length < 5)
                {
                    this._errorHandler.HandleError(XenoDataParserError.FailedToParseLineOfPlaylist, line);
                    return AttemptResult<IEnumerable<IXenoVideo>>.Fail(this._errorHandler.GetMessageForResult(XenoDataParserError.FailedToParseLineOfPlaylist, line));
                }

                string rawTitle = splitted[4];
                if (!Regex.IsMatch(rawTitle, @"\[(sm|nm|so|.+)?\d+\].+"))
                {
                    this._errorHandler.HandleError(XenoDataParserError.FailedToParseVideo, rawTitle);
                    return AttemptResult<IEnumerable<IXenoVideo>>.Fail(this._errorHandler.GetMessageForResult(XenoDataParserError.FailedToParseVideo, rawTitle));
                }

                int bracketIndex = rawTitle.IndexOf(']');
                string niconicoID = rawTitle[1..bracketIndex];
                string title = rawTitle[(bracketIndex + 1)..];

                data.Add(new XenoVideo(niconicoID, title));
            }

            return AttemptResult<IEnumerable<IXenoVideo>>.Succeeded(data);

        }

        /// <summary>
        /// ルートを解析
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private IAttemptResult<IEnumerable<XenoRootNode>> ParseRoot(string content)
        {
            var data = new List<XenoRootNode>();

            foreach (var line in content.Split(Environment.NewLine))
            {
                string[] splitted = line.Split("\t");

                if (splitted.Length < 4)
                {
                    this._errorHandler.HandleError(XenoDataParserError.FailedToParseLineOfRoot, line);
                    return AttemptResult<IEnumerable<XenoRootNode>>.Fail(this._errorHandler.GetMessageForResult(XenoDataParserError.FailedToParseLineOfRoot, line));
                }

                if (!int.TryParse(splitted[0], out var layer))
                {
                    this._errorHandler.HandleError(XenoDataParserError.FailedToParseayer, splitted[0]);
                    return AttemptResult<IEnumerable<XenoRootNode>>.Fail(this._errorHandler.GetMessageForResult(XenoDataParserError.FailedToParseayer, splitted[0]));
                }

                string detailedCOntent = splitted.Length >= 5 ? splitted[4] : string.Empty;
                string folderPath = splitted.Length >= 6 ? splitted[5] : string.Empty;

                data.Add(new XenoRootNode(layer, splitted[3], detailedCOntent, folderPath));
            }

            return AttemptResult<IEnumerable<XenoRootNode>>.Succeeded(data);
        }

        public record XenoRootNode(int Layer, string Title, string PlaylistInfo, string FolderPath);

        #endregion
    }
}
