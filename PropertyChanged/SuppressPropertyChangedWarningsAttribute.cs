using System;

namespace PropertyChanged;

/// <summary>
/// Suppresses warnings emitted by PropertyChanged.Fody
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
public sealed class SuppressPropertyChangedWarningsAttribute : Attribute;