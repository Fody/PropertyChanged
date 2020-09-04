using System.ComponentModel;

public abstract class BaseClassForInferredShouldAlsoNotifyFor : INotifyPropertyChanged
{
    public virtual string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
