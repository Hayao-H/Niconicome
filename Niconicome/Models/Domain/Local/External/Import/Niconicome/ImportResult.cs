﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.External.Import.Niconicome
{
    public record ImportResult(int ImportSucceededPlaylistsCount, int ImpotySucceededVideosCount);
}
