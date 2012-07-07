using System.ComponentModel;

namespace AssemblyWithBase.Simple
{
	public class BaseClass : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void OnPropertyChanged(string text1)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(text1));
			}
		}

	}
}