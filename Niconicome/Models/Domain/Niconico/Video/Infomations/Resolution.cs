using System;
using System.Linq;

namespace Niconicome.Models.Domain.Niconico.Video.Infomations
{

    public interface IResolution
    {
        uint Vertical { get; }
        uint Horizontal { get; }
        static IResolution Default { get; } = new Resolution("480x360");
    }

    /// <summary>
    /// 解像度クラス
    /// </summary>
    public class Resolution : IResolution
    {
        /// <summary>
        /// 高さ
        /// </summary>
        public uint Vertical { get; private set; }

        /// <summary>
        /// 横幅
        /// </summary>
        public uint Horizontal { get; private set; }

        public static Resolution Default { get; } = new Resolution("480x360");

        public Resolution(uint vertical, uint horizontal)
        {
            this.Vertical = vertical;
            this.Horizontal = horizontal;
        }

        public Resolution(string resolution)
        {
            string[] resolutions = resolution.Split('x');
            this.Horizontal = uint.Parse(resolutions[0]);
            this.Vertical = uint.Parse(resolutions[1]);
        }
    }

}
