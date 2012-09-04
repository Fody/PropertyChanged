using System.ComponentModel;

public class ClassWithLdflda : INotifyPropertyChanged
{
    decimal? property1;
    public event PropertyChangedEventHandler PropertyChanged;

    public decimal? Property1
    {
        get { return property1; }
        set
        {
            if (value == 0.0m)
            {
                property1 = null;
            }
            else
            {
                property1 = value;
            }
        }
    }
}