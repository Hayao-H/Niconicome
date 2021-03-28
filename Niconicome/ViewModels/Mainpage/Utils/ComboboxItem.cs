using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.ViewModels.Mainpage.Utils
{
    class ComboboxItem<T>
    {
        public ComboboxItem(T value,string displayvalue)
        {
            this.DisplayValue = displayvalue;
            this.Value = value;
        }

        public T Value { get; init; }

        public string DisplayValue { get; init; }
    }
}
