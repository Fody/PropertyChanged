using System.ComponentModel;

public class TransitiveDependencies:INotifyPropertyChanged
{
    public string My { get; set; }
    public string MyA { get { return My + "A"; } }
    public string MyAB { get { return MyA + "B"; } }
    public string MyABC { get { return MyAB + "C"; } }
    public event PropertyChangedEventHandler PropertyChanged;
}
