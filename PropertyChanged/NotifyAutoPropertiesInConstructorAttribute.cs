using System;

namespace PropertyChanged
{
    /// <summary>
    /// Enables property changed events for auto-properties to be raised already when a value is assigned in the constructor.
    /// </summary>
    /// <remarks>
    /// To be used with care - when raising events from within the constructor even before the constructor is finished, 
    /// event handlers will work on an only partialy initialized object!
    /// Also when the OnPropertyChanged method is virtual (the default), you get a <see href="https://msdn.microsoft.com/en-us/library/ms182331.aspx">CA2214</see> warning when this feature is enabled. 
    /// </remarks>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly)]
    public class NotifyAutoPropertiesInConstructorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyAutoPropertiesInConstructorAttribute"/> class.
        /// </summary>
        /// <param name="value">
        /// if set to <c>true</c>, assingning values to auto-properites in the constructor will raise property changed events; 
        /// otherwise the value will be assigend only to the backing field, to avoid risks when property changed events are raised 
        /// on only partially initialized objects from within the constructor.
        /// </param>
        // ReSharper disable once UnusedParameter.Local
        public NotifyAutoPropertiesInConstructorAttribute(bool value)
        {
            
        }
    }
}
