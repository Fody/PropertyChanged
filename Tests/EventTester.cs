using System;
using System.ComponentModel;
using System.Reflection;

public static class EventTester
{
    internal static void TestPropertyNotCalled(dynamic instance)
    {
        var property1EventCalled = false;
        ((INotifyPropertyChanged) instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        };
        instance.Property1 = "a";
        Assert.False(property1EventCalled);
    }

    internal static void TestProperty(dynamic instance, bool checkProperty2, bool delayAfterSetProperty1 = false)
    {
        var property1EventCalled = false;
        var property2EventCalled = false;
        ((INotifyPropertyChanged) instance).PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }

            if (args.PropertyName == "Property2")
            {
                property2EventCalled = true;
            }
        };
        instance.Property1 = "a";

        if(delayAfterSetProperty1)
        {
            Task.Delay(TimeSpan.FromMilliseconds(25)).Wait();
        }

        Assert.True(property1EventCalled);
        if (checkProperty2)
        {
            Assert.True(property2EventCalled);
        }

        property1EventCalled = false;
        property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.False(property1EventCalled);
        if (checkProperty2)
        {
            Assert.False(property2EventCalled);
        }
    }

    internal static void TestProperty<T>(dynamic instance, string propertyName, T propertyValue, bool ignoreEquality = false)
    {
        var eventCalled = false;
        instance.PropertyChanged += new PropertyChangedEventHandler((sender, args) =>
        {
            if (args.PropertyName == propertyName)
            {
                eventCalled = true;
            }
        });

        var type = (Type) instance.GetType();
        var propertyInfo = type.GetProperties().First(_ => _.Name == propertyName);
        propertyInfo.SetValue(instance, propertyValue, null);

        Assert.True(eventCalled);
        if (ignoreEquality)
        {
            return;
        }

        eventCalled = false;
        propertyInfo.SetValue(instance, propertyValue, null);
        Assert.False(eventCalled);
    }

    internal static void TestValueTypeProperty(dynamic instance)
    {
        var property1EventCalled = false;

        instance.PropertyChanged += new PropertyChangedEventHandler((_, args) =>
        {
            if (args.PropertyName == "Property1")
            {
                property1EventCalled = true;
            }
        });

        instance.Property1 = "a";
        Assert.True(property1EventCalled);

        property1EventCalled = false;
        instance.Property1 = "a";
        Assert.False(property1EventCalled);
    }

    public static dynamic GetInstance(this Assembly assembly, string className, params object[] args)
    {
        var type = assembly.GetType(className, true);
        //dynamic instance = FormatterServices.GetUninitializedObject(type);
        return Activator.CreateInstance(type, args);
    }

    public static dynamic GetGenericInstance(this Assembly assembly, string className, Type[] typeArguments)
    {
        var type = assembly.GetType(className, true);
        var constructedType = type.MakeGenericType(typeArguments);
        return Activator.CreateInstance(constructedType);
    }
}
