using System;
using System.Collections.Generic;
using System.Linq;
using Windows = System.Windows;
using LiteDB;
using Niconicome.Extensions.System;
using Niconicome.Extensions;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Niconico = Niconicome.Models.Domain.Niconico;
using Utils = Niconicome.Models.Domain.Utils;
using Local = Niconicome.Models.Local;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Domain.Utils;
using System.Linq.Expressions;

namespace Niconicome.Models.Domain.Local
{
    public interface IStorable
    {
        public int Id { get; }

        public static string TableName { get; set; } = string.Empty;
    }

    public interface ISetting
    {
        public string? SettingName { get; }
    }

    public interface IDataBase : IDisposable
    {
        /// <summary>
        /// コレクションを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IAttemptResult<IEnumerable<T>> GetCollection<T>(string tableName) where T : IStorable;

        /// <summary>
        /// レコード数を取得する
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IAttemptResult<int> GetRecordCount(string tableName);

        /// <summary>
        /// 条件を指定してレコードを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IAttemptResult<T> GetRecord<T>(string tablename, Expression<Func<T, bool>> predicate) where T : IStorable;

        /// <summary>
        /// IDを指定してレコードを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult<T> GetRecord<T>(string tableName, int id) where T : IStorable;

        /// <summary>
        /// IDを指定してレコードを取得＆マップ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="MemberT"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IAttemptResult<T> GetRecord<T, MemberT>(string tableName, int id, Expression<Func<T, MemberT>> factory) where T : IStorable;

        /// <summary>
        /// レコードの存在を確認する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists<T>(string tablename, int id) where T : IStorable;

        /// <summary>
        /// レコードの存在を確認する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Exists<T>(string tablename, Expression<Func<T, bool>> predicate) where T : IStorable;

        /// <summary>
        /// 追加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IAttemptResult<int> Store<T>(T data, string tableName) where T : IStorable;

        /// <summary>
        /// 条件に一致するレコードを削除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IAttemptResult<bool> DeleteAll<T>(string tableName, Expression<Func<T, bool>> predicate) where T : IStorable;

        /// <summary>
        /// レコードを削除
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult<bool> Delete(string tableName, BsonValue id);

        /// <summary>
        /// レコードを更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IAttemptResult Update<T>(T data, string tableName) where T : IStorable;

        /// <summary>
        /// すべてのレコードを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IAttemptResult<List<T>> GetAllRecords<T>(string tableName) where T : IStorable;

        /// <summary>
        /// 条件に一致するレコードをすべて取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IAttemptResult<List<T>> GetAllRecords<T>(string tableName, Func<T, bool> predicate) where T : IStorable;

        /// <summary>
        /// すべてのレコードを取得&マップ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IAttemptResult<List<T>> GetAllRecords<T>(string tableName, Func<ILiteCollection<T>, ILiteCollection<T>> factory) where T : IStorable;

        /// <summary>
        /// コレクションをクリア
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="setUpAgain"></param>
        /// <returns></returns>
        IAttemptResult Clear(string tableName, bool setUpAgain = true);

        /// <summary>
        /// データベースを開く
        /// </summary>
        /// <returns></returns>
        IAttemptResult Open();

        ILiteDatabase? DB { get; }
    }

    public class DataBase : IDataBase
    {

        public DataBase(ILogger logger)
        {
            this.logger = logger;

            this.Configure();

            var result = this.Open();
            if (!result.IsSucceeded)
            {
                this.logger.Error("データベースファイルのオープンに失敗しました。", result.Exception!);
                return;
            }
        }

        public DataBase(ILiteDatabase liteInstance, ILogger logger, bool autoDispose = true)
        {
            this.DbInstance = liteInstance;
            this.logger = logger;
            this.autoDispose = autoDispose;
        }

        ~DataBase()
        {
            this.Dispose();
        }

        public ILiteDatabase? DB => this.DbInstance;

