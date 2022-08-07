namespace SmokeTest1
{
    using System.ComponentModel;
    using PropertyChanged;

    public partial class Class1234 : INotifyPropertyChanged
    {
        public string P1 { get; set; }
    }

    public partial class Class1234 // : INotifyPropertyChanged
    {
        public string P2 { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public partial class Class1234
    {
        public string P3 { get; set; }
    }

    public partial class XYZ123 : INotifyPropertyChanged
    {
        public string P3 { get; set; }
    }
    public partial class XYZ123<T> : INotifyPropertyChanged
    {
        public T P3 { get; set; }
    }
}