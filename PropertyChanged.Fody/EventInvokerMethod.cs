using Mono.Cecil;

public enum InvokerTypes
{
    String,
    BeforeAfter,
    PropertyChangedArg
}

public class EventInvokerMethod 
{
    public MethodReference MethodReference;
    public InvokerTypes InvokerType;
}