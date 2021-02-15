using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public VideoDirectoryStoreHandler(IDataBase dataBase)
        {
            this.database = dataBase;
        }

        private readonly IDataBase database;

        /// <summary>
        /// 全てのディレクトリーを取得する
        /// </summary>
        /// <returns></returns>
        public List<STypes::VideoDirectory> GetVideoDirectories()
        {
            return this.database.GetAllRecords<STypes::VideoDirectory>(STypes::VideoDirectory.TableName);
        }

        /// <summary>
        /// ディレクトリーを削除する
        /// </summary>
        /// <param name="path"></param>
        public void DeleteDirectory(string path)
        {
            this.database.DeleteAll<STypes::VideoDirectory>(STypes::VideoDirectory.TableName, d => d.Path == path);
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

            var id = this.database.Store(data, STypes::VideoDirectory.TableName);

            return id;
        }
    }
}
