using System.Text;

internal class CodeBuilder
{
    private readonly StringBuilder _stringBuilder = new();

    private int _indent;

    public CodeBuilder Add(string line = "")
    {
        if (string.IsNullOrEmpty(line))
        {
            _stringBuilder.AppendLine();
            return this;
        }

        if (line.StartsWith("}"))
        {
            --_indent;
        }

        _stringBuilder.Append(new string(' ', 4 * _indent));
        _stringBuilder.AppendLine(line);

        if (line.StartsWith("{"))
        {
            ++_indent;
        }

        return this;
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }
}

