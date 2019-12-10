using System;
using System.ComponentModel;
#pragma warning disable 649
// ReSharper disable RedundantJumpStatement

public class ClassWithBranchingReturn1 :
    INotifyPropertyChanged
{
    string property1;
    bool isInSomeMode;

    public string Property1
    {
        get => property1;
        set
        {
            property1 = value;
            if (isInSomeMode)
            {
                Console.WriteLine("code here so 'if' does not get optimized away in release mode");
                return;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}