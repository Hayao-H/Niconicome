namespace Niconicome.Models.Local.State
{

    public interface ILocalState
    {
        bool IsSettingWindowOpen { get; set; }
        bool IsDebugMode { get; set; }
    }

    public class LocalState : ILocalState
    {
        /// <summary>
        /// 設定ウィンドウフラグ
        /// </summary>
        public bool IsSettingWindowOpen { get; set; }

        /// <summary>
        /// デバッグフラグ
        /// </summary>
        public bool IsDebugMode { get; set; }

    }
}
