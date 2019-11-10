using System;

namespace PropertyChanged
{
    /// <summary>
    /// Specifies a method to call when the property value changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class OnChangedMethodAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OnChangedMethodAttribute"/>.
        /// </summary>
        /// <param name="methodName">The name of a method to call when the property value changes.</param>
        public OnChangedMethodAttribute(string methodName)
        {
        }
    }
}
