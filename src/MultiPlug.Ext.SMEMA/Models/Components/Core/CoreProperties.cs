using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Components.Lane;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Components.Core
{
    public class CoreProperties : MultiPlugBase
    {
        [DataMember]
        public Event GlobalStatusEvent { get; set; }

        [DataMember]
        public LaneComponent[] Lanes { get; set; } = new LaneComponent[0];
    }
}
