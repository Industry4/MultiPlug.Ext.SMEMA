
namespace MultiPlug.Ext.SMEMA.Models.Settings.BoardAvailable
{
    public class BoardAvailableUpdateModel
    {
        public string Guid { get; set; }

        public string SMEMAMachineReadyEventId { get; set; }
        public string SMEMAMachineReadyEventDescription { get; set; }
        public string SMEMAMachineReadyEventSubject { get; set; }
        public string SMEMAMachineReadyEventHighValue { get; set; }
        public string SMEMAMachineReadyEventLowValue { get; set; }
        public string SMEMABoardAvailableSubscriptionId { get; set; }
        public string SMEMABoardAvailableSubscriptionAvailableValue { get; set; }
        public bool SMEMABoardAvailableAlways { get; set; }
        public string SMEMAFailedBoardAvailableSubscriptionId { get; set; }
        public string SMEMAFailedBoardAvailableSubscriptionAvailableValue { get; set; }
        public bool SMEMAFailedBoardAvailableAlways { get; set; }
        public string SMEMAFlipBoardSubscriptionId { get; set; }
        public string SMEMAFlipBoardSubscriptionFlipValue { get; set; }
        public bool SMEMAFlipBoardAlways { get; set; }
        public int SMEMAMachineReadyEventHighDelay { get; set; }
        public int SMEMAMachineReadyEventLowDelay { get; set; }
    }
}
