using System.Linq;
using NUnit.Framework;

// ReSharper disable UnusedParameter.Local

[TestFixture]
public class AlreadyNotifyFinderTest
{


    [Test]
    public void ContainsNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotifactionProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanged");
        Assert.AreEqual(1,propertyNames.Count());
    }
    [Test]
    public void MultipleNotifications()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Multiple().Property);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanged").ToList();
        Assert.Contains("Property1",propertyNames);
        Assert.Contains("Property2",propertyNames);
    }

    [Test]
    public void WithoutNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithoutNotificationProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanged");
        Assert.IsEmpty(propertyNames);
    }


    [Test]
    public void AlreadyContainsNotificationVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Virtual().WithNotifactionProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanged");
        Assert.IsNotEmpty(propertyNames);
    }

    [Test]
    public void AlreadyContainsNotificationNonVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotifactionProperty);

        var propertyNames = propertyDefinition.GetAlreadyNotifies("OnPropertyChanged");
        Assert.IsNotEmpty(propertyNames);
    }

    public class NonVirtual
    {
        public int WithoutNotificationProperty { get; set; }

        public int WithNotifactionProperty
        {
            get
            {
                return 0;
            }
            set
            {
                OnPropertyChanged("WithNotifactionProperty");
            }
        }

        void OnPropertyChanged(string property){}

    }
    public class Multiple
    {

        public int Property
        {
            get
            {
                return 0;
            }
            set
            {
                OnPropertyChanged("Property1");
                OnPropertyChanged("Property2");
            }
        }

        void OnPropertyChanged(string property){}
    }
    public class Virtual
    {
        public int WithoutNotificationProperty { get; set; }

        public int WithNotifactionProperty
        {
            get
            {
                return 0;
            }
            set
            {
                OnPropertyChanged("WithNotifactionProperty");
            }
        }

        public virtual void OnPropertyChanged(string property)
        {


        }
    }
}
// ReSharper restore UnusedParameter.Local