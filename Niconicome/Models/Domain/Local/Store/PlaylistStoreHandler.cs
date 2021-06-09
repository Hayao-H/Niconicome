using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Domain.Local.Store
{
    public interface IPlaylistStoreHandler
    {
        STypes::Playlist GetRootPlaylist();
        STypes::Playlist? GetPlaylist(int id);
        STypes::Playlist? GetPlaylist(Expression<Func<STypes::Playlist, bool>> predicate);
        public int AddPlaylist(int parentID, string name);
        public int AddVideo(IListVideoInfo video, int playlistId);
        public void RemoveVideo(int id, int playlistId);
        public void Update(ITreePlaylistInfo newplaylist);
        List<STypes::Playlist> GetChildPlaylists(STypes::Playlist self);
        List<STypes::Playlist> GetChildPlaylists(int id);
        List<STypes::Playlist> GetAllPlaylists();
        IAttemptResult MoveVideoToPrev(int playlistID, int videoIndex);
        IAttemptResult MoveVideoToForward(int playlistID, int videoIndex);
        void DeletePlaylist(int id);
        bool Exists(int id);
        bool Exists(Expression<Func<STypes::Playlist, bool>> predicate);
        bool JustifyPlaylists(IEnumerable<int> Id);
        bool ContainsVideo(string niconicoId, int playlistId);
        void Move(int id, int destId);
        void Copy(int id, int destId);
        void SetAsRemotePlaylist(int id, string remoteId, RemoteType type);
        void SetAsLocalPlaylist(int id);
        int GetPlaylistsCount();
        void Refresh();
        IAttemptResult Initialize();
    }

    public class PlaylistStoreHandler : IPlaylistStoreHandler
    {
        public PlaylistStoreHandler(IDataBase dataBase, IVideoStoreHandler videoHandler, ILogger logger)
        {
            this.databaseInstance = dataBase;
            this.videoHandler = videoHandler;
            this.logger = logger;
        }

        #region field
        private readonly IDataBase databaseInstance;

        private readonly IVideoStoreHandler videoHandler;

        private readonly ILogger logger;
        #endregion

        #region CRUD
        /// <summary>
        /// プレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public STypes::Playlist? GetPlaylist(int id)
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

                return null;
            }


            if (result.Data.Videos.Count > 0 && result.Data.CustomVideoSequence.Count != result.Data.Videos.Count)
            {
                var ids = result.Data.Videos.Select(v => v.Id).Where(id => !result.Data.CustomVideoSequence.Contains(id));
                result.Data.CustomVideoSequence.AddRange(ids);
            }

            this.logger.Log($"プレイリスト(name:{result.Data.PlaylistName}, ID:{id})を取得しました。");
            return result.Data;

        }

        /// <summary>
        /// 指定した条件でプレイリストを取得する
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public STypes::Playlist? GetPlaylist(Expression<Func<STypes::Playlist, bool>> predicate)
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

                return null;
            }


            if (result.Data.Videos.Count > 0 && result.Data.CustomVideoSequence.Count != result.Data.Videos.Count)
            {
                var ids = result.Data.Videos.Select(v => v.Id).Where(id => !result.Data.CustomVideoSequence.Contains(id));
                result.Data.CustomVideoSequence.AddRange(ids);
            }

            this.logger.Log($"プレイリスト(name:{result.Data.PlaylistName}, ID:{result.Data.Id})を取得しました。");
            return result.Data;
        }


        /// <summary>
        /// ルートレベルプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public STypes::Playlist GetRootPlaylist()
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

                return new STypes::Playlist();
            }

            this.logger.Log("ルートプレイリストを取得しました。");
            return result.Data;
        }

        /// <summary>
        /// 子プレイリストを取得する
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public List<STypes::Playlist> GetChildPlaylists(STypes::Playlist self)
        {
            return this.GetChildPlaylists(self.Id);
        }

        /// <summary>
        /// 子プレイリストをIDから取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<STypes::Playlist> GetChildPlaylists(int id)
        {
            var result = this.databaseInstance.GetAllRecords<STypes::Playlist>(STypes::Playlist.TableName, p => p.ParentPlaylist is null ? false : p.ParentPlaylist.Id == id);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("子プレイリストの更新に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("子プレイリストの更新に失敗しました。");
                }

                return new List<STypes::Playlist>();
            }

            this.logger.Log($"{id}の子プレイリストを取得しました。");
            return result.Data;

        }

        /// <summary>
        /// 全てのプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public List<STypes::Playlist> GetAllPlaylists()
        {
            var result = this.databaseInstance.GetAllRecords<STypes::Playlist>(STypes::Playlist.TableName);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("全プレイリストの更新に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("全プレイリストの更新に失敗しました。");
                }

                return new List<STypes::Playlist>();
            }

            this.logger.Log("すべてのプレイリストを取得しました。");
            return result.Data;
        }

        /// <summary>
        /// プレイリストを追加する
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="name"></param>
        public int AddPlaylist(int parentId, string name)
        {

            if (this.databaseInstance.Exists<STypes::Playlist>(STypes::Playlist.TableName, parentId))
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
                    return -1;
                }

                //動画を保持している場合はキャンセル
                if (parent.Data!.IsConcretePlaylist) return -1;

                //5階層よりも深い場合はキャンセル
                if (parent.Data.Layer > 5) return -1;

                //特殊なプレイリストの場合はキャンセル
                if (parent.Data.IsTemporary || parent.Data.IsDownloadFailedHistory || parent.Data.IsDownloadSucceededHistory) return -1;

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

                    return -1;
                }


                var updateResult = this.databaseInstance.Update(playlist, STypes::Playlist.TableName);

                if (!updateResult.IsSucceeded)
                {
                    if (parent.Exception is not null)
                    {
                        this.logger.Error("親プレイリストの更新に失敗しました。", parent.Exception);
                    }
                    else
                    {
                        this.logger.Error("親プレイリストの更新に失敗しました。");
                    }
                    return -1;
                }

                this.logger.Log($"新規プレイリスト`{name}`を追加しました。");

                return storeResult.Data;
            }
            else
            {
                this.logger.Error($"親プレイリストが存在しなかったため新規プレイリストを保存できませんでした。");
                return -1;
            }

        }

        /// <summary>
        /// プレイリスト情報を更新する
        /// </summary>
        /// <param name="newPlaylist"></param>
        public void Update(ITreePlaylistInfo newPlaylist)
        {
            var dbPlaylist = this.GetPlaylist(newPlaylist.Id);
            if (dbPlaylist is null)
            {
                this.logger.Error($"存在しないプレイリストに対して更新が試行されました。(name:{newPlaylist.Name}, ID:{newPlaylist.Id})");
            }
            this.SetData(dbPlaylist!, newPlaylist);
            this.Update(dbPlaylist!);

            this.logger.Log($"プレイリスト(name:{newPlaylist.Name}, ID:{newPlaylist.Id})を更新しました。");
        }

        /// <summary>
        /// プレイリストを削除する
        /// </summary>
        /// <param name="playlistID"></param>
        public void DeletePlaylist(int playlistID)
        {
            var playlist = this.GetPlaylist(playlistID);

            if (playlist is null)
            {
                this.logger.Error("存在しないプレイリストに対して削除が試行されました。");
            }


            //ルートプレイリストは削除禁止
            if (playlist!.IsRoot || playlist.Layer == 1 || playlist!.IsTemporary || playlist!.IsDownloadFailedHistory || playlist!.IsDownloadSucceededHistory)
            {
                this.logger.Log($"削除できないプレイリストに対する削除が試行されました。(isRoot:{playlist.IsRoot}, layer:{playlist.Layer}, isTmp:{playlist.IsTemporary}, isFailed:{ playlist.IsDownloadFailedHistory}, IsSucceeeded:{playlist.IsDownloadSucceededHistory})");
                return;
            }

            this.RemoveChildPlaylist(playlistID);

            var result = this.databaseInstance.Delete(STypes::Playlist.TableName, playlistID);
            if (!result.IsSucceeded || !result.Data)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error($"{playlistID}の削除に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error($"{playlistID}の削除に失敗しました。");
                }
                return;
            }

            this.logger.Log($"{playlistID}を削除しました。");
        }
        #endregion

        #region プレイリスト情報

        /// <summary>
        /// プレイリストの存在をチェックする
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(int id)
        {
            return this.databaseInstance.Exists<STypes::Playlist>(STypes::Playlist.TableName, id);
        }

        /// <summary>
        ///関数でプレイリストの存在をチェックする
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Exists(Expression<Func<STypes::Playlist, bool>> predicate)
        {
            return this.databaseInstance.Exists(STypes::Playlist.TableName, predicate);
        }



        /// <summary>
        /// データベース上のプレイリスト数を取得
        /// </summary>
        /// <returns></returns>
        public int GetPlaylistsCount()
        {
            var result = this.databaseInstance.GetRecordCount(STypes::Playlist.TableName);

            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("レコード数の更新に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("レコード数の更新に失敗しました。");
                }

                return -1;
            }

            return result.Data;
        }
        #endregion

        /// <summary>
        /// リモートプレイリストとして登録する
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="remoteId"></param>
        /// <param name="type"></param>
        public void SetAsRemotePlaylist(int playlistId, string remoteId, RemoteType type)
        {
            if (this.Exists(playlistId))
            {
                var playlist = this.GetPlaylist(playlistId);
                playlist!.IsRemotePlaylist = true;
                playlist.RemoteId = remoteId;

                switch (type)
                {
                    case RemoteType.Mylist:
                        playlist.IsMylist = true;
                        playlist.IsUserVideos = false;
                        playlist.IsWatchLater = false;
                        playlist.IsChannel = false;
                        break;
                    case RemoteType.UserVideos:
                        playlist.IsUserVideos = true;
                        playlist.IsMylist = false;
                        playlist.IsWatchLater = false;
                        playlist.IsChannel = false;
                        break;
                    case RemoteType.WatchLater:
                        playlist.IsUserVideos = false;
                        playlist.IsMylist = false;
                        playlist.IsWatchLater = true;
                        playlist.IsChannel = false;
                        break;
                    case RemoteType.Channel:
                        playlist.IsUserVideos = false;
                        playlist.IsMylist = false;
                        playlist.IsWatchLater = false;
                        playlist.IsChannel = true;
                        break;
                }
                this.Update(playlist);
                this.logger.Log($"{playlistId}をリモートプレイリスト({type})に設定しました。");
            }
            else
            {
                this.logger.Error($"存在しないプレイリストをリモートプレイリストに設定しようと試行しました。");
            }
        }

        /// <summary>
        /// ローカルプレイリストとして設定する
        /// </summary>
        /// <param name="playlistId"></param>
        public void SetAsLocalPlaylist(int playlistId)
        {
            if (this.Exists(playlistId))
            {
                var playlist = this.GetPlaylist(playlistId);
                playlist!.IsRemotePlaylist = false;
                playlist.IsMylist = false;
                playlist.IsUserVideos = false;
                playlist.IsWatchLater = false;
                this.Update(playlist);
                this.logger.Log($"{playlistId}をローカルプレイリストに設定しました。");
            }
            else
            {
                this.logger.Error($"存在しないプレイリストをローカルプレイリストに設定しようと試行しました。");
            }
        }

        /// <summary>
        /// プレイリストを移動する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destId"></param>
        public void Move(int id, int destId)
        {
            this.InternalMove(id, destId, false);
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
        /// データベースのレコードを更新する
        /// </summary>
        public void Refresh()
        {
            IAttemptResult result = this.Initialize();
            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error($"プレイリストのリフレッシュに失敗しました。({result.Message})", result.Exception);
                }
                else
                {
                    this.logger.Error($"プレイリストのリフレッシュに失敗しました。({result.Message})");
                }
            }
            else
            {
                this.logger.Log("プレイリストがリフレッシュされました。");
            }
        }

        /// <summary>
        /// プレイリストの不整合性を修復する
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool JustifyPlaylists(IEnumerable<int> ids)
        {
            return !ids.Select(id => this.JustifyPlaylists(id)).Any(ok => ok == false);
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

            return new AttemptResult() { IsSucceeded = true };
        }


        #region 動画操作系メソッド

        /// <summary>
        /// 指定されたIDの動画を含むかどうかを返す
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        public bool ContainsVideo(string niconicoId, int playlistId)
        {
            var playlist = this.GetPlaylist(playlistId);
            if (playlist is null) return false;
            return playlist.Videos.Any(v => v.NiconicoId == niconicoId);
        }

        /// <summary>
        /// プレイリストに動画を追加する
        /// </summary>
        /// <param name="videoData"></param>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        public int AddVideo(IListVideoInfo videoData, int playlistId)
        {
            if (!this.Exists(playlistId)) throw new InvalidOperationException($"指定したプレイリストが存在しません。(id:{playlistId})");

            var playlist = this.GetPlaylist(playlistId);
            int videoId = this.videoHandler.AddVideo(videoData, playlistId);
            var video = this.videoHandler.GetVideo(videoId)!;

            if (playlist!.Videos is null) playlist.Videos = new List<STypes.Video>();

            if (playlist.Videos.Any(v => v.NiconicoId == videoData.NiconicoId.Value)) return -1;

            playlist.Videos.Add(video);
            playlist.CustomVideoSequence.Add(video.Id);

            var pResult = this.databaseInstance.Update(playlist, STypes::Playlist.TableName);

            if (!pResult.IsSucceeded)
            {
                if (pResult.Exception is not null)
                {
                    this.logger.Error($"{playlist.PlaylistName}への動画({videoData.NiconicoId})の追加に失敗しました。", pResult.Exception);
                }
                else
                {
                    this.logger.Error($"{playlist.PlaylistName}への動画({videoData.NiconicoId})の追加に失敗しました。");
                }
                return -1;
            }

            this.logger.Log($"{videoData.NiconicoId}をプレイリスト({playlist.PlaylistName})に追加しました。");

            return videoId;
        }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="playlistId"></param>
        public void RemoveVideo(int videoId, int playlistId)
        {
            if (!this.Exists(playlistId)) throw new InvalidOperationException($"指定したプレイリストが存在しません。(id:{playlistId})");
            if (!this.videoHandler.Exists(videoId)) throw new InvalidOperationException($"指定した動画が存在しません。(id:{playlistId})");

            var vieo = this.videoHandler.GetVideo(videoId);
            var playlist = this.GetPlaylist(playlistId);

            this.videoHandler.RemoveVideo(videoId, playlistId);
            playlist!.Videos.RemoveAll(v => v.Id == videoId);
            playlist!.CustomVideoSequence.RemoveAll(v => v == videoId);

            var result = this.databaseInstance.Update(playlist, STypes::Playlist.TableName);

            if (!result.IsSucceeded)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error($"{playlist.PlaylistName}からの動画({videoId})の削除に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error($"{playlist.PlaylistName}からの動画({videoId})の削除に失敗しました。");
                }
            }

            this.logger.Log($"{playlist.PlaylistName}からの動画({videoId})を削除しました。");

        }

        /// <summary>
        /// 動画を後ろに挿入する
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="videoIndex"></param>
        /// <returns></returns>
        public IAttemptResult MoveVideoToPrev(int playlistID, int videoIndex)
        {
            if (!this.Exists(playlistID)) return new AttemptResult() { Message = $"指定されたプレイリストは存在しません。(id={playlistID})" };

            var playlist = this.GetPlaylist(playlistID)!;

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

        /// <summary>
        /// 動画をうしろに挿入する
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="videoIndex"></param>
        /// <returns></returns>
        public IAttemptResult MoveVideoToForward(int playlistID, int videoIndex)
        {
            if (!this.Exists(playlistID)) return new AttemptResult() { Message = $"指定されたプレイリストは存在しません。(id={playlistID})" };

            var playlist = this.GetPlaylist(playlistID)!;

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

        /// <summary>
        /// プレイリストの不整合を修復する
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool JustifyPlaylists(int Id)
        {
            var playlist = this.GetPlaylist(Id);

            if (playlist is null)
            {
                return false;
            }
            else
            {
                //親が存在しないプレイリスト(孤児プレイリスト)の場合はそれ自体を削除
                if (playlist.ParentPlaylist is not null && !this.databaseInstance.Exists<STypes::Playlist>(STypes::Playlist.TableName, playlist.ParentPlaylist.Id))
                {
                    this.DeletePlaylist(playlist.Id);
                    this.logger.Log($"孤児プレイリスト(name:{playlist.PlaylistName}, ID:{playlist.Id})を削除しました。");
                    return false;
                }
                if (playlist.Videos.Count > 0)
                {
                    var videos = playlist.Videos.Distinct(v => v.NiconicoId).ToList();
                    if (videos.Count != playlist.Videos.Count)
                    {
                        this.logger.Log($"重複登録された動画をプレイリスト(name:{playlist.PlaylistName}, ID:{playlist.Id})から削除しました。");
                    }
                    playlist.Videos.Clear();
                    playlist.Videos.AddRange(videos);
                    this.databaseInstance.Update(playlist, STypes::Playlist.TableName);
                }
                return true;
            }

        }

        /// <summary>
        /// 子プレイリストを辿って削除する
        /// </summary>
        /// <param name="parentId"></param>
        private void RemoveChildPlaylist(int selfId)
        {
            if (this.Exists(selfId))
            {
                IEnumerable<STypes::Playlist> childPlaylists = this.GetChildPlaylists(selfId);

                foreach (var childPlaylist in childPlaylists)
                {
                    this.RemoveChildPlaylist(childPlaylist.Id);
                    this.DeletePlaylist(childPlaylist.Id);
                    this.logger.Log($"{selfId}の子プレイリストを削除しました。");
                }

            }
        }

        /// <summary>
        /// プレイリストのコピー・移動を行う内部メソッド
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destId"></param>
        /// <param name="IsCopy"></param>
        private void InternalMove(int id, int destId, bool IsCopy)
        {
            //移動先と異動元が同一のプレイリストの場合キャンセル
            //(自分に自分を入れることは出来ない。)
            if (id == destId) return;

            if (this.Exists(id) && this.Exists(destId))
            {
                STypes::Playlist destination = this.GetPlaylist(destId)!;
                STypes::Playlist target = this.GetPlaylist(id)!;

                //フォルダー・プレイリストでない場合はキャンセル
                if (destination.IsConcretePlaylist) return;


                if (IsCopy)
                {
                    STypes::Playlist newtarget = target with { ParentPlaylist = destination };
                    this.databaseInstance.Store(newtarget, STypes::Playlist.TableName);
                }
                else
                {
                    target.ParentPlaylist = destination;
                    this.databaseInstance.Update(target, STypes::Playlist.TableName);
                    this.logger.Log($"プレイリスト(name:{target.PlaylistName}, ID:{target.Id})を{target.ParentPlaylist.PlaylistName}から{destination.PlaylistName}に移動しました。");
                }

            }
        }

        /// <summary>
        /// プレイリストを更新する
        /// </summary>
        /// <param name="playlist"></param>
        private void Update(STypes::Playlist playlist)
        {
            if (this.Exists(playlist.Id))
            {
                this.databaseInstance.Update(playlist, STypes::Playlist.TableName);
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
        }

        #endregion
    }
}
