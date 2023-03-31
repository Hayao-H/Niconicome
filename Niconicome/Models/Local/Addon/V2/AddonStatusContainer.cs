using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Loader;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Update;

namespace Niconicome.Models.Local.Addon.V2
{

    public interface IAddonStatusContainer
    {
        /// <summary>
        /// 実行中のアドオン
        /// </summary>
        IReadOnlyList<IAddonInfomation> LoadedAddons { get; }

        /// <summary>
        /// 読み込みに失敗したアドオン
        /// </summary>
        IReadOnlyList<FailedResult> LoadFailedAddons { get; }

        /// <summary>
        /// アップデートが存在するアドオン
        /// </summary>
        IReadOnlyList<UpdateCheckInfomation> ToBeUpdatedAddons { get; }

        /// <summary>
        /// アイテムを追加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="type"></param>
        void Add<T>(T item, ListType type);

        /// <summary>
        /// 指定されたIDを持つアドオンの情報を削除
        /// </summary>
        /// <param name="ID"></param>
        void Remove(string ID);

        /// <summary>
        /// リストをクリア
        /// </summary>
        /// <param name="type"></param>
        void Clear(ListType type);

        /// <summary>
        /// リスト変更イベント
        /// </summary>

        event EventHandler? ListChanged;
    }

    public class AddonStatusContainer : IAddonStatusContainer
    {

        #region Event

        public event EventHandler? ListChanged;

        #endregion

        #region field

        private List<IAddonInfomation> _loadedAddonsField = new();

        private List<FailedResult> _loadFailedAddonsField = new();

        private List<UpdateCheckInfomation> _toBeUpdatedAddonsField = new();

        #endregion

        #region Props

        public IReadOnlyList<IAddonInfomation> LoadedAddons { get; private set; } = new List<IAddonInfomation>().AsReadOnly();


        public IReadOnlyList<FailedResult> LoadFailedAddons { get; private set; } = new List<FailedResult>().AsReadOnly();

        public IReadOnlyList<UpdateCheckInfomation> ToBeUpdatedAddons { get; private set; } = new List<UpdateCheckInfomation>().AsReadOnly();

        #endregion

        #region Methods

        public void Add<T>(T item, ListType type)
        {
            if (item is IAddonInfomation info && type == ListType.Loaded)
            {
                this._loadedAddonsField.Add(info);
            }
            else if (item is UpdateCheckInfomation update && type == ListType.Update)
            {
                this._toBeUpdatedAddonsField.Add(update);
            }
            else if (type == ListType.Failed && item is FailedResult result)
            {
                this._loadFailedAddonsField.Add(result);
            }

            this.Reflesh();
        }


        public void Remove(string ID)
        {
            this._loadedAddonsField.RemoveAll(i => i.ID == ID);
            this._toBeUpdatedAddonsField.RemoveAll(i => i.Infomation.ID == ID);

            this.Reflesh();
        }

        public void Clear(ListType type)
        {
            if (type == ListType.Loaded)
            {
                this._loadedAddonsField.Clear();
            }
            else if (type == ListType.Update)
            {
                this._toBeUpdatedAddonsField.Clear();
            }
            else if (type == ListType.Failed)
            {
                this._loadFailedAddonsField.Clear();
            }

            this.Reflesh();
        }

        #endregion

        #region private

        /// <summary>
        /// リストを更新する
        /// </summary>
        private void Reflesh()
        {
            this.LoadedAddons = this._loadedAddonsField.AsReadOnly();
            this.LoadFailedAddons = this._loadFailedAddonsField.AsReadOnly();
            this.ToBeUpdatedAddons = this._toBeUpdatedAddonsField.AsReadOnly();

            try
            {
                this.ListChanged?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        #endregion
    }

    public enum ListType
    {
        Loaded,
        Update,
        Failed
    }

}
