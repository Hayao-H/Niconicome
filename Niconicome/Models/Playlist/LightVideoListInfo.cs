using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Playlist
{
    public interface ILightVideoListInfo
    {
        string MessageGuid { get; }
        string FileName { get; }
        int PlaylistId { get; }
        int VideoId { get; }
        bool IsSelected { get; }
    }

    /// <summary>
    /// ハンドラ
    /// </summary>
    public static class LightVideoListinfoHandler
    {
        private static readonly List<ILightVideoListInfo> videoListInfos = new();

        public static bool Contains(int videoID, int playlistID)
        {
            return LightVideoListinfoHandler.videoListInfos.Any(v => v.VideoId == videoID && v.PlaylistId == playlistID);
        }

        public static void AddVideo(ILightVideoListInfo video)
        {
            if (LightVideoListinfoHandler.Contains(video.VideoId, video.PlaylistId))
            {
                LightVideoListinfoHandler.videoListInfos.RemoveAll(v => v.PlaylistId == video.PlaylistId && v.VideoId == video.VideoId);
            }

            LightVideoListinfoHandler.videoListInfos.Add(video);

        }

        public static ILightVideoListInfo? GetLightVideoListInfo(int videoId, int playlistId)
        {
            return LightVideoListinfoHandler.videoListInfos.FirstOrDefault(v => v.VideoId == videoId && v.PlaylistId == playlistId);
        }
    }

    /// <summary>
    /// データを保持しておくための軽量なクラス
    /// </summary>
    public class LightVideoListInfo : ILightVideoListInfo
    {
        public LightVideoListInfo(string messageGud, string fileName, int playlistId, int videoId, bool isSelected)
        {
            this.MessageGuid = messageGud;
            this.FileName = fileName;
            this.VideoId = videoId;
            this.PlaylistId = playlistId;
            this.IsSelected = isSelected;
        }

        public string MessageGuid { get; init; }

        public int PlaylistId { get; init; }

        public int VideoId { get; init; }

        public bool IsSelected { get; init; }

        public string FileName { get; init; }


    }
}
