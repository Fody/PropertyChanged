using NUnit.Framework;


[TestFixture]
public class Net35WeavingTaskTests : BaseTaskTests
{

    public Net35WeavingTaskTests()
        : base(@"AssemblyToProcess\AssemblyToProcessDotNet3.5.csproj")
    {
    }

}