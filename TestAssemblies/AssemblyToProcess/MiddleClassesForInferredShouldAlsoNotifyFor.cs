public class MiddleClassEmptyForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
}
public class MiddleClassNewForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public new string Property1 { get; set; }
}
public class MiddleClassNewVirtualForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public new virtual string Property1 { get; set; }
}
public class MiddleClassOverrideForInferredShouldAlsoNotifyFor : BaseClassForInferredShouldAlsoNotifyFor
{
    public override string Property1 { get; set; }
}