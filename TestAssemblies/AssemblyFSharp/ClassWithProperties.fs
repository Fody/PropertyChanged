namespace Namespace

open System.ComponentModel

type ClassWithProperties() =
    let mutable propval =""

    let event = new Event<_,_>()
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = event.Publish

    member x.OnPropertyChanged(name)=
         event.Trigger(x, new PropertyChangedEventArgs(name))

    member this.Property1
        with get() = propval
        and  set(v) = propval <- v