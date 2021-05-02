using System;

namespace Niconicome.ViewModels
{
    [AttributeUsage(AttributeTargets.All)]
    class ViewModelAttribute:Attribute
    {
        public Type ViewModelType { get; init; }

        public ViewModelAttribute(Type vmType)
        {
            this.ViewModelType = vmType;
        }
    }
}
