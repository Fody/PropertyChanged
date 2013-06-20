using System;
using System.ComponentModel;

public class ClassEquality : INotifyPropertyChanged
{
    DateTimeOffset? mixDateTimeOffset;
    public string StringProperty { get; set; }
    public int IntProperty { get; set; }
    public int? NullableIntProperty { get; set; }
    public bool BoolProperty { get; set; }
    public bool? NullableBoolProperty { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public DateTime DateTime { get; set; }
    public DateTimeOffset? NullableDateTimeOffset { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; }

    public DateTimeOffset MixDateTimeOffset
    {
        get { return mixDateTimeOffset.GetValueOrDefault(); }
        set { mixDateTimeOffset = value; }
    }

    public object ObjectProperty { get; set; }
    public short ShortProperty { get; set; }
    public ushort UShortProperty { get; set; }
    public byte ByteProperty { get; set; }
    public sbyte SByteProperty { get; set; }
    public char CharProperty { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}