using Caliburn.PresentationFramework;


public class ClassCaliburnOverriddenInvoker : PropertyChangedBase
{
    public string Property1 { get; set; }
    public bool OverrideCalled { get; set; }

    public override void NotifyOfPropertyChange(string propertyName)
    {
        OverrideCalled = true;
        base.NotifyOfPropertyChange(propertyName);
    }

}