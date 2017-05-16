using System;
// ReSharper disable UnusedParameter.Local

namespace PropertyChanged
{
    /// <summary>
    /// Injects this property to be notified when a dependent property is set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DependsOnAttribute"/>.
        /// </summary>
        /// <param name="dependency">A property that the assigned property depends on.</param>
        public DependsOnAttribute(string dependency)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DependsOnAttribute"/>.
        /// </summary>
        /// <param name="dependency">A property that the assigned property depends on.</param>
        /// <param name="otherDependencies">The properties that the assigned property depends on.</param>
        public DependsOnAttribute(string dependency, params string[] otherDependencies)
        {
        }
    }
}
// ReSharper restore UnusedParameter.Local