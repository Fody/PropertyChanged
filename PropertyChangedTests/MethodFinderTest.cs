using System.ComponentModel;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
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


    [Test]
    public void WithStringParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithStringParam");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.IsNotNull(methodReference);
        Assert.AreEqual("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.AreEqual(InvokerTypes.String, methodReference.InvokerType);
    }

    public class WithStringParam
    {
        public void OnPropertyChanged(string propertyName)
        {
        }
    }


    [Test]
    public void WithStringAndBeforeAfterParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithStringAndBeforeAfter");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.IsNotNull(methodReference);
        Assert.AreEqual("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.AreEqual(InvokerTypes.BeforeAfter, methodReference.InvokerType);
    }

    public class WithStringAndBeforeAfter
    {
        public void OnPropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    [Test]
    public void WithPropertyChangedArgTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithPropertyChangedArg");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.IsNotNull(methodReference);
        Assert.AreEqual("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.AreEqual(InvokerTypes.PropertyChangedArg, methodReference.InvokerType);
    }

    public class WithPropertyChangedArg
    {
        public void OnPropertyChanged(PropertyChangedEventArgs arg)
        {
        }
    }

    [Test]
    public void WithSenderPropertyChangedArgTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithSenderPropertyChangedArg");
        var methodReference = methodFinder.RecursiveFindEventInvoker(definitionToProcess);
        Assert.IsNotNull(methodReference);
        Assert.AreEqual("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.AreEqual(InvokerTypes.SenderPropertyChangedArg, methodReference.InvokerType);
    }

    public class WithSenderPropertyChangedArg
    {
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs arg)
        {
        }
    }

    [Test]
    public void NoMethodTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "NoMethod");
        Assert.IsNull(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class NoMethod
    {
    }
    [Test]
    public void NoParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "NoParams");
        Assert.IsNull(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class NoParams
    {
        public void OnPropertyChanged()
        {
        }
    }

    [Test]
    public void WrongParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WrongParams");
        Assert.IsNull(methodFinder.RecursiveFindEventInvoker(definitionToProcess));
    }

    public class WrongParams
    {
        public void OnPropertyChanged(int propertyName)
        {
        }
    }

}
