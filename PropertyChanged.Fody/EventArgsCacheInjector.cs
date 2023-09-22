partial class ModuleWeaver
{
    public void InitEventArgsCache()
    {
        EventArgsCache = new(this);
    }

    public void InjectEventArgsCache()
        => EventArgsCache.InjectType();

    public EventArgsCache EventArgsCache;
}
