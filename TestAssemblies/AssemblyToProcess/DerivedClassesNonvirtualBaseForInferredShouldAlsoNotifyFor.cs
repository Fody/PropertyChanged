public class DerivedClassNonvirtualBaseNoneForInferredShouldAlsoNotifyFor : NonvirtualBaseClassForInferredShouldAlsoNotifyFor
{
    public string Property2 => Property1;
}
public class DerivedClassNonvirtualBaseNewForInferredShouldAlsoNotifyFor : NonvirtualBaseClassForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => Property1;
}
public class DerivedClassNonvirtualBaseNewVirtualNonvirtualBaseForInferredShouldAlsoNotifyFor : NonvirtualBaseClassForInferredShouldAlsoNotifyFor
{
    public new virtual string Property1 { get; set; }
    public string Property2 => Property1;
}

public class DerivedClassNonvirtualBaseNoneExplicitBaseCallForInferredShouldAlsoNotifyFor : NonvirtualBaseClassForInferredShouldAlsoNotifyFor
{
    public string Property2 => base.Property1;
}
public class DerivedClassNonvirtualBaseNewExplicitBaseCallForInferredShouldAlsoNotifyFor : NonvirtualBaseClassForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
    public string Property2 => base.Property1;
}
public class DerivedClassNonvirtualBaseNewVirtualExplicitBaseCallForInferredShouldAlsoNotifyFor : NonvirtualBaseClassForInferredShouldAlsoNotifyFor
{
    public new virtual string Property1 { get; set; }
    public string Property2 => base.Property1;
}