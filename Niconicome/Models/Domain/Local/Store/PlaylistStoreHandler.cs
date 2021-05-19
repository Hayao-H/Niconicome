using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Playlist;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.Playlist;

namespace Niconicome.Models.Domain.Local.Store
{
    public interface IPlaylistStoreHandler
    {
        STypes::Playlist GetRootPlaylist();
        STypes::Playlist? GetPlaylist(int id);
        public int AddPlaylist(int parentID, string name);
        public int AddVideo(IListVideoInfo video, int playlistId);
        public void RemoveVideo(int id, int playlistId);
        public void Update(ITreePlaylistInfo newplaylist);
        IEnumerable<STypes::Playlist> GetChildPlaylists(STypes::Playlist self);
        IEnumerable<STypes::Playlist> GetChildPlaylists(int id);
        IEnumerable<STypes::Playlist> GetAllPlaylists();
        IAttemptResult MoveVideoToPrev(int playlistID, int videoIndex);
        IAttemptResult MoveVideoToForward(int playlistID, int videoIndex);
        void RemoveChildPlaylist(int selfId);
        void DeletePlaylist(int id);
        bool Exists(int id);
        bool JustifyPlaylists(int Id);
        bool JustifyPlaylists(IEnumerable<int> Id);
        bool ContainsVideo(string niconicoId, int playlistId);
        void Move(int id, int destId);
        void Copy(int id, int destId);
        void SetAsRemotePlaylist(int id, string remoteId, RemoteType type);
        void SetAsLocalPlaylist(int id);
        int GetPlaylistsCount();
        void Refresh();
    }

    public class PlaylistStoreHandler : IPlaylistStoreHandler
    {
        public PlaylistStoreHandler(IDataBase dataBase, IVideoStoreHandler videoHandler)
        {
            this.databaseInstance = dataBase;
            this.videoHandler = videoHandler;
        }

        private readonly IDataBase databaseInstance;

        private readonly IVideoStoreHandler videoHandler;

        /// <summary>
        /// プレイリストを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public STypes::Playlist? GetPlaylist(int id)
        {

            var playlist = this.databaseInstance.GetCollection<STypes::Playlist>(STypes::Playlist.TableName)
                    .Include(p => p.Videos)
                    .FindById(id);

            if (playlist is null) return null;

            if (playlist.Videos.Count > 0 && playlist.CustomVideoSequence.Count == 0)
            {
                playlist.CustomVideoSequence.AddRange(playlist.Videos.Select(v => v.Id));
            }

            return playlist;

        }

        /// <summary>
        /// ルートレベルプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public STypes::Playlist GetRootPlaylist()
        {
            return this.databaseInstance.GetRecord<STypes::Playlist>(STypes::Playlist.TableName, playlist => playlist.IsRoot)!;
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
                var parent = this.databaseInstance.GetCollection<STypes::Playlist>(STypes::Playlist.TableName)
                    .Include(p => p.Videos)
                    .FindById(parentId);

                //動画を保持している場合はキャンセル
                if (parent!.IsConcretePlaylist) return -1;

                //5階層よりも深い場合はキャンセル
                if (parent.Layer > 5) return -1;

                var playlist = new STypes::Playlist(parent)
                {
                    PlaylistName = "新しいプレイリスト"
                };

                int childId = this.databaseInstance.Store(playlist, STypes::Playlist.TableName);


                this.databaseInstance.Update(parent, STypes::Playlist.TableName); ;
                return childId;
            }
            else
            {
                return -1;
            }

        }

        /// <summary>
        /// 子プレイリストを取得する
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public IEnumerable<STypes::Playlist> GetChildPlaylists(STypes::Playlist self)
        {
            return this.GetChildPlaylists(self.Id);
        }

        /// <summary>
        /// 子プレイリストをIDから取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<STypes::Playlist> GetChildPlaylists(int id)
        {
            return this.databaseInstance.GetAllRecords<STypes::Playlist>(STypes::Playlist.TableName).FindAll(p => p.ParentPlaylist?.Id == id);

        }

