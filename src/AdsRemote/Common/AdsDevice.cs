using System.Collections.Generic;
using System.Threading;
using TwinCAT.Ads;

namespace AdsRemote.Common
{
    public class AdsDevice
    {
        public readonly AmsAddress Address;
        public readonly AdsClient AdsClient;
        internal List<Var> Vars = new List<Var>();
        internal SynchronizationContext UiContext;  // TODO to refactor

        public bool Ready { get; internal set; }

        internal AdsDevice(AmsNetId amsNetId, int port)
        {
            Address = new AmsAddress(amsNetId, port);
            AdsClient = new AdsClient();
            AdsClient.Connect(Address);
        }

        internal void SetActive(bool isActive)
        {
            Ready = isActive;
        }
    } // class
}
