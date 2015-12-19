using System.ComponentModel;

namespace AssemblyWithBase.Simple
{
	public class BaseClass : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void OnPropertyChanged(string text1)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(text1));
		}

	}
}