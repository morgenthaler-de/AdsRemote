using Ads.Remote.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using TwinCAT.Ads;

namespace Ads.Remote
{
    public class PLC : IDisposable, INotifyPropertyChanged
    {
        public readonly AmsRouter Router;

        private Dictionary<int, AdsDevice> dict_PortDevice = new Dictionary<int, AdsDevice>();
        private readonly object _locker_dict_PortDevice = new object();
        private Dictionary<string, Var> dict_NameVar = new Dictionary<string, Var>();

        Thread pingThread;
        CancellationTokenSource cancelTokenSource;

        #region Default Runtimes
        public AdsDevice Runtime1
        {
            get
            {
                AdsDevice d = null;
                dict_PortDevice.TryGetValue((int)AmsPort3.PlcRuntime1, out d);
                return d;
            }
        }
        public AdsDevice Runtime2
        {
            get
            {
                AdsDevice d = null;
                dict_PortDevice.TryGetValue((int)AmsPort3.PlcRuntime2, out d);
                return d;
            }
        }
        public AdsDevice Runtime3
        {
            get
            {
                AdsDevice d = null;
                dict_PortDevice.TryGetValue((int)AmsPort3.PlcRuntime3, out d);
                return d;
            }
        }
        public AdsDevice Runtime4
        {
            get
            {
                AdsDevice d = null;
                dict_PortDevice.TryGetValue((int)AmsPort3.PlcRuntime4, out d);
                return d;
            }
        }
        #endregion

        public int Tune_PingSleepInterval = 0;      // How long ping thread sleeps between iterations
        public int Tune_ReinitInterval = 100;       // Interval after connection but before vars subscription

        private int tune_AdsClientTimeout = 1000;   // I/O operations timeout
        public int Tune_AdsClientTimeout
        {
            get { return tune_AdsClientTimeout; }
            set
            {
                tune_AdsClientTimeout = value;

                List<AdsDevice> devices = new List<AdsDevice>();
                lock (_locker_dict_PortDevice)
                {
                    if (devices.Count != dict_PortDevice.Values.Count)
                    {
                        devices = new List<AdsDevice>(dict_PortDevice.Values);
                    }
                }

                foreach (AdsDevice device in devices)
                {
                    device.AdsClient.Timeout = tune_AdsClientTimeout;
                }
            }
        }

        #region ctor
        public PLC(AmsNetId amsNetId)
        {
            Router = new AmsRouter(amsNetId);
            ctor_initialize();
        }

        public PLC(string amsNetId)
        {
            Router = new AmsRouter(new AmsNetId(amsNetId));
            ctor_initialize();
        }

        private void ctor_initialize()
        {
            cancelTokenSource = new CancellationTokenSource();
            SynchronizationContext uiContext = SynchronizationContext.Current;
            pingThread = new Thread(() => PingThread(cancelTokenSource.Token, uiContext));
            pingThread.IsBackground = true;
            pingThread.Start();
        }
        #endregion

        private void PingThread(CancellationToken token, SynchronizationContext uiContext)
        {
            List<AdsDevice> devices = new List<AdsDevice>();
            List<AdsDevice> updateList = new List<AdsDevice>();
            while (true)
            {
                if (token.IsCancellationRequested)
                    break;

                lock (_locker_dict_PortDevice)
                {
                    if (devices.Count != dict_PortDevice.Values.Count)
                        devices = new List<AdsDevice>(dict_PortDevice.Values);
                }

                foreach (AdsDevice device in devices)
                {
                    if (token.IsCancellationRequested)
                        break;

                    if (device.Vars.Count > 0)
                    {
                        bool isActive = false;
                        try
                        {
                            Var v = device.Vars[0];
                            if(v.IndexGroup > -1 && v.IndexOffset > -1)
                                device.AdsClient.ReadAny(v.IndexGroup, v.IndexOffset, v.ValueType);
                            else
                                device.AdsClient.ReadSymbol(v.Name, v.ValueType, !device.Ready);

                            isActive = true;
                        }
                        catch { }

                        if (isActive && (!device.Ready) || (!isActive) && device.Ready)
                            updateList.Add(device);
                    }

                    if (token.IsCancellationRequested)
                        break;
                } // foreach (AdsDevice device in devices)

                if (!token.IsCancellationRequested)
                    Thread.Sleep(updateList.Count > 0 ? Tune_ReinitInterval : Tune_PingSleepInterval);

                foreach(AdsDevice device in updateList)
                {
                    if (token.IsCancellationRequested)
                        break;

                    if (!device.Ready)
                    {
                        device.SetActive(true);
                        if (uiContext != null)
                            uiContext.Send(OnDeviceReady, device);
                        else
                            OnDeviceReady(device);
                    }
                    else
                    {
                        device.SetActive(false);
                        if (uiContext != null)
                            uiContext.Send(OnDeviceLost, device);
                        else
                            OnDeviceLost(device);
                    }

                    if (token.IsCancellationRequested)
                        break;
                }

                if (!token.IsCancellationRequested)
                    updateList.Clear();
            } // while(true)
        } // private void PingThreadMethod(...

