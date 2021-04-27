using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.State;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Domain.Local.LocalFile
{

    public interface IEncodeutility
    {
        Task EncodeAsync(string inputFilePath, string outputFileName, CancellationToken token, EncodeOptions options = EncodeOptions.Default);
    }

    class Encodeutility : IEncodeutility
    {

        public Encodeutility(ILocalSettingHandler settingHandler, ILocalState localState, ILogger logger)
        {
            this.settingHandler = settingHandler;
            this.localState = localState;
            this.logger = logger;
        }

        private readonly ILocalSettingHandler settingHandler;

        private readonly ILocalState localState;

        private readonly ILogger logger;

        /// <summary>
        /// 非同期でエンコードする
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFileName"></param>
        /// <param name="token"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task EncodeAsync(string inputFilePath, string outputFileName, CancellationToken token, EncodeOptions options = EncodeOptions.Default)
        {
            var errorOutput = new StringBuilder();
            var useShell = this.settingHandler.GetBoolSetting(SettingsEnum.FFmpegShell);

            using var p = new Process();
            p.StartInfo.FileName = useShell ? Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe" : this.GetffmpegPath();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = this.GetCommand(inputFilePath, outputFileName, options, useShell);
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            p.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data is not null) this.logger.Log($"[ffmpeg]{e.Data}");
                errorOutput.AppendLine(e.Data);
            };
            p.OutputDataReceived += (sender, e) =>
            {
                if (e.Data is not null) this.logger.Log($"[ffmpeg]{e.Data}");
                errorOutput.AppendLine(e.Data);
            };

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            await p.WaitForExitAsync(token);

            if (p.ExitCode != 0)
            {
                string errorMsg = Regex.Replace(errorOutput.ToString(), @"^(\n|\r\n?)", "", RegexOptions.Multiline);
                throw new IOException($"ファイルの変換に失敗しました。(詳細: {errorMsg})");
            }
        }

        /// <summary>
        /// コマンドを取得する
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFileName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private string GetCommand(string inputFilePath, string outputFileName, EncodeOptions options, bool useShell)
        {
            var args = new List<string>();
            string ffmpegPath = this.GetffmpegPath();

            if (useShell)
            {
                args.Add("/c");
                args.Add(ffmpegPath);
            }

            args.Add($"-i \"{inputFilePath}\"");
            args.Add("-y");

            if (!this.localState.IsDebugMode)
            {
                args.Add("-loglevel error");
            }

            if (options == EncodeOptions.Copy)
            {
                args.Add("-c:a copy -c:v copy");
            }

            args.Add($"\"{outputFileName}\"");


            return string.Join(" ", args);
        }

        /// <summary>
        /// ffmpegの実行パスを取得する
        /// </summary>
        /// <returns></returns>
        private string GetffmpegPath()
        {

            string defaultPath = Path.Combine(AppContext.BaseDirectory, "bin", "ffmpeg.exe");
            string? ffmpegPath = this.settingHandler.GetStringSetting(SettingsEnum.FfmpegPath);
            var path = string.IsNullOrEmpty(ffmpegPath) ? defaultPath : ffmpegPath;

            return path;
        }
    }

    public enum FileTypes
    {
        Mp4,
        Mp3,
    }

    public enum EncodeOptions
    {
        Default,
        Copy,
        HiighQualityAudio,
    }
}
