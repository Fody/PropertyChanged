using System;

namespace PropertyChanged
{
    /// <summary>
    /// Suppresses warnings emitted by PropertyChanged.Fody
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SuppressPropertyChangedWarningsAttribute : Attribute
    {
    }
}
