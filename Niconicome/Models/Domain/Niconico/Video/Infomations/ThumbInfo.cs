using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V2 = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;

namespace Niconicome.Models.Domain.Niconico.Video.Infomations
{
    public interface IThumbInfo
    {
        string? Large { get; set; }
        string? Normal { get; set; }
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

        public ThumbInfo(V2::Thumbnail thumbnail)
        {
            this.large = thumbnail.LargeUrl;
            this.middle = thumbnail.MiddleUrl;
            this.normal = thumbnail.Url;
            this.player = thumbnail.Player;
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
