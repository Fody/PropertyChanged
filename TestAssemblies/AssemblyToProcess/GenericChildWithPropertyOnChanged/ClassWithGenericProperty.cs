using System.ComponentModel;

namespace GenericChildWithPropertyOnChanged
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}