using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;


public static class Verifier
{
    static string exePath;
    static bool peverifyFound = true;

    static Verifier()
    {
        
        exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");

        if (!File.Exists(exePath))
        {
            exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");
        }
        peverifyFound = File.Exists(exePath);
        if (!peverifyFound)
        {
#if(!DEBUG)
            throw new Exception("Could not fund PEVerify");
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
        var message = string.Format("Failed processing {0}\r\n{1}", Path.GetFileName(afterAssemblyPath), after);
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