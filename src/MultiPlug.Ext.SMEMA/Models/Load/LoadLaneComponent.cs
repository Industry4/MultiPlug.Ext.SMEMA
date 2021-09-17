
using System.Runtime.Serialization;
using MultiPlug.Ext.SMEMA.Models.Components.Downstream;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;
using MultiPlug.Ext.SMEMA.Models.Components.Upstream;

namespace MultiPlug.Ext.SMEMA.Models.Load
{
    public class LoadLaneComponent
    {
        [DataMember]
        public string Guid { get; set; }
        [DataMember]
        public string LaneId { get; set; }
        [DataMember]
        public string MachineId { get; set; }
        [DataMember]
        public UpstreamProperties Upstream { get; set; }
        [DataMember]
        public DownstreamProperties Downstream { get; set; }
        [DataMember]
        public InterlockProperties Interlock { get; set; }
    }
}
