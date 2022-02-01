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

            MachineReady.StateMachine.MachineReady += Interlock.OnMachineReady;

            Interlock.MachineReady += BoardAvailable.OnMachineReady;
        }

        private void OnEventsUpdated()
        {
            //AggregateEvents();
            EventsUpdated?.Invoke();
        }

        private void OnSubscriptionsUpdated()
        {
            //AggregateSubscriptions();
            SubscriptionsUpdated?.Invoke();
        }



        internal void UpdateProperties(LaneProperties theNewProperties)
        {
            MachineId = theNewProperties.MachineId;
            LaneId = theNewProperties.LaneId;
        }

    }
}
