using System.Xml;
using System.Xml.Serialization;

public record Configuration
{
    static readonly XmlSerializer Serializer = new(typeof(Configuration));

    public bool IsCodeGeneratorDisabled { get; set; }

    public string? EventInvokerName { get; set; }

    public static Configuration Read(string? configuration)
    {
        try
        {
            if (configuration != null)
            {
                return Deserialize(configuration);
            }
        }
        catch
        {
            // just go with default options
        }

        return new();
    }

    static Configuration Deserialize(string configuration)
    {
        using var stringReader = new StringReader($"<Configuration>{configuration}</Configuration>");
        using var xmlReader = new CaseInsensitiveXmlReader(stringReader);

        return (Configuration)Serializer.Deserialize(xmlReader);
    }

    class CaseInsensitiveXmlReader : XmlTextReader
    {
        public CaseInsensitiveXmlReader(TextReader reader) : base(reader)
        {
        }

        public override string ReadElementString()
        {
            var text = base.ReadElementString();

            // bool TryParse accepts case-insensitive 'true' and 'false'
            if (bool.TryParse(text, out var result))
            {
                text = XmlConvert.ToString(result);
            }

            return text;
        }
    }
}