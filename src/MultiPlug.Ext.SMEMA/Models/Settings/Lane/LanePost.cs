
namespace MultiPlug.Ext.SMEMA.Models.Settings.Lane
{
    public class LanePost
    {
        public string Guid { get; set; }
        public string LaneId { get; set; }
        public string MachineId { get; set; }
        public int LoggingLevel { get; set; }
        public bool RightToLeft { get; set; }
        public string[] NewBeaconTowerEventId { get; set; }
        public string[] NewBeaconTowerEventDescription { get; set; }
    }
}
