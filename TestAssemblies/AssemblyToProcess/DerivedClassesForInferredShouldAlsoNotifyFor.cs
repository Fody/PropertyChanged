// Derived classes that inherit Property1 (should not notify for Property2)
public class DerivedClassNoneEmptyForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public string Property2 => Property1;
}
public class DerivedClassEmptyEmptyForInferredShouldAlsoNotifyFor : MiddleClassEmptyForInferredShouldAlsoNotifyFor
{
    public string Property2 => Property1;
}
public class DerivedClassOverrideEmptyForInferredShouldAlsoNotifyFor : MiddleClassOverrideForInferredShouldAlsoNotifyFor
{
    public string Property2 => Property1;
}
public class DerivedClassNewEmptyForInferredShouldAlsoNotifyFor : MiddleClassNewForInferredShouldAlsoNotifyFor
{
    public string Property2 => Property1;
}
public class DerivedClassNewVirtualEmptyForInferredShouldAlsoNotifyFor : MiddleClassNewVirtualForInferredShouldAlsoNotifyFor
{
    public string Property2 => Property1;
}

// Derived classes that override Property1 (should notify for Property2)
public class DerivedClassNoneOverrideForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassEmptyOverrideForInferredShouldAlsoNotifyFor : MiddleClassEmptyForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassOverrideOverrideForInferredShouldAlsoNotifyFor : MiddleClassOverrideForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
    public string Property2 => Property1;
}
// Not a valid combination of property declarations
//public class DerivedClassNewOverrideForInferredShouldAlsoNotifyFor : MiddleClassNewForInferredShouldAlsoNotifyFor
//{
//    public override string Property1 { get; set; }
//    public string Property2 => Property1;
//}
public class DerivedClassNewVirtualOverrideForInferredShouldAlsoNotifyFor : MiddleClassNewVirtualForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
    public string Property2 => Property1;
}

// Derived classes with new Property1 (should notify for Property2)
public class DerivedClassNoneNewForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassEmptyNewForInferredShouldAlsoNotifyFor : MiddleClassEmptyForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassOverrideNewForInferredShouldAlsoNotifyFor : MiddleClassOverrideForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassNewNewForInferredShouldAlsoNotifyFor : MiddleClassNewForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassNewVirtualNewForInferredShouldAlsoNotifyFor : MiddleClassNewVirtualForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}

// Derived classes with new virtual Property1 (should notify for Property2)
public class DerivedClassNoneNewVirtualForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassEmptyNewVirtualForInferredShouldAlsoNotifyFor : MiddleClassEmptyForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassOverrideNewVirtualForInferredShouldAlsoNotifyFor : MiddleClassOverrideForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassNewNewVirtualForInferredShouldAlsoNotifyFor : MiddleClassNewForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassNewVirtualNewVirtualForInferredShouldAlsoNotifyFor : MiddleClassNewVirtualForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}

// Derived classes that explicitly call base.Property1 (should not notify for Property2)
public class DerivedClassNoneEmptyExplicitBaseCallForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public string Property2 => base.Property1;
}
public class DerivedClassEmptyEmptyExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassEmptyForInferredShouldAlsoNotifyFor
{
    public string Property2 => base.Property1;
}
public class DerivedClassOverrideEmptyExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassOverrideForInferredShouldAlsoNotifyFor
{
    public string Property2 => base.Property1;
}
public class DerivedClassNewEmptyExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassNewForInferredShouldAlsoNotifyFor
{
    public string Property2 => base.Property1;
}
public class DerivedClassNewVirtualEmptyExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassNewVirtualForInferredShouldAlsoNotifyFor
{
    public string Property2 => base.Property1;
}
public class DerivedClassNoneOverrideExplicitBaseCallForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassEmptyOverrideExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassEmptyForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassOverrideOverrideExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassOverrideForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
    public string Property2 => base.Property1;
}
// Not a valid combination of property declarations
//public class DerivedClassNewOverrideExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassNewForInferredShouldAlsoNotifyFor
//{
//    public override string Property1 { get; set; }
//    public string Property2 => base.Property1;
//}
public class DerivedClassNewVirtualOverrideExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassNewVirtualForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassNoneNewExplicitBaseCallForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassEmptyNewExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassEmptyForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassOverrideNewExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassOverrideForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassNewNewExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassNewForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassNewVirtualNewExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassNewVirtualForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassNoneNewVirtualExplicitBaseCallForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassEmptyNewVirtualExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassEmptyForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassOverrideNewVirtualExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassOverrideForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassNewNewVirtualExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassNewForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassNewVirtualNewVirtualExplicitBaseCallForInferredShouldAlsoNotifyFor : MiddleClassNewVirtualForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
