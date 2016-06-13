using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

public static class EventTester
{

    internal static void TestPropertyNotCalled(dynamic instance)
    {
        var property1EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
                                                                  {
                                                                      if (args.PropertyName == "Property1")
                                                                      {
                                                                          property1EventCalled = true;
                                                                      }
                                                                  };
        instance.Property1 = "a";
        Assert.IsFalse(property1EventCalled);
    }
    internal static void TestProperty(dynamic instance, bool checkProperty2)
    {
        var property1EventCalled = false;
        var property2EventCalled = false;
        ((INotifyPropertyChanged)instance).PropertyChanged += (sender, args) =>
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

        Assert.IsTrue(property1EventCalled);
        if (checkProperty2)
        {
            Assert.IsTrue(property2EventCalled);
        }
        property1EventCalled = false;
        property2EventCalled = false;
        //Property has not changed on re-set so event not fired
        instance.Property1 = "a";
        Assert.IsFalse(property1EventCalled);
        if (checkProperty2)
        {
            Assert.IsFalse(property2EventCalled);
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

        var type = (Type)instance.GetType();
        var propertyInfo = type.GetProperties().First(x => x.Name == propertyName);
        propertyInfo.SetValue(instance, propertyValue, null);

        Assert.IsTrue(eventCalled);
        if (ignoreEquality)
        {
            return;
        }
        eventCalled = false;
        propertyInfo.SetValue(instance, propertyValue, null);
        Assert.IsFalse(eventCalled);
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