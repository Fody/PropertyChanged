using System;
using System.Collections.Generic;
using System.ComponentModel;
#if (DEBUG)
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
#endif
using System.Reflection;
using NUnit.Framework;

public abstract class BaseTaskTests
{
    string projectPath;
    Assembly assembly;
    WeaverHelper weaverHelper;

    protected BaseTaskTests(string projectPath)
    {

#if (RELEASE)
            projectPath = projectPath.Replace("Debug", "Release");
#endif
        this.projectPath = projectPath;
    }

    [TestFixtureSetUp]
    public void Setup()
    {
        weaverHelper = new WeaverHelper(projectPath);
        assembly = weaverHelper.Assembly;
    }



#if (DEBUG)
    [Test]
    [Ignore]
    public void EnsureOnly1RefToMscorLib()
    {
        var moduleDefinition = ModuleDefinition.ReadModule(assembly.CodeBase.Remove(0, 8));
        foreach (var assemblyNameReference in moduleDefinition.AssemblyReferences)
        {
            Trace.WriteLine(assemblyNameReference.FullName);
        }
        Assert.AreEqual(1, moduleDefinition.AssemblyReferences.Count(x => x.Name == "mscorlib"));
    }
#endif
    [Test]
    public void AlsoNotifyFor()
    {
        var instance = assembly.GetInstance("ClassAlsoNotifyFor");
        EventTester.TestProperty(instance, true);
    }
    [Test]
    public void AlreadyHasNotifcation()
    {
        var instance = assembly.GetInstance("ClassAlreadyHasNotifcation");
        var property1EventCount = 0;
        var property2EventCount = 0;
        ((INotifyPropertyChanged) instance).PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Property1")
                {
                    property1EventCount++;
                }
                if (args.PropertyName == "Property2")
                {
                    property2EventCount++;
                }
            };
        instance.Property1 = "a";

