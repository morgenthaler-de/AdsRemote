using AdsRemote;
using AdsRemote.Common;

namespace OperatorPi
{
    public class ViewModel
    {
        [LinkedTo("MAIN.Counter")]
        public Var<ushort> Counter { get; set; }

        [LinkedTo("MAIN.HeatSetpoint")]
        public Var<double> HeatSetpoint { get; set; }

        [LinkedTo("MAIN.CoolSetpoint")]
        public Var<double> CoolSetpoint { get; set; }

        [LinkedTo("MAIN.HeatActual")]
        public Var<double> HeatActual { get; set; }

        [LinkedTo("MAIN.CoolActual")]
        public Var<double> CoolActual { get; set; }

        [LinkedTo("MAIN.Temp")]
        public Var<double> Temp { get; set; }

        [LinkedTo("MAIN.Reset")]
        public Var<bool> Reset { get; set; }
    }
}
