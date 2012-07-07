using System.ComponentModel;

namespace GenericBaseWithProperty
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanged
    {
        public T Property1 { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}