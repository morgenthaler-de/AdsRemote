using AdsRemote.Router;
using System;
using AdsRemote.Common;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.Collections.Generic;
using System.Text;

namespace AdsRemote
{
    public class AmsRouter
    {
        public readonly AmsNetId AmsNetId;

        public AmsRouter(AmsNetId amsNetId)
        {
            AmsNetId = amsNetId;
        }

        /// <summary>
        /// Add new record to the AMS router.
        /// </summary>
        /// <param name="localhost"></param>
        /// <param name="remoteHost"></param>
        /// <param name="localAmsNetId"></param>
        /// <param name="name">The name of a new record</param>
        /// <param name="isTemporaryRoute"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="localIpName">IP or machine name</param>
        /// <param name="timeout"></param>
        /// <param name="adsUdpPort"></param>
        /// <returns>True - if route added, False - otherwise</returns>
        public static async Task<bool> AddRecordAsync(
            IPAddress localhost,
            IPAddress remoteHost,
            AmsNetId localAmsNetId,
            string localIpName = null,
            string name = null,
            string login = "Administrator",
            string password = "1",
            bool isTemporaryRoute = false,
            int timeout = 10000,
            int adsUdpPort = Request.DEFAULT_UDP_PORT)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = Environment.MachineName;
            if (string.IsNullOrWhiteSpace(localIpName))
                localIpName = Environment.MachineName;
            if (string.IsNullOrWhiteSpace(login))
                login = Environment.UserName;

            byte[] Segment_AMSNETID = localAmsNetId.ToBytes();

            byte[] Segment_ROUTENAME = name.GetAdsBytes();
            byte[] Segment_ROUTENAME_LENGTH = Segment.ROUTENAME_L;
            Segment_ROUTENAME_LENGTH[2] = (byte)Segment_ROUTENAME.Length;

            byte[] Segment_USERNAME = login.GetAdsBytes();
            byte[] Segment_USERNAME_LENGTH = Segment.USERNAME_L;
            Segment_USERNAME_LENGTH[2] = (byte)Segment_USERNAME.Length;

            byte[] Segment_PASSWORD = password.GetAdsBytes();
            byte[] Segment_PASSWORD_LENGTH = Segment.PASSWORD_L;
            Segment_PASSWORD_LENGTH[2] = (byte)Segment_PASSWORD.Length;

            byte[] Segment_IPADDRESS = localIpName.GetAdsBytes();
            byte[] Segment_IPADDRESS_LENGTH = Segment.LOCALHOST_L;
            Segment_IPADDRESS_LENGTH[2] = (byte)Segment_IPADDRESS.Length;


            request.Add(Segment.HEADER);
            request.Add(Segment.END);
            request.Add(Segment.REQUEST_ADDROUTE);
            request.Add(Segment_AMSNETID);
            request.Add(Segment.PORT);
            request.Add(isTemporaryRoute ?
                        Segment.ROUTETYPE_TEMP :
                        Segment.ROUTETYPE_STATIC);
            request.Add(Segment_ROUTENAME_LENGTH);
            request.Add(Segment_ROUTENAME);
            request.Add(Segment.AMSNETID_L);
            request.Add(Segment_AMSNETID);
            request.Add(Segment.USERNAME_L);
            request.Add(Segment_USERNAME);
            request.Add(Segment.PASSWORD_L);
            request.Add(Segment_PASSWORD);
            request.Add(Segment.LOCALHOST_L);
            request.Add(Segment_IPADDRESS);

            if (isTemporaryRoute)
                request.Add(
                        Segment.TEMPROUTE_TAIL);

            IPEndPoint endpoint = new IPEndPoint(remoteHost, adsUdpPort);
            Response response = await request.SendAsync(endpoint);
            ResponseResult rr = await response.ReceiveAsync();
            bool isAck = ParseAddRecordResponse(rr);

            return isAck;
        }

        /// <summary>
        /// Parses response early recieved by AdsFinder
        /// </summary>
        /// <param name="rr"></param>
        /// <returns>True - if route added, False - otherwise</returns>
        private static bool ParseAddRecordResponse(ResponseResult rr)
        {
            if (rr == null)
                return false;

            if (!rr.Buffer.Take(4).ToArray().SequenceEqual(Segment.HEADER))
                return false;
            if (!rr.Buffer.Skip(4).Take(Segment.END.Length).ToArray().SequenceEqual(Segment.END))
                return false;
            if (!rr.Buffer.Skip(8).Take(Segment.RESPONSE_DISCOVER.Length).ToArray().SequenceEqual(Segment.RESPONSE_ADDROUTE))
                return false;

            rr.Shift =
                Segment.HEADER.Length +
                Segment.END.Length +
                Segment.RESPONSE_ADDROUTE.Length +
                Segment.AMSNETID.Length +
                Segment.PORT.Length +
                Segment.END.Length +
                Segment.END.Length;

            byte[] ack = rr.NextChunk(Segment.L_ROUTEACK);

            return (ack[0] == 0) && (ack[1] == 0);
        }

        #region Device Finder
        public static async Task<List<RemotePlcInfo>> BroadcastSearchAsync(IPAddress localhost, int timeout = 10000, int adsUdpPort = Request.DEFAULT_UDP_PORT)
        {
            Request request = CreateSearchRequest(localhost, timeout);

            IPEndPoint broadcast =
                new IPEndPoint(
                    IPHelper.GetBroadcastAddress(localhost),
                    adsUdpPort);

            Response response = await request.SendAsync(broadcast);
            List<ResponseResult> responses = await response.ReceiveMultipleAsync();

            List<RemotePlcInfo> devices = new List<RemotePlcInfo>();
            foreach (var r in responses)
            {
                RemotePlcInfo device = ParseBroadcastSearchResponse(r);
                devices.Add(device);
            }

            return devices;
        }

