using System.Runtime.Serialization;
using MultiPlug.Base;
using MultiPlug.Ext.SMEMA.Components.Interlock;
using MultiPlug.Ext.SMEMA.Components.BoardAvailable;
using MultiPlug.Ext.SMEMA.Components.MachineReady;

namespace MultiPlug.Ext.SMEMA.Models.Components.Lane
{
    public class LaneProperties : MultiPlugBase
    {
        [DataMember]
        public BoardAvailableComponent BoardAvailable { get; set; }
        [DataMember]
        public MachineReadyComponent MachineReady { get; set; }
        [DataMember]
        public InterlockComponent Interlock { get; set; }
        [DataMember]
        public string Guid { get; set; }
        [DataMember]
        public string LaneId { get; set; }
        [DataMember]
        public string MachineId { get; set; }

        public string EventSuffix
        {
            get
            {
                return ":Machine:" + MachineId + ":Lane:" + LaneId;
            }
        }
    }
}
