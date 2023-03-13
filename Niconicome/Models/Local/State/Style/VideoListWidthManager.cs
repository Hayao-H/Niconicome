using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Niconicome.Extensions.System.Collections.Generic;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.State.Style
{
    public interface IVideoListWidthManager
    {
        /// <summary>
        /// 幅を設定する
        /// </summary>
        /// <param name="width"></param>
        /// <param name="columnType"></param>
        /// <returns></returns>
        void SetWidth(int width, ColumnType columnType);

        /// <summary>
        /// 幅の設定を保存する
        /// </summary>
        void SaveWidth();

        /// <summary>
        /// 幅を取得する
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        IAttemptResult<int> GetWidth(ColumnType columnType);
    }

    public class VideoListWidthManager : IVideoListWidthManager
    {
        public VideoListWidthManager(ISettingsContainer settingsContainer)
        {
            this._settingsContainer = settingsContainer;
        }

        #region field

        private readonly ISettingsContainer _settingsContainer;

        private ISettingInfo<bool>? _isRestoreWidthDisabled;

        private readonly Dictionary<ColumnType, int> _width = new();

        #endregion

        #region Method

        public void SetWidth(int width, ColumnType columnType)
        {
            this._width[columnType] = width;
        }

        public IAttemptResult<int> GetWidth(ColumnType columnType)
        {
            if (this._isRestoreWidthDisabled is null)
            {
                IAttemptResult<ISettingInfo<bool>> restoreResult = this._settingsContainer.GetSetting(SettingNames.IsRestoringColumnWidthDisabled, false);
                if (!restoreResult.IsSucceeded || restoreResult.Data is null)
                {
                    return AttemptResult<int>.Fail(restoreResult.Message);
                }
                this._isRestoreWidthDisabled = restoreResult.Data;
            }

            if (this._isRestoreWidthDisabled.Value)
            {
                return AttemptResult<int>.Succeeded(-1);
            }

            if (this._width.ContainsKey(columnType))
            {
                return AttemptResult<int>.Succeeded(this._width[columnType]);
            }

            IAttemptResult<ISettingInfo<int>> result = this.GetWidthSetting(columnType);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<int>.Fail(result.Message);
            }

            this._width.Add(columnType, result.Data.Value);
            return AttemptResult<int>.Succeeded(result.Data.Value);
        }

        public void SaveWidth()
        {
            foreach (var value in Enum.GetValues(typeof(ColumnType)))
            {
                if (value is not ColumnType type)
                {
                    continue;
                }

                if (!this._width.ContainsKey(type))
                {
                    continue;
                }

                IAttemptResult<ISettingInfo<int>> result = this.GetWidthSetting(type);
                if (!result.IsSucceeded || result.Data is null)
                {
                    continue;
                }

                result.Data.Value = this._width[type];
            }
        }


        #endregion

        #region private

        private IAttemptResult<ISettingInfo<int>> GetWidthSetting(ColumnType columnType)
        {
            IAttemptResult<ISettingInfo<int>> result = this._settingsContainer.GetSetting(columnType switch
            {
                ColumnType.CheckBoxColumn => SettingNames.VideoListCheckBoxColumnWidth,
                ColumnType.ThumbnailColumn => SettingNames.VideoListThumbnailColumnWidth,
                ColumnType.TitleColumn => SettingNames.VideoListTitleColumnWidth,
                ColumnType.UploadedDateTimeColumn => SettingNames.VideoListUploadedDateTimeColumnWidth,
                ColumnType.IsDownloadedColumn => SettingNames.VideoListIsDownloadedColumnWidth,
                ColumnType.ViewCountColumn => SettingNames.VideoListViewCountColumnWidth,
                ColumnType.CommentCountColumn => SettingNames.VideoListCommentCountColumnWidth,
                ColumnType.MylistCountColumn => SettingNames.VideoListMylistCountColumnWidth,
                ColumnType.LikeCountColumn => SettingNames.VideoListLikeCountColumnWidth,
                ColumnType.MessageColumn => SettingNames.VideoListMessageColumnWidth,
                _ => throw new InvalidOperationException()
            }, -1);

            return result;
        }

        #endregion
    }

    public enum ColumnType
    {
        CheckBoxColumn,
        ThumbnailColumn,
        TitleColumn,
        UploadedDateTimeColumn,
        IsDownloadedColumn,
        ViewCountColumn,
        CommentCountColumn,
        MylistCountColumn,
        LikeCountColumn,
        MessageColumn,
    }
}
