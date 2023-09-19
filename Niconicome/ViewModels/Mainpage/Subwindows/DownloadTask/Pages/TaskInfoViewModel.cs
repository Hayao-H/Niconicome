using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Utils.Reactive;

namespace Niconicome.ViewModels.Mainpage.Subwindows.DownloadTask.Pages
{
    public class TaskInfoViewModel
    {
        public TaskInfoViewModel()
        {
            this.StateClass = new BindableProperty<string>("hidden").AddTo(this.Bindables);
        }

        #region field

        private IDownloadTaskViewModel? _vm;

        #endregion

        #region Props

        public Bindables Bindables { get; init; } = new();

        public IBindableProperty<string> StateClass { get; init; }

        public IReadOnlyCollection<string> FullMessage => this._vm?.FullMessage ?? new List<string>().AsReadOnly();

        public string Title => this._vm?.Title ?? string.Empty;

        public uint Resolution
        {
            get => this._vm?.Resolution ?? 1080;
            set
            {
                if (this._vm is null) return;
                this._vm.Resolution = value;
            }
        }

        public string DirectoryPath
        {
            get => this._vm?.DirectoryPath ?? string.Empty;
            set
            {
                if (this._vm is null) return;
                this._vm.DirectoryPath = value;
            }
        }

        public string FIleNameFormat
        {
            get => this._vm?.FileNameFormat ?? string.Empty;
            set
            {
                if (this._vm is null) return;
                this._vm.FileNameFormat = value;
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// 開く
        /// </summary>
        /// <param name="vm"></param>
        public void Open(IDownloadTaskViewModel vm)
        {
            this._vm = vm;
            this.StateClass.Value = string.Empty;
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void Close()
        {
            this._vm = null;
            this.StateClass.Value = "hidden";
            this.Bindables.Dispose();
        }

        #endregion
    }
}
