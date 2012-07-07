using System.ComponentModel;

namespace ComplexHierachy
{
    public class ClassParent: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}