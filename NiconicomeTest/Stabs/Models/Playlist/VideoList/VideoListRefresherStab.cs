﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.V2.Manager;
using Niconicome.Models.Playlist.VideoList;

namespace NiconicomeTest.Stabs.Models.Playlist.VideoList
{
    class VideoListRefresherStab : IVideoListRefresher
    {
        public IAttemptResult Refresh(IEnumerable<IListVideoInfo> videos, Action<IListVideoInfo> addFunc, bool isTemporary = false)
        {
            return new AttemptResult() { IsSucceeded = true };
        }
    }
}
