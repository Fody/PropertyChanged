.method public hidebysig specialname 
	instance void set_Property2 (
		int32 'value'
	) cil managed 
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	// Header size: 12
	// Code size: 45 (0x2d)
	.maxstack 2

	IL_0000: ldarg.0
	IL_0001: ldfld int32 SmokeTest.Testee::'<Property2>k__BackingField'
	IL_0006: ldarg.1
	IL_0007: ceq
	IL_0009: brfalse.s IL_000d

	IL_000b: br.s IL_002b

	IL_000d: nop
	IL_000e: ldarg.0
	IL_000f: ldstr "Property2"
	IL_0014: callvirt instance void SmokeTest.Testee::OnPropertyChanging(string)
	IL_0019: ldarg.0
	IL_001a: ldarg.1
	IL_001b: stfld int32 SmokeTest.Testee::'<Property2>k__BackingField'
	IL_0020: ldarg.0
	IL_0021: ldsfld class [System]System.ComponentModel.PropertyChangedEventArgs 'SmokeTest.<>PropertyChangedEventArgs'::Property2
	IL_0026: callvirt instance void SmokeTest.Testee::'<>OnPropertyChanged'(class [System]System.ComponentModel.PropertyChangedEventArgs)

	IL_002b: nop
	IL_002c: ret
} // end of method Testee::set_Property2
