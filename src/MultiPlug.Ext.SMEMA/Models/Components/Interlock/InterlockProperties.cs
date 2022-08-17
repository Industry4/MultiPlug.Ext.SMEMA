using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Components.Interlock
{
    public class InterlockProperties : MultiPlugBase
    {
        [DataMember]
        public Exchange.MachineReadyInterlockSubscription MachineReadyInterlockSubscription { get; set; }

        [DataMember]
        public Exchange.GoodBadInterlockSubscription GoodBoardInterlockSubscription { get; set; }

        [DataMember]
        public Exchange.GoodBadInterlockSubscription BadBoardInterlockSubscription { get; set; }

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

        /// <summary>
        /// 0 = Blocked, 1 = Unblocked and Latched, 2 = Shutdown Latched State
        /// </summary>
        [DataMember]
        public int StartupMachineReady { get; set; }

        /// <summary>
        /// 0 = Blocked, 1 = Unblocked and Latched, 2 = Shutdown Latched State
        /// </summary>
        [DataMember]
        public int StartupGoodBoard { get; set; }

        /// <summary>
        /// 0 = Blocked, 1 = Unblocked and Latched, 2 = Shutdown Latched State
        /// </summary>
        [DataMember]
        public int StartupBadBoard { get; set; }

        [DataMember]
        public bool PersistentMachineReady { get; set; }

        [DataMember]
        public bool PersistentMachineReadyLatch { get; set; }

        [DataMember]
        public bool PersistentGoodBoard { get; set; }

        [DataMember]
        public bool PersistentGoodBoardLatch { get; set; }

        [DataMember]
        public bool PersistentBadBoard { get; set; }

        [DataMember]
        public bool PersistentBadBoardLatch { get; set; }

        [DataMember]
        public bool? PermissionInterfaceUI { get; set; } = true;
        [DataMember]
        public bool? PermissionInterfaceREST { get; set; } = true;
        [DataMember]
        public bool? PermissionInterfaceSubscriptions { get; set; } = true;
    }
}
