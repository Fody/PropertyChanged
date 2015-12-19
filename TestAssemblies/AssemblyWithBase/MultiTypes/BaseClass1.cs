using System.ComponentModel;

namespace AssemblyWithBase.MultiTypes
{
    public class BaseClass1<T,Z> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}