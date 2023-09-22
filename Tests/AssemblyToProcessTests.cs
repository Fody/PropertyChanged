using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ComplexHierarchy;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xunit;
using Xunit.Abstractions;

public class AssemblyToProcessTests(ITestOutputHelper outputHelper)
{
    static TestResult testResult;

    static AssemblyToProcessTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun(
            "AssemblyToProcess.dll",
            ignoreCodes: new[] {"0x80131869"}
#if NETCOREAPP2_0
            , runPeVerify:false
#endif
        );
    }

    [Theory]
    [InlineData("ClassWithInlineInitializedAutoProperties",
        "Test", "Test2", false, new string[0])]
    [InlineData("ClassWithExplicitInitializedAutoProperties",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    [InlineData("ClassWithExplicitInitializedAutoPropertiesDerivedWeakDesign",
        "test", "test2", true, new[] { "IsChanged", "Property1", "Property2", "Property1", "Property2", "Property3" })]
    [InlineData("ClassWithExplicitInitializedAutoPropertiesDerivedProperDesign",
        "test", "test2", true, new[] { "IsChanged", "Property1", "Property2", "Property3" })]
    [InlineData("ClassWithAutoPropertiesInitializedInSeparateMethod",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    [InlineData("ClassWithExplicitInitializedBackingFieldProperties",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    public void TypesWithInitializedPropertiesTest(string className, string property1Value, string property2Value, bool isChangedStateAfterConstructor, string[] propertyChangedCallsInConstructor)
    {
        var instance = testResult.GetInstance(className);

        var eventCount = 0;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            eventCount++;
        };

        Assert.Equal(property1Value, instance.Property1);
        Assert.Equal(property2Value, instance.Property2);

        var actualPropertyChangedCalls = (IList<string>)instance.PropertyChangedCalls;
        Debug.WriteLine("PropertyChanged calls: " + string.Join(", ", actualPropertyChangedCalls));

        Assert.True(propertyChangedCallsInConstructor.SequenceEqual(actualPropertyChangedCalls));
        Assert.Equal(isChangedStateAfterConstructor, instance.IsChanged);

        var initial = isChangedStateAfterConstructor ? 1 : 2;

        instance.Property1 = "a";
        Assert.Equal(initial, eventCount);
        Assert.True(instance.IsChanged);

        instance.IsChanged = false;
        Assert.Equal(initial + 1, eventCount);

        instance.Property2 = "b";
        Assert.Equal(initial + 3, eventCount);
        Assert.True(instance.IsChanged);
    }

    [Fact]
    public void ClassWithIndirectImplementation()
    {
        var instance = testResult.GetInstance("ClassWithIndirectImplementation");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void ClassWithTaskReturningPropertyChangedNotifier()
    {
        var instance = testResult.GetInstance("ClassWithTaskReturningPropertyChangedNotifier");
        EventTester.TestProperty(instance, false, true);
    }

    [Fact]
    public void ClassWithInferredShouldAlsoNotifyFor()
    {
        var instance = testResult.GetInstance("ClassWithInferredShouldAlsoNotifyFor");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public void ClassWithAlsoNotifyFor()
    {
        var instance = testResult.GetInstance("ClassWithAlsoNotifyFor");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public void ClassWithDependsOn()
    {
        var instance = testResult.GetInstance("ClassWithDependsOn");
        EventTester.TestProperty(instance, true);
    }

    [Fact]
    public void ClassWithDependsOnAndPropertyChanged()
    {
        var instance = testResult.GetInstance("ClassWithDependsOnAndPropertyChanged");
        EventTester.TestProperty(instance, true);
        Assert.Equal(1, instance.Property2ChangedCalled);
    }

    [Fact]
    public void ClassWithIndexerReferencingPropertyAndBeforeAfter()
    {
        var instance = testResult.GetInstance(nameof(ClassWithIndexerReferencingPropertyAndBeforeAfter));
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void ClassWithIndexerDependsOnAndBeforeAfter()
    {
        var instance = testResult.GetInstance(nameof(ClassWithIndexerDependsOnAndBeforeAfter));
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void ClassWithDoNotNotifyField()
    {
        var instance = testResult.GetInstance(nameof(ClassWithDoNotNotifyField));
        EventTester.TestPropertyNotCalled(instance);
    }

    [Fact]
    public void UseSingleEventInstance()
    {
        var instance = testResult.GetInstance("ClassWithNotifyPropertyChangedAttribute");

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property1 = "b";

        Assert.Equal(2, argsList.Count);
        Assert.Same(argsList[0], argsList[1]);
    }

    [Fact]
    public void SupportedLibrariesClassReactiveUI()
    {
        var instance = testResult.GetInstance("ClassReactiveUI2");

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property2 = "b";

        Assert.Equal(2, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property2", argsList[1].PropertyName);
    }

    [Fact]
    public void ClassWithOpenGenerics()
    {
        var instance = testResult.GetGenericInstance("ClassWithOpenGenerics`1", typeof(int));

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        var value1 = new KeyValuePair<string, int>("a", 1);

        instance.Property1 = value1;

        Assert.Single(argsList);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal(value1, instance.Property1);
        Assert.Equal(new KeyValuePair<string, int>("a", 1), instance.Property1);

        instance.Property1 = new KeyValuePair<string, int>("a", 1);

        Assert.Single(argsList);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal(value1, instance.Property1);
        Assert.Equal(new KeyValuePair<string, int>("a", 1), instance.Property1);

        instance.Property1 = new KeyValuePair<string, int>("a", 2);

        Assert.Equal(2, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property1", argsList[1].PropertyName);
        Assert.NotEqual(value1, instance.Property1);
        Assert.NotEqual(new KeyValuePair<string, int>("a", 1), instance.Property1);

        var value2 = new Tuple<string, int>("b", 2);

        instance.Property2 = value2;

        Assert.Equal(3, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property1", argsList[1].PropertyName);
        Assert.Equal("Property2", argsList[2].PropertyName);
        Assert.Equal(value2, instance.Property2);
        Assert.Equal(new Tuple<string, int>("b", 2), instance.Property2);

        instance.Property2 = new Tuple<string, int>("b", 2);

        Assert.Equal(3, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property1", argsList[1].PropertyName);
        Assert.Equal("Property2", argsList[2].PropertyName);
        Assert.Equal(value2, instance.Property2);
        Assert.Equal(new Tuple<string, int>("b", 2), instance.Property2);

        instance.Property2 = new Tuple<string, int>("b", 1);

        Assert.Equal(4, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property1", argsList[1].PropertyName);
        Assert.Equal("Property2", argsList[2].PropertyName);
        Assert.Equal("Property2", argsList[3].PropertyName);
        Assert.NotEqual(value2, instance.Property2);
        Assert.NotEqual(new Tuple<string, int>("b", 2), instance.Property2);
    }

    [Fact]
    public void InvalidOnPropertyNameChangedMethodSignatureEmitsWarning()
    {
        const string className = nameof(ClassWithInvalidOnChanged);

        Assert.Contains(testResult.Warnings, w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.PropertyWithInvalidOnChangedMethod)));
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.PropertyWithInvalidOnChangedMethodSuppressed)));
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.PropertyWithValidOnChangedMethod)));
    }

    [Fact]
    public void ClassWithWarnings()
    {
        var instance = testResult.GetInstance("ClassWithWarnings");
        instance.Property1 = "foo";

        var warnings = testResult.Warnings
            .Where(w => w.Text.ContainsWholeWord("ClassWithWarnings"))
            .Select(w => w.Text.Replace(" You can suppress this warning with [SuppressPropertyChangedWarnings].", ""))
            .ToArray();

        outputHelper.WriteLine(string.Join(Environment.NewLine, warnings.Select(w => $"\"{w}\"")));

        Assert.Equal(warnings, new[]
        {
            "Type ClassWithWarnings contains a method OnProperty1Changed which will not be called as Property1 is attributed with [DoNotNotify].",
            "Type ClassWithWarnings contains a method OnProperty2Changed which will not be called as Property2 is attributed with an alternative [OnChangedMethod].",
            "Type ClassWithWarnings contains a method OnPropertyXChanged which will not be called as PropertyX is not found.",
            "Type ClassWithWarnings contains a method OnBaseClassPropertyChanged which will not be called as BaseClassProperty is declared on base class ClassWithWarningsBase."
        });
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithoutMatchingPropertyEmitsWarning()
    {
        const string className = nameof(ClassWithInvalidOnChanged);

        DumpWarnings(nameof(ClassWithInvalidOnChanged));

        Assert.Contains(testResult.Warnings, w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.OnNonExistingPropertyChanged)));
        Assert.Contains(testResult.Warnings, w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.OnIgnoredPropertyChanged)));
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.OnNonExistingPropertySuppressedChanged)));

        DumpWarnings(nameof(ClassWithOnChangedConcrete));
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.Contains(nameof(ClassWithOnChangedConcrete)) && w.Text.Contains(nameof(ClassWithOnChangedConcrete.OnProperty1Changed)));
    }

    [Fact]
    public void ClassWithIgnoredPropertyOnChanged()
    {
        var instance = testResult.GetInstance(nameof(ClassWithInvalidOnChanged));
        instance.IgnoredProperty = "ignore me";

        Assert.False(instance.OnIgnorePropertyChangedCalled);
    }

    [Fact]
    public void IgnoreSuppressedProperties()
    {
        const string className = nameof(ClassWithInvalidOnChanged);
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.Contains(className) && w.Text.Contains(ClassWithInvalidOnChanged.IndexerName));
    }

    [Fact]
    public void IgnoreSuppressedClasses()
    {
        const string className = nameof(ClassWithSuppressedInvalidOnChanged);
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithSuppressedInvalidOnChanged.OnNonExistingPropertyChanged)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChanged));
        instance.Property1 = "foo";

        Assert.True(instance.OnProperty1ChangedCalled);
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChanged)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodIsCalledForCalculatedProperty()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedCalculatedProperty));
        instance.Property1 = "foo";

        Assert.True(instance.OnProperty2ChangedCalled);
        Assert.True(instance.OnProperty3ChangedCalled);
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedCalculatedProperty)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfter));
        instance.Property2 = "foo";

        Assert.True(instance.OnProperty2ChangedCalled);
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfter)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterTypedIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTyped));
        instance.Property1 = "foo";
        instance.Property2 = 1;

        Assert.Equal("-foo", instance.OnProperty1ChangedCalled);
        Assert.Equal("0-1", instance.OnProperty2ChangedCalled);

        instance.Property1 = "bar";
        instance.Property2 = 2;

        Assert.Equal("foo-bar", instance.OnProperty1ChangedCalled);
        Assert.Equal("1-2", instance.OnProperty2ChangedCalled);

        Assert.DoesNotContain(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTyped)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterTypedWithNullableValueTypeIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedWithNullableValueType));
        instance.Property1 = 1;

        Assert.Equal("-1", instance.OnProperty1ChangedCalled);

        instance.Property1 = 2;

        Assert.Equal("1-2", instance.OnProperty1ChangedCalled);

        Assert.DoesNotContain(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTypedWithNullableValueType)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterTypedWithWithGenericObjectIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedWithGenericObject));
        instance.Property1 = new List<int> { 1, 2 };

        Assert.Equal("-1,2", instance.OnProperty1ChangedCalled);

        instance.Property1 = new List<int> { 3, 4 };

        Assert.Equal("1,2-3,4", instance.OnProperty1ChangedCalled);

        Assert.DoesNotContain(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTypedWithGenericObject)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterTypedWithInvalidSignatureDefaultIsNotCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedInvalidSignatureDefault));
        instance.Property1 = "foo";

        Assert.Null(instance.OnProperty1ChangedCalled);

        instance.Property1 = "bar";

        Assert.Null(instance.OnProperty1ChangedCalled);

        Assert.Contains(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTypedInvalidSignatureDefault)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterTypedGenericIntegerIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedGenericInteger));
        instance.Property1 = 1;

        Assert.Equal("0-1", instance.OnProperty1ChangedCalled);

        instance.Property1 = 2;

        Assert.Equal("1-2", instance.OnProperty1ChangedCalled);
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterTypedGenericStringIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedGenericString));
        instance.Property1 = "foo";

        Assert.Equal("-foo", instance.OnProperty1ChangedCalled);

        instance.Property1 = "bar";

        Assert.Equal("foo-bar", instance.OnProperty1ChangedCalled);
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterTypedWithInvalidSignatureExplicitIsNotCalledAndAWarningIsGenerated()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedInvalidSignatureExplicit));
        instance.Property1 = "foo";

        Assert.Null(instance.OnProperty1ChangedCalled);

        instance.Property1 = "bar";

        Assert.Null(instance.OnProperty1ChangedCalled);

        Assert.Contains(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTypedInvalidSignatureExplicit)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodWithBeforeAfterCalculatedPropertyIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterCalculatedProperty));
        instance.Property1 = "foo";

        DumpWarnings(nameof(ClassWithOnChangedBeforeAfterCalculatedProperty));

        Assert.Equal("From 0 to 3", instance.Property2ChangeValue);
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterCalculatedProperty)));
    }

    [Fact]
    public void OnPropertyNameChangedMethodCallInOriginalCodePreventsInsertingAdditionalCall()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedAndNoPropertyChanged));
        instance.Property1 = "foo";

        Assert.Equal(1, instance.OnProperty1ChangedCalled);
        Assert.DoesNotContain(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedAndNoPropertyChanged)));
    }

    [Fact]
    public void OnChangedMethodAttributeCustomizesCalledMethods()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedCustomized));
        instance.Property1 = "foo";

        DumpWarnings(nameof(ClassWithOnChangedCustomized));

        Assert.False(instance.OnProperty1ChangedCalled);
        Assert.True(instance.FirstCustomCalled);
        Assert.True(instance.SecondCustomCalled);
        //Warnings tested in ClassWithOnChangedCustomizedWarnings
    }

    [Fact]
    public void ClassWithOnChangedCustomizedWarnings()
    {
        var warnings = testResult.Warnings
            .Where(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedCustomized)))
            .ToArray();

        Assert.Contains(warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedCustomized))
                                       && w.Text.Contains(nameof(ClassWithOnChangedCustomized.OnProperty1Changed)));

        Assert.True(warnings.Length == 1);
    }


    [Fact]
    public void OnChangedMethodAttributeAlwaysCallsMethod()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedCustomized));
        instance.Property2 = "foo";

        DumpWarnings(nameof(ClassWithOnChangedCustomized));

        Assert.False(instance.OnProperty1ChangedCalled);
        Assert.Equal(2, instance.PropertyChangedCounterValue);

        //Warnings tested in ClassWithOnChangedCustomizedWarnings
    }

    [Fact]
    public void OnChangedMethodAttributeCanCallSameMethodSeveralTimes()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedCustomized));
        instance.Property3 = "foo";

        Assert.Equal(3, instance.PropertyChangedCounterValue);
        //Warnings tested in ClassWithOnChangedCustomizedWarnings
    }

    [Fact]
    public void OnChangedMethodAttributeSuppressedDefaultMethodsWhenMethodNameIsNullOrEmpty()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedSuppressed));
        instance.Property1 = "foo";
        instance.Property2 = "bar";

        DumpWarnings(nameof(ClassWithOnChangedSuppressed));

        Assert.False(instance.OnProperty1ChangedCalled);
        Assert.False(instance.OnProperty2ChangedCalled);

        Assert.Contains(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedSuppressed)) && w.Text.Contains(nameof(ClassWithOnChangedSuppressed.Property1)));
        Assert.Contains(testResult.Warnings, w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedSuppressed)) && w.Text.Contains(nameof(ClassWithOnChangedSuppressed.Property2)));
    }

    [Fact]
    public void ClassWithIntermediateGenericBaseHandlesPropertyChanged()
    {
        var instance = testResult.GetInstance(nameof(ClassWithIntermediateGenericBase));

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property2 = "b";
        instance.Property3 = "c";

        Assert.Equal(3, argsList.Count);
        Assert.Equal("Property1", argsList[0].PropertyName);
        Assert.Equal("Property2", argsList[1].PropertyName);
        Assert.Equal("Property3", argsList[2].PropertyName);
    }

    [Fact]
    public void EventInvokersUseCorrectMethodDeclaringType()
    {
        using (var module = ModuleDefinition.ReadModule(testResult.AssemblyPath))
        {
            // Non generic
            AssertInvoker(typeof(ClassChild1), nameof(ClassChild1.Property1), typeof(ClassParent).FullName);
            AssertInvoker(typeof(ClassChild3), nameof(ClassChild3.Property2), typeof(ClassParent).FullName);

            // Issue #477
            AssertInvoker(typeof(ClassWithGenericMiddleBase), nameof(ClassWithGenericMiddleBase.Property1), nameof(ClassWithGenericMiddleBase));
            AssertInvoker(typeof(ClassWithGenericMiddle<>), nameof(ClassWithGenericMiddle<int>.Property2), nameof(ClassWithGenericMiddleBase));
            AssertInvoker(typeof(ClassWithGenericMiddleChild), nameof(ClassWithGenericMiddleChild.Property3), nameof(ClassWithGenericMiddleBase));

            // Issue #516
            AssertInvoker(typeof(ClassWithGenericParent<>), nameof(ClassWithGenericParent<int>.Property1), "ClassWithGenericParent`1<T>");
            AssertInvoker(typeof(IntermediateGenericClass<>), nameof(IntermediateGenericClass<int>.Property2), "ClassWithGenericParent`1<T>");
            AssertInvoker(typeof(ClassWithIntermediateGenericBase), nameof(ClassWithIntermediateGenericBase.Property3), "IntermediateGenericClass`1<System.String>");

            void AssertInvoker(Type type, string propertyName, string invokerDeclaringType)
            {
                var typeDef = module.GetType(type.FullName);
                var setter = typeDef.Methods.Single(m => m.Name == "set_" + propertyName);
                var callInstruction = setter.Body.Instructions.Single(i => i.OpCode == OpCodes.Callvirt);
                Assert.Equal(invokerDeclaringType, ((MethodReference)callInstruction.Operand).DeclaringType.FullName);
            }
        }
    }

    [Fact]
    public void ClassWithNullableBackingField()
    {
        var instance = testResult.GetInstance("ClassWithNullableBackingField");
        var isFlagEventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "IsFlag")
            {
                isFlagEventCalled = true;
            }
        };
        instance.IsFlag = true;
        Assert.True(isFlagEventCalled);

        isFlagEventCalled = false;
        instance.IsFlag = true;
        Assert.False(isFlagEventCalled);
    }

    [Fact]
    public void ClassWithGeneratedPropertyChanged()
    {
        var instance = testResult.GetInstance("ClassWithGeneratedPropertyChanged");
        EventTester.TestProperty(instance, false);
    }

    [Fact]
    public void StructWithNotify()
    {
        var instance = testResult.GetInstance("StructWithNotify");
        EventTester.TestValueTypeProperty(instance);
    }

    [Fact]
    public void StructWithNotifyGeneric()
    {
        var instance = testResult.GetGenericInstance("StructWithNotify`1", typeof(string));
        EventTester.TestValueTypeProperty(instance);
    }

    [Fact]
    public void StructWithNotifyAttribute()
    {
        var instance = testResult.GetInstance("StructWithNotifyAttribute");
        EventTester.TestValueTypeProperty(instance);
    }

    [Fact]
    public void StructWithNotifyAttributeGeneric()
    {
        var instance = testResult.GetGenericInstance("StructWithNotifyAttribute`1", typeof(string));
        EventTester.TestValueTypeProperty(instance);
    }

    void DumpWarnings(string containingWord = null)
    {
        foreach (var warning in testResult.Warnings.Where(w => containingWord == null || w.Text.ContainsWholeWord(containingWord)))
            outputHelper.WriteLine($"WARNING: {warning.Text}");
    }
}
