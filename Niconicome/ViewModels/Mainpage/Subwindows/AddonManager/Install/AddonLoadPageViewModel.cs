using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Helper.Result;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Install
{
    class AddonLoadPageViewModel
    {
        public AddonLoadPageViewModel()
        {
            IAttemptResult resut = WS::AddonPage.InstallManager.LoadAddon();

            var builder = new StringBuilder();
            if (!resut.IsSucceeded)
            {
                builder.AppendLine("アドオンの読み込みに失敗しました。");
                builder.AppendLine($"詳細:{resut.Message}");
            }
            else
            {
                AddonInfomation addon = WS::AddonPage.InstallManager.Infomation.Value!;
                builder.AppendLine($"名前:{addon.Name.Value}");
                builder.AppendLine($"説明:{addon.Description.Value}");
                builder.AppendLine($"作者:{addon.Author.Value}");
            }

            this.Message.Value = builder.ToString();

        }

        #region Props

        public ReactiveProperty<string> Message { get; init; } = new("読み込み中．．．");

        #endregion
    }

    class AddonLoadPageViewModelD
    {
        public ReactiveProperty<string> Message { get; init; } = new("読み込み中．．．");
    }
}
