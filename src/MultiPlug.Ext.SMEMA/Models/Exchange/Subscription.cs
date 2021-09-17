using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Exchange
{
    public class Subscription : Base.Exchange.Subscription
    {
        [DataMember]
        public string Value { get; set; } = "1";
    }
}
