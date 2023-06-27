
namespace MultiPlug.Ext.SMEMA.Models.Settings.BoardAvailable
{
    public class BoardAvailableUpdateModel
    {
        public string Guid { get; set; }

        public string SMEMAMachineReadyEventId { get; set; }
        public string SMEMAMachineReadyEventDescription { get; set; }
        public string SMEMAMachineReadyEventSubject { get; set; }
        public string SMEMABoardAvailableSubscriptionId { get; set; }
        public string SMEMABoardAvailableSubscriptionAvailableValue { get; set; }
        public bool SMEMABoardAvailableAlways { get; set; }
        public string SMEMAFailedBoardAvailableSubscriptionId { get; set; }
        public string SMEMAFailedBoardAvailableSubscriptionAvailableValue { get; set; }
        public bool SMEMAFailedBoardAvailableAlways { get; set; }
    }
}
