using System.ComponentModel;

public class ClassEqualityWithGenericClassOverload : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public SimpleClass<int> Property1 { get; set; }

    public class SimpleClass<T>
    {
        public int X;

        public override bool Equals(object obj)
        {
            var other = obj as SimpleClass<T>;
            return Equals(this, other);
        }

        public static bool Equals(SimpleClass<T> left, SimpleClass<T> right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }
            return left.X == right.X;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode();
        }

        public static bool operator ==(SimpleClass<T> left, SimpleClass<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SimpleClass<T> left, SimpleClass<T> right)
        {
            return !(left == right);
        }
    }
}