using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Utils = Niconicome.Models.Domain.Utils;
using Auth = Niconicome.Models.Auth;
using Niconico = Niconicome.Models.Domain.Niconico;

namespace Niconicome.Workspaces
{
    class LoginPage
    {
        public static Niconico::IAccountManager AccountManager { get; private set; } = Utils::DIFactory.Provider.GetRequiredService<Niconico::IAccountManager>();
        public static Auth::ISession Session { get; private set; } = Utils::DIFactory.Provider.GetRequiredService<Auth::ISession>();
    }
}
