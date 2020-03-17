using Fody;

public partial class ModuleWeaver: BaseModuleWeaver
{
    public override void Execute()
    {
        ResolveOnPropertyNameChangedConfig();
        ResolveCheckForEqualityConfig();
        ResolveCheckForEqualityUsingBaseEqualsConfig();
        ResolveUseStaticEqualsFromBaseConfig();
        ResolveSuppressWarningsConfig();
        ResolveSuppressOnPropertyNameChangedWarningConfig();
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
        InitEventArgsCache();
        ProcessTypes();
        InjectEventArgsCache();
        CleanAttributes();
    }

    public override bool ShouldCleanReference => true;
}
