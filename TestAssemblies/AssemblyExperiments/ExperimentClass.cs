
using System.ComponentModel;
using Telerik.Windows.Controls;

public class ClassTelerik : ViewModelBase
{
    public string Property1 { get; set; }
}
namespace Telerik.Windows.Controls
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public bool BaseNotifyCalled { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChanged(string propertyName)
        {
            BaseNotifyCalled = true;
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }

    }
}