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
        public bool RightToLeft { get; internal set; }

        public ComponentStates SMEMABoardAvailableStates { get; internal set; }
        public ComponentStates SMEMAInterlockStates { get; internal set; }
        public ComponentStates SMEMAInterlockLatchedStates { get; internal set; }
        public ComponentStates SMEMAMachineReadyStates { get; internal set; }

        public ComponentStates SMEMAInterlockDivertStates { get; internal set; }
        public ComponentStates SMEMAInterlockDivertLatchedStates { get; internal set; }
        public bool? UIEnabled { get; internal set; }
        public bool SMEMABoardAvailableAlways { get; internal set; }
        public bool SMEMAFailedBoardAvailableAlways { get; internal set; }
        public bool SMEMAFlipBoardAlways { get; internal set; }
        public bool SMEMAMachineReadyAlways { get; internal set; }
    }

    [Serializable]
    public class ComponentStates
    {
        public bool MachineReady { get; set; }
        public bool GoodBoard { get; set; }
        public bool BadBoard { get; set; }
        public bool FlipBoard { get; set; }
    }
}
