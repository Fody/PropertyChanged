using Fody;
using Xunit;
using Xunit.Abstractions;

public class AssemblyWithNonVoidOnPropertyChangedTests :
    XunitApprovalBase
{
    [Fact]
    public void Simple()
    {
        var weavingTask = new ModuleWeaver();
        var weavingException = Assert.Throws<WeavingException>(() =>
        {
            weavingTask.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll");
        });
        Assert.Equal("The type ClassWithNonVoidOnPropertyChanged has a On_PropertyName_Changed method (OnProperty1Changed) that has a non void return value. Ensure the return type void.", weavingException.Message);
    }

    public AssemblyWithNonVoidOnPropertyChangedTests(ITestOutputHelper output) :
        base(output)
    {
    }
}