using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Infrastructure.Database.Types;

namespace Niconicome.Models.Infrastructure.Database
{
    public class TagDBHandler : ITagStore
    {
        public TagDBHandler(ILiteDBHandler databse)
        {
            this._database = databse;
        }

        #region field

        private readonly ILiteDBHandler _database;

        #endregion


        #region Method

        public IAttemptResult<ITagInfo> GetTag(string tag)
        {
            IAttemptResult<Tag> result = this._database.GetRecord<Tag>(TableNames.Tag, t => t.Name == tag);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<ITagInfo>.Fail(result.Message);
            }

            var data = new TagInfo(this)
            {
                ID = result.Data.Id,
                Name = result.Data.Name,
            };

            data.IsAutoUpdateEnabled = false;
            data.IsNicodicExist = result.Data.IsNicodicExist;
            data.IsAutoUpdateEnabled = true;

            return AttemptResult<ITagInfo>.Succeeded(data);
        }

        public IAttemptResult<ITagInfo> GetTag(int id)
        {
            IAttemptResult<Tag> result = this._database.GetRecord<Tag>(TableNames.Tag, id);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<ITagInfo>.Fail(result.Message);
            }

            var data = new TagInfo(this)
            {
                ID = result.Data.Id,
                Name = result.Data.Name,
            };

            data.IsAutoUpdateEnabled = false;
            data.IsNicodicExist = result.Data.IsNicodicExist;
            data.IsAutoUpdateEnabled = true;

            return AttemptResult<ITagInfo>.Succeeded(data);
        }

        public IAttemptResult<IEnumerable<ITagInfo>> GetAllTag()
        {
            IAttemptResult<IReadOnlyList<Tag>> result = this._database.GetAllRecords<Tag>(TableNames.Tag);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<ITagInfo>>.Fail(result.Message);
            }

            var tags = new List<ITagInfo>();
            foreach (var tag in result.Data)
            {
                var data = new TagInfo(this)
                {
                    ID = tag.Id,
                    Name = tag.Name,
                };

                data.IsAutoUpdateEnabled = false;
                data.IsNicodicExist = tag.IsNicodicExist;
                data.IsAutoUpdateEnabled = true;

                tags.Add(data);
            }


            return AttemptResult<IEnumerable<ITagInfo>>.Succeeded(tags.AsReadOnly());
        }

        public IAttemptResult Create(string tag)
        {
            Tag data = new Tag() { Name = tag };
            return this._database.Insert(data);
        }

        public IAttemptResult Update(ITagInfo tag)
        {
            Tag data = this.Convert(tag);
            return this._database.Update(data);
        }

        public IAttemptResult Delete(int ID)
        {
            return this._database.Delete(TableNames.Tag, ID);
        }
        public IAttemptResult Clear()
        {
            return this._database.Clear(TableNames.Tag);
        }

        public bool Exist(string name)
        {
            return this._database.Exists<Tag>(TableNames.Tag, t => t.Name == name);
        }

        #endregion

        #region private

        /// <summary>
        /// ローカル情報をデータベースの型に変換
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Tag Convert(ITagInfo info)
        {
            return new Tag()
            {
                Id = info.ID,
                Name = info.Name,
                IsNicodicExist = info.IsNicodicExist,
            };
        }

        #endregion


    }
}
