using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.OS;
using Niconicome.Models.Playlist.V2.Utils.Error;

namespace Niconicome.Models.Playlist.V2.Utils
{
    public interface IVideoInfoCopyManager
    {
        /// <summary>
        /// 情報をクリップボードにコピー
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IAttemptResult CopyInfomartion(IReadOnlyList<IVideoInfo> source, CopyTarget target);
    }

    public class VideoInfoCopyManager : IVideoInfoCopyManager
    {
        public VideoInfoCopyManager(IClipbordManager clipbordManager,IErrorHandler errorHandler)
        {
            this._clipbordManager = clipbordManager;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IClipbordManager _clipbordManager;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method
        public IAttemptResult CopyInfomartion(IReadOnlyList<IVideoInfo> source, CopyTarget target)
        {
            if (source.Count == 0)
            {
                this._errorHandler.HandleError(VideoInfoCopyManagerError.SourceIsEmpty);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(VideoInfoCopyManagerError.SourceIsEmpty));
            }

            if (source.Count == 1)
            {
                return this._clipbordManager.SetToClipBoard(this.GetCopyContent(source[0], target));
            }

            var builder = new StringBuilder();
            foreach (var video in source)
            {
                builder.AppendLine(this.GetCopyContent(video, target));
            }

            return this._clipbordManager.SetToClipBoard(builder.ToString());
        }


        #endregion

        #region private

        /// <summary>
        /// コピー内容を取得
        /// </summary>
        /// <param name="video"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private string GetCopyContent(IVideoInfo video, CopyTarget target)
        {
            return target switch
            {
                CopyTarget.NiconicoID => video.NiconicoId,
                CopyTarget.Title => video.Title,
                _ => NetConstant.NiconicoShortUrl + video.NiconicoId
            };
        }

        #endregion
    }

    public enum CopyTarget
    {
        NiconicoID,
        Title,
        URL,
    }
}
