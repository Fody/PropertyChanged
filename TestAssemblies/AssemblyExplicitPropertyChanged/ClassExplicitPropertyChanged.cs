using System.ComponentModel;
#pragma warning disable 649

public class ClassExplicitPropertyChanged :
    INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged
    {
        add { }
        remove { }
    }
}