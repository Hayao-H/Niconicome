﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Playlist;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Network;
using Playlist = Niconicome.Models.Playlist;

namespace Niconicome.Models.Local
{
    public interface IPlaylistCreator
    {
        bool TryCreatePlaylist(IEnumerable<Playlist::IListVideoInfo> videos, string playlistName, string directoryPath, PlaylistType type);
    }


    class PlaylistCreator : IPlaylistCreator
    {
        public PlaylistCreator(IPlaylistFileFactory fileFactory, INetworkVideoHandler videoHandler, ILogger logger, IVideoFileStorehandler videoFileStorehandler)
        {
            this.fileFactory = fileFactory;
            this.videoHandler = videoHandler;
            this.logger = logger;
            this.videoFileStorehandler = videoFileStorehandler;
        }

        private readonly IPlaylistFileFactory fileFactory;

        private readonly INetworkVideoHandler videoHandler;

        private readonly IVideoFileStorehandler videoFileStorehandler;

        private readonly ILogger logger;

        /// <summary>
        /// プレイリストを作成する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="directoryPath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool TryCreatePlaylist(IEnumerable<Playlist::IListVideoInfo> videos, string playlistName, string directoryPath, PlaylistType type)
        {
            videos = videos.Where(v => v.CheckDownloaded(directoryPath));
            if (!videos.Any()) return false;

            string data;
            try
            {
                data = this.fileFactory.GetPlaylist(videos.Select(v => this.videoFileStorehandler.GetFilePath(v.NiconicoId.Value, directoryPath) ?? string.Empty).Where(p => !p.IsNullOrEmpty()), playlistName, type);
            }
            catch (Exception e)
            {
                this.logger.Error("プレイリストの作成に失敗しました。", e);
                return false;
            }

            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception e)
                {
                    this.logger.Error("ディレクトリーの作成に失敗しました。", e);
                    return false;
                }
            }

            try
            {
                using var fs = new StreamWriter(Path.Combine(directoryPath, $"playlist.{this.GetExt(type)}"), false, this.GetEncording(type));
                fs.Write(data);
            }
            catch (Exception e)
            {
                this.logger.Error("プレイリストへの書き込みに失敗しました。", e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 拡張子を取得する
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetExt(PlaylistType type)
        {
            return type switch
            {
                PlaylistType.Aimp => "aimppl4",
                _ => string.Empty,
            };
        }

        private Encoding GetEncording(PlaylistType type)
        {
            return type switch
            {
                PlaylistType.Aimp => Encoding.GetEncoding("utf-16"),
                _ => Encoding.UTF8
            };
        }

    }
}
