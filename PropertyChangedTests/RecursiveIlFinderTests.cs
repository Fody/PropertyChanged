using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class RecursiveIlFinderTests
{


    [Test]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<InnerClass>();
        var recursiveIlFinder = new RecursiveIlFinder(typeDefinition);

        var methodDefinition = typeDefinition.Methods.First(x => x.Name == "Method1");
        recursiveIlFinder.Execute(methodDefinition);
#if(DEBUG)
        Assert.AreEqual(25, recursiveIlFinder.Instructions.Count);
#else
        Assert.AreEqual(22, recursiveIlFinder.Instructions.Count);
#endif
    }

    public abstract class InnerClass
    {


        public abstract string AbstractMethod();


        public void Method1()
        {
            Property = "aString";
            Method2();
        }

        void Method2()
        {
            AbstractMethod();
            Method3();
            Method1();
        }

        void Method3()
        {
            Debug.WriteLine("a");
        }

        public string Property { get; set; }
    }
}