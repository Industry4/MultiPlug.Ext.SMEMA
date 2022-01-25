using MultiPlug.Ext.SMEMA.Components.BoardAvailable;
using MultiPlug.Ext.SMEMA.Components.Interlock;
using MultiPlug.Ext.SMEMA.Components.MachineReady;
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

            BoardAvailable = new BoardAvailableComponent(theGuid, EventSuffix);
            MachineReady = new MachineReadyComponent(theGuid, EventSuffix);
            Interlock = new InterlockComponent(theGuid, EventSuffix);
        }

        internal void UpdateProperties(LaneProperties theNewProperties)
        {
            MachineId = theNewProperties.MachineId;
            LaneId = theNewProperties.LaneId;
        }

    }
}
