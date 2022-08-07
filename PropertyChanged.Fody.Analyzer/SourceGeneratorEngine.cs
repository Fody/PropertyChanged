﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

#pragma warning disable CS0067
static class SourceGeneratorEngine
{
    public static void GenerateSource(SourceProductionContext context, Configuration configuration, ImmutableArray<ClassContext> classes)
    {
        const string sourceFileHintName = "PropertyChanged.g.cs";

        if (configuration.IsDisabled)
        {
            context.AddSource(sourceFileHintName, @"// Source generator is disabled by configuration.");
            return;
        }

        if (!classes.Any())
            return;

        var eventInvokerName = configuration.EventInvokerName?.Trim().NullIfEmpty() ?? "OnPropertyChanged";

        var codeBuilder = new CodeBuilder();

        try
        {
            codeBuilder.AddPreamble();

            foreach (var classContext in classes.Distinct(ClassContext.FullNameComparer))
            {
                GenerateCodeForClass(classContext, codeBuilder, eventInvokerName);
            }
        }
        catch (Exception ex)
        {
            codeBuilder.Add("/*");
            codeBuilder.Add($"GenerateSource failed: {ex}");
            codeBuilder.Add("*/");
        }

        context.AddSource(sourceFileHintName, codeBuilder.ToString());
    }

    static void AddPreamble(this CodeBuilder codeBuilder)
    {
        codeBuilder
            .Add("// <auto-generated/>")
            .Add("#nullable enable")
            .Add("#pragma warning disable CS0067")
            .Add("#pragma warning disable CS8019")
            .Add("using System.ComponentModel;")
            .Add("using System.Runtime.CompilerServices;");
    }

    static void GenerateCodeForClass(ClassContext classContext, CodeBuilder codeBuilder, string eventInvokerName)
    {
        DebugBeep();

        using (codeBuilder.AddBlock("namespace {0}", classContext.ContainingNamespace))
        {
            var isSealed = classContext.IsSealed;
            var hasBase = classContext.HasBase;

            using (codeBuilder.AddBlock("partial class {0}", classContext.ContainingTypeNames.Split('|')))
            {
                var baseDefinition = hasBase ? string.Empty : " : INotifyPropertyChanged";

                using (codeBuilder.AddBlock($"partial class {classContext.Name}{baseDefinition}"))
                {
                    codeBuilder.Add("public event PropertyChangedEventHandler? PropertyChanged;");

                    var modifiers1 = isSealed ? "private" : "protected";

                    using (codeBuilder.AddBlock($"{modifiers1} void {eventInvokerName}([CallerMemberName] string? propertyName = null)"))
                    {
                        if (isSealed)
                        {
                            codeBuilder.Add("PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));");
                        }
                        else
                        {
                            codeBuilder.Add($"{eventInvokerName}(new PropertyChangedEventArgs(propertyName));");
                        }
                    }

                    var modifiers2 = isSealed ? "private" : "protected virtual";

                    using (codeBuilder.AddBlock($"{modifiers2} void {eventInvokerName}(PropertyChangedEventArgs eventArgs)"))
                    {
                        codeBuilder.Add("PropertyChanged?.Invoke(this, eventArgs);");
                    }
                }
            }
        }
    }
}
