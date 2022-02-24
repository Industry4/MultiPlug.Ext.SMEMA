using System;
using MultiPlug.Ext.SMEMA.Components.BoardAvailable;
using MultiPlug.Ext.SMEMA.Components.Interlock;
using MultiPlug.Ext.SMEMA.Components.MachineReady;
using MultiPlug.Ext.SMEMA.Models.Components.Lane;

namespace MultiPlug.Ext.SMEMA.Components.Lane
{
    public class LaneComponent : LaneProperties
    {
        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal event Action StatusUpdated;

        public LaneComponent(string theGuid)
        {
            if (theGuid != null)
            {
                Guid = theGuid;
            }

            BoardAvailable = new BoardAvailableComponent(theGuid, EventSuffix);
            BoardAvailable.EventsUpdated += OnEventsUpdated;
            BoardAvailable.SubscriptionsUpdated += OnSubscriptionsUpdated;
            MachineReady = new MachineReadyComponent(theGuid, EventSuffix);
            MachineReady.EventsUpdated += OnEventsUpdated;
            MachineReady.SubscriptionsUpdated += OnSubscriptionsUpdated;

            Interlock = new InterlockComponent(theGuid, EventSuffix);
            Interlock.EventsUpdated += OnEventsUpdated;
            Interlock.SubscriptionsUpdated += OnSubscriptionsUpdated;

            Interlock.BlockedUpdated += OnInterlockBlockedStatusUpdated;

            MachineReady.StateMachine.MachineReady += Interlock.MachineReadyStateMachine.OnSMEMAIOMachineReady;
            Interlock.MachineReadyStateMachine.MachineReadyUpdated += BoardAvailable.OnMachineReady;

            BoardAvailable.StateMachine.GoodBoard += Interlock.BoardAvailableStateMachine.OnSMEMAIOGoodBoard;
            Interlock.BoardAvailableStateMachine.GoodBoardUpdated += MachineReady.OnGoodBoard;

            BoardAvailable.StateMachine.BadBoard += Interlock.BoardAvailableStateMachine.OnSMEMAIOBadBoard;
            Interlock.BoardAvailableStateMachine.BadBoardUpdated += MachineReady.OnBadBoard;
        }

        internal void Init()
        {
            BoardAvailable.StateMachine.Init();
            MachineReady.StateMachine.Init();
        }

        private void OnEventsUpdated()
        {
            EventsUpdated?.Invoke();
        }

        private void OnSubscriptionsUpdated()
        {
            SubscriptionsUpdated?.Invoke();
        }

        internal void UpdateProperties(LaneProperties theNewProperties)
        {
            MachineId = theNewProperties.MachineId;
            LaneId = theNewProperties.LaneId;
        }

        private void OnInterlockBlockedStatusUpdated()
        {
            StatusUpdated?.Invoke();
        }

        internal bool Blocked
        {
            get
            {
                return Interlock.Blocked;
            }
        }
    }
}
