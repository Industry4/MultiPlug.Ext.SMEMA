using System.Runtime.Serialization;
using System.Threading;

namespace MultiPlug.Ext.SMEMA.Models.Exchange
{
    public class BeaconTowerEvent : Base.Exchange.Event
    {
        [DataMember]
        public string TrueValue { get; set; } = "1";
        [DataMember]
        public string FalseValue { get; set; } = "0";
        [DataMember]
        public int? TrueDelay { get; set; } = 0;
        [DataMember]
        public int? FalseDelay { get; set; } = 0;
        [DataMember]
        public bool? TrueEnabled { get; set; } = true;
        [DataMember]
        public bool? FalseEnabled { get; set; } = true;

        internal bool? LastState;

        internal CancellationTokenSource TrueEventCancellationSource;
        internal CancellationTokenSource FalseEventCancellationSource;

        [DataMember]
        public BeaconTowerEventSequenceStep[] RuleSteps { get; set; } = new BeaconTowerEventSequenceStep[0];

        public static bool Merge(BeaconTowerEvent theMerged, BeaconTowerEvent theMergeFrom)
        {
            if (theMergeFrom.TrueValue != null && theMergeFrom.TrueValue != theMerged.TrueValue)
            {
                theMerged.TrueValue = theMergeFrom.TrueValue;
            }

            if (theMergeFrom.FalseValue != null && theMergeFrom.FalseValue != theMerged.FalseValue)
            {
                theMerged.FalseValue = theMergeFrom.FalseValue;
            }

            if (theMergeFrom.TrueDelay != null && theMergeFrom.TrueDelay != theMerged.TrueDelay && (theMergeFrom.TrueDelay == 0 || theMergeFrom.TrueDelay > 0))
            {
                theMerged.TrueDelay = theMergeFrom.TrueDelay;
            }

            if (theMergeFrom.FalseDelay != null && theMergeFrom.FalseDelay != theMerged.FalseDelay && (theMergeFrom.FalseDelay == 0 || theMergeFrom.FalseDelay > 0))
            {
                theMerged.FalseDelay = theMergeFrom.FalseDelay;
            }

            if (theMergeFrom.TrueEnabled != null && theMergeFrom.TrueEnabled != theMerged.TrueEnabled)
            {
                theMerged.TrueEnabled = theMergeFrom.TrueEnabled;
            }

            if (theMergeFrom.FalseEnabled != null && theMergeFrom.FalseEnabled != theMerged.FalseEnabled)
            {
                theMerged.FalseEnabled = theMergeFrom.FalseEnabled;
            }

            if(theMergeFrom.RuleSteps != null)
            {
                theMerged.RuleSteps = theMergeFrom.RuleSteps;
            }

            return Base.Exchange.Event.Merge(theMerged, theMergeFrom);
        }
    }
}
