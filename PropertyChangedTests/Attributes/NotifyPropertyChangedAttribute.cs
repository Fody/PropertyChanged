using System;

namespace PropertyChanged
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NotifyPropertyChangedAttribute : Attribute
    {
    }
}