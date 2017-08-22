[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/PropertyChanged.Fody.svg?style=flat)](https://www.nuget.org/packages/PropertyChanged.Fody/)

![Icon](https://raw.github.com/Fody/PropertyChanged/master/Icons/package_icon.png)

Injects code which raises the [`PropertyChanged` event](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.propertychanged(v=vs.110).aspx), into property setters of classes which implement [INotifyPropertyChanged](https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx).

This is an add-in for [Fody](https://github.com/Fody/Fody/); it is available via [NuGet](https://nuget.org/packages/PropertyChanged.Fody/):

    PM> Install-Package PropertyChanged.Fody

---


# Overview

**NOTE: All classes that implement `INotifyPropertyChanged` will have notification code injected into property setters.**

Your code:
```csharp
public class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }
    public string FullName => $"{GivenNames} {FamilyName}";
}
```

What gets compiled:

```csharp
public class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    string givenNames;
    public string GivenNames
    {
        get => givenNames;
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
        get => familyName;
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

    public string FullName => $"{GivenNames} {FamilyName}";

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
---


# Notes

* **Dependent properties** -- In the above sample, the getter for `FullName` depends on the getters for `GivenName` and `FamilyName`. Therefore, when either `GivenName` or `FamilyName` is set, `PropertyChanged` is raised for `FullName` as well.   This behavior can be configured manually using the [`AlsoNotifyFor` attribute](https://github.com/Fody/PropertyChanged/wiki/Attributes#alsonotifyforattribute) on the source property, or the [`DependsOn` attribute](https://github.com/Fody/PropertyChanged/wiki/Attributes#dependsonattribute) on the target property).
* **Intercepting the notification call**
    * [**Global interception**](https://github.com/Fody/PropertyChanged/wiki/NotificationInterception)
    * **Class-level interception** --The `OnPropertyChanged` method will only be injected if there is no such existing method on the class; if there is such a method, then calls to that method will be injected into the setters -- see [here](https://github.com/Fody/PropertyChanged/wiki/EventInvokerSelectionInjection).
    * **Property-level interception** -- For a given property, if there is a method of the form `On<PropertyName>Changed`, then that method will be called -- see [here](https://github.com/Fody/PropertyChanged/wiki/On_PropertyName_Changed).
* To get the [**before / after values**](https://github.com/Fody/PropertyChanged/wiki/BeforeAfter), use the following signature for `OnPropertyChanged` / `On<PropertyName>Changed`:

      public void OnPropertyChanged(string propertyName, object before, object after)
* To prevent a specific class from having the notification call injection, use the [`DoNotNotify` attribute](https://github.com/Fody/PropertyChanged/wiki/Attributes#donotnotifyattribute).
* To scope the rewriting only to specific classes, and not the whole Assembly, you can use the [`FilterType` attribute](https://github.com/Fody/PropertyChanged/wiki/Attributes#filtertypeattribute). This changes the general behavior from from opt-out to opt-in.
* The `INotifyPropertyChanged` interface can be automatically implemented for a specific class using the [`AddINotifyPropertyChangedInterfaceAttribute` attribute](https://github.com/Fody/PropertyChanged/wiki/Attributes#addinotifypropertychangedinterfaceattribute). **Raising an issue about "this attribute does not behave as expected" will result in a RTFM and the issue being closed.**
* Behavior is configured via [attributes](https://github.com/Fody/PropertyChanged/wiki/Attributes), or via [options in the `Weavers.xml` file](https://github.com/Fody/PropertyChanged/wiki/Options).

For more information, see the [wiki pages](https://github.com/Fody/PropertyChanged/wiki).


# Contributors

 * [Cameron MacFarland](https://github.com/distantcam)
 * [Geert van Horrik](https://github.com/GeertvanHorrik)
 * [Simon Cropp](https://github.com/simoncropp)

Icon courtesy of [The Noun Project](https://thenounproject.com)
