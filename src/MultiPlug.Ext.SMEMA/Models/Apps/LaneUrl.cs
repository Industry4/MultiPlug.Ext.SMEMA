using System;

namespace MultiPlug.Ext.SMEMA.Models.Apps
{
    [Serializable]
    public class LaneUrl
    {
        public string Guid { get; internal set; }
        public string LaneName { get; internal set; }
        public string MachineName { get; internal set; }
    }
}
