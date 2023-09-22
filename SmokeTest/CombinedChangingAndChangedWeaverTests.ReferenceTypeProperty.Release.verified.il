.method public hidebysig specialname 
	instance void set_Property1 (
		string 'value'
	) cil managed 
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	// Header size: 12
	// Code size: 49 (0x31)
	.maxstack 3

	IL_0000: ldarg.0
	IL_0001: ldfld string SmokeTest.Testee::'<Property1>k__BackingField'
	IL_0006: ldarg.1
	IL_0007: ldc.i4.4
	IL_0008: call bool [mscorlib]System.String::Equals(string, string, valuetype [mscorlib]System.StringComparison)
	IL_000d: brfalse.s IL_0011

	IL_000f: br.s IL_002f

	IL_0011: nop
	IL_0012: ldarg.0
	IL_0013: ldstr "Property1"
	IL_0018: callvirt instance void SmokeTest.Testee::OnPropertyChanging(string)
	IL_001d: ldarg.0
	IL_001e: ldarg.1
	IL_001f: stfld string SmokeTest.Testee::'<Property1>k__BackingField'
	IL_0024: ldarg.0
	IL_0025: ldsfld class [System]System.ComponentModel.PropertyChangedEventArgs 'SmokeTest.<>PropertyChangedEventArgs'::Property1
	IL_002a: callvirt instance void SmokeTest.Testee::'<>OnPropertyChanged'(class [System]System.ComponentModel.PropertyChangedEventArgs)

	IL_002f: nop
	IL_0030: ret
} // end of method Testee::set_Property1
