using System;
using MultiPlug.Ext.SMEMA.Components.BoardAvailable;
using MultiPlug.Ext.SMEMA.Components.Interlock;
using MultiPlug.Ext.SMEMA.Components.MachineReady;
using MultiPlug.Ext.SMEMA.Models.Components.Lane;
using MultiPlug.Ext.SMEMA.Components.BeaconTower;

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

            BoardAvailable = new BoardAvailableComponent(theGuid);
            BoardAvailable.EventsUpdated += OnEventsUpdated;
            BoardAvailable.SubscriptionsUpdated += OnSubscriptionsUpdated;
            MachineReady = new MachineReadyComponent(theGuid);
            MachineReady.EventsUpdated += OnEventsUpdated;
            MachineReady.SubscriptionsUpdated += OnSubscriptionsUpdated;

            Interlock = new InterlockComponent(theGuid);
            Interlock.EventsUpdated += OnEventsUpdated;
            Interlock.SubscriptionsUpdated += OnSubscriptionsUpdated;

            Interlock.BlockedUpdated += OnInterlockBlockedStatusUpdated;

            MachineReady.StateMachine.MachineReady += Interlock.MachineReadyStateMachine.OnSMEMAIOMachineReady;
            Interlock.MachineReadyStateMachine.SMEMADownlineStateMachine.MachineReady.Updated += BoardAvailable.StateMachine.OnMachineReady;

            BoardAvailable.StateMachine.GoodBoard += Interlock.BoardAvailableStateMachine.OnSMEMAIOGoodBoard;
            Interlock.BoardAvailableStateMachine.SMEMADownlineStateMachine.GoodBoard.Updated += MachineReady.StateMachine.OnGoodBoard;
            Interlock.BoardAvailableStateMachine.SMEMADownlineStateMachine.BadBoardDiverted.Updated += MachineReady.StateMachine.OnBadBoardDiverted;

            BoardAvailable.StateMachine.BadBoard += Interlock.BoardAvailableStateMachine.OnSMEMAIOBadBoard;
            Interlock.BoardAvailableStateMachine.SMEMADownlineStateMachine.BadBoard.Updated += MachineReady.StateMachine.OnBadBoard;
            Interlock.BoardAvailableStateMachine.SMEMADownlineStateMachine.GoodBoardDiverted.Updated += MachineReady.StateMachine.OnGoodBoardDiverted;
            BoardAvailable.StateMachine.FlipBoard += Interlock.BoardAvailableStateMachine.OnSMEMAIOFlipBoard;
            Interlock.BoardAvailableStateMachine.SMEMADownlineStateMachine.FlipBoard.Updated += MachineReady.StateMachine.OnFlipBoard;

            MachineReady.StateMachine.MachineReady += Interlock.BoardAvailableStateMachine.OnSMEMAIOMachineReady;

            BeaconTower = new BeaconTowerComponent(theGuid, Interlock);
            BeaconTower.EventsUpdated += OnEventsUpdated;

        }

        internal void Init()
        {
            BoardAvailable.StateMachine.Init();
            MachineReady.StateMachine.Init();
            Interlock.Init();
            BeaconTower.Init();
        }

        internal void Shutdown()
        {
            BeaconTower.Shutdown();
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
            RightToLeft = theNewProperties.RightToLeft;
            LoggingLevel = theNewProperties.LoggingLevel;
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
