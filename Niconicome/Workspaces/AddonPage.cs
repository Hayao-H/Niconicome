﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Addon;

namespace Niconicome.Workspaces
{
    public static class AddonPage
    {
        public static IAddonHandler AddonHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IAddonHandler>();
    }
}