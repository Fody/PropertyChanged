using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("ReSharper", "DelegateSubtraction")]
public class MethodInjectorTests
{
    [Fact]
    public void ShouldFindCorrectHandlerFieldInDefaultImpl()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassWithMultipleHandlerFieldsDefaultImpl).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.NotNull(field);
        Assert.NotEqual(ClassWithMultipleHandlerFieldsDefaultImpl.UnexpectedFieldName, field.Name);
    }

    [Fact]
    public void ShouldFindCorrectHandlerFieldInCustomImpl()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassWithMultipleHandlerFieldsCustomImpl).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.NotNull(field);
        Assert.Equal(ClassWithMultipleHandlerFieldsCustomImpl.ExpectedFieldName, field.Name);
    }

    [Fact]
    public void ShouldFindCorrectHandlerFieldInExplicitImpl()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassWithMultipleHandlerFieldsExplicitImpl).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.NotNull(field);
        Assert.Equal(ClassWithMultipleHandlerFieldsExplicitImpl.ExpectedFieldName, field.Name);
    }

    [Fact]
    public void ShouldNotConfuseExplicitImplWithUnrelatedEvent()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassWithExplicitImplAndPropertyChangedEvent).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.NotNull(field);
        Assert.Equal(ClassWithExplicitImplAndPropertyChangedEvent.ExpectedFieldName, field.Name);
    }

    [Fact]
    public void ShouldFindCorrectHandlerFieldInClassThatReImplementsInterface()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassThatReImplementsInterface).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.NotNull(field);
        Assert.Equal(ClassThatReImplementsInterface.ExpectedFieldName, field.Name);
    }

    [Fact]
    public void ShouldNotFindUnrelatedFields()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassWithMultipleHandlerFieldsCustomInvalid).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.Null(field);
    }

    [Fact]
    public void ShouldNotFindUnrelatedFieldsInExplicitImpl()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassWithMultipleHandlerFieldsExplicitInvalid).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.Null(field);
    }

    [Fact]
    public void ShouldNotFindHandlerFieldInDerivedClass()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassDerivedFromExplicit).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.Null(field);
    }

    [Fact]
    public void ShouldNotFindAmbiguousField()
    {
        var type = methodInjector.ModuleDefinition.GetType(typeof(ClassWithMultipleHandlerFieldReferences).FullName, true).Resolve();
        var field = methodInjector.GetEventHandlerField(type);

        Assert.Null(field);
    }

    ModuleWeaver methodInjector = new()
    {
        ModuleDefinition = ModuleDefinition.ReadModule(typeof(MethodInjectorTests).Assembly.Location)
    };
#pragma warning disable CS0067, CS0169, CS0649

    class ClassWithMultipleHandlerFieldsDefaultImpl :
    INotifyPropertyChanged    {
        public event PropertyChangedEventHandler PropertyChanged;

        PropertyChangedEventHandler Other;
        public const string UnexpectedFieldName = nameof(Other);
    }

    class ClassWithMultipleHandlerFieldsCustomImpl :
    INotifyPropertyChanged    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Second += value;
            remove => Second -= value;
        }

        PropertyChangedEventHandler First;
        PropertyChangedEventHandler Second;
        PropertyChangedEventHandler Third;
        public const string ExpectedFieldName = nameof(Second);
    }

    class ClassWithMultipleHandlerFieldsExplicitImpl :
    INotifyPropertyChanged    {
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => Second += value;
            remove => Second -= value;
        }

        PropertyChangedEventHandler First;
        PropertyChangedEventHandler Second;
        PropertyChangedEventHandler Third;
        public const string ExpectedFieldName = nameof(Second);
    }

    class ClassWithExplicitImplAndPropertyChangedEvent :
    INotifyPropertyChanged    {
        event PropertyChangedEventHandler PropertyChanged
        {
            add => First += value;
            remove => First -= value;
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => Second += value;
            remove => Second -= value;
        }

        PropertyChangedEventHandler First;
        PropertyChangedEventHandler Second;
        PropertyChangedEventHandler Third;
        public const string ExpectedFieldName = nameof(Second);
    }

    class ClassWithMultipleHandlerFieldsCustomInvalid :
    INotifyPropertyChanged    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { }
            remove { }
        }

        PropertyChangedEventHandler Other;
    }

    class ClassWithMultipleHandlerFieldsExplicitInvalid :
    INotifyPropertyChanged    {
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }

        PropertyChangedEventHandler Other;
    }

    class ClassWithMultipleHandlerFieldReferences :
    INotifyPropertyChanged    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                First += value;
                Second += value;
            }
            remove
            {
                First -= value;
                Second -= value;
            }
        }

        PropertyChangedEventHandler First;
        PropertyChangedEventHandler Second;
    }

    class ClassDerivedFromExplicit :
        ClassWithMultipleHandlerFieldsDefaultImpl
    {
        PropertyChangedEventHandler Other;
    }

    class ClassThatReImplementsInterface :
        ClassWithMultipleHandlerFieldsExplicitImpl,
        INotifyPropertyChanged
    {
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => RealTarget += value;
            remove => RealTarget -= value;
        }

        PropertyChangedEventHandler Other;
        PropertyChangedEventHandler RealTarget;
        public new const string ExpectedFieldName = nameof(RealTarget);
    }
}
