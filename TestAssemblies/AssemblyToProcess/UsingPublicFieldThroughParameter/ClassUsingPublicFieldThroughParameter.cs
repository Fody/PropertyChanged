using System.Diagnostics;

public class ClassUsingPublicFieldThroughParameter
{
    public void Write(ClassWithPublicField application)
    {
        Debug.WriteLine(application.Property1.ToString());
    }
}