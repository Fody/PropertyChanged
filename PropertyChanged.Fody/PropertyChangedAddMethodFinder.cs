using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mono.Cecil;

public static class PropertyChangedAddMethodFinder
{
    public static IEnumerable<MethodDefinition> GetPropertyChangedAddMethods(this TypeDefinition targetType)
    {
        var foundExplicit = false;

        foreach (var method in targetType.Methods.Where(i => i.Overrides.Any(o => o.FullName == "System.Void System.ComponentModel.INotifyPropertyChanged::add_PropertyChanged(System.ComponentModel.PropertyChangedEventHandler)")))
        {
            foundExplicit = true;
            yield return method;
        }

        if (foundExplicit)
        {
            yield break;
        }

        foreach (var method in targetType.Events.Where(i => i.Name == nameof(INotifyPropertyChanged.PropertyChanged)))
        {
            yield return method.AddMethod;
        }
    }
}
