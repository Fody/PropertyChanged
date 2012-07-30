## This is an add-in for [Fody](https://github.com/SimonCropp/Fody/) 

Injects [INotifyPropertyChanged](http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx)  code into properties at compile time.

[Introduction to Fody](http://github.com/SimonCropp/Fody/wiki/SampleUsage)

## Nuget package http://nuget.org/packages/PropertyChanged.Fody 

### Your Code

    public class Person : INotifyPropertyChanged
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

## More Info

* [Attributes](https://github.com/SimonCropp/PropertyChanged/wiki/Attributes)
* [BeforeAfter](https://github.com/SimonCropp/PropertyChanged/wiki/BeforeAfter)
* [EqualityChecking](https://github.com/SimonCropp/PropertyChanged/wiki/EqualityChecking)
* [EventInvokerSelectionInjection](https://github.com/SimonCropp/PropertyChanged/wiki/EventInvokerSelectionInjection)
* [ExampleUsage](https://github.com/SimonCropp/PropertyChanged/wiki/ExampleUsage)
* [ImplementingAnIsChangedFlag](https://github.com/SimonCropp/PropertyChanged/wiki/ImplementingAnIsChangedFlag)
* [MVVMLightBroadcast](https://github.com/SimonCropp/PropertyChanged/wiki/MVVMLightBroadcast)
* [NotificationInterception](https://github.com/SimonCropp/PropertyChanged/wiki/NotificationInterception)
* [On_PropertyName_Changed](https://github.com/SimonCropp/PropertyChanged/wiki/On_PropertyName_Changed)
* [PropertyDependencies](https://github.com/SimonCropp/PropertyChanged/wiki/PropertyDependencies)
* [SupportedToolkits](https://github.com/SimonCropp/PropertyChanged/wiki/SupportedToolkits)
* [WeavingTaskOptions](https://github.com/SimonCropp/PropertyChanged/wiki/WeavingTaskOptions)
* [WeavingWithoutAddingAReference](https://github.com/SimonCropp/PropertyChanged/wiki/WeavingWithoutAddingAReference)
