using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Exchange
{
    public class MachineReadyInterlockSubscription : Base.Exchange.Subscription
    {
        [DataMember]
        public string Block { get; set; } = string.Empty;
        [DataMember]
        public string Unblock { get; set; } = string.Empty;
        [DataMember]
        public string LatchOn { get; set; } = string.Empty;
        [DataMember]
        public string LatchOff { get; set; } = string.Empty;
    }
}
