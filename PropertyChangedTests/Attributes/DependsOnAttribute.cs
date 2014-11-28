using System;

// ReSharper disable UnusedParameter.Local
namespace PropertyChanged
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
        public DependsOnAttribute(string dependency, params string[] otherDependencies)
        {

        }
    }
}

// ReSharper restore UnusedParameter.Local