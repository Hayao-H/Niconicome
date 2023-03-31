using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Playlist.V2.Migration
{
    public record MigrationResult(IReadOnlyList<DetailedMigrationResult> FailedVideos,IReadOnlyList<DetailedMigrationResult> FailedPlaylist,IReadOnlyList<DetailedMigrationResult> PartlyFailedPlaylist);
}
