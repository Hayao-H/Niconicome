using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video.Resume
{
    public interface ISegmentsDirectoryInfo
    {
        List<string> ExistsFileNames { get; }
        string NiconicoID { get; }
        string DirectoryName { get; }
        uint Resolution { get; }
        DateTime StartedOn { get; set; }
    }

    public class SegmntsDirectoryInfo : ISegmentsDirectoryInfo
    {
        public SegmntsDirectoryInfo(string niconicoID,uint resolution,string directoryName)
        {
            this.NiconicoID = niconicoID;
            this.Resolution = resolution;
            this.DirectoryName = directoryName;
            this.ExistsFileNames = new List<string>();
        }

        public List<string> ExistsFileNames { get; init; }

        public string NiconicoID { get; init; }

        public string DirectoryName { get; init; }


        public uint Resolution { get; init; }

        public DateTime StartedOn { get; set; }

    }
}