using System.ComponentModel;

namespace AssemblyWithBase.BaseWithGenericParent
{
    public class BaseClass1<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}