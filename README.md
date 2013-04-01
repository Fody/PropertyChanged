## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Injects [INotifyPropertyChanged](http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx)  code into properties at compile time.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

### Note: NotifyPropertyWeaver

Users of the [NotifyPropertyWeaver](https://github.com/SimonCropp/NotifyPropertyWeaver) extension who are migrating to [Fody](https://github.com/Fody/fody) will want to use NuGet to Install the PropertyChanged.Fody package along with Fody itself to get the same functionality as before. This is because Fody is a general purpose weaver with plugins while NotifyPropertyWeaver was specific to one scenario. That scenario now lives in the [PropertyChanged addin](https://github.com/Fody/PropertyChanged). See [Converting from NotifyPropertyWeaver](https://github.com/Fody/PropertyChanged/wiki/ConvertingFromNotifyPropertyWeaver) for more information 

## Nuget 

Nuget package http://nuget.org/packages/PropertyChanged.Fody 

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package PropertyChanged.Fody

### Your Code

    [ImplementPropertyChanged]
    public class Person 
    {        
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
	


### What gets compiled

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
    
## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)

## More Info

* [Attributes](https://github.com/Fody/PropertyChanged/wiki/Attributes)
* [BeforeAfter](https://github.com/Fody/PropertyChanged/wiki/BeforeAfter)
* [EqualityChecking](https://github.com/Fody/PropertyChanged/wiki/EqualityChecking)
* [EventInvokerSelectionInjection](https://github.com/Fody/PropertyChanged/wiki/EventInvokerSelectionInjection)
* [ExampleUsage](https://github.com/Fody/PropertyChanged/wiki/ExampleUsage)
* [ImplementingAnIsChangedFlag](https://github.com/Fody/PropertyChanged/wiki/ImplementingAnIsChangedFlag)
* [MVVMLightBroadcast](https://github.com/Fody/PropertyChanged/wiki/MVVMLightBroadcast)
* [NotificationInterception](https://github.com/Fody/PropertyChanged/wiki/NotificationInterception)
* [On_PropertyName_Changed](https://github.com/Fody/PropertyChanged/wiki/On_PropertyName_Changed)
* [PropertyDependencies](https://github.com/Fody/PropertyChanged/wiki/PropertyDependencies)
* [SupportedToolkits](https://github.com/Fody/PropertyChanged/wiki/SupportedToolkits)
* [Options](https://github.com/Fody/PropertyChanged/wiki/Options)
* [WeavingWithoutAddingAReference](https://github.com/Fody/PropertyChanged/wiki/WeavingWithoutAddingAReference)
