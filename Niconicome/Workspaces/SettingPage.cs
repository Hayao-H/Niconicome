using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local;
using Niconicome.Models.Local.External.Import;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using MaterialDesign = MaterialDesignThemes.Wpf;

namespace Niconicome.Workspaces
{
    static class SettingPage
    {
        public static ILocalSettingHandler SettingHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalSettingHandler>();
        public static ILocalState State { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalState>();
        public static ILocalInfo LocalInfo { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalInfo>();
        public static IRestore Restore { get; private set; } = DIFactory.Provider.GetRequiredService<IRestore>();
        public static MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; private set; } = new MaterialDesign::SnackbarMessageQueue();
        public static IPlaylistHandler PlaylistTreeHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistHandler>();
        public static IXenoImportGeneralManager XenoImportManager { get; private set; } = DIFactory.Provider.GetRequiredService<IXenoImportGeneralManager>();
        public static IMessageHandler MessageHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IMessageHandler>();
    }
}
