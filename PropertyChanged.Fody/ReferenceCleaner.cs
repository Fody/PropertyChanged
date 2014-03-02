using System.Linq;

public partial class ModuleWeaver
{

    public void CleanReferences()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "PropertyChanged");
        if (referenceToRemove == null)
        {
            LogInfo("\tNo reference to 'PropertyChanged' found. References not modified.");
            return;
        }

        ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        LogInfo("\tRemoving reference to 'PropertyChanged'.");
    }
}
