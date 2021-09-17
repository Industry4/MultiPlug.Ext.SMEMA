using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlug.Ext.SMEMA.Models.Components.Downstream
{
    public class DownstreamProperties : MultiPlugBase
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
    }
}
