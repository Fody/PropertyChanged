using System;

namespace PropertyChanged
{
    /// <summary>
    /// Injects this property to be notified when a dependant property is set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
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