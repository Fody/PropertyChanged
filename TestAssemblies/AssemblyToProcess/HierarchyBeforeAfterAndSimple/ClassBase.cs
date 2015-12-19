using System.ComponentModel;

namespace HierarchyBeforeAfterAndSimple
{
    public class ClassBase : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}