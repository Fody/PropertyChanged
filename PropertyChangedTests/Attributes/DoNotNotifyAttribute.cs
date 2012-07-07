using System;

namespace PropertyChanged
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DoNotNotifyAttribute : Attribute
    {
    }
}