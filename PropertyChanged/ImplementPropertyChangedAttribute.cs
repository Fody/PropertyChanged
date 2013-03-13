using System;
using System.ComponentModel;

namespace PropertyChanged
{
    /// <summary>
    /// Include a <see cref="Type"/> for notification.
    /// The <see cref="INotifyPropertyChanged"/> interface is added to the type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementPropertyChangedAttribute : Attribute
    {
    }
}