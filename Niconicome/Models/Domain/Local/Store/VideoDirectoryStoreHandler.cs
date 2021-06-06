using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Domain.Local.Store
{
    public interface IVideoDirectoryStoreHandler
    {
        int AddDirectory(string path);
        void DeleteDirectory(string path);
        List<STypes.VideoDirectory> GetVideoDirectories();
    }

    public class VideoDirectoryStoreHandler : IVideoDirectoryStoreHandler
    {
        public VideoDirectoryStoreHandler(IDataBase dataBase, ILogger logger)
        {
            this.database = dataBase;
            this.logger = logger;
        }

        #region private
        private readonly IDataBase database;

        private readonly ILogger logger;
        #endregion

        /// <summary>
        /// 全てのディレクトリーを取得する
        /// </summary>
        /// <returns></returns>
        public List<STypes::VideoDirectory> GetVideoDirectories()
        {
            var result = this.database.GetAllRecords<STypes::VideoDirectory>(STypes::VideoDirectory.TableName);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("動画ディレクトリの取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("動画ディレクトリの取得に失敗しました。");
                }

                return new List<STypes::VideoDirectory>();
            }

            return result.Data;
        }

        /// <summary>
        /// ディレクトリーを削除する
        /// </summary>
        /// <param name="path"></param>
        public void DeleteDirectory(string path)
        {
            var result = this.database.DeleteAll<STypes::VideoDirectory>(STypes::VideoDirectory.TableName, d => d.Path == path);

            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("動画ディレクトリのDBからの削除に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("動画ディレクトリのDBからの削除に失敗しました。");
                }
            }
        }

        /// <summary>
        /// ディレクトリーを追加する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int AddDirectory(string path)
        {
            var data = new STypes::VideoDirectory()
            {
                Path = path
            };

            var result = this.database.Store(data, STypes::VideoDirectory.TableName);

            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("動画ディレクトリの取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("動画ディレクトリの取得に失敗しました。");
                }

                return -1;
            }

            return result.Data;
        }
    }
}
