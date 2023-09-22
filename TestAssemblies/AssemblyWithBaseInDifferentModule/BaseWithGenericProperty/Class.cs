using System.ComponentModel;
using System.Runtime.CompilerServices;
using AssemblyWithBase.BaseWithEquals;

namespace AssemblyWithBaseInDifferentModule.BaseWithGenericProperty;

public class Class :
    INotifyPropertyChanged    {
    string property1;

    public string Property1
    {
        get => property1;
        set
        {
            property1 = value;
            Property2 = new();
        }
    }

    public BaseClass1<int> Property2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}