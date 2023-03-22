using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.StringContent
{
    public enum XenoImportHandlerStringContent
    {
        [StringEnum("Xeno")]
        XenoPlaylistName,
        [StringEnum("データを解析中")]
        ParsingData,
        [StringEnum("処理中：「({0})」")]
        ProcessingPlaylist,
        [StringEnum("プレイリストの親子関係を構築中...")]
        ResolveChildren,
        [StringEnum("インポートに失敗")]
        ImportFailed,
        [StringEnum("インポート完了")]
        ImportCompleted,
    }
}
