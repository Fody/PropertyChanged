using NUnit.Framework;

[TestFixture]
public class AssemblyWithNonVoidOnPropertyChangedTests
{
    [Test]
    public void Simple()
    {
        var weavingException = Assert.Throws<WeavingException>(() => new WeaverHelper("AssemblyWithNonVoidOnPropertyNameChanged"));
        Assert.AreEqual("The type ClassWithNonVoidOnPropertyChanged has a On_PropertyName_Changed method (OnProperty1Changed) that has a non void return value. Please make the return type void.", weavingException.Message);
    }
}