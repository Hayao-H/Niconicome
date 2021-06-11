﻿using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Local.External.Import;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Playlist.VideoList;
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
        public static IVideoListContainer VideoListContainer { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoListContainer>();
        public static IEnumSettingsHandler EnumSettingsHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IEnumSettingsHandler>();
        public static IAutoLogin AutoLogin { get; private set; } = DIFactory.Provider.GetRequiredService<IAutoLogin>();
        public static IThemehandler Themehandler { get; private set; } = DIFactory.Provider.GetRequiredService<IThemehandler>();
    }
}
