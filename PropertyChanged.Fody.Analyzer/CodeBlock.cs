using System.Globalization;

static class CodeBlock
{
    public static IDisposable AddBlock(this CodeBuilder codeBuilder, string template, params string?[] parameters)
    {
        return new DisposableItemCollection(parameters.Select(parameter => new CodeBlockHandler(codeBuilder, template, parameter)).ToArray());
    }

    public static IDisposable AddBlock(this CodeBuilder codeBuilder, string line)
    {
        return new CodeBlockHandler(codeBuilder, line);
    }

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

    sealed class DisposableItemCollection : IDisposable
    {
        readonly IReadOnlyCollection<IDisposable> _items;

        public DisposableItemCollection(IReadOnlyCollection<IDisposable> items)
        {
            _items = items;
        }

        public void Dispose()
        {
            foreach (var item in _items)
            {
                item.Dispose();
            }
        }
    }
}
