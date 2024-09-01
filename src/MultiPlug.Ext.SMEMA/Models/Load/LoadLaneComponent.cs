
using System.Runtime.Serialization;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;
using MultiPlug.Ext.SMEMA.Models.Components.BoardAvailable;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;
using MultiPlug.Ext.SMEMA.Models.Components.BeaconTower;

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
        public BoardAvailableProperties BoardAvailable { get; set; }
        [DataMember]
        public MachineReadyProperties MachineReady { get; set; }
        [DataMember]
        public InterlockProperties Interlock { get; set; }
        [DataMember]
        public BeaconTowerProperties BeaconTower { get; set; }
        [DataMember]
        public bool RightToLeft { get; set; }
        [DataMember]
        public int LoggingLevel { get; set; } = 0;
    }
}
