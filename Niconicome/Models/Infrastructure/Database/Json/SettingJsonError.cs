using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.Database.Json
{
    public enum SettingJSONError
    {
        [ErrorEnum(ErrorLevel.Error, "設定用Jsonディレクトリの検出に失敗しました。")]
        SettingDirectoryDetectionFailed,
        [ErrorEnum(ErrorLevel.Error, "設定用Jsonディレクトリの作成に失敗しました。")]
        SettingDirectoryCreationFailed,
        [ErrorEnum(ErrorLevel.Error, "設定用Jsonファイルの作成に失敗しました。")]
        SettingJsonCreationFailed,
        [ErrorEnum(ErrorLevel.Error, "設定用Jsonファイルへの書き込みに失敗しました。")]
        SettingJsonInitializationFailed,
        [ErrorEnum(ErrorLevel.Error, "設定用Jsonファイルの読み込みに失敗しました。")]
        SettingJsonReadingFailed,
        [ErrorEnum(ErrorLevel.Error, "設定用Jsonファイルへの書き込みに失敗しました。")]
        SettingJsonWritingFailed,
        [ErrorEnum(ErrorLevel.Error, "設定用Jsonデータのデシリアライズに失敗しました。")]
        SettingJsonDeserializationFailed,
        [ErrorEnum(ErrorLevel.Error, "設定用Jsonデータのシリアライズに失敗しました。")]
        SettingJsonSerializationFailed,
        [ErrorEnum(ErrorLevel.Error,"指定された設定名({0})の値を発見できませんでした。")]
        SettingNotFound,
        [ErrorEnum(ErrorLevel.Error,"設定値{0}の型({1})が指定された型({2})と一致しません。")]
        SettingTypeNotMatch,
        [ErrorEnum(ErrorLevel.Warning, "設定がリセットされました。")]
        SettingCleared,

    }
}
