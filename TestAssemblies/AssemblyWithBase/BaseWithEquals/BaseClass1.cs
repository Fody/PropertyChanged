using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AssemblyWithBase.BaseWithEquals;

public class BaseClass1<T> :
    INotifyPropertyChanged    {
    public event PropertyChangedEventHandler PropertyChanged;

    public static bool EqualsCalled { get; set; }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    public static bool operator ==(BaseClass1<T> first, BaseClass1<T> second)
    {
        EqualsCalled = true;
        return false;
    }

    public static bool operator !=(BaseClass1<T> first, BaseClass1<T> second)
    {
        return false;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((BaseClass1<T>)obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}