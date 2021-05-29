using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Niconicome.Models.Domain.Local.IO;

namespace Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox
{
    public interface IFirefoxProfileManager
    {
        string ProfileFolder { get; }
        bool HasProfile(string name);
        IEnumerable<IFirefoxProfileInfo> GetAllProfiles();
        IFirefoxProfileInfo GetProfile(string name);
    }

    public class FirefoxProfileManager : IFirefoxProfileManager
    {
        public FirefoxProfileManager(INicoDirectoryIO directoryIO)
        {
            this.directoryIO = directoryIO;
            this.ProfileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Mozilla\Firefox\Profiles");
        }

        #region DIされるクラス
        private readonly INicoDirectoryIO directoryIO;
        #endregion

        /// <summary>
        /// プロファイルフォルダー
        /// </summary>
        public string ProfileFolder { get; init; }

        /// <summary>
        /// /プロファイルが存在するかどうかを確かめる
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasProfile(string name)
        {
            var profiles = this.directoryIO.GetDirectorys(this.ProfileFolder);
            return profiles.Any(p => p.EndsWith(name));
        }

        /// <summary>
        /// すべてのプロファイルを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IFirefoxProfileInfo> GetAllProfiles()
        {
            if (!this.directoryIO.Exists(this.ProfileFolder)) return Enumerable.Empty<IFirefoxProfileInfo>();
            var profiles = this.directoryIO.GetDirectorys(this.ProfileFolder);
            var infos = profiles.Select(p => new FirefoxProfileInfo()
            {
                ProfilePath = Path.Combine(this.ProfileFolder, p),
                ProfileName = p[(p.IndexOf('.') + 1)..],
            });

            return infos;
        }

        /// <summary>
        /// 指定したプロファイルを取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFirefoxProfileInfo GetProfile(string name)
        {
            if (!this.HasProfile(name)) throw new InvalidOperationException($"{name}というプロファイルは存在しません。");

            var profiles = this.directoryIO.GetDirectorys(this.ProfileFolder);
            var p = profiles.First(p => p.EndsWith(name));

            return new FirefoxProfileInfo()
            {
                ProfilePath = Path.Combine(this.ProfileFolder, p),
                ProfileName = p[(p.IndexOf('.') + 1)..],
            };
        }



    }
}
