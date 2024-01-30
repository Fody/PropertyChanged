using System.ComponentModel;
using Mono.Cecil;

public class MethodFinderTest
{
    TypeDefinition typeDefinition;
    ModuleWeaver methodFinder;

    public MethodFinderTest()
    {
        var location = typeof(MethodFinderTest).Assembly.Location;
        var module = ModuleDefinition.ReadModule(location);
        methodFinder = new()
        {
            ModuleDefinition = module
        };

        typeDefinition = module.Types.First(_ => _.Name.EndsWith("MethodFinderTest"));
    }

    [Fact]
    public void WithStringParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithStringParam");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.NotNull(methodReference);
        Assert.Equal("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.Equal(InvokerTypes.String, methodReference.InvokerType);
    }

    public class WithStringParam
    {
        public void OnPropertyChanged(string propertyName)
        {
        }
    }

    [Fact]
    public void WithStringAndBeforeAfterParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithStringAndBeforeAfter");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.NotNull(methodReference);
        Assert.Equal("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.Equal(InvokerTypes.BeforeAfter, methodReference.InvokerType);
    }

    public class WithStringAndBeforeAfter
    {
        public void OnPropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    [Fact]
    public void WithPropertyChangedArgTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithPropertyChangedArg");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.NotNull(methodReference);
        Assert.Equal("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.Equal(InvokerTypes.PropertyChangedArg, methodReference.InvokerType);
    }

    public class WithPropertyChangedArg
    {
        public void OnPropertyChanged(PropertyChangedEventArgs arg)
        {
        }
    }

    [Fact]
    public void WithSenderPropertyChangedArgTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithSenderPropertyChangedArg");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.NotNull(methodReference);
        Assert.Equal("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.Equal(InvokerTypes.SenderPropertyChangedArg, methodReference.InvokerType);
    }

    public class WithSenderPropertyChangedArg
    {
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs arg)
        {
        }
    }

    [Fact]
    public void NoMethodTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "NoMethod");
        Assert.Null(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class NoMethod;

    [Fact]
    public void NoParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "NoParams");
        Assert.Null(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class NoParams
    {
        public void OnPropertyChanged()
        {
        }
    }

    [Fact]
    public void WrongParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WrongParams");
        Assert.Null(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class WrongParams
    {
        public void OnPropertyChanged(int propertyName)
        {
        }
    }

    [Theory]
    [InlineData(nameof(MultipleInvokersStringFirst))]
    [InlineData(nameof(ClassWithMultipleInvokersEventArgsFirst))]
    public void PreferEventArgsOverString(string typeName)
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == typeName);
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.NotNull(methodReference);
        Assert.Equal("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.Equal(nameof(PropertyChangedEventArgs), methodReference.MethodReference.Parameters.First().ParameterType.Name);
        Assert.Equal(InvokerTypes.PropertyChangedArg, methodReference.InvokerType);
    }

    public class MultipleInvokersStringFirst
    {
        protected void OnPropertyChanged(string propertyName)
        {
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
        }
    }

    public class ClassWithMultipleInvokersEventArgsFirst
    {
        protected void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
        }

        protected void OnPropertyChanged(string propertyName)
        {
        }
    }
}