        public static async Task<RemotePlcInfo> GetRemotePlcInfoAsync(IPAddress localhost, IPAddress remoteHost, int timeout = 10000, int adsUdpPort = Request.DEFAULT_UDP_PORT)
        {
            Request request = CreateSearchRequest(localhost, timeout);

            IPEndPoint broadcast = new IPEndPoint(remoteHost, adsUdpPort);

            Response response = await request.SendAsync(broadcast);
            ResponseResult rr = await response.ReceiveAsync();

            return device;
        }

        private static Request CreateSearchRequest(IPAddress localhost, int timeout = 10000)
        {
            Request request = new Request(timeout);

            byte[] Segment_AMSNETID = Segment.AMSNETID;
            localhost.GetAddressBytes().CopyTo(Segment_AMSNETID, 0);

            request.Add(Segment.HEADER);
            request.Add(Segment.END);
            request.Add(Segment.REQUEST_DISCOVER);
            request.Add(Segment_AMSNETID);
            request.Add(Segment.PORT);
            request.Add(Segment.END);

            return request;
        }

        private static RemotePlcInfo ParseBroadcastSearchResponse(ResponseResult rr)
        {
            RemotePlcInfo device = new RemotePlcInfo();

            device.Address = rr.RemoteHost;

            if (!rr.Buffer.Take(4).ToArray().SequenceEqual(Segment.HEADER))
                return device;
            if (!rr.Buffer.Skip(4).Take(Segment.END.Length).ToArray().SequenceEqual(Segment.END))
                return device;
            if (!rr.Buffer.Skip(8).Take(Segment.RESPONSE_DISCOVER.Length).ToArray().SequenceEqual(Segment.RESPONSE_DISCOVER))
                return device;

            rr.Shift = Segment.HEADER.Length + Segment.END.Length + Segment.RESPONSE_DISCOVER.Length;

            // AmsNetId
            // then skip 2 bytes of PORT + 4 bytes of ROUTE_TYPE
            byte[] amsNetId = rr.NextChunk(Segment.AMSNETID.Length, add: Segment.PORT.Length + Segment.ROUTETYPE_STATIC.Length);
            device.AmsNetId = new AmsNetId(amsNetId);

            // PLC NameLength
            byte[] bNameLen = rr.NextChunk(Segment.L_NAMELENGTH);
            int nameLen =
                bNameLen[0] == 5 && bNameLen[1] == 0 ?
                    bNameLen[2] + bNameLen[3] * 256 :
                    0;

            byte[] bName = rr.NextChunk(nameLen - 1, add: 1);
            device.Name = System.Text.ASCIIEncoding.Default.GetString(bName);

            // TCat type
            byte[] tcatType = rr.NextChunk(Segment.TCATTYPE_RUNTIME.Length);
            if (tcatType[0] == Segment.TCATTYPE_RUNTIME[0])
                if (tcatType[2] == Segment.TCATTYPE_RUNTIME[2])
                    device.IsRuntime = true;

            // OS version
            byte[] osVer = rr.NextChunk(Segment.L_OSVERSION);
            ushort osKey = (ushort)(osVer[0] * 256 + osVer[4]);
            device.OsVersion = OS_IDS.ContainsKey(osKey) ? OS_IDS[osKey] : osKey.ToString("X2");
            //??? device.OS += " build " + (osVer[8] + osVer[9] * 256).ToString();

            bool isUnicode = false;

            // looking for packet with tcat version;
            // usually it is in the end of the packet
            byte[] tail = rr.NextChunk(rr.Buffer.Length - rr.Shift, true);

            int ci = tail.Length - 4;
            for (int i = ci; i > 0; i -= 4)
                if (tail[i + 0] == 3 &&
                    tail[i + 2] == 4)
                {
                    isUnicode = tail[i + 4] > 2; // Tc3 uses unicode

                    device.TcVersion.Version = tail[i + 4];
                    device.TcVersion.Revision = tail[i + 5];
                    device.TcVersion.Build = tail[i + 6] + tail[i + 7] * 256;
                    break;
                }

            // Comment
            byte[] descMarker = rr.NextChunk(Segment.L_DESCRIPTIONMARKER);
            int len = 0;
            int c = rr.Buffer.Length;
            if (descMarker[0] == 2)
            {
                if (isUnicode)
                    for (int i = 0; i < c; i += 2)
                    {
                        if (rr.Buffer[rr.Shift + i] == 0 &&
                            rr.Buffer[rr.Shift + i + 1] == 0)
                            break;
                        len += 2;
                    }
                else
                    for (int i = 0; i < c; i++)
                    {
                        if (rr.Buffer[rr.Shift + i] == 0)
                            break;
                        len++;
                    }

                if (len > 0)
                {
                    byte[] description = rr.NextChunk(len);

                    if (!isUnicode)
                        device.Comment = ASCIIEncoding.Default.GetString(description);
                    else
                    {
                        byte[] asciiBytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, description);
                        char[] asciiChars = new char[Encoding.ASCII.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                        Encoding.ASCII.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                        device.Comment = new string(asciiChars);
                    }
                }
            } // if (descMarker[0] == 2)

            return device;
        }

        public static readonly Dictionary<ushort, string> OS_IDS =
            new Dictionary<ushort, string>
            {
                {0x0700, "Windows CE 7"},
                {0x0602, "Windows 8/8.1/10"},
                {0x0601, "Windows 7 Embedded Standart"},
                {0x0600, "Windows CE 6"},
                {0x0500, "Windows CE 5"},
                {0x0501, "Windows XP"}
            };
        #endregion
    }
}
