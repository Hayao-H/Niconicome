using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;
using Reactive.Bindings;

namespace NiconicomeTest.Stabs.Models.Domain.Niconico
{
    class NiconicoContextStab : INiconicoContext
    {
        public bool IsLogin { get => true; }

        public ReactiveProperty<User?> User { get; } = new ReactiveProperty<User?>();

        public async Task<bool> LoginAsync(string u, string p)
        {
            await Task.Delay(1);
            return true;
        }

        public async Task LogoutAsync()
        {
            await Task.Delay(1);
        }

        public async Task<string> GetUserNameAsync(string i)
        {
            await Task.Delay(1);
            return string.Empty;
        }

        public Uri GetPageUri(string id)
        {
            return new Uri("https://nicovideo.jp");
        }

        public async Task RefreshUser()
        {
            await Task.Delay(1);
        }
    }
}
