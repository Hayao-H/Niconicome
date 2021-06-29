using Niconicome.Models.Domain.Local.Style;
using Types = Niconicome.Models.Domain.Local.Style.Type;
using Niconicome.Models.Helper.Result;
using Reactive.Bindings;

namespace Niconicome.Models.Local.State
{
    interface IStyleHandler
    {
        ReadOnlyReactiveProperty<Types::UserChrome?> UserChrome { get; }
        IAttemptResult SaveUserChrome();
    }

    class StyleHandler : IStyleHandler
    {
        public StyleHandler(IUserChromeHandler userChromeHandler)
        {
            this.userChromeHandler = userChromeHandler;
            this.UserChrome = this.userChromeHandler.UserChrome.ToReadOnlyReactiveProperty();
        }

        #region field

        private readonly IUserChromeHandler userChromeHandler;

        #endregion

        /// <summary>
        /// userChrome
        /// </summary>
        public ReadOnlyReactiveProperty<Types::UserChrome?> UserChrome { get; init; }

        /// <summary>
        /// userChrome.jsonを書き込む
        /// </summary>
        /// <returns></returns>
        public IAttemptResult SaveUserChrome()
        {
            Types::UserChrome userChrome = this.UserChrome.Value ?? new Types::UserChrome();

            return this.userChromeHandler.SaveStyle(userChrome);
        }

    }
}
