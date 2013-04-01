public class Class1<T> : Caliburn.Micro.PropertyChangedBase, IClass1<T> where T : class, new()
{
    public T[] SelectedItems { get; set; }

    public T SelectedItem { get; set; }
}

public interface IClass1<T> where T : class, new()
{
    T[] SelectedItems { get; }

    T SelectedItem { get; }
}