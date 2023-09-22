using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PropertyChanged;

public abstract class BaseClass : INotifyPropertyChanged
{
    public IList<string> Notifications = new List<string>();

    public virtual int Property1 { get; set; }
    [OnChangedMethod(nameof(On_Property2_Changed))]
    public virtual int Property2 { get; set; }
    public abstract int Property3 { get; set; }

    public int Property4 => Property1 - Property3 + Property2;

    [OnChangedMethod(nameof(On_Property9_Changed))]
    public object Property9 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        Notifications.Add(propertyName);
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    void OnProperty1Changed()
    {
        ReportOnChanged();
    }

    void On_Property2_Changed()
    {
        ReportOnChanged();
    }

    void OnProperty3Changed()
    {
        ReportOnChanged();
    }

    void OnProperty4Changed()
    {
        ReportOnChanged();
    }

    void OnProperty5Changed()
    {
        ReportOnChanged();
    }

    void On_Property9_Changed()
    {
        ReportOnChanged();
    }

    void ReportOnChanged([CallerMemberName] string callerMemberName = null)
    {
        Notifications.Add("base:" + callerMemberName);
    }
}

public class DerivedClass : BaseClass
{
    public override int Property1
    {
        get => base.Property1;
        set => base.Property1 = value;
    }

    [OnChangedMethod(nameof(On_Property2_Changed))]
    public override int Property2 { get; set; }
    public override int Property3 { get; set; }

    public int Property5 => Property1 - Property3 + Property2;

    void OnProperty1Changed()
    {
        ReportOnChanged();
    }

    void On_Property2_Changed()
    {
        ReportOnChanged();
    }

    void OnProperty3Changed()
    {
        ReportOnChanged();
    }

    void OnProperty4Changed()
    {
        ReportOnChanged();
    }

    void OnProperty5Changed()
    {
        ReportOnChanged();
    }

    void ReportOnChanged([CallerMemberName] string callerMemberName = null)
    {
        Notifications.Add("derived:" + callerMemberName);
    }
}

public class DerivedNoOverrides : BaseClass
{
    public override int Property3 { get; set; }

    public int Property5 => Property1 - Property3 + Property2;

    void OnProperty1Changed()
    {
        ReportOnChanged();
    }

    void OnProperty2Changed()
    {
        ReportOnChanged();
    }

    void OnProperty3Changed()
    {
        ReportOnChanged();
    }

    void OnProperty4Changed()
    {
        ReportOnChanged();
    }

    void OnProperty5Changed()
    {
        ReportOnChanged();
    }

    void ReportOnChanged([CallerMemberName] string callerMemberName = null)
    {
        Notifications.Add("derived:" + callerMemberName);
    }
}

public class DerivedNewProperties : BaseClass
{
    public new int Property1 { get; set; }
    public override int Property3 { get; set; }

    public int Property5 => Property1 - Property3 + Property2;

    void OnProperty1Changed()
    {
        ReportOnChanged();
    }

    void OnProperty2Changed()
    {
        ReportOnChanged();
    }

    void OnProperty3Changed()
    {
        ReportOnChanged();
    }

    void OnProperty4Changed()
    {
        ReportOnChanged();
    }

    void OnProperty5Changed()
    {
        ReportOnChanged();
    }

    void ReportOnChanged([CallerMemberName] string callerMemberName = null)
    {
        Notifications.Add("derived:" + callerMemberName);
    }
}

public class DerivedDerivedClass : DerivedClass
{
    public override int Property1
    {
        get => base.Property1;
        set => base.Property1 = value;
    }

    public override int Property2 { get; set; }
    public override int Property3 { get; set; }

    public int Property6 => Property1 - Property3 + Property2;

    void OnProperty1Changed()
    {
        ReportOnChanged();
    }

    void OnProperty2Changed()
    {
        ReportOnChanged();
    }

    void OnProperty3Changed()
    {
        ReportOnChanged();
    }

    void OnProperty4Changed()
    {
        ReportOnChanged();
    }

    void OnProperty5Changed()
    {
        ReportOnChanged();
    }

    void OnProperty6Changed()
    {
        ReportOnChanged();
    }

    void ReportOnChanged([CallerMemberName] string callerMemberName = null)
    {
        Notifications.Add("derived++:" + callerMemberName);
    }
}

public class PocoBase
{
    public IList<string> Notifications = new List<string>();

    public virtual int Property1 { get; set; }
}

public class DerivedFromPoco : PocoBase, INotifyPropertyChanged
{
    public override int Property1
    {
        get => base.Property1;
        set => base.Property1 = value;
    }

    public int Property4 => Property1 + 1;

    void OnProperty1Changed()
    {
        ReportOnChanged();
    }

    void OnProperty2Changed()
    {
        ReportOnChanged();
    }

    void OnProperty3Changed()
    {
        ReportOnChanged();
    }

    void OnProperty4Changed()
    {
        ReportOnChanged();
    }

    void OnProperty5Changed()
    {
        ReportOnChanged();
    }

    void ReportOnChanged([CallerMemberName] string callerMemberName = null)
    {
        Notifications.Add("derived:" + callerMemberName);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        Notifications.Add(propertyName);
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}

public class DerivedCallingChild : BaseClass
{
    IList<DerivedCallingChild> someChildren = Array.Empty<DerivedCallingChild>();
    int property2;

    public override int Property1
    {
        get => base.Property1;
        set => base.Property1 = value > 0 ? value : DoSomeDummyStuff(value, 0, 0);
    }

    public override int Property2
    {
        get => property2;
        set
        {
            property2 = value;
            foreach (var child in someChildren)
            {
                child.Property2 = value;
            }
        }
    }

    public override int Property3 { get; set; }

    int DoSomeDummyStuff(int a, int b, int c)
    {
        return a + b + c;
    }
}
