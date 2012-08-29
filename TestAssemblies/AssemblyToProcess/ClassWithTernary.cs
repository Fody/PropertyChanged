using System.ComponentModel;

public class ClassWithTernary : INotifyPropertyChanged
{
    decimal? property1;
    public event PropertyChangedEventHandler PropertyChanged;

    public decimal? Property1
    {
        get { return property1; }
        set
        {
            var newValue = value == 0.0m ? null : value;
            if (property1 != newValue)
            {
                property1 = newValue;
            }
        }
    }
}