        #region Events
        public event EventHandler<AdsDevice> DeviceReady;
        public event EventHandler<AdsDevice> DeviceLost;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            pceh?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnDeviceLost(object obj_Device)
        {
            AdsDevice device = (AdsDevice)obj_Device;

            foreach (Var v in device.Vars)
                v.TryUnsubscribe();

            EventHandler<AdsDevice> handle = DeviceLost;
            handle?.Invoke(this, device);
        }

        protected virtual void OnDeviceReady(object obj_Device)
        {
            AdsDevice device = (AdsDevice)obj_Device;

            foreach (Var var in device.Vars)
            {
                var.TryUnsubscribe();
                var.TrySubscribe();
            }

            EventHandler<AdsDevice> handle = DeviceReady;
            handle?.Invoke(this, device);
        }

        private void Client_AdsNotificationEx(object sender, AdsNotificationExEventArgs e)
        {
            Var v = (Var)e.UserData;
            v.SetInternalValue(e.Value);
        }
        #endregion

        #region PLC Variables and Class
        private Var<T> CreateVariable<T>(string varName, int Port, long IGrp = -1, long IOffs = -1)
        {
            Var v;
            if (dict_NameVar.TryGetValue(varName, out v))
                return (Var<T>)v;

            Var<T> var;
            AdsDevice device;

            if (!dict_PortDevice.TryGetValue(Port, out device))
            {
                device = new AdsDevice(Router.AmsNetId, Port);
                device.AdsClient.Timeout = tune_AdsClientTimeout;
                device.AdsClient.AdsNotificationEx += Client_AdsNotificationEx;

                lock (_locker_dict_PortDevice)
                    dict_PortDevice.Add(Port, device);
            }

            if (IGrp > -1 && IOffs > -1)
            {
                var = new Var<T>(IGrp, IOffs, device);
                device.Vars.Add(var);
            }
            else
            {
                var = new Var<T>(varName, device);
                device.Vars.Add(var);
            }

            dict_NameVar.Add(varName, var);

            if (device.Ready)
                var.TrySubscribe();

            return var;
        }

        public Var<T> Var<T>(string Variable, int Port)
        {
            return CreateVariable<T>(Variable, Port);
        }

        public Var<T> Var<T>(long IGrp, long IOffs, int Port)
        {
            string pseudoName = string.Concat(IGrp.ToString(), ":", IOffs.ToString());

            return CreateVariable<T>(
                pseudoName,
                Port,
                IGrp, IOffs);
        }

        public Var<T> Var<T>(string Variable)
        {
            return Var<T>(Variable, (int)AmsPort3.PlcRuntime1);
        }

        public Var<T> Var<T>(string Variable, AmsPort3 Port)
        {
            return Var<T>(Variable, (int)Port);
        }

        public T Class<T>(T instance = default(T)) where T : new()
        {
            T o = instance == null ? new T() : instance;

            #region Properties
            PropertyInfo[] properties = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach(PropertyInfo pr in properties)
            {
                LinkedToAttribute la = pr.GetCustomAttribute<LinkedToAttribute>();
                if (la != null)
                {
                    Type t = null;

                    if (pr.PropertyType.IsGenericType)
                        t = pr.PropertyType.GetGenericArguments()[0];
                    else
                        t = la.As;

                    if (t != null)
                    {
                        object v = null;
                        if (la.IGrp > -1 && la.IOffs > -1)
                        {
                            MethodInfo mi = this.GetType().GetMethod("Var", new Type[] { typeof(long), typeof(long), typeof(int) }).MakeGenericMethod(t);
                            v = mi.Invoke(this, new object[] { la.IGrp, la.IOffs, la.Port });
                        }
                        else
                        {
                            MethodInfo mi = this.GetType().GetMethod("Var", new Type[] { typeof(string), typeof(int) }).MakeGenericMethod(t);
                            v = mi.Invoke(this, new object[] { la.To, la.Port });
                        }

                        if (v != null)
                            pr.SetValue(o, v);
                    } // if (t != null)
                } // if (la != null)
            } //foreach(PropertyInfo pr in properties)
            #endregion

            #region Fields
            FieldInfo[] fields = o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo fi in fields)
            {
                LinkedToAttribute la = fi.GetCustomAttribute<LinkedToAttribute>();
                if (la != null)
                {
                    Type t = null;

                    if (fi.FieldType.IsGenericType)
                        t = fi.FieldType.GetGenericArguments()[0];
                    else
                        t = la.As;

                    if (t != null)
                    {
                        object v = null;
                        if (la.IGrp > -1 && la.IOffs > -1)
                        {
                            MethodInfo mi = this.GetType().GetMethod("Var", new Type[] { typeof(long), typeof(long), typeof(int) }).MakeGenericMethod(t);
                            v = mi.Invoke(this, new object[] { la.IGrp, la.IOffs, la.Port });
                        }
                        else
                        {
                            MethodInfo mi = this.GetType().GetMethod("Var", new Type[] { typeof(string), typeof(int) }).MakeGenericMethod(t);
                            v = mi.Invoke(this, new object[] { la.To, la.Port });
                        }

                        if (v != null)
                            fi.SetValue(o, v);
                    }
                } //if (la != null)
            } // foreach (FieldInfo fi in fields)
            #endregion

            return o;
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Var v in dict_NameVar.Values)
                    v.TryUnsubscribe();

                if (cancelTokenSource != null)
                    cancelTokenSource.Cancel();
            }
        }
    }
}