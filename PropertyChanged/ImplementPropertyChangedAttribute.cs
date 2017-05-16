using System;
using System.ComponentModel;

namespace PropertyChanged
{
    /// <summary>
    /// Specifies that PropertyChanged Notification will be added to a class.
    /// <para>
    /// PropertyChanged.Fody will weave the <see cref="INotifyPropertyChanged"/> interface and implementation into the class.
    /// When the value of a property changes, the PropertyChanged notification will be raised automatically
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ImplementPropertyChangedAttribute : Attribute
    {
    }
}
