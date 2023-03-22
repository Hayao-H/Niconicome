using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Type
{
    public interface IXenoVideo
    {
        /// <summary>
        /// ID
        /// </summary>
        string NiconicoID { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; }
    }

    public class XenoVideo : IXenoVideo
    {
        public XenoVideo(string niconicoID, string title)
        {
            this.NiconicoID = niconicoID;
            this.Title = title;
        }

        public string NiconicoID { get; init; }

        public string Title { get; init; } 
    }
}
