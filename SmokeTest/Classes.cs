namespace SmokeTest;

using System.ComponentModel;

using PropertyChanged;

#pragma warning disable CS0067

public class Class1 :
    INotifyPropertyChanged
{
    public int Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

[AddINotifyPropertyChangedInterface]
public class Class2
{
    public int Property1 { get; set; }
}

public class Class3 :
    INotifyPropertyChanged
{
    [AlsoNotifyFor(nameof(Property3))]
    public int Property1 { get; set; }

    [DependsOn(nameof(Property1))]
    public int Property2 { get; set; }

    public int Property3 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class Class4 :
    INotifyPropertyChanged
{
    public bool HasChanged { get; set; }

    public int Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}