
using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Exchange
{
    public class Event : Base.Exchange.Event
    {
        [DataMember]
        public string BlockedValue { get; set; } = "true";
        [DataMember]
        public string UnblockedValue { get; set; } = "false";
        [DataMember]
        public bool BlockedEnabled { get; set; } = true;
        [DataMember]
        public bool UnblockedEnabled { get; set; } = true;
    }
}
