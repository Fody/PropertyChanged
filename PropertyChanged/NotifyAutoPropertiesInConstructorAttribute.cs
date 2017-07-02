using System;

namespace PropertyChanged
{
    /// <summary>
    /// Controls whether property changed events for auto-properties are raised when a value is assigned to an auto-property in the constructor.
    /// </summary>
    /// <remarks>
    /// When raising events or calling "On_Property_Changed" from within the constructor even before the constructor is finished, event handlers will work on an only partialy initialized object, which can easily lead to crashes if the event handler is not aware of this.
    /// Also when the OnPropertyChanged method is virtual (the default), you get a <see href="https://msdn.microsoft.com/en-us/library/ms182331.aspx">CA2214</see> warning.
    /// Another side-effect is that if you are using the IsChanged property, it is already true right after you have created a new object.
    /// With old-style properties you can bypass this by initializing the backing field instad of the property, but with auto-properties you have no chance to do so.
    /// With this attribute you can control if the property setter for auto-properties should be bypassed in the constructor, and replaced by setting just the backing field.
    /// You can apply this attribute at assembly level to control the default behavior, or at class, constructor or property level to apply to the specific scope only.
    /// The attribute of the nearest scope will be active, i.e. you can e.g. opt-out at assembly level but opt-in again for a specific class or property.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly)]
    public class NotifyAutoPropertiesInConstructorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyAutoPropertiesInConstructorAttribute"/> class.
        /// </summary>
        /// <param name="value">
        /// if set to <c>true</c>, assingning values to auto-properites in the constructor will raise property changed events; 
        /// otherwise the value will be assigend directly to the backing field, to avoid risks when property changed events are raised 
        /// on only partially initialized objects from within the constructor.
        /// </param>
        // ReSharper disable once UnusedParameter.Local
        public NotifyAutoPropertiesInConstructorAttribute(bool value)
        {
        }
    }
}
