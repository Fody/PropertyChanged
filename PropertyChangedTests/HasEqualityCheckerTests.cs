using System;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedVariable


[TestFixture]
public class HasEqualityCheckerTests
{
    Mono.Collections.Generic.Collection<PropertyDefinition> properties;
    Mono.Collections.Generic.Collection<FieldDefinition> fields;
    public HasEqualityCheckerTests()
    {
        var moduleDefinition = ModuleDefinition.ReadModule(GetType().Assembly.Location);
        var typeDefinition = moduleDefinition.Types.First(definition => definition.Name == "HasEqualityCheckerTests");
        properties = typeDefinition.Properties;
        fields = typeDefinition.Fields;
    }

    [Test]
    public void EqualityShortCutTest()
    {
        var instructions = GetInstructions("EqualityShortCut");
        var field = GetField("intField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Test]
    public void EqualsNoFieldTest()
    {
        var instructions = GetInstructions("EqualsNoField");
        var field = GetField("intField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, null));
    }
    [Test]
    public void NoEqualsNoFieldTest()
    {
        var instructions = GetInstructions("NoEqualsNoField");
        var field = GetField("intField");
        Assert.IsFalse(HasEqualityChecker.AlreadyHasEquality(instructions, null));
    }
    [Test]
    public void EqualityShortCutInverseTest()
    {
        var instructions = GetInstructions("EqualityShortCutInverse");
        var field = GetField("intField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Test]
    public void EqualityNestedTest()
    {
        var instructions = GetInstructions("EqualityNested");
        var field = GetField("intField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Test]
    public void EqualityNestedInverseTest()
    {
        var instructions = GetInstructions("EqualityNestedInverse");
        var field = GetField("intField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Test]
    public void EqualsShortCutTest()
    {
        var instructions = GetInstructions("EqualsShortCut");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Test]
    public void EqualsShortCutInverseTest()
    {
        var instructions = GetInstructions("EqualsShortCutInverse");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Test]
    public void EqualsNestedInverseTest()
    {
        var instructions = GetInstructions("EqualsNestedInverse");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Test]
    public void EqualsNestedTest()
    {
        var instructions = GetInstructions("EqualsNested");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Test]
    public void StringEqualsShortCutTest()
    {
        var instructions = GetInstructions("StringEqualsShortCut");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Test]
    public void StringEqualsShortCutInverseTest()
    {
        var instructions = GetInstructions("StringEqualsShortCutInverse");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Test]
    public void StringEqualsNestedTest()
    {
        var instructions = GetInstructions("StringEqualsNested");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Test]
    public void StringEqualsOrdinalTest()
    {
        var instructions = GetInstructions("StringEqualsOrdinal");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Test]
    public void StringEqualsNestedInverseTest()
    {
        var instructions = GetInstructions("StringEqualsNestedInverse");
        var field = GetField("stringField");
        Assert.IsTrue(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Test]
    public void NoEqualityTest()
    {
        var instructions = GetInstructions("NoEquality");
        var field = GetField("stringField");
        Assert.IsFalse(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    PropertyDefinition GetInstructions(string equalityShortcut)
    {
        return properties.First(definition => definition.Name == equalityShortcut);
    }
    FieldDefinition GetField(string equalityShortcut)
    {
        return fields.First(x => x.Name == equalityShortcut);
    }

    int intField;
    string stringField;

    public int EqualityShortCut
    {
        get { return intField; }
        set
        {
            if (value == intField)
            {
                return;
            }
            intField = value;
        }
    }

    public int EqualityShortCutInverse
    {
        get { return intField; }
        set
        {
            if (intField == value)
            {
                return;
            }
            intField = value;
        }
    }

    public int EqualityNested
    {
        get { return intField; }
        set
        {
            if (value != intField)
            {
                intField = value;
            }
        }
    }
    public int EqualityNestedInverse
    {
        get { return intField; }
        set
        {
            if (intField != value)
            {
                intField = value;
            }
        }
    }
    public string EqualsShortCut
    {
        get { return stringField; }
        set
        {
            if (Equals(value, stringField))
            {
                return;
            }
            stringField = value;
        }
    }
    public string EqualsShortCutInverse
    {
        get { return stringField; }
        set
        {
            if (Equals(stringField, value))
            {
                return;
            }
            stringField = value;
        }
    }
    public string EqualsNested
    {
        get { return stringField; }
        set
        {
            if (!Equals(value, stringField))
            {
                stringField = value;
            }
        }
    }
    public string EqualsNestedInverse
    {
        get { return stringField; }
        set
        {
            if (!Equals(stringField, value))
            {
                stringField = value;
            }
        }
    }
    public string NoEqualsNoField
    {
        get { return ""; }
        set { }
    }
    public string EqualsNoField
    {
        get { return ""; }
        set
        {
            if (EqualsNoField == value)
            {
// ReSharper disable once RedundantJumpStatement
                return;
            }
            Debug.WriteLine(value);
        }
    }
    public string StringEqualsShortCut
    {
        get { return stringField; }
        set
        {
            if (string.Equals(value, stringField))
            {
                return;
            }
            stringField = value;
        }
    }
    public string StringEqualsShortCutInverse
    {
        get { return stringField; }
        set
        {
            if (string.Equals(stringField, value))
            {
                return;
            }
            stringField = value;
        }
    }
    public string StringEqualsNested
    {
        get { return stringField; }
        set
        {
            if (!string.Equals(value, stringField))
            {
                stringField = value;
            }
        }
    }
    public string StringEqualsNestedInverse
    {
        get { return stringField; }
        set
        {
            if (!string.Equals(stringField, value))
            {
                stringField = value;
            }
        }
    }

    public string StringEqualsOrdinal
    {
        get { return stringField; }
        set
        {
            if (!string.Equals(stringField, value, StringComparison.Ordinal))
            {
                stringField = value;
            }
        }
    }
    public string NoEquality
    {
        get { return stringField; }
        set
        {
            stringField = value;
        }
    }
}