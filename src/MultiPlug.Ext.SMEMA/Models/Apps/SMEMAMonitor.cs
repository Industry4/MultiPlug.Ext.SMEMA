using System;

namespace MultiPlug.Ext.SMEMA.Models.Apps
{
    [Serializable]
    public class SMEMAMonitor
    {
        public string LaneGuid { get; internal set; }
        public string LaneName { get; internal set; }
        public LaneUrl[] Lanes { get; internal set; }
        public string MachineName { get; internal set; }

        public ComponentStates SMEMABoardAvailableStates { get; internal set; }
        public ComponentStates SMEMAInterlockStates { get; internal set; }
        public ComponentStates SMEMAInterlockLatchedStates { get; internal set; }
        public ComponentStates SMEMAMachineReadyStates { get; internal set; }

        public ComponentEventIDs SMEMABoardAvailableEventIds { get; internal set; }
        public ComponentEventIDs SMEMAInterlockEventIds { get; internal set; }
        public ComponentEventIDs SMEMAMachineReadyEventIds { get; internal set; }
    }

    [Serializable]
    public class ComponentStates
    {
        public bool MachineReady { get; set; }
        public bool GoodBoard { get; set; }
        public bool BadBoard { get; set; }
    }

    [Serializable]
    public class ComponentEventIDs
    {
        public string MachineReady { get; set; }
        public string GoodBoard { get; set; }
        public string BadBoard { get; set; }
    }
}
