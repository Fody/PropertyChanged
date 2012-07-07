using GalaSoft.MvvmLight;

public class ClassMvvmLightFromViewModel : ViewModelBase
{
    public string Property1 { get; set; }
}
public class ClassMvvmLightFromObservableObject : ObservableObject
{
    public string Property1 { get; set; }
}