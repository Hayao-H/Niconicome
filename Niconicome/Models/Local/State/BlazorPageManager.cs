using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.State
{
    public interface IBlazorPageManager
    {
        /// <summary>
        /// Blazorにルーティングを要求
        /// </summary>
        /// <param name="url"></param>
        void RequestBlazorToNavigate(string url);

        /// <summary>
        /// 遷移すべきページ
        /// </summary>
        string PageToNavigate { get; }
    }

    public class BlazorPageManager : IBlazorPageManager
    {
        public void RequestBlazorToNavigate(string url)
        {
            this.PageToNavigate = url;
        }

        public string PageToNavigate { get; private set; } = "/";

    }
}
