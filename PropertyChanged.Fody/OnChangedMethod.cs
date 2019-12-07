﻿using Mono.Cecil;

public enum OnChangedTypes
{
    None,
    NoArg,
    BeforeAfter,
}

public class OnChangedMethod
{
    public MethodReference MethodReference;
    public OnChangedTypes OnChangedType;
    public bool IsDefaultMethod;
}
