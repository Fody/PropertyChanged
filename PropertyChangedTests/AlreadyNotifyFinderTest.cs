using System.Linq;
using NUnit.Framework;

// ReSharper disable UnusedParameter.Local

[TestFixture]
public class AlreadyNotifyFinderTest
{

    [Test]
    public void ContainsNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotificationProperty);

        var moduleWeaver = new ModuleWeaver();
        var propertyNames = moduleWeaver .GetAlreadyNotifies(propertyDefinition);
        Assert.AreEqual(1,propertyNames.Count());
    }
    [Test]
    public void MultipleNotifications()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Multiple().Property);

        var moduleWeaver = new ModuleWeaver();
        var propertyNames = moduleWeaver.GetAlreadyNotifies(propertyDefinition).ToList();
        Assert.Contains("Property1",propertyNames);
        Assert.Contains("Property2",propertyNames);
    }

    [Test]
    public void WithoutNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithoutNotificationProperty);

        var moduleWeaver = new ModuleWeaver();
        var propertyNames = moduleWeaver.GetAlreadyNotifies(propertyDefinition).ToList();
        Assert.IsEmpty(propertyNames);
    }


    [Test]
    public void AlreadyContainsNotificationVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Virtual().WithNotificationProperty);

        var moduleWeaver = new ModuleWeaver();
        var propertyNames = moduleWeaver.GetAlreadyNotifies(propertyDefinition).ToList();
        Assert.IsNotEmpty(propertyNames);
    }

    [Test]
    public void AlreadyContainsNotificationNonVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotificationProperty);

        var moduleWeaver = new ModuleWeaver();
        var propertyNames = moduleWeaver.GetAlreadyNotifies(propertyDefinition).ToList();
        Assert.IsNotEmpty(propertyNames);
    }

    public class NonVirtual
    {
        public int WithoutNotificationProperty { get; set; }

        public int WithNotificationProperty
        {
            get
            {
                return 0;
            }
            set
            {
                OnPropertyChanged("WithNotificationProperty");
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

        public int WithNotificationProperty
        {
            get
            {
                return 0;
            }
            set
            {
                OnPropertyChanged("WithNotificationProperty");
            }
        }

        public virtual void OnPropertyChanged(string property)
        {


        }
    }
}
// ReSharper restore UnusedParameter.Local