using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Const =Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Niconico.Download.General;
using Niconicome.Models.Helper.Result;

namespace NiconicomeTest.Stabs.Models.Domain.Niconico.Download.General
{
    public class ReplaceHandlerStub : IReplaceHandler
    {
        public ReplaceHandlerStub()
        {
            this.ReplaceRules = new ReadOnlyObservableCollection<IReplaceRule>(new ObservableCollection<IReplaceRule>(this.Convert(Const::Format.DefaultReplaceRules)));
        }

        public ReadOnlyObservableCollection<IReplaceRule> ReplaceRules { get; init; }

        public IAttemptResult AddRule(string replaceFrom, string replaceTo)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult RemoveRule(string replaceFrom, string replaceTo)
        {
            return AttemptResult.Succeeded();
        }

        public IEnumerable<IReplaceRule> Convert(IEnumerable<string> source)
        {
            return source.Select(s =>
            {
                string[] splited = s.Split("to");

                var to = splited.Length == 1 ? string.Empty : splited[1];

                return new ReplaceRule(splited[0], to);
            });
        }
    }
}
