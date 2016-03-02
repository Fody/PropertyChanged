using System;
using System.ComponentModel;
using PropertyChanged;

[BeforeAfterValueCheckField]
public class ClassWithBeforeAfterValueCheckFieldImplementation : INotifyPropertyChanged
{
    private string _property1;

    private string _property2;

    public string Property1
    {
        get
        {
            if (_property1 == null)
            {
                throw new Exception("Please set Property1 before calling get");
            }
            return _property1;
        }
        set
        {
            _property1 = value;
        }
    }

    [DoNotNotify]
    public string BeforeValue1 { get; set; }

    public string Property2
    {
        get
        {
            if (_property2 == null)
            {
                _property2 = "Loaded";
            }
            return _property2;
        }
        set
        {
            _property2 = value;
        }
    }

    [DoNotNotify]
    public string BeforeValue2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        if (propertyName.Equals("Property1"))
        {
            BeforeValue1 = (string)before;
        }

        if (propertyName.Equals("Property2"))
        {
            BeforeValue2 = (string)before;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}