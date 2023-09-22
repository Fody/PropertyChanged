using System;
using System.ComponentModel;
#pragma warning disable 649

public class ClassWithBranchingReturnAndBeforeAfter :
    INotifyPropertyChanged
{
    string property1;
    bool isInSomeMode;
    public event PropertyChangedEventHandler PropertyChanged;

    public string Property1
    {
        get => property1;
        set
        {
            property1 = value;
            if (isInSomeMode)
            {
                Console.WriteLine("code here so 'if' does not get optimized away in release mode");
// ReSharper disable RedundantJumpStatement
                return;
// ReSharper restore RedundantJumpStatement
            }
        }
    }

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}