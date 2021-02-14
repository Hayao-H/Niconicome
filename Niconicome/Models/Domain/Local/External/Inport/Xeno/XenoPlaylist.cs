using System;
using System.Collections.Generic;
using System.IO;

namespace Niconicome.Models.Domain.Local.External.Inport.Xeno
{
    public interface IXenoPlaylist
    {
        string Name { get; }
        bool IsRootPlaylist { get; }
        bool IsChannel { get; }
        string? ChannelId { get; }
        int Layer { get; }
        IXenoPlaylist? ParentPlaylist { get; }
        List<IXenoPlaylist> ChildPlaylists { get; }
        List<string> Videos { get; }
    }

    public interface IXenoVideo
    {
        string NiconicoId { get; }
        string Title { get; }
        string RelativeFolderPath { get; }
        int WatchCount { get; }
        DateTime PostDateTime { get; }
        string GetAbsoluteFolderPath(string basefolder);
    }

    /// <summary>
    /// プレイリスト情報
    /// </summary>
    public class XenoPlaylist : IXenoPlaylist
    {
        public XenoPlaylist(string name,string channelId,IXenoPlaylist parent)
        {
            this.Name = name;
            this.ChannelId = channelId;
            this.ParentPlaylist = parent;
            parent.ChildPlaylists.Add(this);
        }

        public XenoPlaylist(string name,string channelId)
        {
            this.Name = name;
            this.ChannelId = channelId;
        }

        public XenoPlaylist(string name,IXenoPlaylist parent)
        {
            this.Name = name;
            this.ParentPlaylist = parent;
            parent.ChildPlaylists.Add(this);
        }

        public XenoPlaylist(string name)
        {
            this.Name = name;
        }

        public string Name { get; init; }

        public bool IsRootPlaylist { get => this.ParentPlaylist is null; }

        public bool IsChannel { get => this.ChannelId is not null; }

        public string? ChannelId { get; init; }

        public int Layer
        {
            get
            {
                var l = 1;
                IXenoPlaylist playlist = this;
                while (playlist.ParentPlaylist is not null)
                {
                    ++l;
                    playlist = playlist.ParentPlaylist;
                }
                return l;
            }
        }

        public IXenoPlaylist? ParentPlaylist { get; init; }

        public List<IXenoPlaylist> ChildPlaylists { get; init; } = new();

        public List<string> Videos { get; init; } = new();

    }

    /// <summary>
    /// 動画情報
    /// </summary>
    public class XenoVideo : IXenoVideo
    {
        public XenoVideo(string niconicoId, string title, int watchCount, string relativeFolderPath, DateTime postDatetime)
        {
            this.NiconicoId = niconicoId;
            this.Title = title;
            this.WatchCount = watchCount;
            this.RelativeFolderPath = relativeFolderPath;
            this.PostDateTime = postDatetime;

        }

        public string NiconicoId { get; init; }

        public string Title { get; init; }

        public int WatchCount { get; init; }
        public string RelativeFolderPath { get; init; }

        public DateTime PostDateTime { get; init; }



        public string GetAbsoluteFolderPath(string basefolder)
        {
            return Path.Combine(basefolder, this.RelativeFolderPath);
        }
    }
}
