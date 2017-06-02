namespace Namespace
type I1 =
    abstract member X : unit -> int

type XYZ() =
    member val Xyz = 1 with get,set
    member val Z = new System.Collections.Generic.List<I1>() with get

[<PropertyChanged.ImplementPropertyChangedAttribute>]
type Test() =
    let i1 = {new I1 with member i.X () = 33}
    [<DefaultValue>]
    val mutable internal _TextEditorControl : XYZ
    member x.TextEditorControl with get() = x._TextEditorControl
                               and set v =
                                    x._TextEditorControl <- v
                                    x._TextEditorControl.Xyz <- 6
                                    x.TextEditorControl.Z.Add i1
