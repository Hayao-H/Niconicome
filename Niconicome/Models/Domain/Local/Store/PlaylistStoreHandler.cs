using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Domain.Local.Store
{
    public interface IPlaylistStoreHandler
    {
        /// <summary>
        /// すべてのプレイリストを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<List<STypes::Playlist>> GetAllPlaylists();

        /// <summary>
        /// ルートプレイリストを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<STypes::Playlist> GetRootPlaylist();

        /// <summary>
        /// 指定したIDのプレイリストを取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult<STypes::Playlist> GetPlaylist(int id);

        /// <summary>
        /// 指定した条件でプレイリストを取得
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IAttemptResult<STypes::Playlist> GetPlaylist(Expression<Func<STypes::Playlist, bool>> predicate);

        /// <summary>
        /// 子プレイリストを取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult<List<STypes::Playlist>> GetChildPlaylists(int id);

        /// <summary>
        /// プレイリストを追加
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        IAttemptResult<int> AddPlaylist(int parentID, string name);

        /// <summary>
        /// プレイレイストを更新
        /// </summary>
        /// <param name="newplaylist"></param>
        /// <returns></returns>
        IAttemptResult Update(STypes::Playlist playlist);

        /// <summary>
        /// プレイリストを削除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult DeletePlaylist(int id);

        /// <summary>
        /// プレイリストの存在チェック
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(int id);

        /// <summary>
        /// プレイリストの存在チェック
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<STypes::Playlist, bool>> predicate);

        /// <summary>
        /// 動画をプレイリストに追加
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        IAttemptResult WireVideo(STypes::Video video, int playlistId);

        /// <summary>
        /// 動画をプレイリストから削除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        IAttemptResult UnWireVideo(int id, int playlistId);

        /// <summary>
        /// プレイリストを移動
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destId"></param>
        /// <returns></returns>
        IAttemptResult Move(int id, int destId);

        /// <summary>
        /// リモートプレイリストにする
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remoteId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IAttemptResult SetAsRemotePlaylist(int id, string remoteId, RemoteType type);

        /// <summary>
        /// ローカルプレイリストにする
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult SetAsLocalPlaylist(int id);

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        IAttemptResult Initialize();
    }

    public class PlaylistStoreHandler : IPlaylistStoreHandler
    {
        public PlaylistStoreHandler(IDataBase dataBase, ILogger logger)
        {
            this.databaseInstance = dataBase;
            this.logger = logger;
        }

        #region field
        private readonly IDataBase databaseInstance;

        private readonly ILogger logger;
        #endregion

        #region CRUD

        public IAttemptResult<STypes::Playlist> GetPlaylist(int id)
        {

            var result = this.databaseInstance.GetRecord<STypes::Playlist, List<STypes::Video>>(STypes::Playlist.TableName, id, p => p.Videos);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("プレイリストの取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("プレイリストの取得に失敗しました。");
                }

                return AttemptResult<STypes::Playlist>.Fail("プレイリストの取得に失敗しました。");
            }


            if (result.Data.Videos.Count > 0 && result.Data.CustomVideoSequence.Count != result.Data.Videos.Count)
            {
                var ids = result.Data.Videos.Select(v => v.Id).Where(id => !result.Data.CustomVideoSequence.Contains(id));
                result.Data.CustomVideoSequence.AddRange(ids);
            }

            this.logger.Log($"プレイリスト(name:{result.Data.PlaylistName}, ID:{id})を取得しました。");
            return AttemptResult<STypes::Playlist>.Succeeded(result.Data);

        }

        public IAttemptResult<STypes::Playlist> GetPlaylist(Expression<Func<STypes::Playlist, bool>> predicate)
        {

            IAttemptResult<STypes::Playlist> result = this.databaseInstance.GetRecord<STypes::Playlist>(STypes::Playlist.TableName, predicate);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("プレイリストの取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("プレイリストの取得に失敗しました。");
                }

                return AttemptResult<STypes::Playlist>.Fail("プレイリストの取得に失敗しました。");
            }


            if (result.Data.Videos.Count > 0 && result.Data.CustomVideoSequence.Count != result.Data.Videos.Count)
            {
                var ids = result.Data.Videos.Select(v => v.Id).Where(id => !result.Data.CustomVideoSequence.Contains(id));
                result.Data.CustomVideoSequence.AddRange(ids);
            }

            this.logger.Log($"プレイリスト(name:{result.Data.PlaylistName}, ID:{result.Data.Id})を取得しました。");
            return AttemptResult<STypes::Playlist>.Succeeded(result.Data);
        }


        public IAttemptResult<STypes::Playlist> GetRootPlaylist()
        {
            var result = this.databaseInstance.GetRecord<STypes::Playlist>(STypes::Playlist.TableName, playlist => playlist.IsRoot);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("ルートプレイリストの取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("ルートプレイリストの取得に失敗しました。");
                }

                return AttemptResult<STypes::Playlist>.Fail("ルートプレイリストの取得に失敗しました。");
            }

            this.logger.Log("ルートプレイリストを取得しました。");
            return AttemptResult<STypes::Playlist>.Succeeded(result.Data);
        }

        public IAttemptResult<List<STypes::Playlist>> GetChildPlaylists(int id)
        {
            var result = this.databaseInstance.GetAllRecords<STypes::Playlist>(STypes::Playlist.TableName, p => p.ParentPlaylist is null ? false : p.ParentPlaylist.Id == id);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("子プレイリストの取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("子プレイリストの取得に失敗しました。");
                }

                return AttemptResult<List<STypes::Playlist>>.Fail("子プレイリストの取得に失敗しました。");
            }

            this.logger.Log($"{id}の子プレイリストを取得しました。");
            return AttemptResult<List<STypes::Playlist>>.Succeeded(result.Data);

        }

        public IAttemptResult<List<STypes::Playlist>> GetAllPlaylists()
        {
            var result = this.databaseInstance.GetAllRecords<STypes::Playlist>(STypes::Playlist.TableName);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("全プレイリストの取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("全プレイリストの取得に失敗しました。");
                }

                return AttemptResult<List<STypes::Playlist>>.Fail("全プレイリストの取得に失敗しました。");
            }

            this.logger.Log("すべてのプレイリストを取得しました。");
            return AttemptResult<List<STypes::Playlist>>.Succeeded(result.Data);
        }

        public IAttemptResult<int> AddPlaylist(int parentId, string name)
        {

            if (this.Exists(parentId))
            {
                var parent = this.databaseInstance.GetRecord<STypes::Playlist>(STypes::Playlist.TableName, parentId);

                if (!parent.IsSucceeded || parent.Data is null)
                {
                    if (parent.Exception is not null)
                    {
                        this.logger.Error("親プレイリストの取得に失敗しました。", parent.Exception);
                    }
                    else
                    {
                        this.logger.Error("親プレイリストの取得に失敗しました。");
                    }
                    return AttemptResult<int>.Fail("親プレイリストの取得に失敗しました。");
                }

                //動画を保持している、特殊なプレイリスト場合はキャンセル
                if (parent.Data!.IsConcretePlaylist || parent.Data.IsTemporary || parent.Data.IsDownloadFailedHistory || parent.Data.IsDownloadSucceededHistory) return AttemptResult<int>.Fail("指定したプレイリストは子プレイリストを持つことができません。");

                //5階層よりも深い場合はキャンセル
                if (parent.Data.Layer > 5) return AttemptResult<int>.Fail("5階層よりも深くすることはできません。");

                var playlist = new STypes::Playlist(parent.Data)
                {
                    PlaylistName = name
                };

                var storeResult = this.databaseInstance.Store(playlist, STypes::Playlist.TableName);

                if (!storeResult.IsSucceeded)
                {

                    if (storeResult.Exception is not null)
                    {
                        this.logger.Error("プレイリストの保存に失敗しました。", storeResult.Exception);
                    }
                    else
                    {
                        this.logger.Error("プレイリストの保存に失敗しました。");

                    }

                    return AttemptResult<int>.Fail("プレイリストの保存に失敗しました。");
                }

                this.logger.Log($"新規プレイリスト`{name}`を追加しました。");

                return AttemptResult<int>.Succeeded(storeResult.Data);
            }
            else
            {
                this.logger.Error("親プレイリストが存在しなかったため新規プレイリストを保存できませんでした。");
                return AttemptResult<int>.Fail("親プレイリストが存在しなかったため新規プレイリストを保存できませんでした。");
            }

        }

        public IAttemptResult Update(STypes::Playlist playlist)
        {
            if (!this.Exists(playlist.Id))
            {
                this.logger.Error($"存在しないプレイリストに対して更新が試行されました。(name:{playlist.PlaylistName}, ID:{playlist.Id})");
                return AttemptResult.Fail("存在しないプレイリストに対して更新が試行されました。");
            }

            this.databaseInstance.Update(playlist, STypes::Playlist.TableName);

            this.logger.Log($"プレイリスト(name:{playlist.PlaylistName}, ID:{playlist.Id})を更新しました。");

            return AttemptResult.Succeeded();
        }

        public IAttemptResult DeletePlaylist(int playlistID)
        {

            if (!this.Exists(playlistID))
            {
                this.logger.Error("存在しないプレイリストに対して削除が試行されました。");
                return AttemptResult.Fail("存在しないプレイリストに対して削除が試行されました。");
            }

            IAttemptResult<STypes::Playlist> result = this.GetPlaylist(playlistID);

            if (!result.IsSucceeded || result.Data is null)
            {
                this.logger.Error("削除対象のプレイリストの取得に失敗しました。");
                return AttemptResult.Fail("削除対象のプレイリストの取得に失敗しました。");
            }

            STypes::Playlist playlist = result.Data;


            //ルートプレイリストは削除禁止
            if (playlist.IsRoot || playlist.Layer == 1 || playlist!.IsTemporary || playlist!.IsDownloadFailedHistory || playlist!.IsDownloadSucceededHistory)
            {
                this.logger.Log($"削除できないプレイリストに対する削除が試行されました。(isRoot:{playlist.IsRoot}, layer:{playlist.Layer}, isTmp:{playlist.IsTemporary}, isFailed:{ playlist.IsDownloadFailedHistory}, IsSucceeeded:{playlist.IsDownloadSucceededHistory})");
                return AttemptResult.Fail("削除できないプレイリストに対する削除が試行されました。");
            }

            IAttemptResult cResult = this.RemoveChildPlaylist(playlist);
            if (!cResult.IsSucceeded)
            {
                return cResult;
            }

            IAttemptResult<bool> deleteResult = this.databaseInstance.Delete(STypes::Playlist.TableName, playlistID);
            if (!deleteResult.IsSucceeded || !deleteResult.Data)
            {
                return AttemptResult.Fail("プレイリストの削除に失敗しました。");
            }

            this.logger.Log($"{playlistID}を削除しました。");

            return AttemptResult.Succeeded();
        }
        #endregion

        #region プレイリスト情報

        public bool Exists(int id)
        {
            return this.databaseInstance.Exists<STypes::Playlist>(STypes::Playlist.TableName, id);
        }

        public bool Exists(Expression<Func<STypes::Playlist, bool>> predicate)
        {
            return this.databaseInstance.Exists(STypes::Playlist.TableName, predicate);
        }

        public IAttemptResult SetAsRemotePlaylist(int playlistId, string remoteId, RemoteType type)
        {
            if (!this.Exists(playlistId)) return new AttemptResult() { Message = $"指定されたプレイリストは存在しません。(id={playlistId})" };

            IAttemptResult<STypes::Playlist> pResult = this.GetPlaylist(playlistId);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail("プレイリストの取得に失敗しました。");
            }

            STypes::Playlist playlist = pResult.Data;

            playlist!.IsRemotePlaylist = true;
            playlist.RemoteId = remoteId;

            playlist.IsMylist = false;
            playlist.IsUserVideos = false;
            playlist.IsWatchLater = false;
            playlist.IsChannel = false;
            playlist.IsSeries = false;

            switch (type)
            {
                case RemoteType.Mylist:
                    playlist.IsMylist = true;
                    break;
                case RemoteType.UserVideos:
                    playlist.IsUserVideos = true;
                    break;
                case RemoteType.WatchLater:
                    playlist.IsWatchLater = true;
                    break;
                case RemoteType.Channel:
                    playlist.IsChannel = true;
                    break;
                case RemoteType.Series:
                    playlist.IsSeries = true;
                    break;
            }

            IAttemptResult uResult = this.Update(playlist);

            if (!uResult.IsSucceeded)
            {
                return uResult;
            }

            this.logger.Log($"{playlistId}をリモートプレイリスト({type})に設定しました。");

            return AttemptResult.Succeeded();
        }

        public IAttemptResult SetAsLocalPlaylist(int playlistId)
        {
            if (!this.Exists(playlistId)) return new AttemptResult() { Message = $"指定されたプレイリストは存在しません。(id={playlistId})" };

            IAttemptResult<STypes::Playlist> pResult = this.GetPlaylist(playlistId);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail("プレイリストの取得に失敗しました。");
            }

            STypes::Playlist playlist = pResult.Data;

            playlist.IsRemotePlaylist = false;
            playlist.IsMylist = false;
            playlist.IsUserVideos = false;
            playlist.IsWatchLater = false;
            playlist.IsChannel = false;
            playlist.IsSeries = false;

            IAttemptResult uResult = this.Update(playlist);

            if (!uResult.IsSucceeded)
            {
                return uResult;
            }

            this.logger.Log($"{playlistId}をローカルプレイリストに設定しました。");

            return AttemptResult.Succeeded();
        }

        public IAttemptResult Move(int id, int destId)
        {
            return this.InternalMove(id, destId, false);
        }

        /// <summary>
        /// プレイリストをコピーする(未実装)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destId"></param>
        public void Copy(int id, int destId)
        {
            throw new NotImplementedException();
            //this.InternalMove(id, destId, true);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        public IAttemptResult Initialize()
        {
            if (!this.Exists(p => p.IsRoot))
            {
                var root = new STypes::Playlist()
                {
                    IsRoot = true,
                    PlaylistName = "プレイリスト一覧",
                };
                var result = this.databaseInstance.Store(root, STypes::Playlist.TableName);
                if (!result.IsSucceeded)
                {
                    if (result.Exception is not null)
                    {
                        this.logger.Error($"ルートプレイリストの保存に失敗しました。({result.Message})", result.Exception);
                    }
                    else
                    {
                        this.logger.Error($"ルートプレイリストの保存に失敗しました。({result.Message})");
                    }
                    return new AttemptResult() { Message = "ルートプレイリストの保存に失敗しました。", Exception = result.Exception };
                }

                this.logger.Log("ルートプレイリストが存在しなかったので作成しました。");
            }

            if (!this.Exists(p => p.IsDownloadSucceededHistory))
            {
                var succeeded = new STypes::Playlist()
                {
                    IsDownloadSucceededHistory = true,
                    PlaylistName = "最近ダウンロードした動画",
                };
                var result = this.databaseInstance.Store(succeeded, STypes::Playlist.TableName);
                if (!result.IsSucceeded)
                {
                    if (result.Exception is not null)
                    {
                        this.logger.Error($"DL成功履歴の保存に失敗しました。({result.Message})", result.Exception);
                    }
                    else
                    {
                        this.logger.Error($"DL成功履歴の保存に失敗しました。({result.Message})");
                    }
                    return new AttemptResult() { Message = "DL成功履歴の保存に失敗しました。", Exception = result.Exception };
                }

                this.logger.Log("DL成功履歴が存在しなかったので作成しました。");
            }

            if (!this.Exists(p => p.IsDownloadFailedHistory))
            {
                var failed = new STypes::Playlist()
                {
                    IsDownloadFailedHistory = true,
                    PlaylistName = "最近ダウンロードに失敗した動画",
                };
                var result = this.databaseInstance.Store(failed, STypes::Playlist.TableName);
                if (!result.IsSucceeded)
                {
                    if (result.Exception is not null)
                    {
                        this.logger.Error($"DL失敗履歴の保存に失敗しました。({result.Message})", result.Exception);
                    }
                    else
                    {
                        this.logger.Error($"DL失敗履歴の保存に失敗しました。({result.Message})");
                    }
                    return new AttemptResult() { Message = "DL失敗履歴の保存に失敗しました。", Exception = result.Exception };
                }

                this.logger.Log("DL失敗履歴が存在しなかったので作成しました。");
            }

            if (!this.Exists(p => p.IsTemporary))
            {
                var failed = new STypes::Playlist()
                {
                    IsTemporary = true,
                    PlaylistName = "一時プレイリスト",
                };
                var result = this.databaseInstance.Store(failed, STypes::Playlist.TableName);
                if (!result.IsSucceeded)
                {
                    if (result.Exception is not null)
                    {
                        this.logger.Error($"一時プレイリストの保存に失敗しました。({result.Message})", result.Exception);
                    }
                    else
                    {
                        this.logger.Error($"一時プレイリストの保存に失敗しました。({result.Message})");
                    }
                    return new AttemptResult() { Message = "一時プレイリストの保存に失敗しました。", Exception = result.Exception };
                }

                this.logger.Log("一時プレイリストが存在しなかったので作成しました。");
            }

            IAttemptResult<List<STypes::Playlist>> pResult = this.GetAllPlaylists();
            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail("プレイリストの取得に失敗しました。");
            }

            foreach (var playlist in pResult.Data)
            {
                this.FixPlaylist(playlist);
            }

            return AttemptResult.Succeeded();
        }

        #endregion

        #region 動画操作系メソッド
        public IAttemptResult WireVideo(STypes::Video video, int playlistId)
        {
            if (!this.Exists(playlistId))
            {
                return AttemptResult.Fail("指定したプレイリストが存在しません。");
            }

            IAttemptResult<STypes::Playlist> pResult = this.GetPlaylist(playlistId);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail("プレイリストの取得に失敗しました。");
            }

            STypes::Playlist playlist = pResult.Data;


            if (playlist.Videos is null) playlist.Videos = new List<STypes.Video>();

            if (playlist.Videos.Any(v => v.NiconicoId == video.NiconicoId))
            {
                return AttemptResult.Fail("すでに登録されている動画です。");
            }

            playlist.Videos.Add(video);
            playlist.CustomVideoSequence.Add(video.Id);

            IAttemptResult uResult = this.databaseInstance.Update(playlist, STypes::Playlist.TableName);

            if (!uResult.IsSucceeded)
            {
                return AttemptResult.Fail($"動画の追加に失敗しました。（詳細：{uResult.ExceptionMessage}）");
            }

            this.logger.Log($"{video.NiconicoId}をプレイリスト({playlist.PlaylistName})に追加しました。");

            return AttemptResult.Succeeded();
        }

        public IAttemptResult UnWireVideo(int videoId, int playlistId)
        {
            if (!this.Exists(playlistId))
            {
                return AttemptResult.Fail("指定したプレイリストが存在しません。");
            }

            IAttemptResult<STypes::Playlist> pResult = this.GetPlaylist(playlistId);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail("プレイリストの取得に失敗しました。");
            }

            STypes::Playlist playlist = pResult.Data;

            playlist.Videos.RemoveAll(v => v.Id == videoId);
            playlist.CustomVideoSequence.RemoveAll(v => v == videoId);

            var uResult = this.databaseInstance.Update(playlist, STypes::Playlist.TableName);

            if (!uResult.IsSucceeded)
            {
                return AttemptResult.Fail($"動画の削除に失敗しました。(詳細:{uResult.ExceptionMessage})");
            }

            this.logger.Log($"{playlist.PlaylistName}からの動画({videoId})を削除しました。");

            return AttemptResult.Succeeded();

        }

        public IAttemptResult MoveVideoToPrev(int playlistID, int videoIndex)
        {
            if (!this.Exists(playlistID)) return new AttemptResult() { Message = $"指定されたプレイリストは存在しません。(id={playlistID})" };

            IAttemptResult<STypes::Playlist> pResult = this.GetPlaylist(playlistID);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail("プレイリストの取得に失敗しました。");
            }

            STypes::Playlist playlist = pResult.Data;

            if (playlist.Videos.Count < videoIndex + 1) return new AttemptResult() { Message = $"指定されたインデックスは範囲外です。(index={videoIndex}, actual={playlist.Videos.Count})" };

            if (videoIndex == 0) return new AttemptResult() { Message = $"指定されたインデックスは最初の動画です。(index={videoIndex})" };

            try
            {
                playlist.CustomVideoSequence.InsertIntoPrev(videoIndex);
            }
            catch (Exception e)
            {
                return new AttemptResult() { Message = $"挿入操作に失敗しました。", Exception = e };
            }

            playlist.SortType = STypes.VideoSortType.Custom;

            this.Update(playlist);

            this.logger.Log($"{playlistID}の{videoIndex + 1}番目の動画を一つ後ろに挿入しました。");
            return new AttemptResult() { IsSucceeded = true };
        }

        public IAttemptResult MoveVideoToForward(int playlistID, int videoIndex)
        {

            IAttemptResult<STypes::Playlist> pResult = this.GetPlaylist(playlistID);

            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult.Fail("プレイリストの取得に失敗しました。");
            }

            STypes::Playlist playlist = pResult.Data;

            if (playlist.Videos.Count < videoIndex + 1) return new AttemptResult() { Message = $"指定されたインデックスは範囲外です。(index={videoIndex}, actual={playlist.Videos.Count})" };

            if (videoIndex + 1 == playlist.Videos.Count) return new AttemptResult() { Message = $"指定されたインデックスは最後の動画です。(index={videoIndex})" };

            try
            {
                playlist.CustomVideoSequence.InsertIntoForward(videoIndex);
            }
            catch (Exception e)
            {
                return new AttemptResult() { Message = $"挿入操作に失敗しました。", Exception = e };
            }

            playlist.SortType = STypes.VideoSortType.Custom;

            this.Update(playlist);

            this.logger.Log($"{playlistID}の{videoIndex + 1}番目の動画を一つ前に挿入しました。");
            return new AttemptResult() { IsSucceeded = true };
        }

        #endregion

        #region private

        private IAttemptResult FixPlaylist(STypes::Playlist playlist)
        {
            //親が存在しないプレイリスト(孤児プレイリスト)の場合はそれ自体を削除
            if (playlist.ParentPlaylist is not null && !this.Exists(playlist.ParentPlaylist.Id))
            {
                IAttemptResult dResult = this.DeletePlaylist(playlist.Id);

                if (!dResult.IsSucceeded)
                {
                    return dResult;
                }

                this.logger.Log($"孤児プレイリスト(name:{playlist.PlaylistName}, ID:{playlist.Id})を削除しました。");
                return AttemptResult.Succeeded();
            }

            //並び替え情報を実際の動画と揃える
            if (playlist.Videos.Count > 0 && playlist.Videos.Count != playlist.CustomVideoSequence.Count)
            {

                if (playlist.Videos.Count > playlist.CustomVideoSequence.Count)
                {
                    var videosToAdd = playlist.Videos.Select(v => v.Id).Where(id => !playlist.CustomVideoSequence.Contains(id));
                    playlist.CustomVideoSequence.AddRange(videosToAdd);
                }
                else
                {
                    var videoIDs = playlist.Videos.Select(v => v.Id).ToList();
                    var newSequence = playlist.CustomVideoSequence.Where(id => videoIDs.Contains(id));
                    playlist.CustomVideoSequence.Clear();
                    playlist.CustomVideoSequence.AddRange(newSequence);
                }

                IAttemptResult uResult = this.Update(playlist);

                if (!uResult.IsSucceeded)
                {
                    return uResult;
                }

                this.logger.Log($"並び替え順情報を登録されている動画数と同期しました。(id:{playlist.Id})");
            }

            return AttemptResult.Succeeded();

        }

        /// <summary>
        /// 子プレイリストを辿って削除する
        /// </summary>
        /// <param name="parentId"></param>
        private IAttemptResult RemoveChildPlaylist(STypes::Playlist self)
        {
            if (self.IsConcretePlaylist)
            {
                return AttemptResult.Succeeded();
            }

            IAttemptResult<List<STypes::Playlist>> cResult = this.GetChildPlaylists(self.Id);

            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult.Fail("子プレイリストの取得に失敗しました。");
            }

            foreach (var childPlaylist in cResult.Data)
            {
                IAttemptResult crResult = this.RemoveChildPlaylist(childPlaylist);
                if (!crResult.IsSucceeded)
                {
                    return AttemptResult.Fail("子孫プレイリストの削除に失敗しました。");
                }

                IAttemptResult dResul = this.DeletePlaylist(childPlaylist.Id);
                if (!dResul.IsSucceeded)
                {
                    return AttemptResult.Fail("子プレイリストの削除に失敗しました。");
                }

                this.logger.Log($"{self.Id}の子プレイリストを削除しました。");
            }

            return AttemptResult.Succeeded();

        }

        /// <summary>
        /// プレイリストのコピー・移動を行う内部メソッド
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destId"></param>
        /// <param name="IsCopy"></param>
        private IAttemptResult InternalMove(int id, int destId, bool IsCopy)
        {
            //移動先と異動元が同一のプレイリストの場合キャンセル
            //(自分に自分を入れることは出来ない。)
            if (id == destId)
            {
                return AttemptResult.Fail("移動先と移動元が同じです。");
            }

            IAttemptResult<STypes::Playlist> rDestination = this.GetPlaylist(destId)!;
            IAttemptResult<STypes::Playlist> rTarget = this.GetPlaylist(id)!;

            if (!rDestination.IsSucceeded || rDestination.Data is null || !rTarget.IsSucceeded || rTarget.Data is null)
            {
                return AttemptResult.Fail("プレイリストの取得に失敗しました。");
            }

            STypes::Playlist destination = rDestination.Data;
            STypes::Playlist target = rTarget.Data;

            //フォルダー・プレイリストでない場合はキャンセル
            if (destination.IsConcretePlaylist)
            {
                return AttemptResult.Fail("移動先のプレイリストが不適切です。");
            }


            if (IsCopy)
            {
                STypes::Playlist newtarget = target with { ParentPlaylist = destination };
                return this.databaseInstance.Store(newtarget, STypes::Playlist.TableName);
            }
            else
            {
                target.ParentPlaylist = destination;
                this.logger.Log($"プレイリスト(name:{target.PlaylistName}, ID:{target.Id})を{target.ParentPlaylist.PlaylistName}から{destination.PlaylistName}に移動しました。");
                return this.Update(target);
            }

        }

        /// <summary>
        /// データをセットする
        /// </summary>
        /// <param name="dbPlaylist"></param>
        /// <param name="playlistInfo"></param>
        private void SetData(STypes::Playlist dbPlaylist, ITreePlaylistInfo playlistInfo)
        {
            dbPlaylist.PlaylistName = playlistInfo.Name.Value;
            dbPlaylist.FolderPath = playlistInfo.Folderpath;
            dbPlaylist.IsExpanded = playlistInfo.IsExpanded;
            dbPlaylist.CustomVideoSequence.Clear();
            dbPlaylist.CustomVideoSequence.AddRange(playlistInfo.CustomSortSequence);
            dbPlaylist.SortType = playlistInfo.VideoSortType;
            dbPlaylist.IsVideoDescending = playlistInfo.IsVideoDescending;
            dbPlaylist.IsTemporary = playlistInfo.IsTemporary;
            dbPlaylist.IsDownloadSucceededHistory = playlistInfo.IsDownloadSucceededHistory;
            dbPlaylist.IsDownloadFailedHistory = playlistInfo.IsDownloadFailedHistory;
            dbPlaylist.BookMarkedVideoID = playlistInfo.BookMarkedVideoID;
        }

        #endregion
    }
}
