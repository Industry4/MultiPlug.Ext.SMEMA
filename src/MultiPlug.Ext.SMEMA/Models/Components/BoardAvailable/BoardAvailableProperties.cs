using System.Runtime.Serialization;
using MultiPlug.Base;
using MultiPlug.Ext.SMEMA.Models.Exchange;

namespace MultiPlug.Ext.SMEMA.Models.Components.BoardAvailable
{
    public class BoardAvailableProperties : MultiPlugBase
    {
        /// 
        /// SMEMA Machine Ready to Receive
        ///
        [DataMember]
        public SMEMAEvent SMEMAMachineReadyEvent { get; set; }

        ///
        /// SMEMA Board Available
        ///
        [DataMember]
        public SMEMASubscription SMEMABoardAvailableSubscription { get; set; }

        ///
        /// SMEMA Failed Board Available
        ///
        [DataMember]
        public SMEMASubscription SMEMAFailedBoardAvailableSubscription { get; set; }

        ///
        /// SMEMA Flip Board
        ///
        [DataMember]
        public SMEMASubscription SMEMAFlipBoardSubscription { get; set; }

        [DataMember]
        public bool? SMEMABoardAvailableAlways { get; set; }

        [DataMember]
        public bool? SMEMAFailedBoardAvailableAlways { get; set; }

        [DataMember]
        public bool? SMEMAFlipBoardAlways { get; set; }
    }
}
