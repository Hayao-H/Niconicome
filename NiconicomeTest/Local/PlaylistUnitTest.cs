using NUnit.Framework;
using System;
using Niconicome.Models.Domain.Local;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Local.Store;
using Playlist = Niconicome.Models.Playlist;
using Niconicome.Models.Playlist;
using Reactive.Bindings;
using System.Linq;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using System.Windows.Forms;

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
                    this.playlistStorehandler = new PlaylistStoreHandler(this.database, new LoggerStub());
                    this.playlistStorehandler.Initialize();
                }

                [Test]
                public void ルートプレイリストを取得()
                {
                    IAttemptResult<STypes::Playlist> root = this.playlistStorehandler!.GetRootPlaylist();
                    Assert.That(root.IsSucceeded, Is.True);
                    Assert.That(root.Data, Is.Not.Null);
                    Assert.That(root.Data!.IsRoot, Is.True);
                }

                [Test]
                public void ルート直下にプレイリストを作成する()
                {
                    IAttemptResult<STypes::Playlist> rootResult = this.playlistStorehandler!.GetRootPlaylist();
                    Assert.That(rootResult.IsSucceeded, Is.True);
                    Assert.That(rootResult.Data, Is.Not.Null);
                    Assert.That(rootResult.Data!.IsRoot, Is.True);

                    IAttemptResult<int> childId = this.playlistStorehandler!.AddPlaylist(rootResult.Data!.Id, "子プレイリスト");

                    Assert.That(childId.IsSucceeded, Is.True);
                    Assert.That(childId.Data, Is.GreaterThan(-1));

                    STypes::Playlist? child = this.database?.GetCollection<STypes::Playlist>(STypes::Playlist.TableName).Data!.FirstOrDefault(p => p.Id == childId.Data);
                    Assert.IsNotNull(child);
                    Assert.IsNotNull(child?.ParentPlaylist);
                    Assert.AreEqual(rootResult.Data.Id, child?.ParentPlaylist?.Id);
                }

                [Test]
                public void プレイリストを削除する()
                {
                    //親-子-孫の関係を作る
                    IAttemptResult<STypes::Playlist> rootResult = this.playlistStorehandler!.GetRootPlaylist();
                    Assert.That(rootResult.IsSucceeded, Is.True);
                    Assert.That(rootResult.Data, Is.Not.Null);
                    Assert.That(rootResult.Data!.IsRoot, Is.True);

                    int rootId = rootResult.Data!.Id;

                    IAttemptResult<int> childResult = this.playlistStorehandler.AddPlaylist(rootId, "子プレイリスト");
                    Assert.That(childResult.IsSucceeded, Is.True);

                    int childId = childResult.Data;

                    IAttemptResult<int> grandchildResult = this.playlistStorehandler.AddPlaylist(childId, "孫プレイリスト");
                    Assert.That(grandchildResult.IsSucceeded, Is.True);

                    int grandChildId = grandchildResult.Data;

                    //削除
                    this.playlistStorehandler.DeletePlaylist(childId);

                    //プレイリストが存在しないことを確認
                    Assert.IsFalse(this.database?.Exists<STypes::Playlist>(STypes::Playlist.TableName, childId));
                    Assert.IsFalse(this.database?.Exists<STypes::Playlist>(STypes::Playlist.TableName, grandChildId));
                }

                [Test]
                public void プレイリストの移動()
                {
                    /// playlistA=>playlistB
                    /// target:移動するプレイリスト
                    IAttemptResult<STypes::Playlist> rootResult = this.playlistStorehandler!.GetRootPlaylist();
                    Assert.That(rootResult.IsSucceeded, Is.True);
                    Assert.That(rootResult.Data, Is.Not.Null);
                    Assert.That(rootResult.Data!.IsRoot, Is.True);

                    int rootId = rootResult.Data!.Id;
                    IAttemptResult<int> playlistAResult = this.playlistStorehandler.AddPlaylist(rootId, "プレイリストA");
                    IAttemptResult<int> playlistBResult = this.playlistStorehandler.AddPlaylist(rootId, "プレイリストB");

                    Assert.That(playlistAResult.IsSucceeded, Is.True);
                    Assert.That(playlistBResult.IsSucceeded, Is.True);

                    int playlistAId = playlistAResult.Data;
                    int playlistBId = playlistBResult.Data;

                    IAttemptResult<int> targetResult = this.playlistStorehandler.AddPlaylist(playlistAId, "移動するプレイリスト");

                    Assert.That(targetResult.IsSucceeded, Is.True);

                    int targetId = targetResult.Data;

                    //移動
                    this.playlistStorehandler?.Move(targetId, playlistBId);

                    //移動したプレイリストを取得
                    STypes::Playlist? target = this.database?.GetCollection<STypes::Playlist>(STypes::Playlist.TableName).Data!.FirstOrDefault(p => p.Id == targetId);

                    //nullチェック
                    Assert.IsNotNull(target);
                    Assert.IsNotNull(target?.ParentPlaylist);

                    //等価性チェック
                    Assert.AreEqual(target?.ParentPlaylist?.Id, playlistBId);
                }

                [Test]
                public void リモートプレイリストとして設定する()
                {

                    //プレイリストを2つ作成する
                    IAttemptResult<STypes::Playlist> rootResult = this.playlistStorehandler!.GetRootPlaylist();
                    Assert.That(rootResult.IsSucceeded, Is.True);
                    Assert.That(rootResult.Data, Is.Not.Null);
                    Assert.That(rootResult.Data!.IsRoot, Is.True);

                    int rootId = rootResult.Data!.Id;

                    IAttemptResult<int> playlist1Result = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト1");
                    IAttemptResult<int> playlist2Result = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト2");

                    Assert.That(playlist1Result.IsSucceeded, Is.True);
                    Assert.That(playlist2Result.IsSucceeded, Is.True);

                    int playlist1Id = playlist1Result.Data;
                    int playlist2Id = playlist2Result.Data;

                    this.playlistStorehandler.SetAsRemotePlaylist(playlist1Id, "1234", Playlist::Playlist.RemoteType.Mylist);
                    this.playlistStorehandler.SetAsRemotePlaylist(playlist2Id, "1234", Playlist::Playlist.RemoteType.UserVideos);

                    IAttemptResult<STypes::Playlist> playlist1 = this.playlistStorehandler.GetPlaylist(playlist1Id);
                    IAttemptResult<STypes::Playlist> playlist2 = this.playlistStorehandler.GetPlaylist(playlist2Id);

                    Assert.That(playlist1.IsSucceeded, Is.True);
                    Assert.That(playlist2.IsSucceeded, Is.True);
                    Assert.That(playlist1.Data, Is.Not.Null);
                    Assert.That(playlist2.Data, Is.Not.Null);
                    Assert.That(playlist1.Data!.IsRemotePlaylist, Is.True);
                    Assert.That(playlist2.Data!.IsRemotePlaylist, Is.True);
                    Assert.That(playlist1.Data!.IsMylist, Is.True);
                    Assert.That(playlist2.Data!.IsUserVideos, Is.True);
                    Assert.That(playlist1.Data!.IsWatchLater, Is.False);
                    Assert.That(playlist1.Data!.IsChannel, Is.False);
                    Assert.That(playlist1.Data!.IsSeries, Is.False);
                    Assert.That(playlist1.Data!.IsUserVideos, Is.False);
                }

                [Test]
                public void ローカルプレイリストとして設定する()
                {

                    //プレイリストを作成する
                    IAttemptResult<STypes::Playlist> rootResult = this.playlistStorehandler!.GetRootPlaylist();
                    Assert.That(rootResult.IsSucceeded, Is.True);
                    Assert.That(rootResult.Data, Is.Not.Null);
                    Assert.That(rootResult.Data!.IsRoot, Is.True);

                    int rootId = rootResult.Data!.Id;

                    IAttemptResult<int> playlistResult = this.playlistStorehandler.AddPlaylist(rootId, "プレイリスト1");


                    Assert.That(playlistResult.IsSucceeded, Is.True);

                    int playlistId = playlistResult.Data;

                    this.playlistStorehandler.SetAsLocalPlaylist(playlistId);


                    IAttemptResult<STypes::Playlist> playlist = this.playlistStorehandler.GetPlaylist(playlistId);

                    Assert.That(playlist.IsSucceeded, Is.True);
                    Assert.That(playlist.Data, Is.Not.Null);
                    Assert.That(playlist.Data!.IsRemotePlaylist, Is.False);
                    Assert.That(playlist.Data.IsWatchLater, Is.False);
                    Assert.That(playlist.Data.IsMylist, Is.False);
                    Assert.That(playlist.Data.IsSeries, Is.False);
                    Assert.That(playlist.Data.IsChannel, Is.False);
                    Assert.That(playlist.Data.IsUserVideos, Is.False);
                }


            }
        }
    }
}