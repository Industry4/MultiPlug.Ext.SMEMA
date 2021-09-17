using MultiPlug.Ext.SMEMA.Components.Downstream;
using MultiPlug.Ext.SMEMA.Components.Interlock;
using MultiPlug.Ext.SMEMA.Components.Upstream;
using MultiPlug.Ext.SMEMA.Models.Components.Lane;

namespace MultiPlug.Ext.SMEMA.Components.Lane
{
    public class LaneComponent : LaneProperties
    {
        public LaneComponent(string theGuid)
        {
            if (theGuid != null)
            {
                Guid = theGuid;
            }

            Upstream = new UpstreamComponent(theGuid, EventSuffix);
            Downstream = new DownstreamComponent(theGuid, EventSuffix);
            Interlock = new InterlockComponent(theGuid, EventSuffix);
        }

        internal void UpdateProperties(LaneProperties theNewProperties)
        {
            MachineId = theNewProperties.MachineId;
            LaneId = theNewProperties.LaneId;
        }

    }
}
