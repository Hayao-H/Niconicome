using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Utils;
using LocalFile = Niconicome.Models.Domain.Local.LocalFile;

namespace Niconicome.Models.Domain.Niconico.Download.Video
{

    public interface IVideoEncoader
    {
        Task EncodeAsync(IEncodeSettings settings, IDownloadMessenger messenger, CancellationToken token);
        string Mp4FilePath { get; }
    }

    public interface ITsMerge
    {
        void Merge(IEnumerable<string> sourceFiles, string targetFilePath);
    }

    public interface IEncodeSettings
    {
        string FileName { get; }
        string FolderName { get; }
        string CommandFormat { get; }
        bool IsOverwriteEnable { get; }
        bool IsOverrideDTEnable { get; }
        bool IsNoEncodeEnable { get; }
        DateTime UploadedOn { get; }
        IEnumerable<string> TsFilePaths { get; }
    }

    class VideoEncoader : IVideoEncoader
    {

        public VideoEncoader(ITsMerge tsMerge, LocalFile::IEncodeutility encodeutility)
        {
            this.tsMerge = tsMerge;
            this.encodeutility = encodeutility;
        }

        /// <summary>
        /// tsファイルマージャー
        /// </summary>
        private readonly ITsMerge tsMerge;

        /// <summary>
        /// エンコーダー
        /// </summary>
        private readonly LocalFile::IEncodeutility encodeutility;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string Mp4FilePath { get; private set; } = string.Empty;


        public async Task EncodeAsync(IEncodeSettings settings, IDownloadMessenger messenger, CancellationToken token)
        {
            string tsFolderName = Path.GetDirectoryName(settings.TsFilePaths.First()) ?? string.Empty;
            string targetFilePath = Path.Combine(tsFolderName, "combined.ts");
            string mp4Foldername = this.GetFolderPath(settings.FolderName);
            string mp4Filename = this.GetFilePath(settings.FileName, mp4Foldername, settings.IsOverwriteEnable);

            IOUtils.CreateDirectoryIfNotExist(mp4Foldername, mp4Filename);

            this.Mp4FilePath = mp4Filename;

            if (token.IsCancellationRequested) return;

            messenger.SendMessage("セグメントファイルの結合を開始");
            var e = await Task.Run(() =>
             {
                 Exception? e = null;

                 try
                 {
                     this.tsMerge.Merge(settings.TsFilePaths, targetFilePath);
                 }
                 catch (Exception ex)
                 {
                     return ex;
                 }

                 return e;
             });
            if (e is not null)
            {
                throw new IOException($"セグメントファイルのマージ中にエラーが発生しました。(詳細: {e.Message})");
            }
            messenger.SendMessage("セグメントファイルの結合が完了");

            if (token.IsCancellationRequested) return;

            if (settings.IsNoEncodeEnable)
            {
                messenger.SendMessage("動画ファイルをコピー中");
                File.Copy(targetFilePath, mp4Filename);
                messenger.SendMessage("動画ファイルのコピーが完了");
            }
            else
            {
                messenger.SendMessage("ffmpegで変換を開始(.ts=>.mp4)");
                await this.encodeutility.EncodeAsync(targetFilePath, mp4Filename, settings.CommandFormat, token, LocalFile::EncodeOptions.Copy);
                messenger.SendMessage("ffmpegの変換が完了");
            }

            if (settings.IsOverrideDTEnable)
            {
                try
                {
                    File.SetLastWriteTime(mp4Filename, settings.UploadedOn);
                }
                catch (Exception ex)
                {
                    throw new IOException($"動画ファイルの更新日時書き換え中にエラーが発生しました。(詳細:{ex.Message})");
                }
            }
        }

        /// <summary>
        /// ファイルのパスを取得する
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="folderpath"></param>
        /// <returns></returns>
        private string GetFilePath(string filename, string folderpath, bool overwrite = false)
        {

            string mp4Foldername = this.GetFolderPath(folderpath);
            string path = Path.Combine(mp4Foldername, filename);
            if (!overwrite)
            {
                path = IOUtils.CheclFileExistsAndReturnNewFilename(path);
            }

            return path;
        }

        private string GetFolderPath(string foldername)
        {
            if (Path.IsPathRooted(foldername))
            {
                return foldername;
            }
            else
            {
                return Path.Combine(AppContext.BaseDirectory, foldername);
            }
        }
    }

    class TsMerge : ITsMerge
    {
        public void Merge(IEnumerable<string> sourceFiles, string targetFilePath)
        {
            using var fsTarget = new FileStream(targetFilePath, FileMode.OpenOrCreate);
            foreach (var file in sourceFiles.OrderBy(p =>
            {
                p = Path.GetFileName(p) ?? string.Empty;
                return int.Parse(p.Contains(".") ? p[0..p.LastIndexOf(".")] : p);
            }))
            {
                using var fsSource = new FileStream(file, FileMode.Open);

                this.JoinFile(fsSource, fsTarget);

            }
        }

        private void JoinFile(Stream source, Stream destination)
        {
            int count;
            byte[] buffer = new byte[1024 * 1024 * 10];
            while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
            {

                destination.Write(buffer, 0, count);
            }
        }
    }

    public class EncodeSettings : IEncodeSettings
    {
        public string FileName { get; set; } = string.Empty;

        public string FolderName { get; set; } = string.Empty;

        public string CommandFormat { get; set; } = Format.DefaultFFmpegFormat;


        public bool IsOverwriteEnable { get; set; }

        public bool IsOverrideDTEnable { get; set; }

        public bool IsNoEncodeEnable { get; set; }

        public DateTime UploadedOn { get; set; }

        public IEnumerable<string> TsFilePaths { get; set; } = new List<string>();
    }
}
