using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Setting.V2.Page.StringContent
{
    public enum RestoreViewModelStringContent
    {
        [StringEnum("入力欄が空です。")]
        InputIsEmpty,
        [StringEnum("詳細:{0}")]
        DetailInfo,
        [StringEnum("操作対象が選択されていません。")]
        TargetIsNotSelected,
        [StringEnum("バックアップの作成に成功しました。")]
        BackupCreationSucceeded,
        [StringEnum("バックアップの作成に失敗しました。")]
        BackupCreationFailed,
        [StringEnum("バックアップの削除に成功しました。")]
        BackupDeletionSucceeded,
        [StringEnum("バックアップの削除に失敗しました。")]
        BackupDeletionFailed,
        [StringEnum("バックアップの適用に成功しました。")]
        BackupApplyingSuceeded,
        [StringEnum("バックアップの適用に失敗しました。")]
        BackupApplyingFailed,
        [StringEnum("動画ディレクトリの追加に失敗しました。")]
        AddingVideoDirectoryFailed,
        [StringEnum("動画ディレクトリの削除に失敗しました。")]
        DeletingVideoDirectoryFailed,
        [StringEnum("動画の読み込みに失敗しました。")]
        LoadingVideosFailed,
        [StringEnum("動画ディレクトリの追加に成功しました。({0}件の動画を読み込み)")]
        AddingVideoDirectorySucceeded,
        [StringEnum("動画ディレクトリの削除に成功しました。")]
        DeletingVideoDirectorySucceeded,
        [StringEnum("動画の読み込みに成功しました。。({0}件の動画を読み込み)")]
        LoadingVideosSucceeded,
        [StringEnum("データの整理に失敗しました。")]
        DataCleaningFailed,
        [StringEnum("データの整理に成功しました。")]
        DataCleaningSucceeded,
    }
}
