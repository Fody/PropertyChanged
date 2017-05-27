using System;

namespace PropertyChanged
{
    /// <summary>
    /// Obsolete
    /// </summary>
    [Obsolete(
        message: "This configuration option has been deprecated. The use of this attribute was to add INotifyPropertyChanged to a class with its associated event definition. After that all classes that implement INotifyPropertyChanged have their properties weaved, weather they have the ImplementPropertyChangedAttribute or not. This attribute was often incorrectly interpreted as an opt in approach to having properties weaved, which was never the intent nor how it ever operated.",
        error:true)]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ImplementPropertyChangedAttribute : Attribute
    {
    }
}
