using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    public class InterlockComponent : InterlockProperties
    {
        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal event Action BlockedUpdated;

        internal InterlockMachineReadyStateMachine MachineReadyStateMachine { get; private set; }
        internal InterlockBoardAvailableStateMachine BoardAvailableStateMachine { get; private set; }

        private InterlockSMEMAStateMachine m_SMEMAUplineStateMachine = new InterlockSMEMAStateMachine();
        private InterlockSMEMAStateMachine m_SMEMADownlineStateMachine = new InterlockSMEMAStateMachine();

        public InterlockComponent(string theGuid, string theEventSuffix)
        {
            InterlockSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Value = "1" };

            MachineReadyEvent = new Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.MachineReady",
                Description = "Machine Ready Interlock",
                Subjects = new[] { "value", "enabled", "interlock-latched" }
            };
            GoodBoardEvent = new Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.GoodBoard",
                Description = "Good Board Interlock",
                Subjects = new[] { "value", "enabled", "interlock-latched", "divert", "divert-latched" }
            };
            BadBoardEvent = new Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.BadBoard",
                Description = "Bad Board Interlock",
                Subjects = new[] { "value", "enabled", "interlock-latched", "divert", "divert-latched" }
            };

            MachineReadyBlockEvent = new Models.Exchange.Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.MachineReady.Block",
                Description = "Machine Ready Interlock Blocking",
                Subjects = new[] { "blocked", "smema" }
            };

            GoodBoardBlockEvent = new Models.Exchange.Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.GoodBoard.Block",
                Description = "Good Board Interlock Blocking",
                Subjects = new[] { "blocked", "smema" }
            };

            BadBoardBlockEvent = new Models.Exchange.Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.BadBoard.Block",
                Description = "Bad Board Interlock Blocking",
                Subjects = new[] { "blocked", "smema" }
            };

            MachineReadyStateMachine = new InterlockMachineReadyStateMachine(
                m_SMEMAUplineStateMachine,
                m_SMEMADownlineStateMachine,
                MachineReadyEvent,
                MachineReadyBlockEvent);

            MachineReadyStateMachine.BlockedUpdated += OnBlockedStatusUpdated;

            BoardAvailableStateMachine = new InterlockBoardAvailableStateMachine(
                m_SMEMAUplineStateMachine,
                m_SMEMADownlineStateMachine,
                GoodBoardEvent,
                BadBoardEvent,
                GoodBoardBlockEvent,
                BadBoardBlockEvent);

            BoardAvailableStateMachine.BlockedUpdated += OnBlockedStatusUpdated;
        }

        internal void UpdateProperties(InterlockProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (Subscription.Merge(InterlockSubscription, theNewProperties.InterlockSubscription)) { FlagSubscriptionUpdated = true; }

            InterlockSubscription.Value = theNewProperties.InterlockSubscription.Value;

            if(Event.Merge( MachineReadyBlockEvent, theNewProperties.MachineReadyBlockEvent)) { FlagEventUpdated = true; }

            MergeEvent(MachineReadyBlockEvent, theNewProperties.MachineReadyBlockEvent);

            if (Event.Merge(GoodBoardBlockEvent, theNewProperties.GoodBoardBlockEvent)) { FlagEventUpdated = true; }

            MergeEvent(GoodBoardBlockEvent, theNewProperties.GoodBoardBlockEvent);

            if (Event.Merge(BadBoardBlockEvent, theNewProperties.BadBoardBlockEvent)) { FlagEventUpdated = true; }

            MergeEvent(BadBoardBlockEvent, theNewProperties.BadBoardBlockEvent);

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }
        }

        private void MergeEvent(Models.Exchange.Event Into, Models.Exchange.Event From )
        {
            Into.BlockedEnabled = From.BlockedEnabled;
            Into.BlockedValue = From.BlockedValue;
            Into.UnblockedEnabled = From.UnblockedEnabled;
            Into.UnblockedValue = From.UnblockedValue;
        }

        private void OnBlockedStatusUpdated()
        {
            BlockedUpdated?.Invoke();
        }

        public bool Blocked
        {
            get
            {
                return MachineReadyStateMachine.Blocked || BoardAvailableStateMachine.Blocked;
            }
        }
    }
}
