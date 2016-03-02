using System;

namespace PropertyChanged
{
    /// <summary>
    /// Get the before/after value from backing fied instead of calling get method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class BeforeAfterValueCheckFieldAttribute : Attribute
    {
    }
}