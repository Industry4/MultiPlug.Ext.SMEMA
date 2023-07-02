using System.Runtime.Serialization;
using MultiPlug.Base;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.SMEMA.Models.Components.BoardAvailable
{
    public class BoardAvailableProperties : MultiPlugBase
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

        ///
        /// SMEMA Flip Board
        ///
        [DataMember]
        public Exchange.Subscription SMEMAFlipBoardSubscription { get; set; }

        [DataMember]
        public bool? SMEMABoardAvailableAlways { get; set; }

        [DataMember]
        public bool? SMEMAFailedBoardAvailableAlways { get; set; }

        [DataMember]
        public bool? SMEMAFlipBoardAlways { get; set; }
    }
}
