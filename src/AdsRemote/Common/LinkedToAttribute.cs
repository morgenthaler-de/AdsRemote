using System;

namespace AdsRemote.Common
{
    public class LinkedToAttribute : Attribute
    {
        public readonly string To;
        public readonly long IGrp;
        public readonly long IOffs;
        public readonly int Port;
        public readonly Type As;

        public LinkedToAttribute(string Variable, Type As = null, int Port = (int)AmsPort3.PlcRuntime1)
        {
            this.To = Variable;
            this.IGrp = -1;
            this.IOffs = -1;
            this.Port = Port;
            this.As = As;
        }

        public LinkedToAttribute(long IGrp, long IOffs, Type As = null, int Port = (int)AmsPort3.PlcRuntime1)
        {
            this.To = null;
            this.IGrp = IGrp;
            this.IOffs = IOffs;
            this.Port = Port;
            this.As = As;
        }
    }
}