using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Utils;
using IO = System.IO;

namespace Niconicome.Models.Domain.Local.Store
{

    public interface IVideoFileStorehandler
    {
        bool Exists(string niconicoId);
        IEnumerable<string> GetFilePaths(string niconicoId);
        string? GetFilePath(string niconicoId);
        string? GetFilePath(string niconicoID, string folderPath);
        void Add(string niconicoId, string filePath);
        void Delete(string niconicoId, string filePath);
        void Clean();
    }

    /// <summary>
    /// 動画ファイルの情報を管理する
    /// </summary>
    public class VideoFileStorehandler : IVideoFileStorehandler
    {

        public VideoFileStorehandler(IDataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        private readonly IDataBase dataBase;

        /// <summary>
        /// 設定を取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        private Types.VideoFile? GetFileData(string niconicoId)
        {
            return this.dataBase.GetRecord<Types.VideoFile>(Types.VideoFile.TableName, v => v.NiconicoId == niconicoId);
        }

        /// <summary>
        /// ファイルが存在するかどうかを確かめる
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        public bool Exists(string niconicoId)
        {
            if (this.dataBase.Exists<Types.VideoFile>(Types.VideoFile.TableName, v => v.NiconicoId == niconicoId))
            {
                return this.GetFilePaths(niconicoId).Any();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ファイルパスのリストを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetFilePaths(string niconicoId)
        {
            var data = this.GetFileData(niconicoId);
            if (data is null) return Enumerable.Empty<string>();
            return data.FilePaths
                .Select(p =>
                IOUtils.GetRootedPath(p)
                )
                .Where(p => IO::File.Exists(IOUtils.GetPrefixedPath(p)));
        }


        /// <summary>
        ///　指定したフォルダーに存在するファイルパスを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public string? GetFilePath(string niconicoId, string foldername)
        {
            var paths = this.GetFilePaths(niconicoId);
            return paths.FirstOrDefault(p =>
                (Path.GetDirectoryName(p) ?? string.Empty) == foldername);
        }

        /// <summary>
        /// 最初のファイルパスを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        public string? GetFilePath(string niconicoId)
        {
            return this.GetFilePaths(niconicoId).FirstOrDefault();
        }

        /// <summary>
        /// パスを削除する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="filePath"></param>
        public void Delete(string niconicoId, string filePath)
        {
            if (!this.Exists(niconicoId)) return;

            var data = this.GetFileData(niconicoId);

            if (data!.FilePaths.Contains(filePath))
            {
                data.FilePaths.Remove(filePath);
                this.dataBase.Update(data, Types.VideoFile.TableName);
            }

        }

        /// <summary>
        /// データを整理する
        /// </summary>
        public void Clean()
        {
            var allData = this.dataBase.GetAllRecords<Types.VideoFile>(Types.VideoFile.TableName);

            foreach (var data in allData)
            {
                var paths = data.FilePaths.Select(p => IOUtils.GetRootedPath(p)).Where(p =>
                {
                    var lp = IOUtils.GetPrefixedPath(p);
                    return File.Exists(p);
                }).Copy();

                data.FilePaths.Clear();
                data.FilePaths.AddRange(paths);

                if (data.FilePaths.Count == 0)
                {
                    this.dataBase.Delete(Types.VideoFile.TableName, data.Id);
                }
                else
                {
                    this.dataBase.Update(data, Types.VideoFile.TableName);
                }
            }
        }

        /// <summary>
        /// パスを追加する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="filePath"></param>
        public void Add(string niconicoId, string filePath)
        {
            filePath = IOUtils.GetRootedPath(filePath);

            if (this.Exists(niconicoId))
            {
                var data = this.GetFileData(niconicoId);
                data!.FilePaths.AddUnique(filePath);
                this.dataBase.Update(data, Types.VideoFile.TableName);
            }
            else
            {
                var data = new Types.VideoFile()
                {
                    NiconicoId = niconicoId,
                };
                data.FilePaths.Add(filePath);
                this.dataBase.Store(data, Types.VideoFile.TableName);
            }
        }
    }
}
