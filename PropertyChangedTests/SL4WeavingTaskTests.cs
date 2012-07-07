using NUnit.Framework;

[TestFixture]
public class SL4WeavingTaskTests : BaseTaskTests
{

    public SL4WeavingTaskTests()
        : base(@"AssemblyToProcess\AssemblyToProcessSilverlight4.csproj")
    {
    }

}