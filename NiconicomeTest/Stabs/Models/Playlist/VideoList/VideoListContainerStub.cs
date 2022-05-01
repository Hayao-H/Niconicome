using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.VideoList;

namespace NiconicomeTest.Stabs.Models.Playlist.VideoList
{
    internal class VideoListContainerStub : IVideoListContainer
    {
        public IAttemptResult Remove(IListVideoInfo video, int? playlistID = null, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult RemoveRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Add(IListVideoInfo video, int? playlistID = null, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult AddRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Update(IListVideoInfo video, int? playlistID = null, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult UpdateRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Refresh()
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Clear()
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult ForEach(Action<IListVideoInfo> foreachFunc)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Sort(VideoSortType sortType, bool isDescending, List<int>? customSortSequence = null)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult MovevideotoPrev(int videoIndex, int? playlistID = null, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult MovevideotoForward(int videoIndex, int? playlistID = null, bool commit = true)
        {
            return AttemptResult.Succeeded();
        }

        public int Count { get; init; } = 0;

        public ObservableCollection<IListVideoInfo> Videos { get; init; } = new();

        public event EventHandler<ListChangedEventArgs<IListVideoInfo>>? ListChanged;
    }
}
