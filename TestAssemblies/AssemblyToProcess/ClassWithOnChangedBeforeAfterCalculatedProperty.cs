using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public class ClassWithOnChangedBeforeAfterCalculatedProperty
{

    public string Property1 { get; set; }
    public int Property2 => Property1?.Length ?? 0;

    public string Property2ChangeValue;

    public void OnProperty2Changed(object before, object after)
    {
        Property2ChangeValue = $"From {before} to {after}";
    }
}