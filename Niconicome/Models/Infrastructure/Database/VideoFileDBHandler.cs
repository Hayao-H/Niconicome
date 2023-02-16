using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Text;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Infrastructure.Database.Types;

namespace Niconicome.Models.Infrastructure.Database
{
    public class VideoFileDBHandler : IVideoFileStore
    {
        public VideoFileDBHandler(ILiteDBHandler liteDB,INiconicomeFileIO fileIO)
        {
            this._liteDB = liteDB;
            this._fileIO = fileIO;
        }

        #region field

        private readonly ILiteDBHandler _liteDB;

        private readonly INiconicomeFileIO _fileIO;

        #endregion

        #region Method

        public IAttemptResult<string> GetFilePath(string niconicoID, uint verticalResolution)
        {
            IAttemptResult<VideoFile> result = this._liteDB.GetRecord<VideoFile>(TableNames.VideoFile, v => v.NiconicoID == niconicoID && v.VerticalResolution == verticalResolution);

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<string>.Fail(result.Message);
            }

            return AttemptResult<string>.Succeeded(result.Data.FilePath);
        }

        public IAttemptResult AddFile(string niconicoID, string filePath)
        {
            if (this._liteDB.Exists<VideoFile>(TableNames.Video, v => v.FilePath == filePath))
            {
                return AttemptResult.Succeeded();
            }

            IAttemptResult<int> resolutionResult = this._fileIO.GetVerticalResolution(filePath);
            if (!resolutionResult.IsSucceeded)
            {
                return AttemptResult.Fail(resolutionResult.Message);
            }

            var file = new VideoFile()
            {
                NiconicoID = niconicoID,
                FilePath = filePath,
                VerticalResolution = (uint)resolutionResult.Data,
            };

            return this._liteDB.Insert(file);
        }

        public bool Exist(string niconicoID, uint verticalResolution)
        {
            return this._liteDB.Exists<VideoFile>(TableNames.Video, v => v.NiconicoID == niconicoID && v.VerticalResolution == verticalResolution);
        }


        #endregion

        #region private

        #endregion
    }
}
