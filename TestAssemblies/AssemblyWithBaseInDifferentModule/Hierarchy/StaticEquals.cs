using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssemblyWithBaseInDifferentModule.Hierarchy;

public class StaticEquals :
    INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}