using System.ComponentModel;

public class ClassWithNested
{

    public class ClassNested : INotifyPropertyChanged
    {
        public string Property1 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public class ClassNestedNested : INotifyPropertyChanged
        {
            public string Property1 { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

        }
    }
}