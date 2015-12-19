using System.ComponentModel;

namespace GenericChildWithPropertyBeforeAfter
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}