        #region field
        /// <summary>
        /// 自動でインスタンス破棄フラグ
        /// </summary>
        private readonly bool autoDispose = true;

        /// <summary>
        /// dbファイル名
        /// </summary>
        private readonly string dbFileName = @"niconicome.db";

        /// <summary>
        /// ログ
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// データベースのインスタンスを保持
        /// </summary>
        private ILiteDatabase? DbInstance;
        #endregion

        public IAttemptResult<IEnumerable<T>> GetCollection<T>(string tableName) where T : IStorable
        {
            var result = this.GetCollectionInternal<T>(tableName);

            if (!result.IsSucceeded || result.Data is null)
            {
                return new AttemptResult<IEnumerable<T>>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = result.Exception };
            }

            return new AttemptResult<IEnumerable<T>>() { Data = result.Data.FindAll(), IsSucceeded = true };
        }

        public IAttemptResult<int> GetRecordCount(string tableName)
        {
            var result = this.GetCollectionInternal<BsonDocument>(tableName);

            if (!result.IsSucceeded || result.Data is null)
            {
                return new AttemptResult<int>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = result.Exception };
            }

            int count;

            try
            {
                count = result.Data.Count();
            }
            catch (Exception e)
            {
                return new AttemptResult<int>() { Message = $"テーブル「{tableName}」のレコード数取得に失敗しました。", Exception = e };
            }

