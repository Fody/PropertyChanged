﻿using System.Text;

class CodeBuilder
{
    readonly StringBuilder _stringBuilder = new();

    int _indent;

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

        _stringBuilder.Append(' ', 4 * _indent).AppendLine(line);

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

