using System.Globalization;

static class CodeBlock
{
    sealed class CodeBlockHandler : IDisposable
    {
        private readonly CodeBuilder? _codeBuilder;

        public CodeBlockHandler(CodeBuilder codeBuilder, string lineTemplate, string? parameter)
        {
            if (string.IsNullOrEmpty(parameter))
                return;

            codeBuilder.Add(string.Format(CultureInfo.InvariantCulture, lineTemplate, parameter));
            codeBuilder.Add("{");
            _codeBuilder = codeBuilder;
        }

        public CodeBlockHandler(CodeBuilder codeBuilder, string line)
        {
            codeBuilder.Add(line);
            codeBuilder.Add("{");
            _codeBuilder = codeBuilder;
        }

        public void Dispose()
        {
            _codeBuilder?.Add("}");
        }
    }

    public static IDisposable AddBlock(this CodeBuilder codeBuilder, string template, string? parameter)
    {
        return new CodeBlockHandler(codeBuilder, template, parameter);
    }

    public static IDisposable AddBlock(this CodeBuilder codeBuilder, string line)
    {
        return new CodeBlockHandler(codeBuilder, line);
    }

}
