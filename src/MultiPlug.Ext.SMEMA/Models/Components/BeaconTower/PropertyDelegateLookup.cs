using System;

namespace MultiPlug.Ext.SMEMA.Models.Components.BeaconTower
{
    class PropertyDelegateLookup
    {
        public Func<bool> FunctionDelegate { get; set; }
        public string Description { get; set; }
    }
}
