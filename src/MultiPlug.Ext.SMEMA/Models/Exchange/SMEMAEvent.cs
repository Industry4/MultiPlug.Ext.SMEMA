using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Exchange
{
    public class SMEMAEvent : Base.Exchange.Event
    {
        [DataMember]
        public string HighValue { get; set; } = "1";
        [DataMember]
        public string LowValue { get; set; } = "0";
        [DataMember]
        public int? HighDelay { get; set; } = 0;
        [DataMember]
        public int? LowDelay { get; set; } = 0;

        public static bool Merge(SMEMAEvent theMerged, SMEMAEvent theMergeFrom)
        {
            if( theMergeFrom.HighValue != null && theMergeFrom.HighValue != theMerged.HighValue )
            {
                theMerged.HighValue = theMergeFrom.HighValue;
            }

            if (theMergeFrom.LowValue != null && theMergeFrom.LowValue != theMerged.LowValue)
            {
                theMerged.LowValue = theMergeFrom.LowValue;
            }

            if(theMergeFrom.HighDelay != null && theMergeFrom.HighDelay != theMerged.HighDelay )
            {
                theMerged.HighDelay = theMergeFrom.HighDelay;
            }

            if (theMergeFrom.LowDelay != null && theMergeFrom.LowDelay != theMerged.LowDelay)
            {
                theMerged.LowDelay = theMergeFrom.LowDelay;
            }

            return Base.Exchange.Event.Merge(theMerged, theMergeFrom);
        }

    }
}
