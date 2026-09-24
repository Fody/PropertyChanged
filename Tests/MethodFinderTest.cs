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

    [Test]
    public async Task WithStringParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithStringParam");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        await Assert.That(methodReference).IsNotNull();
        await Assert.That(methodReference.MethodReference.Name).IsEqualTo("OnPropertyChanged");
        await Assert.That(methodReference.InvokerType).IsEqualTo(InvokerTypes.String);
    }

    public class WithStringParam
    {
        public void OnPropertyChanged(string propertyName)
        {
        }
    }

    [Test]
    public async Task WithStringAndBeforeAfterParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithStringAndBeforeAfter");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        await Assert.That(methodReference).IsNotNull();
        await Assert.That(methodReference.MethodReference.Name).IsEqualTo("OnPropertyChanged");
        await Assert.That(methodReference.InvokerType).IsEqualTo(InvokerTypes.BeforeAfter);
    }

    public class WithStringAndBeforeAfter
    {
        public void OnPropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    [Test]
    public async Task WithPropertyChangedArgTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithPropertyChangedArg");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        await Assert.That(methodReference).IsNotNull();
        await Assert.That(methodReference.MethodReference.Name).IsEqualTo("OnPropertyChanged");
        await Assert.That(methodReference.InvokerType).IsEqualTo(InvokerTypes.PropertyChangedArg);
    }

    public class WithPropertyChangedArg
    {
        public void OnPropertyChanged(PropertyChangedEventArgs arg)
        {
        }
    }

    [Test]
    public async Task WithSenderPropertyChangedArgTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WithSenderPropertyChangedArg");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        await Assert.That(methodReference).IsNotNull();
        await Assert.That(methodReference.MethodReference.Name).IsEqualTo("OnPropertyChanged");
        await Assert.That(methodReference.InvokerType).IsEqualTo(InvokerTypes.SenderPropertyChangedArg);
    }

    public class WithSenderPropertyChangedArg
    {
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs arg)
        {
        }
    }

    [Test]
    public async Task NoMethodTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "NoMethod");
        await Assert.That(methodFinder.RecursiveFindEventInvoker(definitionToProcess)).IsNull();
    }

    public class NoMethod;

    [Test]
    public async Task NoParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "NoParams");
        await Assert.That(methodFinder.RecursiveFindEventInvoker(definitionToProcess)).IsNull();
    }

    public class NoParams
    {
        public void OnPropertyChanged()
        {
        }
    }

    [Test]
    public async Task WrongParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == "WrongParams");
        await Assert.That(methodFinder.RecursiveFindEventInvoker(definitionToProcess)).IsNull();
    }

    public class WrongParams
    {
        public void OnPropertyChanged(int propertyName)
        {
        }
    }

    [Test]
    [Arguments(nameof(MultipleInvokersStringFirst))]
    [Arguments(nameof(ClassWithMultipleInvokersEventArgsFirst))]
    public async Task PreferEventArgsOverString(string typeName)
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(_ => _.Name == typeName);
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        await Assert.That(methodReference).IsNotNull();
        await Assert.That(methodReference.MethodReference.Name).IsEqualTo("OnPropertyChanged");
        await Assert.That(methodReference.MethodReference.Parameters.First().ParameterType.Name).IsEqualTo(nameof(PropertyChangedEventArgs));
        await Assert.That(methodReference.InvokerType).IsEqualTo(InvokerTypes.PropertyChangedArg);
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
