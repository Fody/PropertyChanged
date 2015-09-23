using System.ComponentModel;

public class TestClassExclude : INotifyPropertyChanged
{
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

namespace PropertyChangedTest
{
    public class TestClassInclude : INotifyPropertyChanged
    {
        public string Property1 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}