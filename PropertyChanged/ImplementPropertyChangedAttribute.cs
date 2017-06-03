#pragma warning disable 1591
using System;

namespace PropertyChanged
{
    [Obsolete(
        message: "This configuration option has been deprecated. The use of this attribute was to add INotifyPropertyChanged to a class with its associated event definition. After that all classes that implement INotifyPropertyChanged have their properties weaved, weather they have the ImplementPropertyChangedAttribute or not. This attribute was often incorrectly interpreted as an opt in approach to having properties weaved, which was never the intent nor how it ever operated. This attribute has been replaced by AddINotifyPropertyChangedInterfaceAttribute.",
        error: true)]
    public class ImplementPropertyChangedAttribute : Attribute
    {
    }
}
