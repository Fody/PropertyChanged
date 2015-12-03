using System;

namespace PropertyChanged
{
    /// <summary>
    /// Injects this method to be called when a dependent property is set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OnPropertyChangedAttribute : Attribute
    {
        ///<summary>
        /// Initializes a new instance of <see cref="OnPropertyChangedAttribute"/>.
        ///</summary>
        ///<param name="property">A property that the assigned method depends on.</param>
        public OnPropertyChangedAttribute(string property)
        {
        }

        ///<summary>
        /// Initializes a new instance of <see cref="OnPropertyChangedAttribute"/>.
        ///</summary>
        ///<param name="property">A property that the assigned method depends on.</param>
        ///<param name="otherProperties">The properties that the assigned method depends on.</param>
        public OnPropertyChangedAttribute(string property, params string[] otherProperties)
        {
        }
    }
}
