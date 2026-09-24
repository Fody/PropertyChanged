using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using ComplexHierarchy;
using Mono.Cecil;
using Mono.Cecil.Cil;

using TestResult = Fody.TestResult;

// weaved assemblies are written to a shared fodytemp folder and loaded into the process
[NotInParallel]
public class AssemblyToProcessTests
{
    static TestResult testResult;

    static AssemblyToProcessTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun(
            "AssemblyToProcess.dll",
            ignoreCodes: ["0x80131869"]
#if NETCOREAPP2_0
            , runPeVerify:false
#endif
        );
    }

    [Test]
    [Arguments("ClassWithInlineInitializedAutoProperties",
        "Test", "Test2", false, new string[0])]
    [Arguments("ClassWithExplicitInitializedAutoProperties",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    [Arguments("ClassWithExplicitInitializedAutoPropertiesDerivedWeakDesign",
        "test", "test2", true, new[] { "IsChanged", "Property1", "Property2", "Property1", "Property2", "Property3" })]
    [Arguments("ClassWithExplicitInitializedAutoPropertiesDerivedProperDesign",
        "test", "test2", true, new[] { "IsChanged", "Property1", "Property2", "Property3" })]
    [Arguments("ClassWithAutoPropertiesInitializedInSeparateMethod",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    [Arguments("ClassWithExplicitInitializedBackingFieldProperties",
        "Test", "Test2", true, new[] { "IsChanged", "Property1", "Property2" })]
    public async Task TypesWithInitializedPropertiesTest(string className, string property1Value, string property2Value, bool isChangedStateAfterConstructor, string[] propertyChangedCallsInConstructor)
    {
        var instance = testResult.GetInstance(className);

        var eventCount = 0;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
        {
            eventCount++;
        };

        await Assert.That((object)instance.Property1).IsEqualTo(property1Value);
        await Assert.That((object)instance.Property2).IsEqualTo(property2Value);

        var actualPropertyChangedCalls = (IList<string>)instance.PropertyChangedCalls;
        Debug.WriteLine("PropertyChanged calls: " + string.Join(", ", actualPropertyChangedCalls));

        await Assert.That(propertyChangedCallsInConstructor.SequenceEqual(actualPropertyChangedCalls)).IsTrue();
        await Assert.That((object)instance.IsChanged).IsEqualTo(isChangedStateAfterConstructor);

        var initial = isChangedStateAfterConstructor ? 1 : 2;

        instance.Property1 = "a";
        await Assert.That(eventCount).IsEqualTo(initial);
        await Assert.That((bool)instance.IsChanged).IsTrue();

        instance.IsChanged = false;
        await Assert.That(eventCount).IsEqualTo(initial + 1);

        instance.Property2 = "b";
        await Assert.That(eventCount).IsEqualTo(initial + 3);
        await Assert.That((bool)instance.IsChanged).IsTrue();
    }

    [Test]
    public void ClassWithIndirectImplementation()
    {
        var instance = testResult.GetInstance("ClassWithIndirectImplementation");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void ClassWithTaskReturningPropertyChangedNotifier()
    {
        var instance = testResult.GetInstance("ClassWithTaskReturningPropertyChangedNotifier");
        EventTester.TestProperty(instance, false, true);
    }

    [Test]
    public void ClassWithInferredShouldAlsoNotifyFor()
    {
        var instance = testResult.GetInstance("ClassWithInferredShouldAlsoNotifyFor");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public void ClassWithAlsoNotifyFor()
    {
        var instance = testResult.GetInstance("ClassWithAlsoNotifyFor");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public void ClassWithDependsOn()
    {
        var instance = testResult.GetInstance("ClassWithDependsOn");
        EventTester.TestProperty(instance, true);
    }

    [Test]
    public async Task ClassWithDependsOnAndPropertyChanged()
    {
        var instance = testResult.GetInstance("ClassWithDependsOnAndPropertyChanged");
        EventTester.TestProperty(instance, true);
        await Assert.That((int)instance.Property2ChangedCalled).IsEqualTo(1);
    }

    [Test]
    public void ClassWithIndexerReferencingPropertyAndBeforeAfter()
    {
        var instance = testResult.GetInstance(nameof(ClassWithIndexerReferencingPropertyAndBeforeAfter));
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void ClassWithIndexerDependsOnAndBeforeAfter()
    {
        var instance = testResult.GetInstance(nameof(ClassWithIndexerDependsOnAndBeforeAfter));
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void ClassWithDoNotNotifyField()
    {
        var instance = testResult.GetInstance(nameof(ClassWithDoNotNotifyField));
        EventTester.TestPropertyNotCalled(instance);
    }

    [Test]
    public async Task UseSingleEventInstance()
    {
        var instance = testResult.GetInstance("ClassWithNotifyPropertyChangedAttribute");

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property1 = "b";

        await Assert.That(argsList.Count).IsEqualTo(2);
        await Assert.That(argsList[1]).IsSameReferenceAs(argsList[0]);
    }

    [Test]
    public async Task SupportedLibrariesClassReactiveUI()
    {
        var instance = testResult.GetInstance("ClassReactiveUI2");

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property2 = "b";

        await Assert.That(argsList.Count).IsEqualTo(2);
        await Assert.That(argsList[0].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[1].PropertyName).IsEqualTo("Property2");
    }

    [Test]
    public async Task ClassWithOpenGenerics()
    {
        var instance = testResult.GetGenericInstance("ClassWithOpenGenerics`1", typeof(int));

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        var value1 = new KeyValuePair<string, int>("a", 1);

        instance.Property1 = value1;

        await Assert.That(argsList).HasSingleItem();
        await Assert.That(argsList[0].PropertyName).IsEqualTo("Property1");
        await Assert.That((KeyValuePair<string, int>)instance.Property1).IsEqualTo(value1);
        await Assert.That((KeyValuePair<string, int>)instance.Property1).IsEqualTo(new KeyValuePair<string, int>("a", 1));

        instance.Property1 = new KeyValuePair<string, int>("a", 1);

        await Assert.That(argsList).HasSingleItem();
        await Assert.That(argsList[0].PropertyName).IsEqualTo("Property1");
        await Assert.That((KeyValuePair<string, int>)instance.Property1).IsEqualTo(value1);
        await Assert.That((KeyValuePair<string, int>)instance.Property1).IsEqualTo(new KeyValuePair<string, int>("a", 1));

        instance.Property1 = new KeyValuePair<string, int>("a", 2);

        await Assert.That(argsList.Count).IsEqualTo(2);
        await Assert.That(argsList[0].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[1].PropertyName).IsEqualTo("Property1");
        await Assert.That((KeyValuePair<string, int>)instance.Property1).IsNotEqualTo(value1);
        await Assert.That((KeyValuePair<string, int>)instance.Property1).IsNotEqualTo(new KeyValuePair<string, int>("a", 1));

        var value2 = new Tuple<string, int>("b", 2);

        instance.Property2 = value2;

        await Assert.That(argsList.Count).IsEqualTo(3);
        await Assert.That(argsList[0].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[1].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[2].PropertyName).IsEqualTo("Property2");
        await Assert.That((Tuple<string, int>)instance.Property2).IsEqualTo(value2);
        await Assert.That((Tuple<string, int>)instance.Property2).IsEqualTo(new Tuple<string, int>("b", 2));

        instance.Property2 = new Tuple<string, int>("b", 2);

        await Assert.That(argsList.Count).IsEqualTo(3);
        await Assert.That(argsList[0].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[1].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[2].PropertyName).IsEqualTo("Property2");
        await Assert.That((Tuple<string, int>)instance.Property2).IsEqualTo(value2);
        await Assert.That((Tuple<string, int>)instance.Property2).IsEqualTo(new Tuple<string, int>("b", 2));

        instance.Property2 = new Tuple<string, int>("b", 1);

        await Assert.That(argsList.Count).IsEqualTo(4);
        await Assert.That(argsList[0].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[1].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[2].PropertyName).IsEqualTo("Property2");
        await Assert.That(argsList[3].PropertyName).IsEqualTo("Property2");
        await Assert.That((Tuple<string, int>)instance.Property2).IsNotEqualTo(value2);
        await Assert.That((Tuple<string, int>)instance.Property2).IsNotEqualTo(new Tuple<string, int>("b", 2));
    }

    [Test]
    public async Task InvalidOnPropertyNameChangedMethodSignatureEmitsWarning()
    {
        const string className = nameof(ClassWithInvalidOnChanged);

        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.PropertyWithInvalidOnChangedMethod)))).IsTrue();
        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.PropertyWithInvalidOnChangedMethodSuppressed)))).IsFalse();
        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.PropertyWithValidOnChangedMethod)))).IsFalse();
    }

    [Test]
    public async Task ClassWithWarnings()
    {
        var instance = testResult.GetInstance("ClassWithWarnings");
        instance.Property1 = "foo";

        var warnings = testResult.Warnings
            .Where(w => w.Text.ContainsWholeWord("ClassWithWarnings"))
            .Select(w => w.Text.Replace(" You can suppress this warning with [SuppressPropertyChangedWarnings].", ""))
            .ToArray();

        Console.WriteLine(string.Join(Environment.NewLine, warnings.Select(w => $"\"{w}\"")));

        await Assert.That(warnings).IsEquivalentTo(new[]
        {
            "Type ClassWithWarnings contains a method OnProperty1Changed which will not be called as Property1 is attributed with [DoNotNotify].",
            "Type ClassWithWarnings contains a method OnProperty2Changed which will not be called as Property2 is attributed with an alternative [OnChangedMethod].",
            "Type ClassWithWarnings contains a method OnPropertyXChanged which will not be called as PropertyX is not found.",
            "Type ClassWithWarnings contains a method OnBaseClassPropertyChanged which will not be called as BaseClassProperty is declared on base class ClassWithWarningsBase."
        });
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithoutMatchingPropertyEmitsWarning()
    {
        const string className = nameof(ClassWithInvalidOnChanged);

        DumpWarnings(nameof(ClassWithInvalidOnChanged));

        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.OnNonExistingPropertyChanged)))).IsTrue();
        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.OnIgnoredPropertyChanged)))).IsTrue();
        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithInvalidOnChanged.OnNonExistingPropertySuppressedChanged)))).IsFalse();

        DumpWarnings(nameof(ClassWithOnChangedConcrete));
        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(nameof(ClassWithOnChangedConcrete)) && w.Text.Contains(nameof(ClassWithOnChangedConcrete.OnProperty1Changed)))).IsFalse();
    }

    [Test]
    public async Task ClassWithIgnoredPropertyOnChanged()
    {
        var instance = testResult.GetInstance(nameof(ClassWithInvalidOnChanged));
        instance.IgnoredProperty = "ignore me";

        await Assert.That((bool)instance.OnIgnorePropertyChangedCalled).IsFalse();
    }

    [Test]
    public async Task IgnoreSuppressedProperties()
    {
        const string className = nameof(ClassWithInvalidOnChanged);
        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(className) && w.Text.Contains(ClassWithInvalidOnChanged.IndexerName))).IsFalse();
    }

    [Test]
    public async Task IgnoreSuppressedClasses()
    {
        const string className = nameof(ClassWithSuppressedInvalidOnChanged);
        await Assert.That(testResult.Warnings.Any(w => w.Text.Contains(className) && w.Text.Contains(nameof(ClassWithSuppressedInvalidOnChanged.OnNonExistingPropertyChanged)))).IsFalse();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChanged));
        instance.Property1 = "foo";

        await Assert.That((bool)instance.OnProperty1ChangedCalled).IsTrue();
        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChanged)))).IsFalse();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodIsCalledForCalculatedProperty()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedCalculatedProperty));
        instance.Property1 = "foo";

        await Assert.That((bool)instance.OnProperty2ChangedCalled).IsTrue();
        await Assert.That((bool)instance.OnProperty3ChangedCalled).IsTrue();
        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedCalculatedProperty)))).IsFalse();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfter));
        instance.Property2 = "foo";

        await Assert.That((bool)instance.OnProperty2ChangedCalled).IsTrue();
        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfter)))).IsFalse();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterTypedIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTyped));
        instance.Property1 = "foo";
        instance.Property2 = 1;

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("-foo");
        await Assert.That((string)instance.OnProperty2ChangedCalled).IsEqualTo("0-1");

        instance.Property1 = "bar";
        instance.Property2 = 2;

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("foo-bar");
        await Assert.That((string)instance.OnProperty2ChangedCalled).IsEqualTo("1-2");

        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTyped)))).IsFalse();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterTypedWithNullableValueTypeIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedWithNullableValueType));
        instance.Property1 = 1;

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("-1");

        instance.Property1 = 2;

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("1-2");

        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTypedWithNullableValueType)))).IsFalse();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterTypedWithWithGenericObjectIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedWithGenericObject));
        instance.Property1 = new List<int> { 1, 2 };

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("-1,2");

        instance.Property1 = new List<int> { 3, 4 };

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("1,2-3,4");

        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTypedWithGenericObject)))).IsFalse();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterTypedWithInvalidSignatureDefaultIsNotCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedInvalidSignatureDefault));
        instance.Property1 = "foo";

        await Assert.That((object)instance.OnProperty1ChangedCalled).IsNull();

        instance.Property1 = "bar";

        await Assert.That((object)instance.OnProperty1ChangedCalled).IsNull();

        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTypedInvalidSignatureDefault)))).IsTrue();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterTypedGenericIntegerIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedGenericInteger));
        instance.Property1 = 1;

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("0-1");

        instance.Property1 = 2;

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("1-2");
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterTypedGenericStringIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedGenericString));
        instance.Property1 = "foo";

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("-foo");

        instance.Property1 = "bar";

        await Assert.That((string)instance.OnProperty1ChangedCalled).IsEqualTo("foo-bar");
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterTypedWithInvalidSignatureExplicitIsNotCalledAndAWarningIsGenerated()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterTypedInvalidSignatureExplicit));
        instance.Property1 = "foo";

        await Assert.That((object)instance.OnProperty1ChangedCalled).IsNull();

        instance.Property1 = "bar";

        await Assert.That((object)instance.OnProperty1ChangedCalled).IsNull();

        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterTypedInvalidSignatureExplicit)))).IsTrue();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodWithBeforeAfterCalculatedPropertyIsCalled()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedBeforeAfterCalculatedProperty));
        instance.Property1 = "foo";

        DumpWarnings(nameof(ClassWithOnChangedBeforeAfterCalculatedProperty));

        await Assert.That((string)instance.Property2ChangeValue).IsEqualTo("From 0 to 3");
        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedBeforeAfterCalculatedProperty)))).IsFalse();
    }

    [Test]
    public async Task OnPropertyNameChangedMethodCallInOriginalCodePreventsInsertingAdditionalCall()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedAndNoPropertyChanged));
        instance.Property1 = "foo";

        await Assert.That((int)instance.OnProperty1ChangedCalled).IsEqualTo(1);
        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedAndNoPropertyChanged)))).IsFalse();
    }

    [Test]
    public async Task OnChangedMethodAttributeCustomizesCalledMethods()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedCustomized));
        instance.Property1 = "foo";

        DumpWarnings(nameof(ClassWithOnChangedCustomized));

        await Assert.That((bool)instance.OnProperty1ChangedCalled).IsFalse();
        await Assert.That((bool)instance.FirstCustomCalled).IsTrue();
        await Assert.That((bool)instance.SecondCustomCalled).IsTrue();
        //Warnings tested in ClassWithOnChangedCustomizedWarnings
    }

    [Test]
    public async Task ClassWithOnChangedCustomizedWarnings()
    {
        var warnings = testResult.Warnings
            .Where(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedCustomized)))
            .ToArray();

        await Assert.That(warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedCustomized))
                                       && w.Text.Contains(nameof(ClassWithOnChangedCustomized.OnProperty1Changed)))).IsTrue();

        await Assert.That(warnings.Length == 1).IsTrue();
    }


    [Test]
    public async Task OnChangedMethodAttributeAlwaysCallsMethod()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedCustomized));
        instance.Property2 = "foo";

        DumpWarnings(nameof(ClassWithOnChangedCustomized));

        await Assert.That((bool)instance.OnProperty1ChangedCalled).IsFalse();
        await Assert.That((int)instance.PropertyChangedCounterValue).IsEqualTo(2);

        //Warnings tested in ClassWithOnChangedCustomizedWarnings
    }

    [Test]
    public async Task OnChangedMethodAttributeCanCallSameMethodSeveralTimes()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedCustomized));
        instance.Property3 = "foo";

        await Assert.That((int)instance.PropertyChangedCounterValue).IsEqualTo(3);
        //Warnings tested in ClassWithOnChangedCustomizedWarnings
    }

    [Test]
    public async Task OnChangedMethodAttributeSuppressedDefaultMethodsWhenMethodNameIsNullOrEmpty()
    {
        var instance = testResult.GetInstance(nameof(ClassWithOnChangedSuppressed));
        instance.Property1 = "foo";
        instance.Property2 = "bar";

        DumpWarnings(nameof(ClassWithOnChangedSuppressed));

        await Assert.That((bool)instance.OnProperty1ChangedCalled).IsFalse();
        await Assert.That((bool)instance.OnProperty2ChangedCalled).IsFalse();

        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedSuppressed)) && w.Text.Contains(nameof(ClassWithOnChangedSuppressed.Property1)))).IsTrue();
        await Assert.That(testResult.Warnings.Any(w => w.Text.ContainsWholeWord(nameof(ClassWithOnChangedSuppressed)) && w.Text.Contains(nameof(ClassWithOnChangedSuppressed.Property2)))).IsTrue();
    }

    [Test]
    public async Task ClassWithIntermediateGenericBaseHandlesPropertyChanged()
    {
        var instance = testResult.GetInstance(nameof(ClassWithIntermediateGenericBase));

        var argsList = new List<PropertyChangedEventArgs>();
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) => argsList.Add(args);

        instance.Property1 = "a";
        instance.Property2 = "b";
        instance.Property3 = "c";

        await Assert.That(argsList.Count).IsEqualTo(3);
        await Assert.That(argsList[0].PropertyName).IsEqualTo("Property1");
        await Assert.That(argsList[1].PropertyName).IsEqualTo("Property2");
        await Assert.That(argsList[2].PropertyName).IsEqualTo("Property3");
    }

    [Test]
    public async Task EventInvokersUseCorrectMethodDeclaringType()
    {
        using (var module = ModuleDefinition.ReadModule(testResult.AssemblyPath))
        {
            // Non generic
            await AssertInvoker(typeof(ClassChild1), nameof(ClassChild1.Property1), typeof(ClassParent).FullName);
            await AssertInvoker(typeof(ClassChild3), nameof(ClassChild3.Property2), typeof(ClassParent).FullName);

            // Issue #477
            await AssertInvoker(typeof(ClassWithGenericMiddleBase), nameof(ClassWithGenericMiddleBase.Property1), nameof(ClassWithGenericMiddleBase));
            await AssertInvoker(typeof(ClassWithGenericMiddle<>), nameof(ClassWithGenericMiddle<int>.Property2), nameof(ClassWithGenericMiddleBase));
            await AssertInvoker(typeof(ClassWithGenericMiddleChild), nameof(ClassWithGenericMiddleChild.Property3), nameof(ClassWithGenericMiddleBase));

            // Issue #516
            await AssertInvoker(typeof(ClassWithGenericParent<>), nameof(ClassWithGenericParent<int>.Property1), "ClassWithGenericParent`1<T>");
            await AssertInvoker(typeof(IntermediateGenericClass<>), nameof(IntermediateGenericClass<int>.Property2), "ClassWithGenericParent`1<T>");
            await AssertInvoker(typeof(ClassWithIntermediateGenericBase), nameof(ClassWithIntermediateGenericBase.Property3), "IntermediateGenericClass`1<System.String>");

            async Task AssertInvoker(Type type, string propertyName, string invokerDeclaringType)
            {
                var typeDef = module.GetType(type.FullName);
                var setter = typeDef.Methods.Single(m => m.Name == "set_" + propertyName);
                var callInstruction = setter.Body.Instructions.Single(i => i.OpCode == OpCodes.Callvirt);
                await Assert.That(((MethodReference)callInstruction.Operand).DeclaringType.FullName).IsEqualTo(invokerDeclaringType);
            }
        }
    }

    [Test]
    public async Task ClassWithNullableBackingField()
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
        await Assert.That(isFlagEventCalled).IsTrue();

        isFlagEventCalled = false;
        instance.IsFlag = true;
        await Assert.That(isFlagEventCalled).IsFalse();
    }

    [Test]
    public void ClassWithGeneratedPropertyChanged()
    {
        var instance = testResult.GetInstance("ClassWithGeneratedPropertyChanged");
        EventTester.TestProperty(instance, false);
    }

    [Test]
    public void StructWithNotify()
    {
        var instance = testResult.GetInstance("StructWithNotify");
        EventTester.TestValueTypeProperty(instance);
    }

    [Test]
    public void StructWithNotifyGeneric()
    {
        var instance = testResult.GetGenericInstance("StructWithNotify`1", typeof(string));
        EventTester.TestValueTypeProperty(instance);
    }

    [Test]
    public void StructWithNotifyAttribute()
    {
        var instance = testResult.GetInstance("StructWithNotifyAttribute");
        EventTester.TestValueTypeProperty(instance);
    }

    [Test]
    public void StructWithNotifyAttributeGeneric()
    {
        var instance = testResult.GetGenericInstance("StructWithNotifyAttribute`1", typeof(string));
        EventTester.TestValueTypeProperty(instance);
    }

    void DumpWarnings(string containingWord = null)
    {
        foreach (var warning in testResult.Warnings.Where(w => containingWord == null || w.Text.ContainsWholeWord(containingWord)))
            Console.WriteLine($"WARNING: {warning.Text}");
    }
}
