using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.Command;

namespace Niconicome.Extensions.System
{
    public static class Array
    {
        public static IBindableCommand CombineLatestAreAllTrue(this IBindableProperty<bool>[] source, Action command)
        {
            return new BindableCommand(command, source);
        }
    }
}
