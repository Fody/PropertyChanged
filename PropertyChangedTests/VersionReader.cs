using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

public class VersionReader
{
    public decimal FrameworkVersionAsNumber;
    public string FrameworkVersionAsString;
    public string TargetFrameworkProfile;
    public bool IsSilverlight;

    public VersionReader(string projectPath)
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();
        GetTargetFrameworkIdentifier(xDocument);
        GetFrameworkVersion(xDocument);
        GetTargetFrameworkProfile(xDocument);
    }

    void GetFrameworkVersion(XDocument xDocument)
    {
        FrameworkVersionAsString = xDocument.Descendants("TargetFrameworkVersion")
            .Select(c => c.Value)
            .First();
        FrameworkVersionAsNumber = decimal.Parse(FrameworkVersionAsString.Remove(0, 1), CultureInfo.InvariantCulture);
    }


    void GetTargetFrameworkProfile(XDocument xDocument)
    {
        TargetFrameworkProfile = xDocument.Descendants("TargetFrameworkProfile")
            .Select(c => c.Value)
            .FirstOrDefault();
    }

    void GetTargetFrameworkIdentifier(XDocument xDocument)
    {
        var targetFrameworkIdentifier = xDocument.Descendants("TargetFrameworkIdentifier")
            .Select(c => c.Value)
            .FirstOrDefault();
        if (string.Equals(targetFrameworkIdentifier, "Silverlight", StringComparison.OrdinalIgnoreCase))
        {
            IsSilverlight = true;
        }

    }

}