using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Network.Download.DLTask
{
    public class TaskPoolEventArgs:EventArgs
    {
        public TaskPoolEventArgs(IDownloadTask chaneObject,ChangeType changeType)
        {
            this.ChangedObject = chaneObject;
            this.ChangeType = changeType;
        }
        
        /// <summary>
        /// 変更されたオブジェクト
        /// </summary>
        IDownloadTask ChangedObject { get;init;}

        /// <summary>
        /// 変更のタイプF
        /// </summary>
        ChangeType ChangeType { get;init;}
    }

    public enum ChangeType
    {
        Add,
        Remove,
    }
}
