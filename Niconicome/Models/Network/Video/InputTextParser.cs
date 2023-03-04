using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Network.Video
{
    public interface IInputTextParser
    {
        /// <summary>
        /// 入力された文字列の形式を解析する
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        InputInfomation GetInputInfomation(string inputText);
    }

    public class InputTextParser : IInputTextParser
    {
        #region Method

        public InputInfomation GetInputInfomation(string inputText)
        {
            InputType type = this.GetInputType(inputText);
            RemoteType remoteType = RemoteType.None;

            if (type == InputType.RemotePlaylist)
            {
                remoteType = this.GetRemoteType(inputText);
                inputText = this.GetID(inputText, remoteType);

                if (remoteType == RemoteType.WatchPage)
                {
                    type = InputType.NiconicoID;
                    remoteType = RemoteType.None;
                }
            }

            return new InputInfomation(inputText, type, remoteType, type == InputType.RemotePlaylist);
        }


        #endregion

        #region private

        public InputType GetInputType(string inputText)
        {
            if (Regex.IsMatch(inputText, @"^(sm|so|nm)?\d+$"))
            {
                return InputType.NiconicoID;
            }
            else if (Path.IsPathRooted(inputText))
            {
                return InputType.LocalVideo;
            }
            else
            {
                return InputType.RemotePlaylist;
            }
        }

        public RemoteType GetRemoteType(string inputText)
        {
            if (Regex.IsMatch(inputText, @"^https?://www\.nicovideo\.jp/watch/(sm|so|nm)?\d+.*$"))
            {
                return RemoteType.WatchPage;
            }
            else if (Regex.IsMatch(inputText, @"^https?://(www\.)?ch\.nicovideo\.jp.*"))
            {
                return RemoteType.Channel;
            }
            else if (Regex.IsMatch(inputText, @"^https?://(www\.)?nicovideo\.jp/user/\d+/mylist/\d+.*"))
            {
                return RemoteType.Mylist;
            }
            else if (Regex.IsMatch(inputText, @"^https?://(www\.)?nicovideo\.jp/my/watchlater.*"))
            {
                return RemoteType.WatchLater;
            }
            else if (Regex.IsMatch(inputText, @"^https?://(www\.)?nicovideo\.jp/user/\d+/video.*"))
            {
                return RemoteType.UserVideos;
            }
            else if (Regex.IsMatch(inputText, @"https?://(www\.)?nicovideo\.jp/series/\d+.*"))
            {
                return RemoteType.Series;
            }
            else
            {
                return RemoteType.SearchResult;
            }
        }

        private string GetID(string inputText, RemoteType remoteType)
        {
            if (remoteType == RemoteType.SearchResult)
            {
                return inputText;
            }
            else if (remoteType == RemoteType.UserVideos)
            {
                return inputText.Split("/")[^2];
            }
            else
            {
                inputText = Regex.Replace(inputText, @"\?.+$", "");
                return inputText.Split("/")[^1];
            }
        }

        #endregion
    }

    public enum InputType
    {
        None,
        NiconicoID,
        LocalVideo,
        RemotePlaylist,
    }

    public enum RemoteType
    {
        None,
        SearchResult,
        WatchPage,
        Mylist,
        Series,
        WatchLater,
        UserVideos,
        Channel
    }

    public record InputInfomation(string Parameter, InputType InputType, RemoteType RemoteType, bool IsRemote);

}
