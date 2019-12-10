using System.ComponentModel;
// ReSharper disable ValueParameterNotUsed
// ReSharper disable NotAccessedField.Local

public class ClassWithBranchingReturnAndNoField :
    INotifyPropertyChanged
{
    int x;
    public bool HasValue;

    public string Property1
    {
        get => null;
        set
        {
            if (HasValue)
            {
                x++;
            }
            else
            {
                x++;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}