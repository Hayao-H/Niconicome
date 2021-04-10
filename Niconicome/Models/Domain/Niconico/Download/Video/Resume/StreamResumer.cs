using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;

namespace Niconicome.Models.Domain.Niconico.Download.Video.Resume
{
    public interface IStreamResumer
    {
        bool SegmentsDirectoryExists(string niconicoID);
        void RemoveSegmentsdirectory(string niconicoID);
        //   IEnumerable<ISegmentsDirectoryInfo> GetAllSegmentsDirectoryInfo();
        ISegmentsDirectoryInfo GetSegmentsDirectoryInfo(string niconicoID);
    }

    public class StreamResumer : IStreamResumer
    {
        public StreamResumer(INicoDirectoryIO directoryIO, ISegmentsDirectoryHandler segmentsDirectoryHandler)
        {
            this.directoryIO = directoryIO;
            this.segmentsDirectoryHandler = segmentsDirectoryHandler;
        }

        /// <summary>
        /// ディレクトリを操作する
        /// </summary>
        private readonly INicoDirectoryIO directoryIO;

        /// <summary>
        /// セグメント情報を取得する
        /// </summary>
        private readonly ISegmentsDirectoryHandler segmentsDirectoryHandler;

        /// <summary>
        /// ディレクトリが存在するかどうかを確かめる
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        public bool SegmentsDirectoryExists(string niconicoID)
        {
            var dirs = this.directoryIO.GetDirectorys(@"tmp", $"{niconicoID}-*");

            dirs.RemoveAll(d => !Regex.IsMatch(d, @"^.+-\d+-\d{4}-\d{2}-\d{2}$"));

            return dirs.Count > 0;
        }

        /// <summary>
        /// セグメントディレクトリ情報を取得する
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        public ISegmentsDirectoryInfo GetSegmentsDirectoryInfo(string niconicoID)
        {
            return this.segmentsDirectoryHandler.GetSegmentsDirectoryInfo(niconicoID);
        }

        /// <summary>
        /// セグメントディレクトリを削除する
        /// </summary>
        /// <param name="niconicoID"></param>
        public void RemoveSegmentsdirectory(string niconicoID)
        {
            this.directoryIO.DeleteAll(p => p.StartsWith(niconicoID));

        }

        /// <summary>
        /// すべてのセグメントディレクトリ情報を取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISegmentsDirectoryInfo> GetAllSegmentsDirectoryInfo()
        {
            var dirs = this.directoryIO.GetDirectorys(@"tmp");
            dirs.RemoveAll(d => !Regex.IsMatch(d, @"^.+-\d+-\d{4}-\d{2}-\d{2}$"));

            var infos = dirs.Select(d => this.segmentsDirectoryHandler.GetSegmentsDirectoryInfoWithDirectoryPath(d));

            return infos;
        }



    }
}
