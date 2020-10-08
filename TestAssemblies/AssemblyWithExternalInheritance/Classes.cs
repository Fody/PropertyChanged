using System.Runtime.CompilerServices;

public class DerivedClass : BaseClass
{
    public override int Property1
    {
        get => base.Property1;
        set => base.Property1 = value;
    }

    public override int Property2 { get; set; }
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