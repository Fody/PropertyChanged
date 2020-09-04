using System.ComponentModel;

public class NonvirtualBaseClassForInferredShouldAlsoNotifyFor : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
