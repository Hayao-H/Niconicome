using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Reactive.Bindings;

namespace NiconicomeTest.Stabs.Models.Domain.Local.Settings
{
    internal class SettingInfoStub<T> : ISettingInfo<T> where T : notnull
    {
        public SettingInfoStub(string settingName, T value)
        {
            this.SettingName = settingName;
            this.Value = value;
            this.ReactiveValue = new ReactiveProperty<T>(value);
        }

        public string SettingName { get; init; }

        public T Value { get; set; }

        public ReactiveProperty<T> ReactiveValue { get; init; }

        public ISettingInfo<T> WithSubscribe(Action<T> handler)
        {
            return this;
        }

        public void UnSubscribe(Action<T> handler)
        {

        }

        public void RegisterWhereFunc(Func<T, bool> predicate)
        {

        }

        public void RegisterSelectFunc(Func<T, T> converter)
        {

        }
    }
}
