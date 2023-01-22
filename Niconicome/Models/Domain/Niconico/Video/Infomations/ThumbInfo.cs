using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using V2 = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;

namespace Niconicome.Models.Domain.Niconico.Video.Infomations
{
    public interface IThumbInfo
    {
        string GetSpecifiedThumbnail(ThumbSize size);
    }

    /// <summary>
    /// サムネイル情報
    /// </summary>
    public class ThumbInfo : IThumbInfo
    {
        #region field

        private readonly string? large;

        private readonly string? middle;

        private readonly string? normal;

        private readonly string? player;

        #endregion

        public ThumbInfo()
        {

        }

        public ThumbInfo(string? large,string? middle,string? url,string? player)
        {
            this.large = large;
            this.middle = middle;
            this.normal = url;
            this.player = player;
        }

        /// <summary>
        /// 指定したサイズの解像度のサムネイルを取得する
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetSpecifiedThumbnail(ThumbSize size)
        {
            return size switch
            {
                ThumbSize.Large => this.large ?? this.GetSpecifiedThumbnail(ThumbSize.Middle),
                ThumbSize.Middle => this.middle ?? this.GetSpecifiedThumbnail(ThumbSize.Normal),
                ThumbSize.Normal => this.normal ?? this.GetSpecifiedThumbnail(ThumbSize.Player),
                _ => this.player ?? throw new InvalidOperationException($"解像度{ThumbSize.Player}がnullです。")
            };
        }

        /// <summary>
        /// 大サムネイル
        /// </summary>
        public string? Large { get; set; }

        /// <summary>
        /// 通常
        /// </summary>
        public string? Normal { get; set; }
    }

    public enum ThumbSize
    {
        Large,
        Middle,
        Normal,
        Player
    }
}
