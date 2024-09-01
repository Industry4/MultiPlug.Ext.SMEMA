using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Exchange
{
    public class BeaconTowerEventSequenceStep
    {
        [DataMember]
        public int Operator { get; set; }
        [DataMember]
        public int Operand { get; set; }
        [DataMember]
        public int Truth { get; set; }
    }
}
