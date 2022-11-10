using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Infrastructure.Database.LiteDB
{
    public interface ILiteDBHandler
    {
        /// <summary>
        /// 条件を指定してレコードを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IAttemptResult<T> GetRecord<T>(string tableName, Func<T, bool> predicate) where T : IBaseStoreClass;

        /// <summary>
        /// IDを指定してレコードを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult<T> GetRecord<T>(string tableName, int id) where T : IBaseStoreClass;

        /// <summary>
        /// すべてのレコードを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IAttemptResult<IReadOnlyList<T>> GetAllRecords<T>(string tableName) where T : IBaseStoreClass;

        /// <summary>
        /// レコードの存在を確認する
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(string tableName, int id);

        /// <summary>
        /// レコードの存在を確認する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Exists<T>(string tableName, Func<T, bool> predicate) where T : IBaseStoreClass;

        /// <summary>
        /// 追加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IAttemptResult<int> Insert<T>(T data) where T : IBaseStoreClass;

        /// <summary>
        /// 条件に一致するレコードを削除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IAttemptResult DeleteAll<T>(string tableName, Func<T, bool> predicate) where T : IBaseStoreClass;

        /// <summary>
        /// レコードを削除
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult Delete(string tableName, int id);

        /// <summary>
        /// レコードを更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IAttemptResult Update<T>(T data) where T : IBaseStoreClass;
    }

    public class LiteDBHandler : ILiteDBHandler
    {
        public LiteDBHandler(IDataBase dataBase, IErrorHandler errorHandler)
        {
            //今のところ、LiteDBのインスタンスを共有する必要がある。
            this._database = dataBase.DB!;

            this._errorHandler = errorHandler;
            this._errorHandler.HandleError(LiteDBError.Initialized);
        }

        #region field

        private readonly ILiteDatabase _database;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<T> GetRecord<T>(string tableName, Func<T, bool> predicate) where T : IBaseStoreClass
        {
            IAttemptResult<ILiteCollection<T>> cResult = this.GetCollection<T>(tableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult<T>.Fail(cResult.Message);
            }

            T? record;
            try
            {
                record = cResult.Data.FindOne(item => predicate(item));
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, tableName);
                return AttemptResult<T>.Fail(this._errorHandler.GetMessageForResult(LiteDBError.AccessFailed, ex, tableName));
            }

            if (record is null)
            {
                this._errorHandler.HandleError(LiteDBError.RecordNotFound);
                return AttemptResult<T>.Fail();
            }
            else
            {
                this._errorHandler.HandleError(LiteDBError.RecordRetrieved, tableName, record.Id);
                return AttemptResult<T>.Succeeded(record);
            }
        }

        public IAttemptResult<T> GetRecord<T>(string tableName, int id) where T : IBaseStoreClass
        {
            IAttemptResult<ILiteCollection<T>> cResult = this.GetCollection<T>(tableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult<T>.Fail(cResult.Message);
            }

            T? record;
            try
            {
                record = cResult.Data.FindById(id);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, tableName);
                return AttemptResult<T>.Fail(this._errorHandler.GetMessageForResult(LiteDBError.AccessFailed, ex, tableName));
            }

            if (record is null)
            {
                this._errorHandler.HandleError(LiteDBError.RecordNotFound);
                return AttemptResult<T>.Fail();
            }
            else
            {
                this._errorHandler.HandleError(LiteDBError.RecordRetrieved, tableName, id);
                return AttemptResult<T>.Succeeded(record);
            }
        }

        public IAttemptResult<IReadOnlyList<T>> GetAllRecords<T>(string tableName) where T : IBaseStoreClass
        {

            IAttemptResult<ILiteCollection<T>> cResult = this.GetCollection<T>(tableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult<IReadOnlyList<T>>.Fail(cResult.Message);
            }

            IReadOnlyList<T> list;
            try
            {
                list = cResult.Data.FindAll().ToList().AsReadOnly();
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, tableName);
                return AttemptResult<IReadOnlyList<T>>.Fail(this._errorHandler.GetMessageForResult(LiteDBError.AccessFailed, ex, tableName));
            }

            this._errorHandler.HandleError(LiteDBError.AllRecordsRetrieved, tableName);
            return AttemptResult<IReadOnlyList<T>>.Succeeded(list);
        }

        public bool Exists(string tableName, int id)
        {
            IAttemptResult<ILiteCollection<IBaseStoreClass>> cResult = this.GetCollection<IBaseStoreClass>(tableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return false;
            }

            try
            {
                return cResult.Data.Exists(record => record.Id == id);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, tableName);
                return false;
            }

        }

        public bool Exists<T>(string tableName, Func<T, bool> predicate) where T : IBaseStoreClass
        {
            IAttemptResult<ILiteCollection<T>> cResult = this.GetCollection<T>(tableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return false;
            }

            try
            {
                return cResult.Data.Exists(record => predicate(record));
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, tableName);
                return false;
            }
        }

        public IAttemptResult<int> Insert<T>(T data) where T : IBaseStoreClass
        {
            IAttemptResult<ILiteCollection<T>> cResult = this.GetCollection<T>(data.TableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult<int>.Fail(cResult.Message);
            }

            int id;

            try
            {
                id = (int)cResult.Data.Insert(data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, data.TableName);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(LiteDBError.AccessFailed, ex, data.TableName));
            }

            this._errorHandler.HandleError(LiteDBError.RecordInserted, data.TableName, id);
            return AttemptResult<int>.Succeeded(id);
        }

        public IAttemptResult DeleteAll<T>(string tableName, Func<T, bool> predicate) where T : IBaseStoreClass
        {
            IAttemptResult<ILiteCollection<T>> cResult = this.GetCollection<T>(tableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult<int>.Fail(cResult.Message);
            }

            int count;

            try
            {
                count = cResult.Data.DeleteMany(record => predicate(record));
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, tableName);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(LiteDBError.AccessFailed, ex, tableName));
            }

            if (count > 0)
            {
                this._errorHandler.HandleError(LiteDBError.RecordsDeleted, tableName, count);
                return AttemptResult.Succeeded();
            }
            else
            {
                this._errorHandler.HandleError(LiteDBError.RecordsDeleteFailed, tableName);
                return AttemptResult.Fail();
            }

        }

        public IAttemptResult Delete(string tableName, int id)
        {
            IAttemptResult<ILiteCollection<BsonDocument>> cResult = this.GetCollection<BsonDocument>(tableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult.Fail(cResult.Message);
            }

            bool result;
            try
            {
                result = cResult.Data.Delete(id);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, tableName);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(LiteDBError.AccessFailed, ex, tableName));
            }

            if (result)
            {
                this._errorHandler.HandleError(LiteDBError.RecordDeleted, tableName, id);
                return AttemptResult.Succeeded();
            }
            else
            {
                this._errorHandler.HandleError(LiteDBError.RecordDeleteFailed, tableName, id);
                return AttemptResult.Fail();
            }
        }

        public IAttemptResult Update<T>(T data) where T : IBaseStoreClass
        {
            IAttemptResult<ILiteCollection<T>> cResult = this.GetCollection<T>(data.TableName);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult.Fail(cResult.Message);
            }

            bool result;
            try
            {
                result = cResult.Data.Update(data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.AccessFailed, ex, data.TableName);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(LiteDBError.AccessFailed, ex, data.TableName));
            }

            if (!result)
            {
                this._errorHandler.HandleError(LiteDBError.UpdateTargetRecordNotExists, data.TableName, data.Id);
                return AttemptResult.Fail();
            }
            else
            {
                this._errorHandler.HandleError(LiteDBError.RecordUpdated, data.TableName, data.Id);
                return AttemptResult.Succeeded();
            }
        }


        #endregion

        #region private

        /// <summary>
        /// 指定したテーブルを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private IAttemptResult<ILiteCollection<T>> GetCollection<T>(string tableName)
        {
            ILiteCollection<T> collection;

            try
            {
                collection = this._database.GetCollection<T>(tableName);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LiteDBError.TableRetrieveFailed, ex, tableName);
                return AttemptResult<ILiteCollection<T>>.Fail(this._errorHandler.GetMessageForResult(LiteDBError.TableRetrieveFailed, ex, tableName));
            }

            if (collection.Count() == 0)
            {
                this._errorHandler.HandleError(LiteDBError.TableCreated, tableName);
            }
            else
            {
                this._errorHandler.HandleError(LiteDBError.TableRetrieved, tableName);
            }

            return AttemptResult<ILiteCollection<T>>.Succeeded(collection);
        }

        #endregion
    }
}
