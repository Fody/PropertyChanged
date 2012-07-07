using System.Linq;

public class ReferenceCleaner
{
    ModuleWeaver moduleWeaver;

    public ReferenceCleaner(ModuleWeaver moduleWeaver)
    {
        this.moduleWeaver = moduleWeaver;
    }

    public void Execute()
    {
        var referenceToRemove = moduleWeaver.ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "PropertyChanged");
        if (referenceToRemove == null)
        {
            moduleWeaver.LogInfo("\tNo reference to 'PropertyChanged' found. References not modified.");
            return;
        }

        moduleWeaver.ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        moduleWeaver.LogInfo("\tRemoving reference to 'PropertyChanged'.");
    }
}
