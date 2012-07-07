using System.Collections.ObjectModel;
using System.ComponentModel;

public class A : ObservableCollection<string>
{
}
public class B : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}