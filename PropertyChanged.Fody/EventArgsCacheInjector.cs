partial class ModuleWeaver
{
    public void InitEventArgsCache()
    {
        EventArgsCache = new EventArgsCache(this);
    }

    public void InjectEventArgsCache() 
        => EventArgsCache.InjectType();

    public EventArgsCache EventArgsCache;
}
