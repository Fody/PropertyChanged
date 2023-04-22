.method public hidebysig specialname 
	instance void set_Property3 (
		valuetype [mscorlib]System.Nullable`1<int32> 'value'
	) cil managed 
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	// Header size: 12
	// Code size: 48 (0x30)
	.maxstack 2

	IL_0000: ldarg.0
	IL_0001: ldfld valuetype [mscorlib]System.Nullable`1<int32> SmokeTest.Testee::'<Property3>k__BackingField'
	IL_0006: ldarg.1
	IL_0007: call bool [mscorlib]System.Nullable::Equals<int32>(valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_000c: brfalse.s IL_0010

	IL_000e: br.s IL_002e

	IL_0010: nop
	IL_0011: ldarg.0
	IL_0012: ldstr "Property3"
	IL_0017: callvirt instance void SmokeTest.Testee::OnPropertyChanging(string)
	IL_001c: ldarg.0
	IL_001d: ldarg.1
	IL_001e: stfld valuetype [mscorlib]System.Nullable`1<int32> SmokeTest.Testee::'<Property3>k__BackingField'
	IL_0023: ldarg.0
	IL_0024: ldsfld class [System]System.ComponentModel.PropertyChangedEventArgs 'SmokeTest.<>PropertyChangedEventArgs'::Property3
	IL_0029: callvirt instance void SmokeTest.Testee::'<>OnPropertyChanged'(class [System]System.ComponentModel.PropertyChangedEventArgs)

	IL_002e: nop
	IL_002f: ret
} // end of method Testee::set_Property3
