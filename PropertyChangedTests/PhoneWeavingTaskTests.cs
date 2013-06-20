#if(DEBUG)

using NUnit.Framework;

[TestFixture]
public class PhoneWeavingTaskTests : BaseTaskTests
{

    public PhoneWeavingTaskTests()
        : base(@"AssemblyToProcess\AssemblyToProcessPhone.csproj")
    {
    }
    public override void WithGenericAndLambda() { }
}
#endif