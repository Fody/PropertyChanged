using System;

// ReSharper disable UnusedParameter.Local

namespace PropertyChanged;

/// <summary>
/// Specifies a method to call when the property value changes.
/// Adding this attribute suppresses the default behavior of calling the On&lt;PropertyName&gt;Changed method if it exists.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class OnChangedMethodAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="OnChangedMethodAttribute"/>.
    /// </summary>
    /// <param name="methodName">
    /// The name of a method to call when the property value changes.
    /// When null or empty, does not call any method - this can be used to suppress the default behavior of calling the On&lt;PropertyName&gt;Changed method.
    /// </param>
    public OnChangedMethodAttribute(string methodName)
    {
    }
}