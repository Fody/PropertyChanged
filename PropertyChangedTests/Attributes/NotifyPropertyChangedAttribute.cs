using System;

namespace PropertyChanged
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NotifyPropertyChangedAttribute : Attribute
    {
    }
}