using Fody;
using Xunit;

public class InjectOnPropertyNameChangedTests
{
    [Fact]
    public void ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_ThrowsWeavingExceptionForNonVoidMethods()
    {
        var moduleWeaver = new ModuleWeaver { InjectOnPropertyNameChanged = true };

        var weavingException = Assert.Throws<WeavingException>(() =>
        {
            moduleWeaver.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll");
        });

        Assert.Equal("The type ClassWithNonVoidOnPropertyChanged has a On_PropertyName_Changed method (OnProperty1Changed) that has a non void return value. Ensure the return type void.", weavingException.Message);
    }

    [Fact]
    public void ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_ThrowsWeavingExceptionForStaticMethods()
    {
        var moduleWeaver = new ModuleWeaver { InjectOnPropertyNameChanged = true };

        var weavingException = Assert.Throws<WeavingException>(() =>
        {
            moduleWeaver.ExecuteTestRun("AssemblyWithStaticOnPropertyNameChanged.dll");
        });

        Assert.Equal("The type ClassWithStaticOnPropertyChanged has a On_PropertyName_Changed method (OnProperty1Changed) which is static.", weavingException.Message);
    }

    [Fact]
    public void ModuleWeaver_WhenInjectOnPropertyNameChangedIsFalse_DoesNotThrowWeavingException()
    {
        var moduleWeaver = new ModuleWeaver { InjectOnPropertyNameChanged = false };

        moduleWeaver.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll");
        moduleWeaver.ExecuteTestRun("AssemblyWithStaticOnPropertyNameChanged.dll");
    }
}
