using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Local.SQLite
{
    public interface ISQliteLoader : IDisposable
    {
        bool IsConnected { get; }
        SqliteDataReader GetDataReader(string path, string commandString);
    }

    /// <summary>
    /// SQliteハンドラー
    /// </summary>
    public class SQliteLoader : ISQliteLoader
    {
        public SQliteLoader(ILogger logger)
        {
            this.logger = logger;
        }

        private readonly ILogger logger;

        private SqliteConnection? connection;

        /// <summary>
        /// コネクションフラグ
        /// </summary>
        public bool IsConnected { get => this.connection is not null; }

        /// <summary>
        /// データリーダーを取得する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="commandString"></param>
        /// <returns></returns>
        public SqliteDataReader GetDataReader(string path, string commandString)
        {
            if (this.IsConnected)
            {
                this.Dispose();
            }

            if (!File.Exists(path))
            {
                throw new IOException($"指定されたデータベースファイルは存在しません(path:{path})");
            }


            try
            {
                this.connection = new SqliteConnection($"Data Source = {path}");
                this.connection.Open();
            }
            catch (Exception e)
            {
                this.logger.Error($"データベースのオープンに失敗しました。(path:{path})", e);
                throw new IOException($"データベースのオープンに失敗しました。(詳細:{e.Message})");
            }

            var command = this.connection.CreateCommand();
            command.CommandText = commandString;

            return command.ExecuteReader();
        }

        /// <summary>
        /// コネクションを破棄する
        /// </summary>
        public void Dispose()
        {
            this.connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
