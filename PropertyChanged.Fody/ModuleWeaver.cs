using System;
using System.Xml.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public XElement Config { get; set; }
    public Action<string> LogDebug { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }

    public ModuleWeaver()
    {
        LogWarning = s => { };
        LogInfo = s => { };
        LogDebug = s => { };
    }

    public void Execute()
    {
        ResolveOnPropertyNameChangedConfig();
        ResolveCheckForEqualityConfig();
        ResolveEventInvokerName();
        FindCoreReferences();
        FindInterceptor();
        ProcessFilterTypeAttributes();
        BuildTypeNodes();
        CleanDoNotNotifyTypes();
        CleanCodeGenedTypes();
        FindMethodsForNodes();
        FindIsChangedMethod();
        FindAllProperties();
        FindMappings();
        DetectIlGeneratedByDependency();
        ProcessDependsOnAttributes();
        WalkPropertyData();
        CheckForWarnings();
        ProcessOnChangedMethods();
        CheckForStackOverflow();
        FindComparisonMethods();
        ProcessTypes();
        CleanAttributes();
        CleanReferences();
    }
}

