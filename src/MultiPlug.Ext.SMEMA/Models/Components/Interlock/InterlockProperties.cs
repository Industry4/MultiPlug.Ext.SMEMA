using MultiPlug.Base;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Components.Interlock
{
    public class InterlockProperties : MultiPlugBase
    {
        [DataMember]
        public Exchange.Subscription InterlockSubscription { get; set; }
    }
}
