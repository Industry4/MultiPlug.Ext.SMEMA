﻿using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Components.Interlock
{
    public class InterlockProperties : MultiPlugBase
    {
        [DataMember]
        public Exchange.Subscription InterlockSubscription { get; set; }

        /// 
        /// Interlock Machine Ready
        ///
        [DataMember]
        public Event MachineReadyEvent { get; set; }

        /// 
        /// Interlock Machine Ready
        ///
        [DataMember]
        public Event GoodBoardEvent { get; set; }

        /// 
        /// Interlock Machine Ready
        ///
        [DataMember]
        public Event BadBoardEvent { get; set; }

        /// 
        /// Interlock Machine Ready Block
        ///
        [DataMember]
        public Exchange.Event MachineReadyBlockEvent { get; set; }

        /// 
        /// Interlock Machine Ready Blcok
        ///
        [DataMember]
        public Exchange.Event GoodBoardBlockEvent { get; set; }

        /// 
        /// Interlock Machine Ready Block
        ///
        [DataMember]
        public Exchange.Event BadBoardBlockEvent { get; set; }
    }
}
