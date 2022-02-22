
namespace MultiPlug.Ext.SMEMA.Models.Settings.BoardAvailable
{
    public class BoardAvailableUpdateModel
    {
        public string Guid { get; set; }

        public string SMEMAMachineReadyEventId { get; set; }
        public string SMEMAMachineReadyEventDescription { get; set; }
        public string SMEMABoardAvailableSubscriptionId { get; set; }
        public string SMEMAFailedBoardAvailableSubscriptionId { get; set; }
    }
}
