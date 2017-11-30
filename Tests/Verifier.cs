﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

public static class Verifier
{
    static string exePath;

    static Verifier()
    {
        var windowsSdk = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\");
        exePath = Directory.EnumerateFiles(windowsSdk, "PEVerify.exe", SearchOption.AllDirectories)
            .OrderByDescending(x =>
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(x);
                return new Version(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart);
            })
            .FirstOrDefault();
        if (exePath == null)
        {
            throw new Exception("Could not find path to PEVerify");
        }
    }

    public static void Verify(string beforeAssemblyPath, string afterAssemblyPath)
    {
        var before = Validate(beforeAssemblyPath);
        var after = Validate(afterAssemblyPath);
        var message = $"Failed processing {Path.GetFileName(afterAssemblyPath)}\r\n{after}";
        Assert.AreEqual(TrimLineNumbers(before), TrimLineNumbers(after), message);
    }

    static string Validate(string assemblyPath2)
    {
        using (var process = Process.Start(new ProcessStartInfo(exePath, $"\"{assemblyPath2}\"")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }))
        {
            process.WaitForExit(10000);
            return process.StandardOutput.ReadToEnd().Trim().Replace(assemblyPath2, "");
        }
    }

    static string TrimLineNumbers(string foo)
    {
        return Regex.Replace(foo, "0x.*]", "");
    }
}