        Assert.AreEqual(1,property1EventCount);
        Assert.AreEqual(1, property2EventCount);
        property1EventCount = 0;
        property2EventCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.AreEqual(0,property1EventCount);
        Assert.AreEqual(0,property2EventCount);
    }
    [Test]
    public void AlreadyHasSingleNotifcation()
    {
        var instance = assembly.GetInstance("ClassAlreadyHasSingleNotifcation");
        var property1EventCount = 0;
        var property2EventCount = 0;
        ((INotifyPropertyChanged) instance).PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Property1")
                {
                    property1EventCount++;
                }
                if (args.PropertyName == "Property2")
                {
                    property2EventCount++;
                }
            };
        instance.Property1 = "a";

        Assert.AreEqual(1,property1EventCount);
        Assert.AreEqual(1, property2EventCount);
        property1EventCount = 0;
        property2EventCount = 0;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.AreEqual(0,property1EventCount);
        Assert.AreEqual(0,property2EventCount);
    }

    [Test]
    public void WithFieldGetButNoFieldSet()
    {
        var instance = assembly.GetInstance("ClassWithFieldGetButNoFieldSet");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void WithGenericStructPropImpl()
    {
        var instance = assembly.GetInstance("ClassWithGenericStructPropImpl");

        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = 2;

        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public void WithDoNotNotify()
    {
        var type = assembly.GetType("ClassWithDoNotNotify", true);
        Assert.IsEmpty(type.GetCustomAttributes(false));
    }

    [Test]
    public void WithDependencyAfterSet()
    {
        var instance = assembly.GetInstance("ClassWithDependencyAfterSet");

        var property1EventCalled = false;
        var property2EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
                                                                   {
                                                                       if (args.PropertyName == "Property1")
                                                                       {
                                                                           property1EventCalled = true;
                                                                       }
                                                                       if (args.PropertyName == "Property2")
                                                                       {
                                                                           property2EventCalled = true;
                                                                           Assert.AreEqual("a", instance.Property2);
                                                                       }
                                                                   };
        instance.Property1 = "a";

        Assert.IsTrue(property1EventCalled);
        Assert.IsTrue(property2EventCalled);
    }

    [Test]
    public void VirtualForNonSealed()
    {
        var type = assembly.GetType("ClassThatIsNotSealed", true);
        var methodInfo = type.GetMethod("OnPropertyChanged");
        Assert.IsTrue(methodInfo.IsVirtual);
    }

    [Test]
    public void SealedForSealed()
    {
        var type = assembly.GetType("ClassThatIsSealed", true);
        var methodInfo = type.GetMethod("OnPropertyChanged");
        Assert.IsFalse(methodInfo.IsVirtual);
    }


    [Test]
    public void WithTryCatchInSet()
    {
        var instance = assembly.GetInstance("ClassWithTryCatchInSet");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void WithPropertySetInCatch()
    {
        var instance = assembly.GetInstance("ClassWithPropertySetInCatch");
        EventTester.TestProperty(instance, false);
    }


    [Test]
    public void UsingPublicFieldThroughParameter()
    {
        var classWithPublicField = assembly.GetInstance("ClassWithPublicField");
        var classUsingPublicFieldThroughParameter = assembly.GetInstance("ClassUsingPublicFieldThroughParameter");
        classUsingPublicFieldThroughParameter.Write(classWithPublicField);
    }

    [Test]
    public void Equality()
    {
        var instance = assembly.GetInstance("ClassEquality");
        EventTester.TestProperty(instance, "StringProperty", "foo");
        EventTester.TestProperty(instance, "IntProperty", 2);
        EventTester.TestProperty(instance, "NullableIntProperty", 2);
        EventTester.TestProperty(instance, "BoolProperty", true);
        EventTester.TestProperty(instance, "NullableBoolProperty", true);
        EventTester.TestProperty(instance, "ObjectProperty", "foo");
        EventTester.TestProperty(instance, "ArrayProperty", new[] { "foo" });
    }
    [Test]
    public void WithCompilerGeneratedAttribute()
    {
        var instance = assembly.GetInstance("ClassWithCompilerGeneratedAttribute");
        EventTester.TestPropertyNotCalled(instance);
    }

    [Test]
    public void WithGeneratedCodeAttribute()
    {
        var instance = assembly.GetInstance("ClassWithGeneratedCodeAttribute");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void NoBackingNoEqualityField()
    {
        var instance = assembly.GetInstance("ClassNoBackingNoEqualityField");

        var eventCalled = false;
        instance.PropertyChanged += new PropertyChangedEventHandler((sender, args) =>
        {
            if (args.PropertyName == "StringProperty")
            {
                eventCalled = true;
            }
        });

        instance.StringProperty = "sfsd";
        Assert.IsTrue(eventCalled);
    }


    [Test]
    public void NoBackingEqualityField()
    {
        var instance = assembly.GetInstance("ClassNoBackingWithEqualityField");

        var eventCalled = false;
        instance.PropertyChanged += new PropertyChangedEventHandler((sender, args) =>
        {
            if (args.PropertyName == "StringProperty")
            {
                eventCalled = true;
            }
        });

        instance.StringProperty = "sfsd";
        Assert.IsTrue(eventCalled);
    }

    [Test]
    public void WithFieldFromOtherClass()
    {
        var instance = assembly.GetInstance("ClassWithFieldFromOtherClass");

        var eventCalled = false;
        instance.PropertyChanged += new PropertyChangedEventHandler((sender, args) =>
                                                                        {
                                                                            if (args.PropertyName == "Property1")
                                                                            {
                                                                                eventCalled = true;
                                                                            }
                                                                        });

        instance.Property1 = "sfsd";
        Assert.IsTrue(eventCalled);
    }

    [Test]
    public void WithIndexerClass()
    {
        var instance = assembly.GetInstance("ClassWithIndexer");

        var eventCalled = false;
        instance.PropertyChanged += new PropertyChangedEventHandler((sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                eventCalled = true;
            }
        });

        instance[4] = "sdfsdf";
        Assert.AreEqual("sdfsdf", instance[4]);
        instance.Property1 = "sfsd";
        Assert.IsTrue(eventCalled);
    }

    [Test]
    public void WithTernary()
    {
        var instance = assembly.GetInstance("ClassWithTernary");

        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = 1;

        Assert.IsTrue(property1EventCalled);
    }
    [Test]
    public void WithLdflda()
    {
        var instance = assembly.GetInstance("ClassWithLdflda");

        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
            {
               property1EventCalled = true;
            };
        instance.Property1 = (Nullable<decimal>) 0.0;

        Assert.IsTrue(property1EventCalled);
    }
    [Test]
    public void WithLdfldaShortCircut()
    {
        var instance = assembly.GetInstance("ClassWithLdfldaShortCircut");

        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
            {
                property1EventCalled = true;
            };
        instance.Property1 = (Nullable<decimal>) 0.0;

        Assert.IsFalse(property1EventCalled);
    }
    



    [Test]
    public void WithOnceRemovedINotify()
    {
        var instance = assembly.GetInstance("ClassWithOnceRemovedINotify");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void WithBranchingReturn1()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturn1");
        EventTester.TestProperty(instance, false);
    }


    [Test]
    public void ClassWithBranchingReturnAndNoFieldTrue()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturnAndNoField");
        var property1EventCalled = false;
        var propertyChanged = ((INotifyPropertyChanged)instance);
        propertyChanged.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };

        instance.HasValue = true;
        
        instance.Property1 = "a";
        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public void ClassWithBranchingReturnAndNoFieldFalse()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturnAndNoField");
        var property1EventCalled = false;
        var propertyChanged = ((INotifyPropertyChanged)instance);
        propertyChanged.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        
        instance.HasValue = false;
        property1EventCalled = false;

        instance.Property1 = "a";
        Assert.IsTrue(property1EventCalled);
    }
    [Test]
    public void WithBranchingReturn2True()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturn2");
        var property1EventCalled = false;
        var propertyChanged = ((INotifyPropertyChanged)instance);
        propertyChanged.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };

        instance.HasValue = true;
        
        instance.Property1 = "a";
        Assert.IsTrue(property1EventCalled);
    }
    [Test]
    public void WithBranchingReturn2False()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturn2");
        var property1EventCalled = false;
        var propertyChanged = ((INotifyPropertyChanged)instance);
        propertyChanged.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        
        instance.HasValue = false;
        property1EventCalled = false;

        instance.Property1 = "a";
        Assert.IsTrue(property1EventCalled);
    }

    [Test]
    public void WithBranchingAndBeforeAfterReturn()
    {
        var instance = assembly.GetInstance("ClassWithBranchingReturnAndBeforeAfter");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void WithGeneric()
    {
        var instance = assembly.GetInstance("ClassWithGenericChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericChildWithProperty()
    {
        var instance = assembly.GetInstance("GenericChildWithProperty.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericBaseWithProperty()
    {
        var instance = assembly.GetInstance("GenericBaseWithProperty.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericChildWithPropertyBeforeAfter()
    {
        var instance = assembly.GetInstance("GenericChildWithPropertyBeforeAfter.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericBaseWithPropertyBeforeAfter()
    {
        var instance = assembly.GetInstance("GenericBaseWithPropertyBeforeAfter.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericChildWithPropertyOnChanged()
    {
        var instance = assembly.GetInstance("GenericChildWithPropertyOnChanged.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void GenericBaseWithPropertyOnChanged()
    {
        var instance = assembly.GetInstance("GenericBaseWithPropertyOnChanged.ClassWithGenericPropertyChild");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void Nested()
    {
        var instance1 = assembly.GetInstance("ClassWithNested+ClassNested");
        EventTester.TestProperty(instance1, false);
        var instance2 = assembly.GetInstance("ClassWithNested+ClassNested+ClassNestedNested");
        EventTester.TestProperty(instance2, false);
    }

    

    [Test]
    public void WithBeforeAfterImplementation()
    {
        var instance = assembly.GetInstance("ClassWithBeforeAfterImplementation");
        EventTester.TestProperty(instance, true);
    }
    [Test]
    public void WithBoolPropUsingStringProp()
    {
        var instance = assembly.GetInstance("ClassWithBoolPropUsingStringProp");
        var boolPropertyCalled = false;
        var stringPropertyCalled = false;
        var stringComparePropertyCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
                                                                              {
                                                                                  if (args.PropertyName == "BoolProperty")
                                                                                  {
                                                                                      boolPropertyCalled = true;
                                                                                  }
                                                                                  if (args.PropertyName == "StringProperty")
                                                                                  {
                                                                                      stringPropertyCalled = true;
                                                                                  }
                                                                                  if (args.PropertyName == "StringCompareProperty")
                                                                                  {
                                                                                      stringComparePropertyCalled = true;
                                                                                  }
                                                                              };
        instance.StringProperty = "magicString";

        Assert.IsTrue(boolPropertyCalled);
        Assert.IsTrue(stringPropertyCalled);
        Assert.IsTrue(stringComparePropertyCalled);

        boolPropertyCalled = false;
        stringPropertyCalled = false;
        stringComparePropertyCalled = false;
        instance.StringProperty = "notMagicString";

        Assert.IsFalse(boolPropertyCalled);
        Assert.IsTrue(stringPropertyCalled);
        Assert.IsTrue(stringComparePropertyCalled);
    }

    [Test]
    public void WithBeforeAfterAndSimpleImplementation()
    {
        var instance = assembly.GetInstance("ClassWithBeforeAfterAndSimpleImplementation");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public void HierachyBeforeAfterAndSimple()
    {
        var instance = assembly.GetInstance("HierachyBeforeAfterAndSimple.ClassChild");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BeforeAfterCalled);
    }

    [Test]
    public void WithCustomPropertyChanged()
    {
        var instance = assembly.GetInstance("ClassWithCustomPropertyChanged");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void WithExplicitPropertyChanged()
    {
        var instance = assembly.GetInstance("ClassWithExplicitPropertyChanged");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void DependsOn()
    {
        var instance = assembly.GetInstance("ClassDependsOn");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public void WithDependsOnAndDoNotNotify()
    {
        var instance = assembly.GetInstance("ClassWithDependsOnAndDoNotNotify");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public void WithNotifyInBase()
    {
        var instance = assembly.GetInstance("ClassWithNotifyInBase");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public void Child1()
    {
        var instance = assembly.GetInstance("ComplexHierachy.ClassChild1");
        EventTester.TestProperty(instance, false);
    }
    [Test]
    public void Child2()
    {
        var instance = assembly.GetInstance("ComplexHierachy.ClassChild2");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void Child3()
    {
        var type = assembly.GetType("ComplexHierachy.ClassChild3", true);
        dynamic instance = Activator.CreateInstance(type);
        var property1EventCalled = false;
        var property2EventCalled = false;
        instance.PropertyChanged += new PropertyChangedEventHandler((sender, args) =>
                                                                        {
                                                                            if (args.PropertyName == "Property1")
                                                                            {
                                                                                property1EventCalled = true;
                                                                            }
                                                                            if (args.PropertyName == "Property2")
                                                                            {
                                                                                property2EventCalled = true;
                                                                            }
                                                                        });
        instance.Property1 = "a";
        instance.Property2 = "a";

        Assert.IsTrue(property1EventCalled);
        Assert.IsTrue(property2EventCalled);
    }

    [Test]
    public void WithLogicInSet()
    {
        var instance = assembly.GetInstance("ClassWithLogicInSet");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void WithOwnImplementation()
    {
        var instance = assembly.GetInstance("ClassWithOwnImplementation");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public void WithIsChanged()
    {
        var instance = assembly.GetInstance("ClassWithIsChanged");
        Assert.IsFalse(instance.IsChanged);
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.IsChanged);
    }


    [Test]
    public void WithDoNotSetChanged()
    {
        var instance = assembly.GetInstance("ClassWithDoNotSetChanged");
        Assert.IsFalse(instance.IsChanged);
        EventTester.TestProperty(instance, false);
        Assert.IsFalse(instance.IsChanged);
    }


    [Test]
    public void WithOnChanged()
    {
        var instance = assembly.GetInstance("ClassWithOnChanged");
        Assert.IsFalse(instance.OnProperty1ChangedCalled);
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.OnProperty1ChangedCalled);
    }

    [Test]
    public virtual void WithGenericAmdLambda()
    {
        var instance = assembly.GetInstance("ClassWithGenericAmdLambdaImp");
        EventTester.TestProperty(instance, false);
    }


    [Test]
    public void WithOnChangedBerforeAfter()
    {
        var instance = assembly.GetInstance("ClassWithOnChangedBerforeAfter");
        Assert.IsFalse(instance.OnProperty1ChangedCalled);
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.OnProperty1ChangedCalled);
    }


    [Test]
    public void Caliburn()
    {
        var instance = assembly.GetInstance("ClassCaliburn");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }
    [Test]
    public void JounceBaseNotify()
    {
        var instance = assembly.GetInstance("ClassJounceBaseNotify");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }
    [Test]
    public void JounceBaseViewModel()
    {
        var instance = assembly.GetInstance("ClassJounceBaseViewModel");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public void Magellan()
    {
        var instance = assembly.GetInstance("ClassMagellan");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public void CaliburnOverriddenInvoker()
    {
        var instance = assembly.GetInstance("ClassCaliburnOverriddenInvoker");
        EventTester.TestProperty(instance, false);

        Assert.IsTrue(instance.BaseNotifyCalled);
        Assert.IsTrue(instance.OverrideCalled);
    }

    [Test]
    public void CaliburnMicro()
    {
        var instance = assembly.GetInstance("ClassCaliburnMicro");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }
    [Test]
    public void TransitiveDependencies()
    {
        var propertyNames = new List<string>();
        var instance = assembly.GetInstance("TransitiveDependencies");
        instance.PropertyChanged += new PropertyChangedEventHandler((sender, x) => propertyNames.Add(x.PropertyName));
        instance.My = "s";
        Assert.Contains("My", propertyNames);
        Assert.Contains("MyA", propertyNames);
        Assert.Contains("MyAB", propertyNames);
        Assert.Contains("MyABC", propertyNames);

    }
    [Test]
    public void CircularProperties()
    {
        var instance = assembly.GetInstance("ClassCircularProperties");
        instance.Self = "s";
        instance.PropertyA1 = "s";
        instance.PropertyA2 = "s";
        instance.PropertyB1 = "s";
        instance.PropertyB2 = "s";

    }

    [Test]
    public void Prism()
    {
        var instance = assembly.GetInstance("ClassPrism");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public void Cinch()
    {
        var instance = assembly.GetInstance("ClassCinch");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public void Catel()
    {
        var instance = assembly.GetInstance("ClassCatel");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public void MvvmLightFromObservableObject()
    {
        var instance = assembly.GetInstance("ClassMvvmLightFromObservableObject");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public void MvvmLightFromViewModel()
    {
        var instance = assembly.GetInstance("ClassMvvmLightFromViewModel");
        EventTester.TestProperty(instance, false);
        Assert.IsTrue(instance.BaseNotifyCalled);
    }

    [Test]
    public void WithPropertyImpOfAbstractProperty()
    {
        var instance = assembly.GetInstance("ClassWithPropertyImp");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void EqualityWithDouble()
    {
        var instance = assembly.GetInstance("ClassEqualityWithDouble");
        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = 2d;

        Assert.IsTrue(property1EventCalled);
        property1EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = 2d;
        Assert.IsFalse(property1EventCalled);
    }
    [Test]
    public void EqualityWithStruct()
    {
        var instance = assembly.GetInstance("ClassEqualityWithStruct");
        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        var property1 = assembly.GetInstance("ClassEqualityWithStruct+SimpleStruct");
        instance.Property1 = property1;
        Assert.IsTrue(property1EventCalled);
    }
    [Test]
    public void EqualityWithStructOverload()
    {
        var instance = assembly.GetInstance("ClassEqualityWithStructOverload");
        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        var property1 = assembly.GetInstance("ClassEqualityWithStructOverload+SimpleStruct");
        property1.X = 5;
        instance.Property1 = property1;

        Assert.IsTrue(property1EventCalled);
        property1EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = property1;
        Assert.IsFalse(property1EventCalled);
    }

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(weaverHelper.BeforeAssemblyPath, weaverHelper.AfterAssemblyPath);
    }

}