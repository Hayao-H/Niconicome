using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Type
{
    public interface IXenoPlaylist
    {
        /// <summary>
        /// 一意なID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// 名前
        /// </summary>
        string Name { get; }

        /// <summary>
        /// チャンネルフラグ
        /// </summary>
        bool IsChannel { get; }

        /// <summary>
        /// チャンネルID
        /// </summary>
        string ChannelID { get; }

        /// <summary>
        /// 保存先
        /// </summary>
        string FolderPath { get; }

        /// <summary>
        /// 動画
        /// </summary>
        List<IXenoVideo> Videos { get; }

        /// <summary>
        /// 子プレイリストのID
        /// </summary>
        List<string> Children { get; }

    }

    public class XenoPlaylist : IXenoPlaylist
    {
        public XenoPlaylist(string name, string folderPath, string? channelID = null)
        {
            this.Name = name;
            this.FolderPath = folderPath;

            if (channelID is not null)
            {
                this.IsChannel = true;
                this.ChannelID = channelID;
            }

        }

        public string ID { get; init; } = Guid.NewGuid().ToString("D");

        public string Name { get; init; }

        public string ChannelID { get; init; } = string.Empty;

        public string FolderPath { get; init; }


        public bool IsChannel { get; init; }

        public List<IXenoVideo> Videos { get; init; } = new();

        public List<string> Children { get; init; } = new();
    }
}
