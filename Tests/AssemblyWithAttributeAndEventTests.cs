// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyWithAttributeAndEventTests
{
    [Test]
    public async Task WithAttributeAndEvent()
    {
        var task = new ModuleWeaver();
        var exception = await Assert.That(() => { task.ExecuteTestRun("AssemblyWithAttributeAndEvent.dll"); }).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("The type 'ClassWithAttributeAndEvent' already has a PropertyChanged event. If type has a [AddINotifyPropertyChangedInterfaceAttribute] then the PropertyChanged event can be removed.");
    }
}