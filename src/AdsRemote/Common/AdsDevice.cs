using System.Collections.Generic;
using TwinCAT.Ads;

namespace Ads.Remote.Common
{
    public class AdsDevice
    {
        public readonly AmsAddress Address;
        public readonly TcAdsClient AdsClient;
        internal List<Var> Vars = new List<Var>();

        private bool isReady = false;
        public bool Ready { get { return isReady; }}

        internal AdsDevice(AmsNetId amsNetId, int port)
        {
            Address = new AmsAddress(amsNetId, port);
            AdsClient = new TcAdsClient();
            AdsClient.Connect(Address);
        }

        internal void SetActive(bool isActive)
        {
            isReady = isActive;
        }
    } // class
}
