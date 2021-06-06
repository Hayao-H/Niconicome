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
using Niconicome.Models.Helper.Result.Generic;
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
        IAttemptResult<IEnumerable<T>> GetCollection<T>(string tableName) where T : IStorable;
        IAttemptResult<int> GetRecordCount(string tableName);
        IAttemptResult<T> GetRecord<T>(string tablename, Expression<Func<T, bool>> predicate) where T : IStorable;
        IAttemptResult<T> GetRecord<T>(string tableName, int id) where T : IStorable;
        bool Exists<T>(string tablename, int id) where T : IStorable;
        bool Exists<T>(string tablename, Expression<Func<T, bool>> predicate) where T : IStorable;
        IAttemptResult<int> Store<T>(T data, string tableName) where T : IStorable;
        IAttemptResult<bool> DeleteAll<T>(string tableName, Expression<Func<T, bool>> predicate) where T : IStorable;
        IAttemptResult<bool> Delete(string tableName, BsonValue id);
        IAttemptResult Update<T>(T data, string tableName) where T : IStorable;
        IAttemptResult<List<T>> GetAllRecords<T>(string tableName) where T : IStorable;
        IAttemptResult<List<T>> GetAllRecords<T>(string tableName, Func<T, bool> predicate) where T : IStorable;
        IAttemptResult Clear(string tableName, bool setUpAgain = true);
        IAttemptResult Open();
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

            try
            {
                this.SetUp();
            }
            catch (Exception e)
            {
                this.logger.Error("データベースの初期化中にエラーが発生しました。", e);
                return;
            }
        }

        public DataBase(ILiteDatabase liteInstance, ILogger logger, bool autoDispose = true)
        {
            this.DbInstance = liteInstance;
            this.logger = logger;
            this.autoDispose = autoDispose;
            this.SetUp();
        }

        ~DataBase()
        {
            this.Dispose();
        }

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

        /// <summary>
        /// 指定したテーブルを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public IAttemptResult<IEnumerable<T>> GetCollection<T>(string tableName) where T : IStorable
        {
            var result = this.GetCollectionInternal<T>(tableName);

            if (!result.IsSucceeded || result.Data is null)
            {
                return new AttemptResult<IEnumerable<T>>() { Message = $"テーブル「{tableName}」の取得に失敗しました。", Exception = result.Exception };
            }

            return new AttemptResult<IEnumerable<T>>() { Data = result.Data.FindAll(), IsSucceeded = true };
        }

        /// <summary>
        /// レコドード数を取得する
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// レコードを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
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

        /// <summary>
        /// レコードをIDで取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// レコードが存在するかどうかをチェックする(ID)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Exists<T>(string tablename, int id) where T : IStorable
        {
            var result = this.GetCollectionInternal<T>(tablename);

            if (!result.IsSucceeded || result.Data is null) return false;

            return result.Data!.Exists(item => item.Id == id);
        }

        /// <summary>
        /// レコードが存在するかどうかをチェックする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Exists<T>(string tablename, Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var collections = this.GetCollectionInternal<T>(tablename);

            if (!collections.IsSucceeded || collections.Data is null) return false;

            return collections.Data.Exists(predicate);
        }

        /// <summary>
        /// インスタンスを保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
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

        /// <summary>
        /// 指定した条件に合致したレコードを全て削除する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
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

        /// <summary>
        /// レコードを削除する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 更新する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// DBのインスタンスを破棄する
        /// </summary>
        public void Dispose()
        {
            if (this.autoDispose) this.DbInstance?.Dispose();
            this.DbInstance = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 全てのレコードを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 条件を指定してすべてのレコードを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 全て削除する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
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

        /// <summary>
        /// データベースを開く
        /// </summary>
        public IAttemptResult Open()
        {
            if (this.DbInstance is not null)
            {
                this.DbInstance.Dispose();
                this.DbInstance = null;
            }
            try
            {
                this.DbInstance = new LiteDatabase($"Filename={this.dbFileName};Mode=Shared;");
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
        /// データベースのセットアップ
        /// </summary>
        private void SetUp()
        {

            if (!this.Exists<STypes::Playlist>(STypes::Playlist.TableName, playlist => playlist.IsRoot))
            {
                var root = new STypes::Playlist()
                {
                    IsRoot = true,
                    PlaylistName = "プレイリスト一覧",
                };
                this.Store(root, STypes::Playlist.TableName);
            }
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
