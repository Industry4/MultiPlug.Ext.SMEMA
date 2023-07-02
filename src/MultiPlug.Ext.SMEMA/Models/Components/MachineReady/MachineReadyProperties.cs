using System.Runtime.Serialization;
using MultiPlug.Base;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.SMEMA.Models.Components.MachineReady
{
    public class MachineReadyProperties : MultiPlugBase
    {
        /// 
        /// SMEMA Machine Ready to Receive
        ///
        [DataMember]
        public Exchange.Subscription SMEMAMachineReadySubscription { get; set; }

        ///
        /// SMEMA Board Available
        ///

        [DataMember]
        public Event SMEMABoardAvailableEvent { get; set; }

        ///
        /// SMEMA Failed Board Available
        ///

        [DataMember]
        public Event SMEMAFailedBoardAvailableEvent { get; set; }

        ///
        /// SMEMA Flip Board
        ///

        [DataMember]
        public Event SMEMAFlipBoardEvent { get; set; }

        [DataMember]
        public bool? SMEMAMachineReadyAlways { get; set; }
    }
}
