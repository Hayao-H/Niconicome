using NUnit.Framework;
using System;
using Niconicome.Models.Domain.Local;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Local.Store;
using Playlist = Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Playlist;
using Reactive.Bindings;

namespace NiconicomeTest
{
    namespace Local
    {
        namespace Playlist
        {
            [TestFixture]
            class PlaylistStoreUnitTest
            {
                private IPlaylistStoreHandler? playlistStorehandler;

                private IDataBase? database;

                [SetUp]
                public void SetUp()
                {
                    this.database = Static.DataBaseInstance;
                    //プレイリストテーブルをクリア
                    this.database.Clear(STypes::Playlist.TableName);
                    this.playlistStorehandler = new PlaylistStoreHandler(this.database, new VideoStoreHandler(this.database));
                    this.playlistStorehandler.Refresh();
                }

                [Test]
                public void ルートプレイリストを取得()
                {
                    STypes::Playlist? root = this.playlistStorehandler?.GetRootPlaylist();
                    Assert.IsNotNull(root);
                    Assert.IsTrue(root?.IsRoot);
                }

                [Test]
                public void ルート直下にプレイリストを作成する()
                {
                    int rootId = this.playlistStorehandler?.GetRootPlaylist().Id ?? -1;
                    int childId = this.playlistStorehandler?.AddPlaylist(rootId, "子プレイリスト") ?? -1;
                    Assert.Greater(rootId, -1);
                    Assert.Greater(childId, -1);
                    STypes::Playlist? child = this.database?.GetCollection<STypes::Playlist>(STypes::Playlist.TableName).Include(x => x.ParentPlaylist).FindById(childId);
                    Assert.IsNotNull(child);
                    Assert.IsNotNull(child?.ParentPlaylist);
                    Assert.AreEqual(rootId, child?.ParentPlaylist?.Id);
                }

                [Test]
                public void プレイリストを削除する()
                {
                    //親-子-孫の関係を作る
                    int rootId = this.playlistStorehandler?.GetRootPlaylist().Id ?? -1;
                    int childId = this.playlistStorehandler?.AddPlaylist(rootId, "子プレイリスト") ?? -1;
                    int grandChildId = this.playlistStorehandler?.AddPlaylist(childId, "孫プレイリスト") ?? -1;

                    //削除
                    this.playlistStorehandler?.DeletePlaylist(childId);

                    //プレイリストが存在しないことを確認
                    Assert.IsFalse(this.database?.Exists<STypes::Playlist>(STypes::Playlist.TableName, childId));
                    Assert.IsFalse(this.database?.Exists<STypes::Playlist>(STypes::Playlist.TableName, grandChildId));
                }

                [Test]
                public void プレイリストの移動()
                {
                    /// playlistA=>playlistB
                    /// target:移動するプレイリスト
                    int rooId = this.playlistStorehandler?.GetRootPlaylist().Id ?? -1;
                    int playlistAId = this.playlistStorehandler?.AddPlaylist(rooId, "プレイリストA") ?? -1;
                    int playlistBId = this.playlistStorehandler?.AddPlaylist(rooId, "プレイリストB") ?? -1;
                    int targetId = this.playlistStorehandler?.AddPlaylist(playlistAId, "移動するプレイリスト") ?? -1;

                    Assert.AreNotEqual(-1, rooId);
                    Assert.AreNotEqual(-1, playlistAId);
                    Assert.AreNotEqual(-1, playlistBId);
                    Assert.AreNotEqual(-1, targetId);

                    //移動
                    this.playlistStorehandler?.Move(targetId, playlistBId);

                    //移動したプレイリストを取得
                    STypes::Playlist? target = this.database?.GetCollection<STypes::Playlist>(STypes::Playlist.TableName).FindById(targetId);

                    //nullチェック
                    Assert.IsNotNull(target);
                    Assert.IsNotNull(target?.ParentPlaylist);

                    //等価性チェック
                    Assert.AreEqual(target?.ParentPlaylist?.Id, playlistBId);
                }

