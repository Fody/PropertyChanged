[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/PropertyChanged.Fody.svg?style=flat)](https://www.nuget.org/packages/PropertyChanged.Fody/)


## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/PropertyChanged/master/Icons/package_icon.png)

Injects [INotifyPropertyChanged](http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx) code into properties at compile time.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage) 


## The NuGet package  

https://nuget.org/packages/PropertyChanged.Fody/

    PM> Install-Package PropertyChanged.Fody


### Your Code

**NOTE: All classes that have `INotifyPropertyChanged` will have notification code injected into property sets.**

```
public class Person :INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }

    public string FullName
    {
        get
        {
            return string.Format("{0} {1}", GivenNames, FamilyName);
        }
    }
}
```


### What gets compiled

```
public class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    string givenNames;
    public string GivenNames
    {
        get { return givenNames; }
        set
        {
            if (value != givenNames)
            {
                givenNames = value;
                OnPropertyChanged("GivenNames");
                OnPropertyChanged("FullName");
            }
        }
    }

    string familyName;
    public string FamilyName
    {
        get { return familyName; }
        set 
        {
            if (value != familyName)
            {
                familyName = value;
                OnPropertyChanged("FamilyName");
                OnPropertyChanged("FullName");
            }
        }
    }

    public string FullName
    {
        get
        {
            return string.Format("{0} {1}", GivenNames, FamilyName);
        }
    }

    public virtual void OnPropertyChanged(string propertyName)
    {
        var propertyChanged = PropertyChanged;
        if (propertyChanged != null)
        {
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```


## Icon

Icon courtesy of [The Noun Project](https://thenounproject.com)


## Contributors

 * [Cameron MacFarland](https://github.com/distantcam)
 * [Geert van Horrik](https://github.com/GeertvanHorrik)
 * [Simon Cropp](https://github.com/simoncropp)


## More Info

* [Attributes](https://github.com/Fody/PropertyChanged/wiki/Attributes)
* [BeforeAfter](https://github.com/Fody/PropertyChanged/wiki/BeforeAfter)
* [EqualityChecking](https://github.com/Fody/PropertyChanged/wiki/EqualityChecking)
* [EventInvokerSelectionInjection](https://github.com/Fody/PropertyChanged/wiki/EventInvokerSelectionInjection)
* [ExampleUsage](https://github.com/Fody/PropertyChanged/wiki/ExampleUsage)
* [ImplementingAnIsChangedFlag](https://github.com/Fody/PropertyChanged/wiki/Implementing-An-IsChanged-Flag)
* [MVVMLightBroadcast](https://github.com/Fody/PropertyChanged/wiki/MVVMLightBroadcast)
* [NotificationInterception](https://github.com/Fody/PropertyChanged/wiki/NotificationInterception)
* [On_PropertyName_Changed](https://github.com/Fody/PropertyChanged/wiki/On_PropertyName_Changed)
* [PropertyDependencies](https://github.com/Fody/PropertyChanged/wiki/PropertyDependencies)
* [SupportedToolkits](https://github.com/Fody/PropertyChanged/wiki/SupportedToolkits)
* [Options](https://github.com/Fody/PropertyChanged/wiki/Options)
* [WeavingWithoutAddingAReference](https://github.com/Fody/PropertyChanged/wiki/WeavingWithoutAddingAReference)
