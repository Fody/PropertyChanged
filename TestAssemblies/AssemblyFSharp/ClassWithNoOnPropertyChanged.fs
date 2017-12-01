namespace Namespace

open System.ComponentModel

type ClassWithNoOnPropertyChanged() =
    let mutable propval =""

    let event = new Event<_,_>()
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged = event.Publish

    member this.Property1
        with get() = propval
        and  set(v) = propval <- v