using Niconicome.Models.Domain.Local.Style;
using Niconicome.Models.Domain.Local.Style.Type;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.State
{
    interface IStyleHandler
    {
        UserChrome UserChrome { get; }
        IAttemptResult SaveUserChrome();
    }

    class StyleHandler : IStyleHandler
    {
        public StyleHandler(IUserChromeHandler userChromeHandler)
        {
            this.userChromeHandler = userChromeHandler;
        }

        #region field

        private readonly IUserChromeHandler userChromeHandler;

        private UserChrome? userChrome;

        #endregion

        /// <summary>
        /// userChrome
        /// </summary>
        public UserChrome UserChrome
        {
            get
            {
                if (this.userChrome is null)
                {
                    this.userChrome = this.userChromeHandler.GetUserChrome().Data;
                }

                return this.userChrome ?? new UserChrome();
            }
        }

        /// <summary>
        /// userChrome.jsonを書き込む
        /// </summary>
        /// <returns></returns>
        public IAttemptResult SaveUserChrome()
        {
            return this.userChromeHandler.SaveStyle(this.UserChrome);
        }
    }
}
