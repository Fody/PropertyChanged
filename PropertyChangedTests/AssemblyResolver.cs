using System;
using System.Collections.Generic;
using System.IO;
using NotifyPropertyWeaverMsBuildTask;

namespace NotifyPropertyWeaverTests
{
    public class AssemblyResolver 
    {


        public List<string> Directories { get; private set; }

        public AssemblyResolver(VersionReader versionReader, string projectPath, string assemblyPath)
        {

            Directories = new List<string>();
            Directories.Add(Path.GetDirectoryName(assemblyPath));

            if (versionReader.IsSilverlight)
            {

                if (versionReader.FrameworkVersionAsNumber < 3)
                {
                    throw new WeavingException("Only Silverlight 3 and up is supported.");
                }
                if (string.IsNullOrEmpty(versionReader.TargetFrameworkProfile))
                {
                    Directories.Add(string.Format(@"{0}\Reference Assemblies\Microsoft\Framework\Silverlight\{1}\", GetProgramFilesPath(), versionReader.FrameworkVersionAsString));
                }
                else
                {
                    Directories.Add(string.Format(@"{0}\Reference Assemblies\Microsoft\Framework\Silverlight\{1}\Profile\{2}", GetProgramFilesPath(), versionReader.FrameworkVersionAsString, versionReader.TargetFrameworkProfile));
                }

            }
            else
            {
                if (versionReader.FrameworkVersionAsNumber < 3.5)
                {
                    throw new WeavingException("Only .net 3.5 and up is supported.");
                }
                if (string.IsNullOrEmpty(versionReader.TargetFrameworkProfile))
                {
                    if (versionReader.FrameworkVersionAsNumber == 3.5)
                    {
                        Directories.Add(string.Format(@"{0}\Reference Assemblies\Microsoft\Framework\v3.5\", GetProgramFilesPath()));
                        Directories.Add(string.Format(@"{0}\Reference Assemblies\Microsoft\Framework\v3.0\", GetProgramFilesPath()));
                        Directories.Add(Environment.ExpandEnvironmentVariables(@"%WINDIR%\Microsoft.NET\Framework\v2.0.50727\"));
                    }
                    else
                    {
                        Directories.Add(string.Format(@"{0}\Reference Assemblies\Microsoft\Framework\.NETFramework\{1}\", GetProgramFilesPath(), versionReader.FrameworkVersionAsString));
                    }
                }
                else
                {
                    Directories.Add(string.Format(@"{0}\Reference Assemblies\Microsoft\Framework\.NETFramework\{1}\Profile\{2}", GetProgramFilesPath(), versionReader.FrameworkVersionAsString, versionReader.TargetFrameworkProfile));
                }
            }
            Directories.Add(Path.GetDirectoryName(projectPath));

        }

       
        public string GetProgramFilesPath()
        {
            var programFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (programFiles == null)
            {
                return Environment.GetEnvironmentVariable("ProgramFiles");
            }

            return programFiles;
        }


  
    }
}