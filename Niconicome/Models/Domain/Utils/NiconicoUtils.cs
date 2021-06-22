using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Search;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;

namespace Niconicome.Models.Domain.Utils
{
    public interface INiconicoUtils
    {
        List<string> GetNiconicoIdsFromText(string source);
        string GetFileName(string format, IDmcInfo dmcInfo, string extension, bool replaceStricted, string? suffix = null);
        string GetFileName(string format, IListVideoInfo video, string extension, bool replaceStricted, string? suffix = null);
        string GetIdFromFIleName(string format, string filenameWithExt);
        string GetIdFromFIleName(string filenameWithExt);
        bool IsNiconicoID(string testString);
        RemoteType GetRemoteType(string url);
        bool IsSearchUrl(string url);
        ISearchQuery GetQueryFromUrl(string url);
        string GetID(string url, RemoteType type);
    }

    public class NiconicoUtils : INiconicoUtils
    {
        /// <summary>
        /// ニコニコ動画のIDを抽出する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public List<string> GetNiconicoIdsFromText(string source)
        {
            var matches = Regex.Matches(source, "(sm|nm|so)?[0-9]+", RegexOptions.Compiled | RegexOptions.Multiline);
            return matches.Select(m => m.Value).Distinct().ToList();
        }

        /// <summary>
        /// セッション情報からファイル名を取得する
        /// </summary>
        /// <param name="format"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string GetFileName(string format, IDmcInfo dmcInfo, string extension, bool replaceStricted, string? suffix = null)
        {
            var info = new VideoInfoForPath()
            {
                Title = dmcInfo.Title,
                OwnerName = dmcInfo.Owner,
                NiconicoID = dmcInfo.Id,
                UploadedOn = dmcInfo.UploadedOn,
                DownloadStartedOn = dmcInfo.DownloadStartedOn,
                OwnerID = dmcInfo.OwnerID.ToString(),
            };

            return this.GetFilenameInternal(format, info, extension, replaceStricted, suffix);
        }

        /// <summary>
        /// 動画情報からファイル名を取得する
        /// </summary>
        /// <param name="format"></param>
        /// <param name="video"></param>
        /// <param name="extension"></param>
        /// <param name="replaceStricted"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public string GetFileName(string format, IListVideoInfo video, string extension, bool replaceStricted, string? suffix = null)
        {
            var info = new VideoInfoForPath()
            {
                Title = video.Title.Value,
                OwnerName = video.OwnerName.Value,
                NiconicoID = video.NiconicoId.Value,
                UploadedOn = video.UploadedOn.Value,
                DownloadStartedOn = DateTime.Now,
                OwnerID = video.OwnerID.Value.ToString(),
            };

            return this.GetFilenameInternal(format, info, extension, replaceStricted, suffix);
        }


        /// <summary>
        /// 日付情報を取得する
        /// </summary>
        /// <param name="format"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string GetDateReplacedString(string format, VideoInfoForPath info)
        {
            if (Regex.IsMatch(format, "^.*<uploadedon:.+>.*$"))
            {
                var match = Regex.Match(format, "<uploadedon:.+>");
                var customFormat = match.Value[12..^1];
                return format.Replace(match.Value, info.UploadedOn.ToString(customFormat));

            }
            else if (Regex.IsMatch(format, "^.*<downloadon:.+>.*$"))
            {
                var match = Regex.Match(format, "<downloadon:.+>");
                var customFormat = match.Value[12..^1];
                return format.Replace(match.Value, info.DownloadStartedOn.ToString(customFormat));

            }
            else
            {
                return format.Replace("<uploadedon>", info.UploadedOn.ToString("yyyy-MM-dd HH-mm-ss"))
                    .Replace("<downloadon>", info.DownloadStartedOn.ToString("yyyy-MM-dd HH-mm-ss"))
                    ;
            }
        }

        /// <summary>
        /// ファイル名からIDを取得する
        /// </summary>
        /// <param name="format"></param>
        /// <param name="filenameWithExt"></param>
        /// <returns></returns>
        public string GetIdFromFIleName(string format, string filenameWithExt)
        {
            string filename = Path.GetFileNameWithoutExtension(filenameWithExt) ?? string.Empty;
            if (filename == string.Empty) throw new InvalidOperationException("ファイル名が空です。");
            if (!format.StartsWith("[<id>]")) throw new InvalidOperationException("<id>から始まらないフォーマットに対する処理は出来ません。");
            int startPoint = filename.IndexOf("[") + 1;
            int endPoint = filename.IndexOf("]");
            if (startPoint == -1 || endPoint == -1) throw new InvalidOperationException("[id]を含まないファイル名です。");
            return filename[startPoint..endPoint];
        }

        /// <summary>
        /// ファイル名からIDを取得する
        /// </summary>
        /// <param name="filenameWithExt"></param>
        /// <returns></returns>
        public string GetIdFromFIleName(string filenameWithExt)
        {
            string filename = Path.GetFileNameWithoutExtension(filenameWithExt) ?? string.Empty;
            if (filename == string.Empty) throw new InvalidOperationException("ファイル名が空です。");

            var match = Regex.Match(filenameWithExt, "(sm|nm|so)?[0-9]+");
            return match.Value;
        }

        /// <summary>
        /// 入力した文字列がニコニコのIDとして正しいかどうかを取得する
        /// </summary>
        /// <param name="testString"></param>
        /// <returns></returns>
        public bool IsNiconicoID(string testString)
        {
            return Regex.IsMatch(testString, "^(sm|nm|so)?[0-9]+$");
        }

