
namespace MultiPlug.Ext.SMEMA.Models.Settings.Interlock
{
    public class InterlockUpdateModel
    {
        public string Guid { get; set; }

        public string InterlockSubscriptionId { get; set; }
        public string InterlockSubscriptionSubjectIndex { get; set; }
        public string InterlockSubscriptionReadyValue { get; set; }
    }
}
