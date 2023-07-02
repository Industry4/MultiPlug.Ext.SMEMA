using System;
using System.Linq;
using System.Security;
using System.Collections.Generic;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Exchange.API;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Models.Components.Core;
using MultiPlug.Ext.SMEMA.Models.Components.Lane;
using MultiPlug.Ext.SMEMA.Models.Load;
using MultiPlug.Extension.Core;

namespace MultiPlug.Ext.SMEMA
{
    internal class Core : CoreProperties
    {
        private static Core m_Instance = null;
        private IMultiPlugServices m_MultiPlugServices;
        private IMultiPlugActions m_MultiPlugActions;

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal Subscription[] Subscriptions { get; private set; }
        internal Event[] Events { get; private set; }

        private Core()
        {
            GlobalStatusEvent = new Event {
                Guid = Guid.NewGuid().ToString(),
                Id = "SMEMA.Global.Status", Description = "Global Status for all Lanes",
                Subjects = new string[] { "blocked" }
            };
        }

        internal static Core Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Core();
                }
                return m_Instance;
            }
        }

        internal void Init(IMultiPlugServices theMultiPlugServices, IMultiPlugActions theMultiPlugActions)
        {
            m_MultiPlugServices = theMultiPlugServices;
            m_MultiPlugActions = theMultiPlugActions;
        }

        internal void LaneAdd(string theLaneId, string theMachineName)
        {
            string NewLaneGuid = Guid.NewGuid().ToString().Substring(9, 4);

            LaneComponent NewLane = new LaneComponent(NewLaneGuid);
            NewLane.EventsUpdated += OnLaneEventsUpdated;
            NewLane.SubscriptionsUpdated += OnLaneSubscriptionsUpdated;
            NewLane.StatusUpdated += OnLaneStatusUpdated;
            NewLane.UpdateProperties(new LaneProperties { Guid = NewLaneGuid, LaneId = theLaneId, MachineId = theMachineName });

            List<LaneComponent> LanesList = Lanes.ToList();
            LanesList.Add(NewLane);
            Lanes = LanesList.ToArray();
            AggregateSubscriptions();
            AggregateEvents();
        }

        private void OnLaneStatusUpdated()
        {
            InvokeGlobalStatusEvent(Lanes.Any(Lane => Lane.Blocked));
        }

        internal void Start()
        {
            foreach (var Lane in Lanes)
            {
                Lane.Init();
            }

            OnLaneStatusUpdated();
        }

        private void InvokeGlobalStatusEvent(bool theValue)
        {
            GlobalStatusEvent.Invoke(new Payload(GlobalStatusEvent.Id, new PayloadSubject[] {
            new PayloadSubject(GlobalStatusEvent.Subjects[0], theValue ? "true" : "false" ),
            }));
        }

        internal bool LaneDelete(string theGuid)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theGuid);

            if (LaneSearch == null)
            {
                return false;
            }

            List<LaneComponent> LanesList = Lanes.ToList();
            LanesList.Remove(LaneSearch);
            Lanes = LanesList.ToArray();

            LaneSearch.EventsUpdated -= OnLaneEventsUpdated;
            LaneSearch.SubscriptionsUpdated -= OnLaneSubscriptionsUpdated;
            LaneSearch.StatusUpdated -= OnLaneStatusUpdated;

            AggregateSubscriptions();
            AggregateEvents();

            return true;
        }

        internal void Load(LoadRoot theConfiguration)
        {
            if (theConfiguration.Lanes != null)
            {
                List<LaneComponent> NewLanes = new List<LaneComponent>();

                foreach (var Lane in theConfiguration.Lanes)
                {
                    if( string.IsNullOrEmpty(Lane.Guid))
                    {
                        continue;
                    }

                    LaneComponent NewLane = new LaneComponent(Lane.Guid);

                    NewLane.EventsUpdated += OnLaneEventsUpdated;
                    NewLane.SubscriptionsUpdated += OnLaneSubscriptionsUpdated;
                    NewLane.StatusUpdated += OnLaneStatusUpdated;

                    NewLane.UpdateProperties(new LaneProperties
                    {
                        LaneId = Lane.LaneId,
                        MachineId = Lane.MachineId,
                        LoggingLevel = Lane.LoggingLevel,
                        RightToLeft = Lane.RightToLeft
                    });

                    if (Lane.BoardAvailable != null)
                    {
                        NewLane.BoardAvailable.UpdateProperties(Lane.BoardAvailable);
                    }

                    if (Lane.MachineReady != null)
                    {
                        NewLane.MachineReady.UpdateProperties(Lane.MachineReady);
                    }

                    if (Lane.Interlock != null)
                    {
                        NewLane.Interlock.UpdateProperties(Lane.Interlock);
                    }

                    NewLanes.Add(NewLane);
                }

                Lanes = NewLanes.ToArray();

                AggregateSubscriptions();
                AggregateEvents();
            }
        }

        private void AggregateEvents()
        {
            var EventsList = new List<Event>();

            EventsList.Add(GlobalStatusEvent);

            foreach (var Lane in Lanes)
            {
                EventsList.Add(Lane.BoardAvailable.SMEMAMachineReadyEvent);
                EventsList.Add(Lane.Interlock.MachineReadyEvent);
                EventsList.Add(Lane.Interlock.GoodBoardEvent);
                EventsList.Add(Lane.Interlock.BadBoardEvent);
                EventsList.Add(Lane.Interlock.FlipBoardEvent);
                EventsList.Add(Lane.Interlock.MachineReadyBlockEvent);
                EventsList.Add(Lane.Interlock.GoodBoardBlockEvent);
                EventsList.Add(Lane.Interlock.BadBoardBlockEvent);
                EventsList.Add(Lane.Interlock.FlipBoardBlockEvent);
                EventsList.Add(Lane.MachineReady.SMEMABoardAvailableEvent);
                EventsList.Add(Lane.MachineReady.SMEMAFailedBoardAvailableEvent);
                EventsList.Add(Lane.MachineReady.SMEMAFlipBoardEvent);
            }

            Events = EventsList.ToArray();
            EventsUpdated?.Invoke();
        }

        private void AggregateSubscriptions()
        {
            var SubscriptionsList = new List<Base.Exchange.Subscription>();

            foreach (var Lane in Lanes)
            {
                SubscriptionsList.Add(Lane.BoardAvailable.SMEMABoardAvailableSubscription);
                SubscriptionsList.Add(Lane.BoardAvailable.SMEMAFailedBoardAvailableSubscription);
                SubscriptionsList.Add(Lane.BoardAvailable.SMEMAFlipBoardSubscription);
                SubscriptionsList.Add(Lane.Interlock.MachineReadyInterlockSubscription);
                SubscriptionsList.Add(Lane.Interlock.GoodBoardInterlockSubscription);
                SubscriptionsList.Add(Lane.Interlock.BadBoardInterlockSubscription);
                SubscriptionsList.Add(Lane.Interlock.FlipBoardInterlockSubscription);
                SubscriptionsList.Add(Lane.MachineReady.SMEMAMachineReadySubscription);
            }

            Subscriptions = SubscriptionsList.ToArray();
            SubscriptionsUpdated?.Invoke();
        }

        private void OnLaneEventsUpdated()
        {
            AggregateEvents();
        }

        private void OnLaneSubscriptionsUpdated()
        {
            AggregateSubscriptions();
        }

        internal bool DoShutdown()
        {
            try
            {
                m_MultiPlugActions.System.Power.Shutdown();
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }

        internal bool DoRestart()
        {
            try
            {
                m_MultiPlugActions.System.Power.Restart();
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }
    }
}
