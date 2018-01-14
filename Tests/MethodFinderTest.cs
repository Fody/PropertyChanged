using System.ComponentModel;
using System.Linq;
using Mono.Cecil;
using Xunit;


public class MethodFinderTest
{
    TypeDefinition typeDefinition;
    ModuleWeaver methodFinder;

    public MethodFinderTest()
    {
        var codeBase = typeof(MethodFinderTest).Assembly.CodeBase.Replace("file:///", string.Empty);
        var module = ModuleDefinition.ReadModule(codeBase);
        methodFinder = new ModuleWeaver
                               {
                                   ModuleDefinition = module
                               };

        typeDefinition = module.Types.First(x => x.Name.EndsWith("MethodFinderTest"));
    }

    [Fact]
    public void WithStringParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithStringParam");
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
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithStringAndBeforeAfter");
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
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithPropertyChangedArg");
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
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithSenderPropertyChangedArg");
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
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "NoMethod");
        Assert.Null(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class NoMethod
    {
    }
    [Fact]
    public void NoParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "NoParams");
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
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WrongParams");
        Assert.Null(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class WrongParams
    {
        public void OnPropertyChanged(int propertyName)
        {
        }
    }
}