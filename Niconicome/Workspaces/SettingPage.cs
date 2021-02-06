using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local.Store;
using MaterialDesign = MaterialDesignThemes.Wpf;
using Niconicome.Models.Playlist;
using Niconicome.Models.Local.State;

namespace Niconicome.Workspaces
{
    static class SettingPage
    {
        public static ILocalSettingHandler SettingHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalSettingHandler>();
        public static ILocalState State { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalState>();
        public static ILocalInfo LocalInfo { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalInfo>();
        public static IRestore Restore { get; private set; } = DIFactory.Provider.GetRequiredService<IRestore>();
        public static MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; private set; } = new MaterialDesign::SnackbarMessageQueue();
        public static ICurrent Current { get; private set; } = DIFactory.Provider.GetRequiredService<ICurrent>();
        public static IPlaylistTreeHandler PlaylistTreeHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistTreeHandler>();
    }
}
