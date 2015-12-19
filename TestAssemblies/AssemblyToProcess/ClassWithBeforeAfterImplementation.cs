using System;
using System.ComponentModel;
using PropertyChanged;

public class ClassWithBeforeAfterImplementation : INotifyPropertyChanged
{

    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        ValidateIsString(after);
        ValidateIsString(before);

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    void ValidateIsString(object value)
    {
        if (value != null)
        {
            var name = value.GetType().Name;
            if (name != "String")
            {
                throw new Exception($"Value shoud be string but is '{name}'.");
            }
        }
    }
}