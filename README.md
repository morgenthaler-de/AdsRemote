High-level interface for Beckhoff's TwinCAT.Ads API library. Project is not affiliated with Beckhoff.

Getting started
===============

Ads Route
---------

First you have to give your device/machine the permission to communicate with the Twincat Ads server by adding a route.

There are different ways of doing this depending on the device. You can use the Twincat Remote Manager for example.

You can use Ads.Remote.Router.AmsRouter for broadcast searching of network PLCs.
With help of Ads.Remote.PLC.Router you can add new route record into the remote PLC.

Installation
------------
TwinCAT 2 or 3 have to be installed.

Examples
===============

## PLC instance

First you have to create an instance of PLC object

```C#
PLC plc = new PLC("5.2.100.109.1.1");
```

## Connection and lost connection

```C#
plc.DeviceReady += Plc_DeviceReady;
plc.DeviceLost += Plc_DeviceLost;

[...]

private void Plc_DeviceReady(object sender, AdsDevice e)
{
	Log("READY [" + e.Address.Port.ToString() + "]");
}
```

##Runtime variables

Create a copy of your PLC's variable then use it like an ordinary variable

```C#
Var<short>	main_count	= plc.Var<short> ("MAIN.count");
Var<ushort>	main_state	= plc.Var<ushort>("MAIN.state");
Var<short>	g_Version	= plc.Var<ushort>(".VERSION");

Var<ushort>	frm0		= plc.Var<ushort>("Inputs.Frm0InputToggle", 27907);
Var<ushort>	devState	= plc.Var<ushort>(0xF030, 0x5FE, 27907);

long framesTotal += frm0 / 2; // automatic type casting
MessageBox.Show(frm0);	// cast into the string type without call of the ToString()
```

Then variable updates its values automatically. You can subscribe on value changing

```C#
main_count.ValueChanged +=
	delegate
	{
		counterStatusLabel.Text = main_count;
    };
```

or

```C#
main_count.ValueChanged +=
	delegate (object src, Var v)
	{
		ushort val = (ushort)v.GetValue();
		framesTotal += val / 2;
		counterStatusLabel.Text = val.ToString();
	};
```


## Writting to the PLC

Use "RemoteValue" propertie to write a new value to the PLC runtime.

```C#
main_count.RemoteValue = 123;
```


## WinForms data bind

```C#
Var<short> main_count = plc.Var<short>("MAIN.count");

Binding b = new Binding("Text", main_count, "RemoteValue");
b.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
b.DataSourceUpdateMode = DataSourceUpdateMode.Never;

label1.DataBindings.Add(b);
```

With using of the binding format converter

```C#
Var<short> main_count = plc.Var<short>("MAIN.count");

Binding b2 = new Binding("ForeColor", main_count, "RemoteValue");
b2.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
b2.DataSourceUpdateMode = DataSourceUpdateMode.Never;
b2.Format += (s, ea) =>
{
	ea.Value =  (short)ea.Value < 0 ? Color.Blue : Color.Red;
};
label1.DataBindings.Add(b2);
```


## WPF bindings
In WPF you must use properties instead of variables.

```C#
PLC plc;
public Var<ushort> frm0 { get; set; }

private void Window_Loaded(object sender, RoutedEventArgs e)
{
	plc = new PLC("5.2.100.109.1.1");
	frm0 = plc.Var<ushort>("Inputs.Frm0InputToggle", Port: 27907);
	
	DataContext = this;
}
```

```XAML
<Grid>
	<Label x:Name="label" Content="{Binding frm0.RemoteValue}" />
</Grid>
```


## Binding by attributes

You can create a class with several variables which marked as remote PLC variables. Remember, all of variables must declare a type. Otherwise you'll get NULL.

Public fields only.

```C#
public class PRG_Main
{
	[LinkedTo("MAIN.count", As: typeof(short), Port: (int)AmsPort3.PlcRuntime1)]
	public Var count;

	[LinkedTo("MAIN.state", Port: (int)AmsPort3.PlcRuntime1)]
	public Var<ushort> state;
	
	[LinkedTo("Inputs.Frm0InputToggle", Port: 27907)]
	public Var<ushort> frm0_1;

	[LinkedTo(IGrp: 0xF030, IOffs: 0x5F4, Port: 27907)]
	public Var<ushort> frm0;
}
```

or more concisely for the PLC's Runtime #1

```C#
public class PRG_Main
{
	[LinkedTo("MAIN.count")]
	public Var<short> count;

	[LinkedTo("MAIN.state")]
	public Var<ushort> state;
}
```

In WPF-project you should use properties

```C#
public class PRG_Main
{
	[LinkedTo("MAIN.count")]
	public Var<short> count { get; set; }

	[LinkedTo("MAIN.state")]
	public Var<ushort> state { get; set; }
}
```

After that, you should initialize class instance.

With default constructor:

```C#
PRG_Main Main = plc.Class<PRG_Main>();
```

or if you want to use complex constructor

```C#
Main = new PRG_Main(param1, param2, ...);
plc.Class(Main);
```
