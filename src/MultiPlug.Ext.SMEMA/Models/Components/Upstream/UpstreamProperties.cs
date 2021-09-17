using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Components.Upstream
{
    public class UpstreamProperties : MultiPlugBase
    {
        /// 
        /// SMEMA Machine Ready to Receive
        ///
        [DataMember]
        public Event SMEMAMachineReadyEvent { get; set; }

        ///
        /// SMEMA Board Available
        ///
        [DataMember]
        public Exchange.Subscription SMEMABoardAvailableSubscription { get; set; }

        ///
        /// SMEMA Failed Board Available
        ///
        [DataMember]
        public Exchange.Subscription SMEMAFailedBoardAvailableSubscription { get; set; }


    }
}
