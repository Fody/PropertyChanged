using System.ComponentModel;

namespace PropertyChangedTestWithDifferentNamespace;

public class TestClassIncludeAlso :
    INotifyPropertyChanged    {
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}