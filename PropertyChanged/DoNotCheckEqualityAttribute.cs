using System;

namespace PropertyChanged
{
    /// <summary>
    /// Skip equality check before change notification
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DoNotCheckEqualityAttribute : Attribute
    {

    }
}