        /// <summary>
        /// リモートプレイリストの種別を取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public RemoteType GetRemoteType(string url)
        {
            if (Regex.IsMatch(url, @"^https?://(www\.)?ch\.nicovideo\.jp.*"))
            {
                return RemoteType.Channel;
            }
            else if (Regex.IsMatch(url, @"^https?://(www\.)?nicovideo\.jp/user/\d+/mylist/\d+.*"))
            {
                return RemoteType.Mylist;
            }
            else if (Regex.IsMatch(url, @"^https?://(www\.)?nicovideo\.jp/my/watchlater.*"))
            {
                return RemoteType.WatchLater;
            }
            else if (Regex.IsMatch(url, @"^https?://(www\.)?nicovideo\.jp/user/\d+/video.*"))
            {
                return RemoteType.UserVideos;
            }
            else if (Regex.IsMatch(url, @"^https?://(www\.)?nicovideo\.jp/watch/(sm|nm|so)?\d+.*"))
            {
                return RemoteType.WatchPage;
            }

            throw new InvalidOperationException("ニコニコ動画のURLではありません。");
        }

        /// <summary>
        /// IDを取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetID(string url, RemoteType type)
        {
            var li = url.LastIndexOf("?");
            if (li != -1)
            {
                url = url[0..li];
            }

            if (type is RemoteType.Channel or RemoteType.Mylist or RemoteType.WatchPage)
            {
                var splited = url.Split("/");
                if (!splited[^1].IsNullOrEmpty())
                {
                    return splited[^1];
                }
                else
                {
                    return splited[^2];
                }
            }
            else if (type is RemoteType.UserVideos)
            {

                var splited = url.Split("/");
                if (!splited[^1].IsNullOrEmpty())
                {
                    return splited[^2];
                }
                else
                {
                    return splited[^3];
                }
            }
            else
            {
                throw new InvalidOperationException($"RemoteType:{type}は不正です。");
            }
        }

        public ISearchQuery GetQueryFromUrl(string url)
        {
            //不正なURL
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException($"URLが不正です。(url:{url})");
            }

            //クエリを含まないURL
            if (!url.Contains("?"))
            {
                throw new ArgumentException($"URLが不正です。(url:{url})");
            }

            var result = new SearchQuery();
            var splitted = url.Split("/");

            //検索タイプ
            var typeString = splitted[^1].IsNullOrEmpty() ? splitted[^3] : splitted[^2];
            result.SearchType = typeString switch
            {
                "tag" => SearchType.Tag,
                _ => SearchType.Keyword,
            };

            //検索文字列
            var rawKweyword = splitted[^1].IsNullOrEmpty() ? splitted[^2] : splitted[^1];
            rawKweyword = rawKweyword[0..rawKweyword.IndexOf("?")];
            rawKweyword = HttpUtility.UrlDecode(rawKweyword);
            result.Query = rawKweyword;

            //クエリ解析
            var rawQuery = url[(url.LastIndexOf("?") + 1)..];
            var query = HttpUtility.ParseQueryString(rawQuery);

            //並び替え
            var isAscending = query["order"] == "a";
            var rawSort = query["sort"];
            var sortType = rawSort switch
            {
                "v" => Sort.ViewCount,
                "m" => Sort.MylistCount,
                "n" => Sort.LastCommentTime,
                "r" => Sort.CommentCount,
                "f" => Sort.UploadedTime,
                "l" => Sort.Length,
                _ => throw new ArgumentException($"対応していないソート形式です。(sort:{rawSort})")
            };
            result.SortOption = new SortOption() { Sort = sortType, IsAscending = isAscending };

            return result;



        }

        /// <summary>
        /// 検索URLをテストする
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsSearchUrl(string url)
        {
            //そもそもURLじゃない
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return false;
            }

            return Regex.IsMatch(url, @"https?://(www\.)?nicovideo\.jp/(search|tag)/.*");
        }



        /// <summary>
        /// 内部メソッド
        /// </summary>
        /// <param name="format"></param>
        /// <param name="info"></param>
        /// <param name="extension"></param>
        /// <param name="replaceStricted"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        private string GetFilenameInternal(string format, VideoInfoForPath info, string extension, bool replaceStricted, string? suffix = null)
        {
            string filename = format.Replace("<id>", info.NiconicoID)
             .Replace("<title>", info.Title)
             .Replace("<owner>", info.OwnerName)
             .Replace("<ownerId>", info.OwnerID)
             + suffix
             + extension;

            filename = this.GetDateReplacedString(filename, info);

            if (replaceStricted)
            {
                filename = filename
                    .Replace("/", "／")
                    .Replace(":", "：")
                    .Replace("*", "＊")
                    .Replace("?", "？")
                    .Replace("<", "＜")
                    .Replace(">", "＞")
                    .Replace("|", "｜")
                    .Replace("\"", "”");
            }
            else
            {
                filename = Regex.Replace(filename, @"[/:\*\?\<\>\|""]", "");
            }


            return filename;
        }


    }

    class VideoInfoForPath
    {
        public string Title { get; set; } = string.Empty;

        public DateTime UploadedOn { get; set; }

        public DateTime DownloadStartedOn { get; set; }

        public string OwnerName { get; set; } = string.Empty;

        public string NiconicoID { get; set; } = string.Empty;

        public string OwnerID { get; set; } = string.Empty;
    }
}
