using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils.Reactive;

namespace NiconicomeTest.Stabs.Models.Domain.Playlist
{
    public class PlaylistInfoStub : IPlaylistInfo
    {
        public bool IsAutoUpdateEnabled { get; set; }

        public int ID => 1;

        public int ParentID { get; set; }

        public string FolderPath { get; set; } = string.Empty;

        public string TemporaryFolderPath { get; set; } = string.Empty;

        public BindableProperty<string> Name { get; } = new(string.Empty);

        public PlaylistType PlaylistType { get; set; }

        public string RemoteParameter { get; set; } = string.Empty;

        public SortType SortType { get; set; }

        public bool IsAscendant { get; set; }

        public IReadonlyBindablePperty<int> SelectedVideosCount { get; init; } = new ReadonlyBindablePperty<int>(new BindableProperty<int>(0));

        public IReadonlyBindablePperty<int> VideosCount { get; init; } = new ReadonlyBindablePperty<int>(new BindableProperty<int>(0));

        public ReadOnlyObservableCollection<IPlaylistInfo> Children { get; init; } = new ReadOnlyObservableCollection<IPlaylistInfo>(new ObservableCollection<IPlaylistInfo>());

        public IReadOnlyList<int> ChildrenID { get; init; } = (new List<int>()).AsReadOnly();

        public IReadOnlyList<string> ParentNames { get; init; } = (new List<string>()).AsReadOnly();

        public IReadOnlyList<IVideoInfo> Videos { get; init; } = (new List<IVideoInfo>()).AsReadOnly();

        public IAttemptResult AddChild(IPlaylistInfo playlistInfo, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult RemoveChild(IPlaylistInfo playlistInfo)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult AddVideo(IVideoInfo video)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult RemoveVideo(IVideoInfo video)
        {
            return AttemptResult.Succeeded();
        }

        public void MoveVideo(string sourceVideoID, string targetVideoID)
        {

        }

        public void SetParentNamesList(List<string> names)
        {

        }

    }
}
