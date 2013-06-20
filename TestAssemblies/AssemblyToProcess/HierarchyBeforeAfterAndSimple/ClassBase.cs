using System.ComponentModel;

namespace HierarchyBeforeAfterAndSimple
{
    public class ClassBase : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
		}

	}
}