using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;

namespace NiconicomeTest.Stabs.Models.Domain.Local.IO
{
    class FileWatcherStab : IFileWatcher
    {
        public void UnWatch()
        {

        }

        public void Watch(string filePath, NotifyFilters filters, Action<FileSystemEventArgs> handler)
        {

        }
    }
}
