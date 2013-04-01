using System.ComponentModel;

public class ClassEquality : INotifyPropertyChanged
{
    public string StringProperty { get; set; }
    public int IntProperty { get; set; }
    public int? NullableIntProperty { get; set; }
    public bool BoolProperty { get; set; }
    public bool? NullableBoolProperty { get; set; }
    public object ObjectProperty { get; set; }
    public short ShortProperty { get; set; }
    public ushort UShortProperty { get; set; }
    public byte ByteProperty { get; set; }
    public sbyte SByteProperty { get; set; }
    public char CharProperty { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}