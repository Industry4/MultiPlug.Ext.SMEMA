using System.Runtime.Serialization;
using MultiPlug.Base;
using MultiPlug.Ext.SMEMA.Models.Exchange;

namespace MultiPlug.Ext.SMEMA.Models.Components.BeaconTower
{
    public class BeaconTowerProperties : MultiPlugBase
    {
        [DataMember]
        public BeaconTowerEvent[] BeaconTowerEvents { get; set; }

    }
}
