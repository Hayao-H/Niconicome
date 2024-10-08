﻿using System.Collections.Generic;
using AngleSharp.Common;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Local.Settings.Enum;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Setting.V2.Utils;
using WS = Niconicome.Workspaces.SettingPageV2;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class DownloadViewModel
    {
        public DownloadViewModel()
        {
            var n1 = new SelectBoxItem<int>("1", 1);
            var n2 = new SelectBoxItem<int>("2", 2);
            var n3 = new SelectBoxItem<int>("3", 3);
            var n4 = new SelectBoxItem<int>("4", 4);
            var n5 = new SelectBoxItem<int>("5", 5);
            this.SelectableMaxParallelDownloadCount = new() { n1, n2, n3, n4 };
            this.SelectableMaxParallelSegmentDownloadCount = new() { n1, n2, n3, n4, n5 };

            var t1 = new SelectBoxItem<VideoInfoTypeSettings>("テキスト(Xeno互換)", VideoInfoTypeSettings.Text);
            var t2 = new SelectBoxItem<VideoInfoTypeSettings>("JSON", VideoInfoTypeSettings.Json);
            var t3 = new SelectBoxItem<VideoInfoTypeSettings>("XML", VideoInfoTypeSettings.Xml);
            this.SelectableVideoInfoType = new() { t1, t2, t3 };

            var i1 = new SelectBoxItem<IchibaInfoTypeSettings>("HTML", IchibaInfoTypeSettings.Html);
            var i2 = new SelectBoxItem<IchibaInfoTypeSettings>("JSON", IchibaInfoTypeSettings.Json);
            var i3 = new SelectBoxItem<IchibaInfoTypeSettings>("XML", IchibaInfoTypeSettings.Xml);
            this.SelectableIchibaInfoType = new() { i1, i2, i3 };

            this.MaxParallelDownloadCount = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.MaxParallelDownloadCount, NetConstant.DefaultMaxParallelDownloadCount), NetConstant.DefaultMaxParallelDownloadCount).AddTo(this.Bindables);

            this.MaxParallelSegmentDownloadCount = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.MaxParallelSegmentDownloadCount, NetConstant.DefaultMaxParallelSegmentDownloadCount), NetConstant.DefaultMaxParallelSegmentDownloadCount).AddTo(this.Bindables);

            this.VideoInfoType = new BindableSettingInfo<VideoInfoTypeSettings>(WS.SettingsContainer.GetSetting(SettingNames.VideoInfoType, VideoInfoTypeSettings.Text), VideoInfoTypeSettings.Text).AddTo(this.Bindables);

            this.MaxTmpDirCount = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.MaxTmpSegmentsDirCount, 20), 20).AddTo(this.Bindables);

            this.IsOverrideVideoFileDTToUploadedDT = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.OverideVideoFileDTToUploadedDT, false), false).AddTo(this.Bindables);

            this.IsOmitXmlDeclarationEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsOmittingXmlDeclarationIsEnable, false), false).AddTo(this.Bindables);

            this.IsDupeOnStageAllowed = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.AllowDupeOnStage, false), false).AddTo(this.Bindables);

            this.IsDownloadResumingEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsResumeEnable, true), true).AddTo(this.Bindables);

            this.IsDownloadFromQueueEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.DownloadAllWhenPushDLButton, false), false).AddTo(this.Bindables);

            this.IsDeletingExistingEconomyFileEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.DeleteExistingEconomyFile, false), false).AddTo(this.Bindables);

            this.CommentFetchWaitSpan = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.CommentFetchWaitSpan, LocalConstant.DefaultCommetFetchWaitSpan), LocalConstant.DefaultCommetFetchWaitSpan, x => x, x => x is int).AddTo(this.Bindables);

            this.IchibaInfoType = new BindableSettingInfo<IchibaInfoTypeSettings>(WS.SettingsContainer.GetSetting(SettingNames.IchibaInfoType, IchibaInfoTypeSettings.Html), IchibaInfoTypeSettings.Html).AddTo(this.Bindables);

            this.CommentCountPerBlock = new BindableSettingInfo<int>(WS.SettingsContainer.GetSetting(SettingNames.CommentCountPerBlock, NetConstant.DefaultCommentCountPerBlock), NetConstant.DefaultCommentCountPerBlock, x => x, x => x is int).AddTo(this.Bindables);

            this.IsExperimentalCommentDownloadSystemEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsExperimentalCommentSafetySystemEnable, false), false).AddTo(this.Bindables);

            var ex1 = new SelectBoxItem<ExternalDownloaderConditionSetting>("無効", ExternalDownloaderConditionSetting.Disable);
            var ex2 = new SelectBoxItem<ExternalDownloaderConditionSetting>("常に", ExternalDownloaderConditionSetting.Always);
            var ex3 = new SelectBoxItem<ExternalDownloaderConditionSetting>("暗号化された動画の場合", ExternalDownloaderConditionSetting.Encrypted);
            var ex4 = new SelectBoxItem<ExternalDownloaderConditionSetting>("公式動画の場合", ExternalDownloaderConditionSetting.Official);

            this.SelectableExternalDownloaderSetting = new List<SelectBoxItem<ExternalDownloaderConditionSetting>>() { ex1, ex2, ex3, ex4 };
            this.ExternalDownloaderSetting = new BindableSettingInfo<ExternalDownloaderConditionSetting>(WS.SettingsContainer.GetSetting(SettingNames.ExternalDownloaderCondition, ExternalDownloaderConditionSetting.Disable), ExternalDownloaderConditionSetting.Disable).AddTo(this.Bindables);
            this.ExternalDownloaderPath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.ExternalDLSoftwarePath, string.Empty), string.Empty).AddTo(this.Bindables);
            this.ExternalDownloaderParam = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.ExternalDLSoftwareParam, string.Empty), string.Empty).AddTo(this.Bindables);

            this.PlaySoundAfterDownloadCompleted = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.PlaySoundAfterDownload, false), false).AddTo(this.Bindables);
            this.AudioFilePath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.DownloadCompletionAudioPath, string.Empty), string.Empty).AddTo(this.Bindables);

            this.IsVideoModificationEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsVideoModificationEnable, false), false).AddTo(this.Bindables);
            this.VideoModificationSoftwarePath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.VideoModificationSoftwarePath, string.Empty), string.Empty).AddTo(this.Bindables);
            this.VideoModificationSoftwareParam = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.VideoModificationSoftwareParam, string.Empty), string.Empty).AddTo(this.Bindables);

        }

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// 選択可能な並列ダウンロード数
        /// </summary>
        public List<SelectBoxItem<int>> SelectableMaxParallelDownloadCount { get; init; }

        /// <summary>
        /// 選択可能な並列ダウンロード数
        /// </summary>
        public List<SelectBoxItem<int>> SelectableMaxParallelSegmentDownloadCount { get; init; }

        /// <summary>
        /// 選択可能な動画情報ファイルの保存形式
        /// </summary>
        public List<SelectBoxItem<VideoInfoTypeSettings>> SelectableVideoInfoType { get; init; }

        /// <summary>
        /// 選択可能な市場情報ファイルの保存形式
        /// </summary>
        public List<SelectBoxItem<IchibaInfoTypeSettings>> SelectableIchibaInfoType { get; init; }

        /// <summary>
        /// 選択可能な外部ダウンローダー設定
        /// </summary>
        public List<SelectBoxItem<ExternalDownloaderConditionSetting>> SelectableExternalDownloaderSetting { get; }

        /// <summary>
        /// 最大並列DL数
        /// </summary>
        public IBindableSettingInfo<int> MaxParallelDownloadCount { get; init; }

        /// <summary>
        /// 最大セグメント並列DL数
        /// </summary>
        public IBindableSettingInfo<int> MaxParallelSegmentDownloadCount { get; init; }

        /// <summary>
        /// 動画情報ファイルの保存形式
        /// </summary>
        public IBindableSettingInfo<VideoInfoTypeSettings> VideoInfoType { get; init; }

        /// <summary>
        /// 市場情報の保存形式
        /// </summary>
        public IBindableSettingInfo<IchibaInfoTypeSettings> IchibaInfoType { get; init; }

        /// <summary>
        /// キューからもDLする
        /// </summary>
        public IBindableSettingInfo<bool> IsDownloadFromQueueEnable { get; init; }

        /// <summary>
        /// ステージングの際の重複を許可する
        /// </summary>
        public IBindableSettingInfo<bool> IsDupeOnStageAllowed { get; init; }

        /// <summary>
        /// 動画ファイルの更新日時を投稿日時にする
        /// </summary>
        public IBindableSettingInfo<bool> IsOverrideVideoFileDTToUploadedDT { get; init; }

        /// <summary>
        /// レジュームを有効にする
        /// </summary>
        public IBindableSettingInfo<bool> IsDownloadResumingEnable { get; init; }

        /// <summary>
        /// Xml宣言を出力
        /// </summary>
        public IBindableSettingInfo<bool> IsOmitXmlDeclarationEnable { get; init; }

        /// <summary>
        /// 1ブロックあたりのコメント数
        /// </summary>
        public IBindableSettingInfo<int> CommentCountPerBlock { get; init; }

        /// <summary>
        /// エコノミーファイル削除
        /// </summary>
        public IBindableSettingInfo<bool> IsDeletingExistingEconomyFileEnable { get; init; }

        /// <summary>
        /// 一時フォルダーの最大保持数
        /// </summary>
        public IBindableSettingInfo<int> MaxTmpDirCount { get; init; }

        /// <summary>
        /// コメント取得時の待機時間
        /// </summary>
        public IBindableSettingInfo<int> CommentFetchWaitSpan { get; init; }

        /// <summary>
        /// 実験的なシステム
        /// </summary>
        public IBindableSettingInfo<bool> IsExperimentalCommentDownloadSystemEnable { get; init; }

        /// <summary>
        /// 外部ダウンローダー設定
        /// </summary>
        public IBindableSettingInfo<ExternalDownloaderConditionSetting> ExternalDownloaderSetting { get; init; }

        /// <summary>
        /// 外部ダウンローダーのパス
        /// </summary>
        public IBindableSettingInfo<string> ExternalDownloaderPath { get; init; }

        /// <summary>
        /// 外部ダウンローダーのパラメータ
        /// </summary>
        public IBindableSettingInfo<string> ExternalDownloaderParam { get; init; }

        /// <summary>
        /// DL完了後に音声を再生する
        /// </summary>
        public IBindableSettingInfo<bool> PlaySoundAfterDownloadCompleted { get; init; }

        /// <summary>
        /// 音声ファイルのパス
        /// </summary>
        public IBindableSettingInfo<string> AudioFilePath { get; init; }

        /// <summary>
        /// 動画の後処理を行うかどうか
        /// </summary>
        public IBindableSettingInfo<bool> IsVideoModificationEnable { get; init; }

        /// <summary>
        /// 動画の後処理ソフトウェアのパス
        /// </summary>
        public IBindableSettingInfo<string> VideoModificationSoftwarePath { get; init; }

        /// <summary>
        /// 動画の後処理ソフトウェアのパラメータ
        /// </summary>
        public IBindableSettingInfo<string> VideoModificationSoftwareParam { get; init; }
    }
}
