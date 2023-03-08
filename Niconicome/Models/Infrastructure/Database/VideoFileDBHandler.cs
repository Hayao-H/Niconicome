using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Infrastructure.Database.Types;

namespace Niconicome.Models.Infrastructure.Database
{
    public class VideoFileDBHandler : IVideoFileStore
    {
        public VideoFileDBHandler(ILiteDBHandler liteDB, INiconicomeFileIO fileIO, INiconicoUtils utils)
        {
            this._liteDB = liteDB;
            this._fileIO = fileIO;
            this._utils = utils;
        }

        #region field

        private readonly ILiteDBHandler _liteDB;

        private readonly INiconicomeFileIO _fileIO;

        private readonly INiconicoUtils _utils;

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

        public async Task<IAttemptResult> AddFileAsync(string niconicoID, string filePath)
        {
            if (this._liteDB.Exists<VideoFile>(TableNames.Video, v => v.FilePath == filePath))
            {
                return AttemptResult.Succeeded();
            }

            IAttemptResult<int> resolutionResult = await this._fileIO.GetVerticalResolutionAsync(filePath);
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

        public async Task<IAttemptResult<int>> AddFilesFromDirectoryListAsync(IEnumerable<string> directories)
        {
            var succeeded = 0;

            foreach (var directory in directories)
            {
                await this._fileIO.EnumerateFilesAsync(directory, "*" + FileFolder.Mp4FileExt, async path =>
                 {
                     string id = this._utils.GetIdFromFIleName(path);
                     if (id.IsNullOrEmpty())
                     {
                         return;
                     }

                     IAttemptResult result = await this.AddFileAsync(id, path);
                     if (result.IsSucceeded)
                     {
                         succeeded++;
                     }

                 }, true);
            }

            return AttemptResult<int>.Succeeded(succeeded);
        }



        #endregion

        #region private

        #endregion
    }
}
