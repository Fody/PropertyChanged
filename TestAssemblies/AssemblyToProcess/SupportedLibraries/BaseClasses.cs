using System.ComponentModel;
using Jounce.Core.Model;

namespace Caliburn.Micro
{
    public class PropertyChangedBase :
        INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

namespace Catel.Data
{
    public class ObservableObject :
        INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

namespace Telerik.Windows.Controls
{
    public abstract class ViewModelBase :
        INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }
    }
}

namespace Magellan.Framework
{
    public abstract class PresentationObject :
        INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }

        protected void NotifyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
        protected void NotifyChanged(string propertyName, params string[] otherProperties)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

namespace Cinch
{
    public class ViewModelBase :
        INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

namespace GalaSoft.MvvmLight
{
    public class ViewModelBase :
        ObservableObject;

    public class ObservableObject :
        INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

namespace Caliburn.PresentationFramework
{
    public class PropertyChangedBase :
        INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

namespace Jounce.Core.Model
{
    public abstract class BaseNotify :
        INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool BaseNotifyCalled { get; set; }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

namespace Jounce.Core.ViewModel
{
    public abstract class BaseViewModel :
        BaseNotify;
}

namespace ReactiveUI
{
    public interface IReactiveObject :
        INotifyPropertyChanged
    {
        void RaisePropertyChanged(PropertyChangedEventArgs args);
    }

    public class ReactiveObject :
        IReactiveObject
    {
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            changed?.Invoke(this, args);
        }

        event PropertyChangedEventHandler changed;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => changed += value;
            remove => changed -= value;
        }
    }
}