            return new AttemptResult<int>() { Data = count, IsSucceeded = true };

        }

        public IAttemptResult<T> GetRecord<T>(string tablename, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var result = this.GetCollectionInternal<T>(tablename);

            if (!result.IsSucceeded || result.Data is null)
            {
                return new AttemptResult<T>() { Message = $"テーブル「{tablename}」の取得に失敗しました。", Exception = result.Exception };
            }

            T data;

            try
            {
                data = result.Data.FindOne(predicate);
            }
            catch (Exception e)
            {
                return new AttemptResult<T>() { Message = $"テーブル「{tablename}」からのレコード取得に失敗しました。", Exception = e };
            }

            return new AttemptResult<T>() { Data = data, IsSucceeded = true };

        }

        public IAttemptResult<T> GetRecord<T>(string tablename, int id) where T : IStorable
        {
            var result = this.GetCollectionInternal<T>(tablename);

            if (!result.IsSucceeded || result.Data is null)
            {
                return new AttemptResult<T>() { Message = $"テーブル「{tablename}」の取得に失敗しました。", Exception = result.Exception };
            }

            T data;

            try
            {
                data = result.Data.FindById(id);
            }
            catch (Exception e)
            {
                return new AttemptResult<T>() { Message = $"テーブル「{tablename}」からのレコード取得に失敗しました。", Exception = e };
            }

            return new AttemptResult<T>() { Data = data, IsSucceeded = true };
        }

        public IAttemptResult<T> GetRecord<T, TMem>(string tablename, int id, Expression<Func<T, TMem>> factory) where T : IStorable
        {
            var result = this.GetCollectionInternal(tablename, factory);

            if (!result.IsSucceeded || result.Data is null)
            {
                return new AttemptResult<T>() { Message = $"テーブル「{tablename}」の取得に失敗しました。", Exception = result.Exception };
            }

            T data;

            try
            {
                data = result.Data.FindById(id);
            }
            catch (Exception e)
            {
                return new AttemptResult<T>() { Message = $"テーブル「{tablename}」からのレコード取得に失敗しました。", Exception = e };
            }

            return new AttemptResult<T>() { Data = data, IsSucceeded = true };
        }

        public bool Exists<T>(string tablename, int id) where T : IStorable
        {
            var result = this.GetCollectionInternal<T>(tablename);

            if (!result.IsSucceeded || result.Data is null) return false;

            return result.Data!.Exists(item => item.Id == id);
        }

        public bool Exists<T>(string tablename, Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var collections = this.GetCollectionInternal<T>(tablename);

            if (!collections.IsSucceeded || collections.Data is null) return false;

            return collections.Data.Exists(predicate);
        }

        public IAttemptResult<int> Store<T>(T data, string tableName) where T : IStorable
        {
            var colResult = this.GetCollectionInternal<T>(tableName);

            if (!colResult.IsSucceeded || colResult.Data is null) return new AttemptResult<int>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = colResult.Exception };

            BsonValue result;

            try
            {
                result = colResult.Data!.Insert(data);

            }
            catch (Exception e)
            {
                return new AttemptResult<int>() { Message = $"テーブル「{tableName}」への挿入に失敗しました。", Exception = e };
            }

            return new AttemptResult<int>() { IsSucceeded = true, Data = (int)result };
        }

        public IAttemptResult<bool> DeleteAll<T>(string tableName, Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var colResult = this.GetCollectionInternal<T>(tableName);

            if (!colResult.IsSucceeded || colResult.Data is null) return new AttemptResult<bool>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = colResult.Exception };

            int count;

            try
            {
                count = colResult.Data.DeleteMany(predicate);

            }
            catch (Exception e)
            {
                return new AttemptResult<bool>() { Message = $"テーブル「{tableName}」からのレコード削除に失敗しました。", Exception = e };
            }

            return new AttemptResult<bool>() { IsSucceeded = true, Data = count > 0 };
        }

        public IAttemptResult<bool> Delete(string tableName, BsonValue id)
        {
            var colResult = this.GetCollectionInternal<BsonDocument>(tableName);

            if (!colResult.IsSucceeded || colResult.Data is null) return new AttemptResult<bool>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = colResult.Exception };

            try
            {
                colResult.Data.Delete(id);
            }
            catch (Exception e)
            {
                return new AttemptResult<bool>() { Message = $"テーブル「{tableName}」からのレコード削除に失敗しました。", Exception = e };
            }

            return new AttemptResult<bool>() { IsSucceeded = true, Data = true };

        }

        public IAttemptResult Update<T>(T data, string tableName) where T : IStorable
        {
            var colResult = this.GetCollectionInternal<T>(tableName);

            if (!colResult.IsSucceeded || colResult.Data is null) return new AttemptResult<bool>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = colResult.Exception };

            try
            {
                colResult.Data.Update(data);
            }
            catch (Exception e)
            {
                return new AttemptResult() { Message = $"テーブル「{tableName}」の更新に失敗しました。", Exception = e };
            }

            return new AttemptResult() { IsSucceeded = true };

        }

        public void Dispose()
        {
            if (this.autoDispose) this.DbInstance?.Dispose();
            this.DbInstance = null;
            GC.SuppressFinalize(this);
        }

        public IAttemptResult<List<T>> GetAllRecords<T>(string tableName) where T : IStorable
        {
            var colResult = this.GetCollectionInternal<T>(tableName);

            if (!colResult.IsSucceeded || colResult.Data is null) return new AttemptResult<List<T>>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = colResult.Exception };

            List<T> data;

            try
            {
                data = colResult.Data.FindAll().ToList();
            }
            catch (Exception e)
            {
                return new AttemptResult<List<T>>() { Message = $"テーブル「{tableName}」からのレコード取得に失敗しました。", Exception = e };
            }

            return new AttemptResult<List<T>>() { IsSucceeded = true, Data = data };
        }

        public IAttemptResult<List<T>> GetAllRecords<T>(string tableName, Func<T, bool> predicate) where T : IStorable
        {
            var colResult = this.GetCollectionInternal<T>(tableName);

            if (!colResult.IsSucceeded || colResult.Data is null) return new AttemptResult<List<T>>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = colResult.Exception };

            List<T> data;

            try
            {
                data = colResult.Data.FindAll().Where(record => predicate(record)).ToList();
            }
            catch (Exception e)
            {
                return new AttemptResult<List<T>>() { Message = $"テーブル「{tableName}」からのレコード取得に失敗しました。", Exception = e };
            }

            return new AttemptResult<List<T>>() { IsSucceeded = true, Data = data };
        }

        public IAttemptResult<List<T>> GetAllRecords<T>(string tableName, Func<ILiteCollection<T>, ILiteCollection<T>> factory) where T : IStorable
        {

            var colResult = this.GetCollectionInternal<T>(tableName);

            if (!colResult.IsSucceeded || colResult.Data is null) return new AttemptResult<List<T>>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = colResult.Exception };

            List<T> data;

            ILiteCollection<T> collection = factory(colResult.Data);

            try
            {
                data = collection.FindAll().ToList();
            }
            catch (Exception e)
            {
                return new AttemptResult<List<T>>() { Message = $"テーブル「{tableName}」からのレコード取得に失敗しました。", Exception = e };
            }

            return new AttemptResult<List<T>>() { IsSucceeded = true, Data = data };
        }

        public IAttemptResult Clear(string tableName, bool setUpAgain = true)
        {
            var colResult = this.GetCollectionInternal<BsonDocument>(tableName);

            if (!colResult.IsSucceeded || colResult.Data is null) return new AttemptResult() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = colResult.Exception };

            try
            {
                colResult.Data.DeleteAll();
            }
            catch (Exception e)
            {
                return new AttemptResult() { Message = $"テーブル「{tableName}」のクリアに失敗しました。", Exception = e };
            }

            return new AttemptResult() { IsSucceeded = true };
        }

        public IAttemptResult Open()
        {
            if (this.DbInstance is not null)
            {
                this.DbInstance.Dispose();
                this.DbInstance = null;
            }
            try
            {
                this.DbInstance = new LiteDatabase($"Filename={this.dbFileName};Mode=shared;");
            }
            catch (Exception e)
            {
                return new AttemptResult() { Exception = e, Message = "データベースファイルのオープンに失敗しました。" };
            }

            return new AttemptResult() { IsSucceeded = true };
        }

        #region private

        /// <summary>
        /// コレクションを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private IAttemptResult<ILiteCollection<T>> GetCollectionInternal<T>(string tableName)
        {
            this.CheckIfDbInstabseIsNull();

            ILiteCollection<T> col;

            try
            {
                col = this.DbInstance!.GetCollection<T>(tableName);
            }
            catch (Exception e)
            {
                return new AttemptResult<ILiteCollection<T>>() { Exception = e };
            }

            return new AttemptResult<ILiteCollection<T>>() { Data = col, IsSucceeded = true };
        }

        /// <summary>
        /// コレクションを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private IAttemptResult<ILiteCollection<T>> GetCollectionInternal<T, TMem>(string tableName, Expression<Func<T, TMem>> factory)
        {
            this.CheckIfDbInstabseIsNull();

            ILiteCollection<T> col;

            try
            {
                col = this.DbInstance!.GetCollection<T>(tableName)
                    .Include(factory);
            }
            catch (Exception e)
            {
                return new AttemptResult<ILiteCollection<T>>() { Exception = e };
            }

            return new AttemptResult<ILiteCollection<T>>() { Data = col, IsSucceeded = true };
        }

        /// <summary>
        /// マッパーの設定
        /// </summary>
        private void Configure()
        {

            BsonMapper.Global.Entity<STypes::Playlist>()
                .DbRef(playlist => playlist.Videos, STypes::Video.TableName);
            BsonMapper.Global.TrimWhitespace = false;
        }

        /// <summary>
        /// DBの読み込み完了状態をチェックする
        /// </summary>
        /// <param name="tableName"></param>
        private void CheckIfDbInstabseIsNull(string tableName = "None")
        {
            if (this.DbInstance is null) throw new InvalidOperationException($"データベースの読み込みが完了していないためテーブル「{tableName}」を取得できませんでした。");
        }
        #endregion

    }
}
