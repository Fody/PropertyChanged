using System.Collections.Generic;

public partial class ModuleWeaver
{
    public void DetectIlGeneratedByDependency()
    {
        DetectIlGeneratedByDependency(NotifyNodes);
    }


    void DetectIlGeneratedByDependency(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            var ilGeneratedByDependencyReader = new IlGeneratedByDependencyReader(node);
            ilGeneratedByDependencyReader.Process();
            DetectIlGeneratedByDependency(node.Nodes);
        }
    }


}