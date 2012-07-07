using System.ComponentModel;

namespace AssemblyWithBase.GenericFromAbove
{
    public class BaseClass2 : BaseClass1<object>
    {
    }
    public class BaseClass3 : BaseClass2, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}