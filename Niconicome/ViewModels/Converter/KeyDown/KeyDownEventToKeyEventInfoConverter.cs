using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Reactive.Bindings.Interactivity;
using System.Reactive.Linq;

namespace Niconicome.ViewModels.Converter.KeyDown
{
    internal class KeyDownEventToKeyEventInfoConverter : ReactiveConverter<KeyEventArgs, KeyEventInfo>
    {
        protected override IObservable<KeyEventInfo> OnConvert(IObservable<KeyEventArgs?> source)
        {
            return source.Select(e =>
            {
                if (e is null)
                {
                    throw new InvalidOperationException("KeyEventArgs is null.");
                }
                return new KeyEventInfo(e.Key);
            });
        }
    }
}