        /// <summary>
        /// 全てのプレイリストを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<STypes::Playlist> GetAllPlaylists()
        {
            return this.databaseInstance.GetAllRecords<STypes::Playlist>(STypes::Playlist.TableName);
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

            this.Update(playlist);

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

            this.Update(playlist);

            return new AttemptResult() { IsSucceeded = true };
        }

        /// <summary>
        /// 子プレイリストを辿って削除する
        /// </summary>
        /// <param name="parentId"></param>
        public void RemoveChildPlaylist(int selfId)
        {
            if (this.Exists(selfId))
            {
                IEnumerable<STypes::Playlist> childPlaylists = this.GetChildPlaylists(selfId);

                foreach (var childPlaylist in childPlaylists)
                {
                    this.RemoveChildPlaylist(childPlaylist.Id);
                    this.DeletePlaylist(childPlaylist.Id);
                }

            }
        }

        /// <summary>
        /// プレイリストを削除する
        /// </summary>
        /// <param name="playlistID"></param>
        public void DeletePlaylist(int playlistID)
        {
            if (this.databaseInstance.Exists<STypes::Playlist>(STypes::Playlist.TableName, playlistID))
            {
                var playlist = this.databaseInstance.GetRecord<STypes::Playlist>(STypes::Playlist.TableName, playlistID);

                //ルートプレイリストは削除禁止
                if (playlist!.IsRoot || playlist.Layer == 1) return;

                this.RemoveChildPlaylist(playlistID);

                this.databaseInstance.Delete(STypes::Playlist.TableName, playlistID);
            }
        }

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
            }
        }

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
        /// プレイリストの不整合を修復する
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool JustifyPlaylists(int Id)
        {
            var playlist = this.GetPlaylist(Id);

            if (playlist == null)
            {
                return false;
            }
            else
            {
                //親が存在しないプレイリスト(孤児プレイリスト)の場合はそれ自体を削除
                if (playlist.ParentPlaylist != null && !this.databaseInstance.Exists<STypes::Playlist>(STypes::Playlist.TableName, playlist.ParentPlaylist.Id))
                {
                    this.DeletePlaylist(playlist.Id);
                    return false;
                }
                if (playlist.Videos.Count > 0)
                {
                    var videos = playlist.Videos.Distinct(v => v.NiconicoId).Copy();
                    playlist.Videos.Clear();
                    playlist.Videos.AddRange(videos);
                    this.databaseInstance.Update(playlist, STypes::Playlist.TableName);
                }
                return true;
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
        /// プレイリスト情報を更新する
        /// </summary>
        /// <param name="newPlaylist"></param>
        public void Update(ITreePlaylistInfo newPlaylist)
        {
            if (!this.Exists(newPlaylist.Id)) throw new InvalidOperationException($"指定したプレイリストが存在しません。(id:{newPlaylist.Id})");
            var dbPlaylist = this.GetPlaylist(newPlaylist.Id)!;
            this.SetData(dbPlaylist, newPlaylist);
            this.Update(dbPlaylist);
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
                }

            }
        }

        /// <summary>
        /// データベース上のプレイリスト数を取得
        /// </summary>
        /// <returns></returns>
        public int GetPlaylistsCount()
        {
            return this.databaseInstance.GetRecordCount(STypes::Playlist.TableName);
        }

        /// <summary>
        /// データベースのレコードを更新する
        /// </summary>
        public void Refresh()
        {
            int records = this.GetPlaylistsCount();
            if (records == 0)
            {
                var playlist = new STypes::Playlist()
                {
                    PlaylistName = "プレイリスト一覧",
                    IsRoot = true
                };
                this.databaseInstance.Store(playlist, STypes::Playlist.TableName);
            }

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
            playlist.CustomVideoSequence.Add(videoId);

            this.databaseInstance.Update(playlist, STypes::Playlist.TableName);

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
            this.databaseInstance.Update(playlist, STypes::Playlist.TableName);
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
            dbPlaylist.PlaylistName = playlistInfo.Name;
            dbPlaylist.FolderPath = playlistInfo.Folderpath;
            dbPlaylist.IsExpanded = playlistInfo.IsExpanded;
        }
    }
}
