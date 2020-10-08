using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public abstract class BaseClass : INotifyPropertyChanged
{
    public IList<string> Notifications = new List<string>();

    public virtual int Property1 { get; set; }
    public virtual int Property4 { get; set; }
    public int Property2 => Property1 + 1;
    public abstract int Property5 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        Notifications.Add(propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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

    public override int Property4 { get; set; }
    public override int Property5 { get; set; }

    public int Property3 => Property1 - Property5 + Property4;

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