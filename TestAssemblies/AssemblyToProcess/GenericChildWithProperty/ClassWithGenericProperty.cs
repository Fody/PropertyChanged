using System.ComponentModel;

namespace GenericChildWithProperty
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

    }
}