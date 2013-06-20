using System.ComponentModel;

namespace ComplexHierarchy
{
    public class ClassParent: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}