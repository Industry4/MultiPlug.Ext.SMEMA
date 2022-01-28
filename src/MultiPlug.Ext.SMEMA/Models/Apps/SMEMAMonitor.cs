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
        public bool SMEMABadBoardState { get; internal set; }
        public string SMEMABoardAvailableSubscriptionId { get; internal set; }
        public string SMEMAFailedBoardAvailableSubscriptionId { get; internal set; }
        public bool SMEMAGoodBoardState { get; internal set; }
        public bool SMEMAMachineReadyState { get; internal set; }
        public string SMEMAMachineReadySubscriptionId { get; internal set; }
    }
}
