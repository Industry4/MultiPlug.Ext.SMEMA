
namespace MultiPlug.Ext.SMEMA.Models.Settings.MachineReady
{
    public class MachineReadyUpdateModel
    {
        public string Guid { get; set; }

        public string SMEMAMachineReadySubscriptionId { get; set; }
        public string SMEMAMachineReadySubscriptionReadyValue { get; set; }

        public string SMEMABoardAvailableEventId { get; set; }
        public string SMEMABoardAvailableEventDescription { get; set; }
        public string SMEMABoardAvailableEventSubject { get; set; }

        public string SMEMAFailedBoardAvailableEventId { get; set; }
        public string SMEMAFailedBoardAvailableEventDescription { get; set; }
        public string SMEMAFailedBoardAvailableEventSubject { get; set; }
    }
}
