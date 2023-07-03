using System.Runtime.Serialization;
using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Exchange;

namespace MultiPlug.Ext.SMEMA.Models.Components.MachineReady
{
    public class MachineReadyProperties : MultiPlugBase
    {
        /// 
        /// SMEMA Machine Ready to Receive
        ///
        [DataMember]
        public Exchange.SMEMASubscription SMEMAMachineReadySubscription { get; set; }

        ///
        /// SMEMA Board Available
        ///

        [DataMember]
        public SMEMAEvent SMEMABoardAvailableEvent { get; set; }

        ///
        /// SMEMA Failed Board Available
        ///

        [DataMember]
        public SMEMAEvent SMEMAFailedBoardAvailableEvent { get; set; }

        ///
        /// SMEMA Flip Board
        ///

        [DataMember]
        public SMEMAEvent SMEMAFlipBoardEvent { get; set; }

        [DataMember]
        public bool? SMEMAMachineReadyAlways { get; set; }
    }
}
