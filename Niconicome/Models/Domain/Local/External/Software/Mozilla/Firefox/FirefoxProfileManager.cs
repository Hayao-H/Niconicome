using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox
{
    public interface IFirefoxProfileManager
    {
        /// <summary>
        /// プロファイルの存在を確認
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasProfile(string name);

        /// <summary>
        /// すべてのプロファイルを取得
        /// </summary>
        /// <returns></returns>
        IEnumerable<IFirefoxProfileInfo> GetAllProfiles();

        /// <summary>
        /// プロファイル情報を取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFirefoxProfileInfo GetProfile(string name);

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        IAttemptResult Initialize();

        /// <summary>
        /// 初期化フラグ
        /// </summary>
        bool IsInitialized { get; }
    }

    public class FirefoxProfileManager : IFirefoxProfileManager
    {
        public FirefoxProfileManager(INicoDirectoryIO directoryIO, ILogger logger)
        {
            this._directoryIO = directoryIO;
            this._logger = logger;
        }

        #region field

        protected readonly INicoDirectoryIO _directoryIO;

        protected string? profileFolder;

        protected readonly ILogger _logger;

        protected bool isInitialized;

        #endregion

        public bool IsInitialized => this.isInitialized;

        public bool HasProfile(string name)
        {
            var profiles = this._directoryIO.GetDirectorys(this.profileFolder!);
            return profiles.Any(p => p.EndsWith(name));
        }

        public IEnumerable<IFirefoxProfileInfo> GetAllProfiles()
        {
            if (!this._directoryIO.Exists(this.profileFolder!)) return Enumerable.Empty<IFirefoxProfileInfo>();
            var profiles = this._directoryIO.GetDirectorys(this.profileFolder!);
            var infos = profiles.Select(p =>
            {
                string dirName = Path.GetFileName(p);
                return new FirefoxProfileInfo()
                {
                    ProfilePath = p,
                    ProfileName = dirName[(dirName.IndexOf('.') + 1)..],
                };
            });

            return infos;
        }

        public IFirefoxProfileInfo GetProfile(string name)
        {
            if (!this.HasProfile(name)) throw new InvalidOperationException($"{name}というプロファイルは存在しません。");

            var profiles = this._directoryIO.GetDirectorys(this.profileFolder!);
            var p = profiles.First(p => p.EndsWith(name));

            return new FirefoxProfileInfo()
            {
                ProfilePath = Path.Combine(this.profileFolder!, p),
                ProfileName = p[(p.IndexOf('.') + 1)..],
            };
        }

        public virtual IAttemptResult Initialize()
        {
            if (this.isInitialized) return AttemptResult.Succeeded();

            string defaultProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Mozilla\Firefox\Profiles");

            if (string.IsNullOrEmpty(defaultProfilePath))
            {
                return AttemptResult.Fail("プロファイルパスの生成に失敗しました。");
            }

            if (this._directoryIO.Exists(defaultProfilePath))
            {
                this.profileFolder = defaultProfilePath;
                this.isInitialized = true;
                return AttemptResult.Succeeded();
            } else
            {
                return AttemptResult.Fail("プロファイルフォルダーが存在しません。");
            }

        }



    }
}
