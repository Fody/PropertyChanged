using System.ComponentModel;

public class ClassWithBranchingReturnAndNoField : INotifyPropertyChanged
{
// ReSharper disable NotAccessedField.Local
    int x;
// ReSharper restore NotAccessedField.Local
    public bool HasValue;

    public string Property1
    {
        get { return null; }
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