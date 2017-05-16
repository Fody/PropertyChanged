using System;

namespace PropertyChanged
{
    /// <summary>
    /// Exclude a <see cref="Type"/> or property from IsChanged flagging.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DoNotSetChangedAttribute : Attribute
    {
    }
}