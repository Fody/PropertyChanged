using System.ComponentModel;

public class BaseClass : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged2(string text1)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(text1));
    }

}