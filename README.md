High-level interface for Beckhoff's TwinCAT.Ads API library that might save a lot of development time. You don't need network threads or handles. Just declare a C# variable and bind it via the variable attribute to the PLC var. That's all.

> Project is not affiliated with Beckhoff.

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

- TwinCAT 2 or 3 have to be installed.
- Use NuGet for full automation or download library from the [release](https://github.com/nikvoronin/AdsRemote/releases) section then add project reference.

Demo
===============

## Broadcast searching

![CxFinder 1.1](https://user-images.githubusercontent.com/11328666/27694544-0857bc9e-5cf5-11e7-9417-97bb868c71dc.png)
 
## Remote variables

![operator-pi](https://user-images.githubusercontent.com/11328666/27694600-3524346e-5cf5-11e7-9d7b-a4dfc6a1d9d5.png)


How To...
===============

> There is an unstable `develop` branch is selected by default. If you want to use this library in production see stable `master` branch or download from `release` section or use NuGet package `AdsRemote`.

## PLC instance

First you have to create an instance of PLC object. This one wiil be like a factory that produces linked variables.

```C#
PLC plc = new PLC("5.2.100.109.1.1");
```

## When device connected or disconnected

```C#
plc.DeviceReady += Plc_DeviceReady;
plc.DeviceLost += Plc_DeviceLost;

[...]

private void Plc_DeviceReady(object sender, AdsDevice e)
{
    Log("READY [" + e.Address.Port.ToString() + "]");
}
```

## How to create and link variables

Create a copy of your PLC's variable then use it like an ordinary variable
We use PLC object that produces linked variables. After that variables will autoupdating their state and value.

```C#
Var<short>  main_count = plc.Var<short> ("MAIN.count");
Var<ushort> main_state = plc.Var<ushort>("MAIN.state");
Var<short>  g_Version  = plc.Var<ushort>(".VERSION");

Var<ushort> frm0       = plc.Var<ushort>("Inputs.Frm0InputToggle", 27907);
Var<ushort> devState   = plc.Var<ushort>(0xF030, 0x5FE, 27907);

long framesTotal += frm0 / 2; // automatic type casting
MessageBox.Show(frm0);        // cast into the string type without call of the ToString()
```

From now you can subscribe on value changing.

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


## Write-back to the PLC

Use "RemoteValue" propertie to write a new value to the PLC runtime.

```C#
main_count.RemoteValue = 123;
```

## WinForms data binding

For example we will bind `Text` propertie of the `Label` control with default name `label1`. At the PLC side we have `MAIN.count` variable that contains value of counter that we should show. 

```C#
Var<short> main_count = plc.Var<short>("MAIN.count");

Binding b = new Binding("Text", main_count, "RemoteValue");
b.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
b.DataSourceUpdateMode = DataSourceUpdateMode.Never;

label1.DataBindings.Add(b);
```

If we have to convert given value we define a format converter

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

## WPF data bindings

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

And explicitly specifying the field `.RemoteValue` of the remote variable

```XAML
<Grid>
    <Label x:Name="label" Content="{Binding frm0.RemoteValue}" />
</Grid>
```


## Create variables with help of attributes

You can create special class with several variables then mark those ones as remote PLC variables. Remember, all variables must declare a type. Otherwise you'll get NULL.

> Public fields only!

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

Again in WPF-project you should use properties

```C#
public class PRG_Main
{
    [LinkedTo("MAIN.count")]
    public Var<short> count { get; set; }

    [LinkedTo("MAIN.state")]
    public Var<ushort> state { get; set; }
}
```

It's time to create instance of our class.

If you don't need of special class constructor just write:

```C#
PRG_Main Main = plc.Class<PRG_Main>();
```

otherwise for cunstructor with parameter list or something else we use it in this maner

```C#
Main = new PRG_Main(param1, param2, ...);
plc.Class(Main);
```
