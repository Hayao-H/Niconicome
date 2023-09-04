using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.AES;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment
{
    public interface ISegmentInfomation
    {
        /// <summary>
        /// メッセージハンドラ
        /// </summary>
        /// <param name="message"></param>
        void OnMessage(string message);

        /// <summary>
        /// ID
        /// </summary>
        string NiconicoID { get; }

        /// <summary>
        /// URL
        /// </summary>
        string SegmentURL { get; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// インデックス
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 解像度
        /// </summary>
        uint VerticalResolution { get; }

        /// <summary>
        /// CT
        /// </summary>
        CancellationToken CT { get; }

        /// <summary>
        /// AES
        /// </summary>
        IAESInfomation? AES { get; }
    }

    public class SegmentInfomation : ISegmentInfomation
    {
        public SegmentInfomation(Action<string> onMessage, string segmentURL, int index, string filePath, uint verticalResolution, string niconicoID, CancellationToken cT, IAESInfomation? aES )
        {
            this._onMessage = onMessage;
            this.SegmentURL = segmentURL;
            this.Index = index;
            this.VerticalResolution = verticalResolution;
            this.FilePath = filePath;
            this.CT = cT;
            this.NiconicoID = niconicoID;
            this.AES = aES;
        }

        #region field

        private readonly Action<string> _onMessage;

        #endregion

        #region Props

        public string NiconicoID { get; init; }

        public string SegmentURL { get; init; }

        public string FilePath { get; init; }

        public int Index { get; init; }

        public uint VerticalResolution { get; init; }

        public CancellationToken CT { get; init; }

        public IAESInfomation? AES { get; init; }


        #endregion

        #region Method

        public void OnMessage(string message)
        {
            this._onMessage(message);
        }


        #endregion
    }
}
