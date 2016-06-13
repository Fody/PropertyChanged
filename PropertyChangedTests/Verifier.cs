using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;

public static class Verifier
{
    static string exePath;
    static bool peverifyFound;

    static Verifier()
    {
        var sdkPath = Path.GetFullPath(Path.Combine(ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.VersionLatest), "..\\.."));
        exePath = Directory.GetFiles(sdkPath, "peverify.exe", SearchOption.AllDirectories).LastOrDefault();

        peverifyFound = File.Exists(exePath);
        if (!peverifyFound)
        {
#if(!DEBUG)
            throw new System.Exception("Could not find PEVerify");
#endif
        }
    }
    public static void Verify(string beforeAssemblyPath, string afterAssemblyPath)
    {
        if (!peverifyFound)
        {
            return;
        }
        Debug.WriteLine(afterAssemblyPath);
        var before = Validate(beforeAssemblyPath);
        var after = Validate(afterAssemblyPath);
        var message = $"Failed processing {Path.GetFileName(afterAssemblyPath)}\r\n{after}";
        NUnit.Framework.Assert.AreEqual(TrimLineNumbers(before), TrimLineNumbers(after), message);
    }

    public static string Validate(string assemblyPath2)
    {

        var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        process.WaitForExit(10000);
        return process.StandardOutput.ReadToEnd().Trim().Replace(assemblyPath2, "");
    }

    static string TrimLineNumbers(string foo)
    {
        return Regex.Replace(foo, @"0x.*]", "");
    }
}