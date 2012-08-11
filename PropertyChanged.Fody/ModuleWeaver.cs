using System;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil;

public class ModuleWeaver
{
    public XElement Config { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public string EventInvokerNames { get; set; }

    public ModuleWeaver()
    {
        LogWarning = s => { };
        LogInfo = s => { };
    }

    public void Execute()
    {
        var eventInvokerNameResolver = new EventInvokerNameResolver(this);
        eventInvokerNameResolver.Execute();
        var msCoreReferenceFinder = new MsCoreReferenceFinder(this, ModuleDefinition.AssemblyResolver);
        msCoreReferenceFinder.Execute();
        var interceptorFinder = new InterceptorFinder(this);
        interceptorFinder.Execute();
        var typeResolver = new TypeResolver();
        var notifyInterfaceFinder = new NotifyInterfaceFinder(typeResolver);


        var typeDefinitions = ModuleDefinition
            .GetTypes()
            .Where(x => x.IsClass && x.BaseType != null)
            .ToList();
        var typeNodeBuilder = new TypeNodeBuilder(this, notifyInterfaceFinder, typeResolver, typeDefinitions);
        typeNodeBuilder.Execute();
        new DoNotNotifyTypeCleaner(typeNodeBuilder).Execute();
        new CodeGenTypeCleaner(typeNodeBuilder).Execute();
        var methodGenerifier = new MethodGenerifier(this);
        var delegateHolderInjector = new DelegateHolderInjector(msCoreReferenceFinder, ModuleDefinition.TypeSystem);
        var methodInjector = new MethodInjector(interceptorFinder, delegateHolderInjector, msCoreReferenceFinder, eventInvokerNameResolver, ModuleDefinition.TypeSystem);
        new MethodFinder(methodGenerifier, methodInjector, typeNodeBuilder, this, typeResolver, eventInvokerNameResolver).Execute();

        new IsChangedMethodFinder(methodGenerifier, this, typeNodeBuilder, typeResolver, ModuleDefinition.TypeSystem).Execute();

        new AllPropertiesFinder(typeNodeBuilder).Execute();
        new MappingFinder(typeNodeBuilder).Execute();
        new IlGeneratedByDependencyProcessor(typeNodeBuilder).Execute();
        new DependsOnDataAttributeReader(typeNodeBuilder, this).Execute();
        var notifyPropertyDataAttributeReader = new NotifyPropertyDataAttributeReader();
        new PropertyDataWalker(typeNodeBuilder, notifyPropertyDataAttributeReader).Execute();
        new WarningChecker(typeNodeBuilder, this).Execute();
        new OnChangedWalker(methodGenerifier, typeNodeBuilder).Execute();
        new StackOverflowChecker(typeNodeBuilder, typeResolver).Execute();
        var typeEqualityFinder = new TypeEqualityFinder(this, msCoreReferenceFinder, typeResolver);
        new TypeProcessor(typeNodeBuilder, this, typeEqualityFinder).Execute();
        new AttributeCleaner(typeDefinitions).Execute();
        new ReferenceCleaner(this).Execute();
    }
}

