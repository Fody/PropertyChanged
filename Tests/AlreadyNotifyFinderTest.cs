
// ReSharper disable MemberCanBeMadeStatic.Local

// ReSharper disable ValueParameterNotUsed
// ReSharper disable UnusedParameter.Local

public class AlreadyNotifyFinderTest
{
    [Fact]
    public void ContainsNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotificationProperty);

        var weaver = new ModuleWeaver();
        var propertyNames = weaver.GetAlreadyNotifies(propertyDefinition);
        Assert.Single(propertyNames);
    }

    [Fact]
    public void MultipleNotifications()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Multiple().Property);

        var weaver = new ModuleWeaver();
        var propertyNames = weaver.GetAlreadyNotifies(propertyDefinition).ToList();
        Assert.Contains("Property1",propertyNames);
        Assert.Contains("Property2",propertyNames);
    }

    [Fact]
    public void WithoutNotification()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithoutNotificationProperty);

        var weaver = new ModuleWeaver();
        var propertyNames = weaver.GetAlreadyNotifies(propertyDefinition).ToList();
        Assert.Empty(propertyNames);
    }

    [Fact]
    public void AlreadyContainsNotificationVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new Virtual().WithNotificationProperty);

        var weaver = new ModuleWeaver();
        var propertyNames = weaver.GetAlreadyNotifies(propertyDefinition).ToList();
        Assert.NotEmpty(propertyNames);
    }

    [Fact]
    public void AlreadyContainsNotificationNonVirtual()
    {
        var propertyDefinition = DefinitionFinder.FindProperty(() => new NonVirtual().WithNotificationProperty);

        var weaver = new ModuleWeaver();
        var propertyNames = weaver.GetAlreadyNotifies(propertyDefinition).ToList();
        Assert.NotEmpty(propertyNames);
    }

    public class NonVirtual
    {
        public int WithoutNotificationProperty { get; set; }

        public int WithNotificationProperty
        {
            get => 0;
            set => OnPropertyChanged("WithNotificationProperty");
        }

        void OnPropertyChanged(string property){}
    }

    public class Multiple
    {
        public int Property
        {
            get => 0;
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
            get => 0;
            set => OnPropertyChanged("WithNotificationProperty");
        }

        public virtual void OnPropertyChanged(string property)
        {
        }
    }
}
// ReSharper restore UnusedParameter.Local