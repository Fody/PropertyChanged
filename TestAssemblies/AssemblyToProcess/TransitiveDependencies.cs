using System.ComponentModel;

public class TransitiveDependencies:INotifyPropertyChanged
{
    public string My { get; set; }
    public string MyA => My + "A";
    public string MyAB => MyA + "B";
    public string MyABC => MyAB + "C";
    public event PropertyChangedEventHandler PropertyChanged;
}
