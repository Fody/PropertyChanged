using System;

namespace PropertyChanged
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class DoNotNotifyAttribute : Attribute
    {
    }
}