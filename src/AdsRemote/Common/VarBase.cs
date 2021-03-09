using System;
using System.ComponentModel;

namespace AdsRemote.Common
{
    public abstract class Var : IDisposable, INotifyPropertyChanged
    {
        public string Name { get { return RemoteName; } }
        protected uint NotifyHandle = 0;

        internal AdsDevice Device;
        internal string RemoteName = null;
        internal uint IndexGroup = 0;
        internal uint IndexOffset = 0;

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
