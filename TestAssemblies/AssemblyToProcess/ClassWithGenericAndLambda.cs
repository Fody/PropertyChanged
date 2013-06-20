using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class ClassWithGenericAndLambda<T> : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string Property1 { get; set; }

    public void MethodWithLambda(object data)
    {
        var list = new List<object>();
        list.First(container => container == data);
    }
}
public class ClassWithGenericAndLambdaImp : ClassWithGenericAndLambda<object>
{
    
}