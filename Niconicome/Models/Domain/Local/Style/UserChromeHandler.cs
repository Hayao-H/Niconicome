using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.Style.Type;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;

namespace Niconicome.Models.Domain.Local.Style
{
    public interface IUserChromeHandler
    {
        IAttemptResult<UserChrome> GetUserChrome();
        IAttemptResult SaveStyle(UserChrome chrome);
    }

    public class UserChromeHandler : IUserChromeHandler
    {
        public UserChromeHandler(INicoFileIO fileIO, ILogger logger)
        {
            this.fileIO = fileIO;
            this.logger = logger;
        }

        #region field

        private readonly INicoFileIO fileIO;

        private readonly ILogger logger;

        #endregion

        /// <summary>
        /// userChromeを取得する
        /// </summary>
        /// <returns></returns>
        public IAttemptResult<UserChrome> GetUserChrome()
        {
            if (this.fileIO.Exists(FileFolder.UserChromePath))
            {
                string content;
                try
                {
                    content = this.fileIO.OpenRead(FileFolder.UserChromePath);
                }
                catch (Exception e)
                {
                    this.logger.Error("userChrome.jsonの読み込みに失敗しました。", e);
                    return new AttemptResult<UserChrome>() { Message = "userChrome.jsonの読み込みに失敗しました。", Exception = e };
                }

                UserChrome chrome;
                try
                {
                    chrome = JsonParser.DeSerialize<UserChrome>(content);
                }
                catch (Exception e)
                {
                    this.logger.Error("userChrome.jsonの解析に失敗しました。", e);
                    return new AttemptResult<UserChrome>() { Message = "userChrome.jsonの解析に失敗しました。", Exception = e };
                }

                this.logger.Log("userChrome.jsonの読み込みに成功しました。");
                return new AttemptResult<UserChrome>() { Data = chrome, IsSucceeded = true };
            }
            else
            {
                this.logger.Log("userChrome.jsonが存在しなかったのでインスタンスを作成しました。");
                return new AttemptResult<UserChrome>() { IsSucceeded = true, Data = new UserChrome() };
            }
        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <param name="chrome"></param>
        /// <returns></returns>
        public IAttemptResult SaveStyle(UserChrome chrome)
        {
            string content;
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            try
            {
                content = JsonParser.Serialize(chrome, options);
            }
            catch (Exception e)
            {
                this.logger.Error("UserChromeのシリアル化に失敗しました。", e);
                return new AttemptResult() { Message = "UserChromeのシリアル化に失敗しました。", Exception = e };
            }

            try
            {
                this.fileIO.Write(FileFolder.UserChromePath, content);
            }
            catch (Exception e)
            {
                this.logger.Error("userChrome.jsonへの書き込みに失敗しました。", e);
                return new AttemptResult() { Message = "userChrome.jsonへの書き込みに失敗しました。", Exception = e };
            }


            this.logger.Log("userChrome.jsonへ書き込みにました。");
            return new AttemptResult() { IsSucceeded = true };
        }
    }
}
