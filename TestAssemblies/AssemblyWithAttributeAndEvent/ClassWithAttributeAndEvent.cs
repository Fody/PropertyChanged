using System.ComponentModel;
using PropertyChanged;

[AddINotifyPropertyChangedInterface]
public class ClassWithAttributeAndEvent
{
    // ReSharper disable once EventNeverSubscribedTo.Global
#pragma warning disable 67
    public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67
}