using System;

namespace PropertyChanged
{
    /// <summary>
    /// Exclude a <see cref="Type"/> or property from notification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public class DoNotNotifyAttribute : Attribute
    {
    }
}