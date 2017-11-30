using System;

// ReSharper disable UnusedParameter.Local
namespace PropertyChanged
{
    /// <summary>
    /// Injects this property to be notified when a dependent property is set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AlsoNotifyForAttribute : Attribute
    {

        ///<summary>
        /// Initializes a new instance of <see cref="DependsOnAttribute"/>.
        ///</summary>
        ///<param name="property">A property that will be notified for.</param>
        public AlsoNotifyForAttribute(string property)
        {
        }

        ///<summary>
        /// Initializes a new instance of <see cref="DependsOnAttribute"/>.
        ///</summary>
        ///<param name="property">A property that will be notified for.</param>
        ///<param name="otherProperties">The properties that will be notified for.</param>
        public AlsoNotifyForAttribute(string property, params string[] otherProperties)
        {
        }
    }
}
// ReSharper restore UnusedParameter.Local