using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Watch = Niconicome.Models.Domain.Niconico.Watch;
using System.IO;

namespace Niconicome.Models.Domain.Utils
{
    public interface INiconicoUtils
    {
        List<string> GetNiconicoIdsFromText(string source);
        string GetFileName(string format, Watch::IDmcInfo dmcInfo, string extension, string? suffix = null);
        string GetIdFromFIleName(string format, string filenameWithExt);
        string GetIdFromFIleName(string filenameWithExt);
        bool IsNiconicoID(string testString);

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
        public string GetFileName(string format, Watch::IDmcInfo dmcInfo, string extension, string? suffix = null)
        {
            string filename = format.Replace("<id>", dmcInfo.Id)
                         .Replace("<title>", dmcInfo.Title)
                         .Replace("<uploadedon>", dmcInfo.UploadedOn.ToString("yyyy/MM/dd HH:mm.ss"))
                         .Replace("<owner>", dmcInfo.Owner)
                         .Replace("\"", "")
                         + suffix
                         + extension;
            filename = Regex.Replace(filename, @"[\\/:\*\?\<\>\|""]", "");
            return filename;
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
        public string GetIdFromFIleName( string filenameWithExt)
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


    }
}
