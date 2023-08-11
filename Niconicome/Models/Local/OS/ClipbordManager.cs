using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils;
using Niconicome.Models.Utils.Reactive;
using Reactive.Bindings;

namespace Niconicome.Models.Local.OS
{
    public interface IClipbordManager
    {
        /// <summary>
        /// クリップボード監視フラグ
        /// </summary>
        IReadonlyBindablePperty<bool> IsMonitoring { get; }

        /// <summary>
        /// クリップボードから情報を取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<string> GetClipboardContent();

        /// <summary>
        /// クリップボードに書き込む
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        IAttemptResult SetToClipBoard(string content);

        /// <summary>
        /// クリップボード監視を開始
        /// </summary>
        IAttemptResult StartMonitoring();

        /// <summary>
        /// クリップボード監視を終了
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// クリップボード変更ハンドラを追加
        /// </summary>
        /// <param name="handler"></param>
        void RegisterClipboardChangeHandler(Action<string> handler);

        /// <summary>
        /// クリップボード変更ハンドラを解除
        /// </summary>
        /// <param name="handler"></param>
        void UnRegisterClipboardChangeHandler(Action<string> handler);

    }

}
