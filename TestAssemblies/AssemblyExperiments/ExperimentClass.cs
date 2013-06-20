using System;
using System.Diagnostics;
using PropertyChanged;

[ImplementPropertyChanged]
public class ClassExperiment
{
    DateTimeOffset? mixDateTimeOffset;
    public DateTimeOffset MixDateTimeOffset
    {
        get { return mixDateTimeOffset.GetValueOrDefault(); }
        set { mixDateTimeOffset = value; }
    }
}

public class ClassExperiment2
{
    DateTimeOffset? mixDateTimeOffset;
    public DateTimeOffset MixDateTimeOffset
    {
        get { return mixDateTimeOffset.GetValueOrDefault(); }
        set
        {
            if (!Nullable.Equals(mixDateTimeOffset, value))
            {
                Trace.WriteLine(null);
            }

        }
    }
}
