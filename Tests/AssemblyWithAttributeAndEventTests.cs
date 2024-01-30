public class AssemblyWithAttributeAndEventTests
{
    [Fact]
    public void WithAttributeAndEvent()
    {
        var task = new ModuleWeaver();
        var exception = Assert.Throws<WeavingException>(() => task.ExecuteTestRun("AssemblyWithAttributeAndEvent.dll"));
        Assert.Equal("The type 'ClassWithAttributeAndEvent' already has a PropertyChanged event. If type has a [AddINotifyPropertyChangedInterfaceAttribute] then the PropertyChanged event can be removed.", exception.Message);
    }
}