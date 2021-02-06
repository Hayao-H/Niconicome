using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Store.Types
{
    public class VideoFile:IStorable
    {

        public static string TableName { get; private set; } = "videofile";

        public int Id { get; set; }

        public string? NiconicoId { get; set; }

        public List<string> FilePaths { get; private set; } = new();
    }
}
