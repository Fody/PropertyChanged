// ReSharper disable RedundantUsingDirective
using System.Reflection;
using System;
// ReSharper restore RedundantUsingDirective
using System.ComponentModel;
using Jounce.Core.Model;

namespace Caliburn.Micro
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

namespace Catel.Data
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
namespace Telerik.Windows.Controls
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }

    }
}

namespace Magellan.Framework
{
    public abstract class PresentationObject : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }

        protected void NotifyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void NotifyChanged(string propertyName, params string[] otherProperties)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}


namespace Cinch
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
#if (!WINDOWS_PHONE)
namespace Microsoft.Practices.Prism.ViewModel
{
    public class NotificationObject : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException("propertyNames");
            }
            foreach (var str in propertyNames)
            {
                RaisePropertyChanged(str);
            }
        }
    }

    public class PropertySupport
    {



        public static string ExtractPropertyName<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            var body = propertyExpression.Body as System.Linq.Expressions.MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("propertyExpression");
            }
            var member = body.Member as PropertyInfo;
            if (member == null)
            {
                throw new ArgumentException("propertyExpression");
            }
            if (member.GetGetMethod(true).IsStatic)
            {
                throw new ArgumentException("propertyExpression");
            }
            return body.Member.Name;
        }



    }
}
#endif
namespace GalaSoft.MvvmLight
{
    public class ViewModelBase : ObservableObject
    {
    }
    public class ObservableObject : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

namespace Caliburn.PresentationFramework
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}


namespace Jounce.Core.Model
{
    public abstract class BaseNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool BaseNotifyCalled { get; set; }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

namespace Jounce.Core.ViewModel
{
    public abstract class BaseViewModel : BaseNotify
    {
    }
}

namespace ReactiveUI
{
    public abstract class ReactiveObject : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void raisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}