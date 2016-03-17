using System.Text;

namespace Ads.Remote.Common
{
    public static class Extensions
    {
        public static byte[] GetAdsBytes(this string s)
        {
            byte[] bytes = new byte[s.Length + 1];
            Encoding.ASCII.GetBytes(s).CopyTo(bytes, 0);
            bytes[bytes.Length - 1] = 0;

            return bytes;
        }
    }
}
