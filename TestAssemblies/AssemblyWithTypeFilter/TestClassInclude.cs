using System.ComponentModel;

namespace PropertyChangedTest
{
    public class TestClassInclude : INotifyPropertyChanged
    {
        public string Property1 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}