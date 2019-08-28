using System.ComponentModel;

namespace AssemblyWithDisabledInjectOnPropertyNameChanged
{
    public class ClassWithNonVoidOnPropertyChangedReturnType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int MyProperty { get; set; }

        private int OnMyPropertyChanged()
        {
            return 1;
        }
    }
}
