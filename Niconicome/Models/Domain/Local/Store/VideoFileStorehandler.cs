using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Extensions.System.List;
using IO = System.IO;

namespace Niconicome.Models.Domain.Local.Store
{

    public interface IVideoFileStorehandler
    {
        bool Exists(string niconicoId);
        IEnumerable<string> GetFilePaths(string niconicoId);
        string? GetFilePath(string niconicoId);
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
                var data = this.GetFileData(niconicoId);
                return data!.FilePaths.Any(p =>
                {
                    if (!Path.IsPathRooted(p))
                    {
                        p = AppContext.BaseDirectory + p;
                    }
                    return File.Exists(@"\\?\" + p);
                });
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
            return data.FilePaths.Where(p => IO::File.Exists(p));
        }

        /// <summary>
        /// 最初のファイルパスを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        public string? GetFilePath(string niconicoId)
        {
            var data = this.GetFileData(niconicoId);
            return data?.FilePaths.First(p => IO::File.Exists(p));
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
                if (data.FilePaths.Any(p => !IO::File.Exists(p)))
                {
                    data.FilePaths.RemoveAll(p => !IO::File.Exists(p));
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
        }

        /// <summary>
        /// パスを追加する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="filePath"></param>
        public void Add(string niconicoId, string filePath)
        {
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
