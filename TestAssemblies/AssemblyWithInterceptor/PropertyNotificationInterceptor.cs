using System;

public static class PropertyChangedNotificationInterceptor
{
    public static void Intercept(object target, Action action, string propertyName)
    {
        action();
        InterceptCalled = true;
    }

    public static bool InterceptCalled { get; set; }
}