                [Test]
                public void リモートプレイリストとして設定する()
                {
                    if (this.playlistStorehandler is null) throw new InvalidOperationException();

                    //プレイリストを2つ作成する
                    int rootId = this.playlistStorehandler.GetRootPlaylist().Id;
                    int playlist1Id = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト1");
                    int playlist2Id = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト2");

                    this.playlistStorehandler.SetAsRemotePlaylist(playlist1Id, "1234", Playlist::RemoteType.Mylist);
                    this.playlistStorehandler.SetAsRemotePlaylist(playlist2Id, "1234", Playlist::RemoteType.UserVideos);

                    var playlist1 = this.playlistStorehandler.GetPlaylist(playlist1Id);
                    var playlist2 = this.playlistStorehandler.GetPlaylist(playlist2Id);

                    Assert.IsTrue(playlist1!.IsRemotePlaylist);
                    Assert.IsTrue(playlist2!.IsRemotePlaylist);
                    Assert.IsTrue(playlist1.IsMylist);
                    Assert.IsTrue(playlist2.IsUserVideos);
                    Assert.IsFalse(playlist1.IsUserVideos);
                    Assert.IsFalse(playlist2.IsMylist);
                }

                [Test]
                public void ローカルプレイリストとして設定する()
                {
                    if (this.playlistStorehandler is null) throw new InvalidOperationException();

                    //プレイリストを作成する
                    int rootId = this.playlistStorehandler.GetRootPlaylist().Id;
                    int playlistId = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト1");

                    this.playlistStorehandler.SetAsLocalPlaylist(playlistId);

                    var playlist = this.playlistStorehandler.GetPlaylist(playlistId);

                    Assert.IsFalse(playlist!.IsRemotePlaylist);
                    Assert.IsFalse(playlist.IsMylist);
                    Assert.IsFalse(playlist.IsUserVideos);
                }

                [Test]
                public void プレイリストのシーケンスを確認する()
                {
                    //設定
                    var rootId = this.playlistStorehandler!.GetRootPlaylist().Id;
                    var mainPlaylstID = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト");
                    var video1Id = this.playlistStorehandler!.AddVideo(new NonBindableListVideoInfo() { NiconicoId = new ReactiveProperty<string>("0") }, mainPlaylstID);
                    var video2Id = this.playlistStorehandler!.AddVideo(new NonBindableListVideoInfo() { NiconicoId = new ReactiveProperty<string>("1") }, mainPlaylstID);

                    var playlist = this.playlistStorehandler!.GetPlaylist(mainPlaylstID)!;
                    Assert.That(playlist.CustomVideoSequence.Count, Is.EqualTo(2));
                    Assert.That(playlist.CustomVideoSequence[0], Is.EqualTo(video1Id));
                    Assert.That(playlist.CustomVideoSequence[1], Is.EqualTo(video2Id));
                }

                [Test]
                public void 二番目のプレイリストを先頭に移動する()
                {
                    //設定
                    var rootId = this.playlistStorehandler!.GetRootPlaylist().Id;
                    var mainPlaylstID = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト");
                    var video1Id = this.playlistStorehandler!.AddVideo(new NonBindableListVideoInfo() { NiconicoId = new ReactiveProperty<string>("0") }, mainPlaylstID);
                    var video2Id = this.playlistStorehandler!.AddVideo(new NonBindableListVideoInfo() { NiconicoId = new ReactiveProperty<string>("1") }, mainPlaylstID);

                    this.playlistStorehandler!.MoveVideoToPrev(mainPlaylstID, 1);

                    var playlist = this.playlistStorehandler!.GetPlaylist(mainPlaylstID)!;
                    Assert.That(playlist.CustomVideoSequence.Count, Is.EqualTo(2));
                    Assert.That(playlist.CustomVideoSequence[0], Is.EqualTo(video2Id));
                    Assert.That(playlist.CustomVideoSequence[1], Is.EqualTo(video1Id));
                }

                [Test]
                public void 先頭のプレイリストを二番目に移動する()
                {
                    //設定
                    var rootId = this.playlistStorehandler!.GetRootPlaylist().Id;
                    var mainPlaylstID = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト");
                    var video1Id = this.playlistStorehandler!.AddVideo(new NonBindableListVideoInfo() { NiconicoId = new ReactiveProperty<string>("0") }, mainPlaylstID);
                    var video2Id = this.playlistStorehandler!.AddVideo(new NonBindableListVideoInfo() { NiconicoId = new ReactiveProperty<string>("1") }, mainPlaylstID);

                    this.playlistStorehandler!.MoveVideoToForward(mainPlaylstID, 0);

                    var playlist = this.playlistStorehandler!.GetPlaylist(mainPlaylstID)!;
                    Assert.That(playlist.CustomVideoSequence.Count, Is.EqualTo(2));
                    Assert.That(playlist.CustomVideoSequence[0], Is.EqualTo(video2Id));
                    Assert.That(playlist.CustomVideoSequence[1], Is.EqualTo(video1Id));
                }

            }
        }
    }
}