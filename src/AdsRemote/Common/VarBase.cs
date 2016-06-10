using System;
using System.ComponentModel;

namespace Ads.Remote.Common
{
    public abstract class Var : IDisposable, INotifyPropertyChanged
    {
        public string Name { get { return RemoteName; } }
        protected int NotifyHandle = -1;

        internal AdsDevice Device;
        internal string RemoteName = null;
        internal long IndexGroup = -1;
        internal long IndexOffset = -1;

        abstract internal bool TrySubscribe();
        abstract internal bool TryUnsubscribe();
        abstract internal void SetInternalValue(object value);
        abstract public object GetValue();
        abstract public Type ValueType { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        abstract public event EventHandler<Var> ValueChanged;
        abstract protected void OnValueChanged();

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            pceh?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                TryUnsubscribe();
        }
    }
}
