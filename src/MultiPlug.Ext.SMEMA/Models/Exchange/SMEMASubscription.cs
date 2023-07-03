using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Exchange
{
    public class SMEMASubscription : Base.Exchange.Subscription
    {
        [DataMember]
        public string Value { get; set; } = "1";

        public static bool Merge(SMEMASubscription theMerged, SMEMASubscription theMergeFrom)
        {
            if (theMergeFrom.Value != null && theMergeFrom.Value != theMerged.Value)
            {
                theMerged.Value = theMergeFrom.Value;
            }

            return Base.Exchange.Subscription.Merge(theMerged, theMergeFrom);
        }
    }
}
