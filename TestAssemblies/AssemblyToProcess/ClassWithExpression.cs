using System;
using System.ComponentModel;
using System.Linq.Expressions;

public class ClassWithExpression : INotifyPropertyChanged
{
    public string Property1;

    public ClassWithExpression()
    {
#pragma warning disable 168
        Expression<Func<ClassWithExpression, string>> expressionFunc = x => x.Property1;
        Func<ClassWithExpression, string> func = x => x.Property1;

        Action<ClassWithExpression, string> expression2 = (expression, s) => { expression.Property1 = s; };
#pragma warning restore 168
    }

    public event PropertyChangedEventHandler PropertyChanged;
}