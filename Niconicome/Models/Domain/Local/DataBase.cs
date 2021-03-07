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
        public ILiteCollection<T> GetCollection<T>(string tableName) where T : IStorable;
        public ILiteCollection<BsonDocument> GetCollection(string tableName);
        public int GetRecordCount(string tableName);
        public T? GetRecord<T>(string tablename, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable;
        public T? GetRecord<T>(string tableName, int id) where T : IStorable;
        public T? GetRecord<T>(string tableName, int id, int retry) where T : IStorable;
        public bool Exists<T>(string tablename, int id) where T : IStorable;
        public bool Exists<T>(string tablename, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable;
        public int Store<T>(T data, string tableName) where T : IStorable;
        public bool TryStoreUnique<T>(T data, string tableName, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable;
        public bool TryStoreUnique<T>(T data, string tableName, System.Linq.Expressions.Expression<Func<T, bool>> predicate, out int id) where T : IStorable;
        public bool DeleteAll<T>(string tableName, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable;
        public bool Delete(string tableName, BsonValue id);
        public ILiteCollection<T> Update<T>(T data, string tableName) where T : IStorable;
        public List<T> GetAllRecords<T>(string tableName) where T : IStorable;
        public void Clear(string tableName, bool setUpAgain = true);
        public void Open();

    }

    public class DataBase : IDataBase
    {

        public DataBase()
        {

            try
            {
                this.Open();
            }
            catch (Exception e)
            {
                Utils::Logger.GetLogger().Error("データベースファイルのオープンに失敗しました。", e);
                return;
            }

            try
            {
                this.SetUp();
            }
            catch (Exception e)
            {
                Utils::Logger.GetLogger().Error("データベースの初期化中にエラーが発生しました。", e);
                return;
            }
        }

        public DataBase(ILiteDatabase liteInstance, bool autoDispose = true)
        {
            this.DbInstance = liteInstance;
            this.autoDispose = autoDispose;
            this.SetUp();
        }

        ~DataBase()
        {
            this.Dispose();
        }

        /// <summary>
        /// 自動でインスタンス破棄フラグ
        /// </summary>
        private readonly bool autoDispose = true;

        /// <summary>
        /// dbファイル名
        /// </summary>
        private readonly string dbFileName = @"niconicome.db";

        /// <summary>
        /// データベースのインスタンスを保持
        /// </summary>
        private ILiteDatabase? DbInstance;

        /// <summary>
        /// データベースのセットアップ
        /// </summary>
        private void SetUp()
        {
            BsonMapper.Global.Entity<STypes::Playlist>()
                .DbRef(playlist => playlist.Videos, STypes::Video.TableName);
            BsonMapper.Global.Entity<STypes::Video>()
                .DbRef(video => video.Owner, Niconico::User.TableName);

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
        /// 指定したテーブルを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ILiteCollection<T> GetCollection<T>(string tableName) where T : IStorable
        {
            if (this.DbInstance is not null)
            {
                return this.DbInstance.GetCollection<T>(tableName);
            }
            else
            {
                throw new InvalidOperationException($"データベースファイルの読み込みが完了していない為、テーブル\"{tableName}\"を取得できませんでした。");
            }
        }

        /// <summary>
        /// 指定したテーブルを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ILiteCollection<BsonDocument> GetCollection(string tableName)
        {
            if (this.DbInstance != null)
            {
                return this.DbInstance.GetCollection(tableName);
            }
            else
            {
                throw new InvalidOperationException($"データベースファイルの読み込みが完了していない為、テーブル\"{tableName}\"を取得できませんでした。");
            }
        }

        /// <summary>
        /// レコドード数を取得する
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetRecordCount(string tableName)
        {
            return this.DbInstance?.GetCollection(tableName).Count() ?? 0;
        }

        /// <summary>
        /// レコードを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T? GetRecord<T>(string tablename, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var collections = this.GetCollection<T>(tablename);
            return collections.FindOne(predicate);
        }

        /// <summary>
        /// レコードをIDで取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T? GetRecord<T>(string tableName, int id) where T : IStorable
        {
            var collections = this.GetCollection<T>(tableName);
            return collections.FindById(id);
        }

        /// <summary>
        /// レコードをIDで取得する(リトライ指定)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T? GetRecord<T>(string tableName, int id, int retry) where T : IStorable
        {
            var collections = this.GetCollection<T>(tableName);
            var retryAttempts = 0;
            T? recored = collections.FindById(id);
            while (recored == null && retryAttempts < retry)
            {
                retryAttempts++;
                recored = this.GetRecord<T>(tableName, id);
            }
            return recored;
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
            var collections = this.GetCollection<T>(tablename);
            return collections.Exists(r => r.Id == id);
        }

        /// <summary>
        /// レコードが存在するかどうかをチェックする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Exists<T>(string tablename, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var collections = this.GetCollection<T>(tablename);
            return collections.Exists(predicate);
        }

        /// <summary>
        /// インスタンスを保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        public int Store<T>(T data, string tableName) where T : IStorable
        {
            var collections = this.GetCollection<T>(tableName);
            try
            {
                BsonValue result = collections.Insert(data);
                return (int)result.RawValue;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// ユニークデータを保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool TryStoreUnique<T>(T data, string tableName, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable
        {
            if (this.Exists(tableName, predicate))
            {
                return false;
            }
            else
            {
                this.Store<T>(data, tableName);
                return true;
            }
        }

        /// <summary>
        /// ユニークデータを保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool TryStoreUnique<T>(T data, string tableName, System.Linq.Expressions.Expression<Func<T, bool>> predicate, out int id) where T : IStorable
        {
            if (this.Exists(tableName, predicate))
            {
                id = -1;
                return false;
            }
            else
            {
                id = this.Store<T>(data, tableName);
                return true;
            }
        }

        /// <summary>
        /// 指定した条件に合致したレコードを全て削除する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool DeleteAll<T>(string tableName, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var collections = this.GetCollection<T>(tableName);
            return collections.DeleteMany(predicate) > 0;
        }

        /// <summary>
        /// レコードを削除する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(string tableName, BsonValue id)
        {
            var collections = this.GetCollection(tableName);
            return collections.Delete(id);
        }

        /// <summary>
        /// 更新する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ILiteCollection<T> Update<T>(T data, string tableName) where T : IStorable
        {
            var col = this.GetCollection<T>(tableName);
            col.Update(data);
            return col;
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
        public List<T> GetAllRecords<T>(string tableName) where T : IStorable
        {
            return this.GetCollection<T>(tableName).FindAll().ToList();
        }

        /// <summary>
        /// 全て削除する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        public void Clear(string tableName, bool setUpAgain = true)
        {
            this.GetCollection(tableName).DeleteAll();
            if (setUpAgain) this.SetUp();
        }

        /// <summary>
        /// データベースを開く
        /// </summary>
        public void Open()
        {
            if (this.DbInstance is not null)
            {
                this.DbInstance.Dispose();
                this.DbInstance = null;
            }
            this.DbInstance = new LiteDatabase($"Filename={this.dbFileName};Mode=Shared;");
        }

    }
}
