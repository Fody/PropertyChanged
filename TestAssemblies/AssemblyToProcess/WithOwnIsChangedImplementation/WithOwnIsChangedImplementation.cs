using System.ComponentModel;

public class WithOwnIsChangedImplementation : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsChanged { get; private set; }

}