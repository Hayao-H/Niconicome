using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
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

        public IAttemptResult<ITagInfo> GetTag(int ID)
        {
            IAttemptResult<Tag> result = this._database.GetRecord<Tag>(TableNames.Tag, ID);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<ITagInfo>.Fail(result.Message);
            }

            var tag = new TagInfo(this)
            {
                ID = result.Data.Id,
                Name = result.Data.Name,
                IsNicodicExist = result.Data.IsNicodicExist,
            };

            return AttemptResult<ITagInfo>.Succeeded(tag);
        }

        public IAttemptResult Create(ITagInfo tag)
        {
            Tag data = this.Convert(tag);
            return this._database.Update(data);
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
