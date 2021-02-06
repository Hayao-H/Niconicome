using System;
using System.Text.RegularExpressions;
using System.IO;

namespace Niconicome.Models.Domain.Utils
{
    public static class IOUtils
    {
        /// <summary>
        /// ファイルが存在するか確かめて、新しいファイル名を返す
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="fource"></param>
        /// <returns></returns>
        public static string CheclFileExistsAndReturnNewFilename(string filepath, bool fource = false)
        {
            if (fource || File.Exists(filepath))
            {
                int lastindex = filepath.LastIndexOf(".");
                string? ext = lastindex == -1 ? null : filepath[lastindex..];
                filepath = lastindex == -1 ? filepath : filepath[..lastindex];
                if (Regex.IsMatch(filepath, @"\(\d+\)$"))
                {
                    string currentNum = Regex.Match(filepath, @"\(\d+\)$").Value.Replace("(", "").Replace(")", "");
                    int parsed = int.Parse(currentNum);
                    string newPath = Regex.Replace(filepath, @"\(\d+\)$", "") + $"({parsed + 1})" + ext;
                    return IOUtils.CheclFileExistsAndReturnNewFilename(newPath);
                }
                else
                {
                    string newPath = filepath + "(1)" + ext;
                    return IOUtils.CheclFileExistsAndReturnNewFilename(newPath);
                }
            }
            else
            {
                return filepath;
            }
        }
    }
}
