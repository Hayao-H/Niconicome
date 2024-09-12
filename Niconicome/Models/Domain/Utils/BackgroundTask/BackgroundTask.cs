using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Utils.Reactive;

namespace Niconicome.Models.Domain.Utils.BackgroundTask
{
    public record BackgroundTask(IBindableProperty<bool> IsDone, string TaskID);
}
