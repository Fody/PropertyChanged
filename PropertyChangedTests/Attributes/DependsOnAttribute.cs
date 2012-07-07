using System;

namespace PropertyChanged
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DependsOnAttribute : Attribute
    {
        public DependsOnAttribute(string dependency, params string[] otherDependencies)
        {

        }
    }
}