using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class ClassWithTaskReturningPropertyChangedNotifier : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;

    public virtual Task RaisePropertyChanged([CallerMemberName] string whichProperty = "")
    {
        var changedArgs = new PropertyChangedEventArgs(whichProperty);
        return RaisePropertyChanged(changedArgs);
    }

    async Task RaisePropertyChanged(PropertyChangedEventArgs changedArgs)
    {
        void raiseChange()
        {
            PropertyChanged?.Invoke(this, changedArgs);
        }

        await Task.Run(() => raiseChange());
    }
}
