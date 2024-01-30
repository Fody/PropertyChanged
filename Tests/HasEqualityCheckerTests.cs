using System;
using System.Diagnostics;
using Mono.Cecil;

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedVariable
// ReSharper disable ValueParameterNotUsed

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

    [Fact]
    public void EqualityShortCutTest()
    {
        var instructions = GetInstructions("EqualityShortCut");
        var field = GetField("intField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Fact]
    public void EqualsNoFieldTest()
    {
        var instructions = GetInstructions("EqualsNoField");
        var field = GetField("intField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, null));
    }
    [Fact]
    public void NoEqualsNoFieldTest()
    {
        var instructions = GetInstructions("NoEqualsNoField");
        var field = GetField("intField");
        Assert.False(HasEqualityChecker.AlreadyHasEquality(instructions, null));
    }
    [Fact]
    public void EqualityShortCutInverseTest()
    {
        var instructions = GetInstructions("EqualityShortCutInverse");
        var field = GetField("intField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Fact]
    public void EqualityNestedTest()
    {
        var instructions = GetInstructions("EqualityNested");
        var field = GetField("intField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Fact]
    public void EqualityNestedInverseTest()
    {
        var instructions = GetInstructions("EqualityNestedInverse");
        var field = GetField("intField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Fact]
    public void EqualsShortCutTest()
    {
        var instructions = GetInstructions("EqualsShortCut");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Fact]
    public void EqualsShortCutInverseTest()
    {
        var instructions = GetInstructions("EqualsShortCutInverse");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Fact]
    public void EqualsNestedInverseTest()
    {
        var instructions = GetInstructions("EqualsNestedInverse");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Fact]
    public void EqualsNestedTest()
    {
        var instructions = GetInstructions("EqualsNested");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Fact]
    public void StringEqualsShortCutTest()
    {
        var instructions = GetInstructions("StringEqualsShortCut");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }
    [Fact]
    public void StringEqualsShortCutInverseTest()
    {
        var instructions = GetInstructions("StringEqualsShortCutInverse");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Fact]
    public void StringEqualsNestedTest()
    {
        var instructions = GetInstructions("StringEqualsNested");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Fact]
    public void StringEqualsOrdinalTest()
    {
        var instructions = GetInstructions("StringEqualsOrdinal");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Fact]
    public void StringEqualsNestedInverseTest()
    {
        var instructions = GetInstructions("StringEqualsNestedInverse");
        var field = GetField("stringField");
        Assert.True(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    [Fact]
    public void NoEqualityTest()
    {
        var instructions = GetInstructions("NoEquality");
        var field = GetField("stringField");
        Assert.False(HasEqualityChecker.AlreadyHasEquality(instructions, field));
    }

    PropertyDefinition GetInstructions(string equalityShortcut)
    {
        return properties.First(definition => definition.Name == equalityShortcut);
    }
    FieldDefinition GetField(string equalityShortcut)
    {
        return fields.First(_ => _.Name == equalityShortcut);
    }

    int intField;
    string stringField;

    public int EqualityShortCut
    {
        get => intField;
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
        get => intField;
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
        get => intField;
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
        get => intField;
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
        get => stringField;
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
        get => stringField;
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
        get => stringField;
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
        get => stringField;
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
        get => "";
        set { }
    }
    public string EqualsNoField
    {
        get => "";
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
        get => stringField;
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
        get => stringField;
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
        get => stringField;
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
        get => stringField;
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
        get => stringField;
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
        get => stringField;
        set => stringField = value;
    }
}