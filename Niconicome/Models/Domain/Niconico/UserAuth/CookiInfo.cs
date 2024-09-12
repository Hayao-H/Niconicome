using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Store.V2;

namespace Niconicome.Models.Domain.Niconico.UserAuth
{
    public interface ICookieInfo
    {
        /// <summary>
        /// UserSession
        /// </summary>
        string UserSession { get; set; }

        /// <summary>
        /// UserSessionSecure
        /// </summary>
        string UserSessionSecure { get; set; }
    }

    public class CookieInfo : ICookieInfo
    {
        public CookieInfo(ICookieStore store,string userSession,string userSessionSecure)
        {
            this._store = store;
            this._userSession = userSession;
            this._userSessionSecure = userSessionSecure;
        }

        private string _userSession = string.Empty;

        private string _userSessionSecure = string.Empty;

        private readonly ICookieStore _store;

        public string UserSession
        {
            get => this._userSession;
            set
            {
                this._userSession = value;
                if (this.CanUpdate())
                {
                    this._store.Update(this);
                }
            }
        }

        public string UserSessionSecure
        {
            get => this._userSessionSecure;
            set
            {
                this._userSessionSecure = value;
                if (this.CanUpdate())
                {
                    this._store.Update(this);
                }
            }
        }

        private bool CanUpdate()
        {
            return !this.UserSessionSecure.IsNullOrEmpty()&&!this.UserSession.IsNullOrEmpty();
        }
    }
}
