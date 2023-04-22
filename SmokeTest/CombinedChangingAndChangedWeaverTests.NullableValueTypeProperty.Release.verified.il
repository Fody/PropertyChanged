.class public auto ansi beforefieldinit SmokeTest.Testee
extends [mscorlib]System.Object
implements [System]System.ComponentModel.INotifyPropertyChanging,
[System]System.ComponentModel.INotifyPropertyChanged
{
.method public hidebysig specialname instance void
set_Property3(valuetype [mscorlib]System.Nullable`1<int32> 'value') cil managed
{
.maxstack  2
IL_0000:  ldarg.0
IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<int32> SmokeTest.Testee::'<Property3>k__BackingField'
IL_0006:  ldarg.1
IL_0007:  call       bool [mscorlib]System.Nullable::Equals<int32>(valuetype [mscorlib]System.Nullable`1<!!0>,
valuetype [mscorlib]System.Nullable`1<!!0>)
IL_000c:  brfalse.s  IL_000f
IL_000e:  ret
IL_000f:  nop
IL_0010:  ldarg.0
IL_0011:  call       instance valuetype [mscorlib]System.Nullable`1<int32> SmokeTest.Testee::get_Property3()
IL_0016:  ldarg.1
IL_0017:  call       bool [mscorlib]System.Nullable::Equals<int32>(valuetype [mscorlib]System.Nullable`1<!!0>,
valuetype [mscorlib]System.Nullable`1<!!0>)
IL_001c:  brfalse.s  IL_0020
IL_001e:  br.s       IL_003e
IL_0020:  nop
IL_0021:  ldarg.0
IL_0022:  ldstr      "Property3"
IL_0027:  callvirt   instance void SmokeTest.Testee::OnPropertyChanging(string)
IL_002c:  ldarg.0
IL_002d:  ldarg.1
IL_002e:  stfld      valuetype [mscorlib]System.Nullable`1<int32> SmokeTest.Testee::'<Property3>k__BackingField'
IL_0033:  ldarg.0
IL_0034:  ldsfld     class [System]System.ComponentModel.PropertyChangedEventArgs SmokeTest.'<>PropertyChangedEventArgs'::Property3
IL_0039:  callvirt   instance void SmokeTest.Testee::'<>OnPropertyChanged'(class [System]System.ComponentModel.PropertyChangedEventArgs)
IL_003e:  nop
IL_003f:  ret
}
}
