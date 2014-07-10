using System;

namespace PropertyChanged
{
    /// <summary>
    /// Specifies that PropertyChanged Notification will be added to a class.
    /// PropertyChanged.Fody will weave the <c>INotifyPropertyChanged</c> interface and implementation into the class.
    /// When the value of a property changes, the PropertyChanged notification will be raised automatically
    /// See https://github.com/Fody/PropertyChanged for more information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementPropertyChangedAttribute : Attribute
    {
    }
}
