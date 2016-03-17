using Ads.Remote.Common;
using System;
using TwinCAT.Ads;

namespace Ads.Remote
{
    public class Var<T> : Var
    {
        internal T internalValue = default(T);
        public virtual T RemoteValue
        {
            get { return internalValue; }
            set
            {
                internalValue = value;
                try {
                    Device.AdsClient.WriteAny(IndexGroup, IndexOffset, internalValue);
                }
                catch { }
                OnValueChanged();
            }
        }

        /// <summary>
        /// Sets internal value without writting to the PLC
        /// </summary>
        /// <param name="value"></param>
        internal override void SetInternalValue(object value)
        {
            internalValue = (T)value;
            OnValueChanged();
        }

        internal Var(long iGroup, long iOffset, AdsDevice adsDevice)
        {
            Device = adsDevice;

            RemoteName = null;
            IndexGroup = iGroup;
            IndexOffset = iOffset;
        }

        internal Var(string name, AdsDevice adsDevice)
        {
            Device = adsDevice;
            RemoteName = name;
            IndexGroup = -1;
            IndexOffset = -1;
        }

        #region Events
        public override event EventHandler<Var> ValueChanged;

        protected override void OnValueChanged()
        {
            EventHandler<Var> eh = ValueChanged;
            eh?.Invoke(this, this);

            OnPropertyChanged("RemoteValue");
        }
        #endregion

        internal override bool TryUnsubscribe()
        {
            try
            {
                if (NotifyHandle > -1)
                {
                    Device.AdsClient.DeleteDeviceNotification(NotifyHandle);
                }
            }
            catch
            {
                NotifyHandle = -1;
            }

            return NotifyHandle == -1;
        }

        internal override bool TrySubscribe()
        {
            try
            {
                if (IndexGroup == -1 && IndexOffset == -1)
                {
                    try
                    {
                        ITcAdsSymbol sym = Device.AdsClient.ReadSymbolInfo(RemoteName);
                        IndexGroup = sym.IndexGroup;
                        IndexOffset = sym.IndexOffset;
                    }
                    catch { }
                }

                if (IndexGroup > -1 && IndexOffset > -1)
                {
                    NotifyHandle =
                        Device.AdsClient.AddDeviceNotificationEx(
                            IndexGroup, IndexOffset,
                            AdsTransMode.OnChange, 0, 0,
                            this,
                            typeof(T)
                            );
                }
                else
                {
                    NotifyHandle =
                        Device.AdsClient.AddDeviceNotificationEx(
                            RemoteName,
                            AdsTransMode.OnChange, 0, 0,
                            this,
                            typeof(T)
                            );
                }
            }
            catch
            {
                NotifyHandle = -1;
            }

            return NotifyHandle > -1;
        }

        public static implicit operator T(Var<T> var)
        {
            return var.internalValue;
        }

        public static implicit operator string(Var<T> var)
        {
            return var.ToString();
        }

        public override string ToString()
        {
            return internalValue.ToString();
        }

        public override object GetValue()
        {
            return internalValue;
        }

        public override Type ValueType { get { return typeof(T); } }
    }
}
