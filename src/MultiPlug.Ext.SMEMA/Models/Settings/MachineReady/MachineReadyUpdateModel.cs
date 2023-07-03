
namespace MultiPlug.Ext.SMEMA.Models.Settings.MachineReady
{
    public class MachineReadyUpdateModel
    {
        public string Guid { get; set; }

        public string SMEMAMachineReadySubscriptionId { get; set; }
        public string SMEMAMachineReadySubscriptionReadyValue { get; set; }
        public bool SMEMAMachineReadyAlways { get; set; }

        public string SMEMABoardAvailableEventId { get; set; }
        public string SMEMABoardAvailableEventDescription { get; set; }
        public string SMEMABoardAvailableEventSubject { get; set; }
        public string SMEMABoardAvailableEventHighValue { get; set; }
        public string SMEMABoardAvailableEventLowValue { get; set; }

        public string SMEMAFailedBoardAvailableEventId { get; set; }
        public string SMEMAFailedBoardAvailableEventDescription { get; set; }
        public string SMEMAFailedBoardAvailableEventSubject { get; set; }
        public string SMEMAFailedBoardAvailableEventHighValue { get; set; }
        public string SMEMAFailedBoardAvailableEventLowValue { get; set; }

        public string SMEMAFlipBoardEventId { get; set; }
        public string SMEMAFlipBoardEventDescription { get; set; }
        public string SMEMAFlipBoardEventSubject { get; set; }
        public string SMEMAFlipBoardEventHighValue { get; set; }
        public string SMEMAFlipBoardEventLowValue { get; set; }

    }
}
