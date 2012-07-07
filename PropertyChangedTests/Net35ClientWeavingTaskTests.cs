using NUnit.Framework;


[TestFixture]
public class Net35ClientWeavingTaskTests : BaseTaskTests
{

    public Net35ClientWeavingTaskTests()
        : base(@"AssemblyToProcess\AssemblyToProcessDotNet3.5Client.csproj")
    {
    }

}