using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Exchange
{
    public class GoodBadInterlockSubscription : Base.Exchange.Subscription
    {
        [DataMember]
        public string Block { get; set; } = string.Empty;
        [DataMember]
        public string Unblock { get; set; } = string.Empty;
        [DataMember]
        public string LatchOn { get; set; } = string.Empty;
        [DataMember]
        public string LatchOff { get; set; } = string.Empty;
        [DataMember]
        public string DivertOn { get; set; } = string.Empty;
        [DataMember]
        public string DivertOff { get; set; } = string.Empty;
        [DataMember]
        public string DivertLatchOn { get; set; } = string.Empty;
        [DataMember]
        public string DivertLatchOff { get; set; } = string.Empty;
    }
}
