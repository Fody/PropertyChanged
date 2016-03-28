using System.ComponentModel;

public class ClassEqualityWithGenericStructOverload : INotifyPropertyChanged
{
    public SimpleStruct<int> Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    
#pragma warning disable 660,661
    public struct SimpleStruct<T>
#pragma warning restore 660,661
    {

        public int X ;
        public static bool operator ==(SimpleStruct<T> left, SimpleStruct<T> right)
        {
            return left.X == right.X;
        }

        public static bool operator !=(SimpleStruct<T> left, SimpleStruct<T> right)
        {
            return !(left == right);
        }
    }

}