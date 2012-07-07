#if (DEBUG)
using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
#endif

public static class Verifier
{
    public static void Verify(string assemblyPath)
    {
#if (DEBUG)
        var exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");

        if (!File.Exists(exePath))
        {
            exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");
        }
        using (var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath + "\"")
                                               {
                                                   RedirectStandardOutput = true,
                                                   UseShellExecute = false,
                                                   CreateNoWindow = true
                                               }))
        {
            process.WaitForExit(10000);
            var readToEnd = process.StandardOutput.ReadToEnd().Trim();
            Assert.IsTrue(readToEnd.Contains(string.Format("All Classes and Methods in {0} Verified.", assemblyPath)), readToEnd);
        }
#endif
    }
}