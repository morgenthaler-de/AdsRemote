using AdsRemote.Common;
using System;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;

namespace AdsRemote
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
                    Device.AdsClient.WriteAny(IndexGroup, IndexOffset, internalValue); // TODO to refactor
                }
                catch { }

                if (Device.UiContext != null)
                    Device.UiContext.Send((o) => OnValueChanged(), null);
                else
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
            if (Device.UiContext != null)
                Device.UiContext.Send((o) => OnValueChanged(), null);
            else
                OnValueChanged();
        }

        internal Var(uint iGroup, uint iOffset, AdsDevice adsDevice)
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
            IndexGroup = 0;
            IndexOffset = 0;
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

        /// <summary>
        /// Trying to unsubscribe from the update events
        /// </summary>
        /// <returns>true - if unsubscribed</returns>
        internal override bool TryUnsubscribe()
        {
            if (!Device.Ready)
                return false;

            try
            {
                if (NotifyHandle > 0)
                    Device.AdsClient.DeleteDeviceNotification(NotifyHandle);
            }
            catch
            {
                NotifyHandle = 0;
            }

            return NotifyHandle == 0;
        }

        /// <summary>
        /// Trying to subscribe to the update events
        /// </summary>
        /// <returns>true - if subscribed</returns>
        internal override bool TrySubscribe()
        {
            if (!Device.Ready)
                return false;

            try
            {
                if (IndexGroup == 0 && IndexOffset == 0)
                    try
                    {
                        IAdsSymbol sym = Device.AdsClient.ReadSymbol(RemoteName);
                        IndexGroup = sym.IndexGroup;
                        IndexOffset = sym.IndexOffset;
                    }
                    catch { }

                if (IndexGroup > 0 && IndexOffset > 0)
                    NotifyHandle =
                        Device.AdsClient.AddDeviceNotificationEx(
                            IndexGroup, IndexOffset,
                            NotificationSettings.Default,
                            this,
                            typeof(T));
                else
                    NotifyHandle =
                       Device.AdsClient.AddDeviceNotificationEx(
                           RemoteName,
                           NotificationSettings.Default,
                           this,
                           typeof(T));
            }
            catch
            {
                NotifyHandle = 0;
            }

            return NotifyHandle > 0;
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
    } // class
}
