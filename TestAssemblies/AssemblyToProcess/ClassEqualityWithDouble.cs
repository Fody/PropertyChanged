using System.ComponentModel;

public class ClassEqualityWithDouble : INotifyPropertyChanged
{
    public double Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}