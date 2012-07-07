using System.ComponentModel;

public class ClassWithStackOverflow : INotifyPropertyChanged
{
    string name;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(name))
            {
                Name = "test";
            }

            return Name;
        }
        set { name = value; }
    }

    public string ValidName { get; set; }

    protected void OnPropertyChanged(string propertyName, object before, object after)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}