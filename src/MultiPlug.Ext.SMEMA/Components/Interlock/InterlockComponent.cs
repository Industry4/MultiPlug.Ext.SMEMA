using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    public class InterlockComponent : InterlockProperties
    {
        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal InterlockMachineReadyStateMachine MachineReadyStateMachine { get; private set; }
        internal InterlockBoardAvailableStateMachine BoardAvailableStateMachine { get; private set; }

        private InterlockSMEMAStateMachine m_SMEMAStateMachine = new InterlockSMEMAStateMachine();

        public InterlockComponent(string theGuid, string theEventSuffix)
        {
            InterlockSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Subjects = new ushort[] { 0 }, Value = "1" };

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

            MachineReadyStateMachine = new InterlockMachineReadyStateMachine(m_SMEMAStateMachine, MachineReadyEvent);
            BoardAvailableStateMachine = new InterlockBoardAvailableStateMachine(m_SMEMAStateMachine, GoodBoardEvent, BadBoardEvent);
        }

        internal void UpdateProperties(InterlockProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (Subscription.Merge(InterlockSubscription, theNewProperties.InterlockSubscription)) { FlagSubscriptionUpdated = true; }

            InterlockSubscription.Value = theNewProperties.InterlockSubscription.Value;

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }
        }
    }
}
