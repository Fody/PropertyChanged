using System;
using System.ComponentModel;

namespace PropertyChanged
{
    /// <summary>
    /// Specifies that the class will be marked with <see cref="INotifyPropertyChanged"/>.
    /// Note that all classes that implement <see cref="INotifyPropertyChanged"/> will have property notification
    /// injected irrespective of the use of this attribute.
    /// Raising an issue about "this attribute does not behave as expected" will result in a RTFM and the issue being closed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AddINotifyPropertyChangedInterfaceAttribute : Attribute
    {
    }
}