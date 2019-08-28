using Fody;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

public class OnPropertyNameChangedConfigTests :
    XunitLoggingBase
{
    [Fact]
    public void False()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='false'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.False(moduleWeaver.InjectOnPropertyNameChanged);
    }

    [Fact]
    public void True()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='true'/>");
        var moduleWeaver = new ModuleWeaver {Config = xElement};
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.True(moduleWeaver.InjectOnPropertyNameChanged);
    }

    [Fact]
    public void Default()
    {
        var moduleWeaver = new ModuleWeaver();
        moduleWeaver.ResolveOnPropertyNameChangedConfig();
        Assert.True(moduleWeaver.InjectOnPropertyNameChanged);
    }

    [Fact]
    public void ModuleWeaver_WhenInjectOnPropertyNameChangedIsTrue_ThrowsWeavingException()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='true'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveOnPropertyNameChangedConfig();

        var weavingException = Assert.Throws<WeavingException>(() =>
        {
            moduleWeaver.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll");
        });

        Assert.Equal("The type ClassWithNonVoidOnPropertyChanged has a On_PropertyName_Changed method (OnProperty1Changed) that has a non void return value. Ensure the return type void.", weavingException.Message);
    }

    [Fact]
    public void ModuleWeaver_WhenInjectOnPropertyNameChangedIsFalse_DoesNotThrowWeavingException()
    {
        var xElement = XElement.Parse("<PropertyChanged InjectOnPropertyNameChanged='false'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ResolveOnPropertyNameChangedConfig();

        moduleWeaver.ExecuteTestRun("AssemblyWithNonVoidOnPropertyNameChanged.dll");
    }

    public OnPropertyNameChangedConfigTests(ITestOutputHelper output) :
        base(output)
    {
    }
}