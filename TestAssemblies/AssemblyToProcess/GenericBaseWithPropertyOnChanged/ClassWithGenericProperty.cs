using System.ComponentModel;

namespace GenericBaseWithPropertyOnChanged
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanged
    {
        public bool OnProperty1ChangedCalled;
        public bool OnProperty2ChangedCalled;

        public T Property1 { get; set; }
        public void OnProperty1Changed()
        {
            OnProperty1ChangedCalled = true;
        }

        public T Property2 { get; set; }
        public void OnProperty2Changed()
        {
            OnProperty2ChangedCalled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}