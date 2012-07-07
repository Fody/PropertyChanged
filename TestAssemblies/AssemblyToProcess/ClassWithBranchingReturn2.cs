using System.ComponentModel;

public class ClassWithBranchingReturn2 : INotifyPropertyChanged
{
    string property1;
    public bool HasValue;

    public string Property1
    {
        get { return property1; }
        set
        {
            if (HasValue)
            {
                property1 = value;
            }
            else
            {
                property1 = value;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}