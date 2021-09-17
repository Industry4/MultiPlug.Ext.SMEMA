
namespace MultiPlug.Ext.SMEMA.Models.Settings.Upstream
{
    public class UpstreamUpdateModel
    {
        public string Guid { get; set; }

        public string SMEMAMachineReadyEventId { get; set; }
        public string SMEMAMachineReadyEventDescription { get; set; }
        public string SMEMAMachineReadyEventSubject { get; set; }

        public string SMEMABoardAvailableSubscriptionId { get; set; }
        public string SMEMABoardAvailableSubscriptionSubjectIndex { get; set; }

        public string SMEMAFailedBoardAvailableSubscriptionId { get; set; }
        public string SMEMAFailedBoardAvailableSubscriptionSubjectIndex { get; set; }
    }
}
