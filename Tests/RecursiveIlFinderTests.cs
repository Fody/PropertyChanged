using System.Diagnostics;

public class RecursiveIlFinderTests
{
    [Test]
    public async Task Run()
    {
        var typeDefinition = DefinitionFinder.FindType<InnerClass>();
        var recursiveIlFinder = new RecursiveIlFinder(typeDefinition);

        var methodDefinition = typeDefinition.Methods.First(_ => _.Name == "Method1");
        recursiveIlFinder.Execute(methodDefinition);
#if(DEBUG)
        await Assert.That(recursiveIlFinder.Instructions.Count).IsEqualTo(25);
#else
        await Assert.That(recursiveIlFinder.Instructions.Count).IsEqualTo(15);
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

        // ReSharper disable once MemberCanBeMadeStatic.Local
        void Method3()
        {
            Debug.WriteLine("a");
        }

        public string Property { get; set; }
    }
}