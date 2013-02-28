using System;
using System.Xml.Linq;
using Mono.Cecil;
using PropertyChanged.Fody;

public partial class ModuleWeaver
{
    private InterfaceInjector interfaceInjector;

    public XElement Config { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }

    public ModuleWeaver()
    {
        LogWarning = s => { };
        LogInfo = s => { };
    }

    public void Execute()
    {
        this.interfaceInjector = new InterfaceInjector(ModuleDefinition);

        ResolveEventInvokerName();
        FindCoreReferences();
        FindInterceptor();
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

