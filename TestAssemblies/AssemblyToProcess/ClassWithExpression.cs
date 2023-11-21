using System;
using System.ComponentModel;
using System.Linq.Expressions;
// ReSharper disable UnusedVariable

public class ClassWithExpression :
    INotifyPropertyChanged
{
    public string Property1;

    public ClassWithExpression()
    {
        Expression<Func<ClassWithExpression, string>> expressionFunc = _ => _.Property1;
        Func<ClassWithExpression, string> func = _ => _.Property1;

        Action<ClassWithExpression, string> expression2 = (expression, s) => { expression.Property1 = s; };
    }

    public event PropertyChangedEventHandler PropertyChanged;
}