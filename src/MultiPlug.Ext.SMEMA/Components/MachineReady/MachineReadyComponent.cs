using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;

namespace MultiPlug.Ext.SMEMA.Components.MachineReady
{
    public class MachineReadyComponent : MachineReadyProperties
    {
        internal const string c_SMEMABoardAvailableEventId = "SMEMABoardAvailable";
        internal const string c_SMEMAFailedBoardAvailableEventId = "SMEMAFailedBoardAvailable";

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal MachineReadySMEMAStateMachine StateMachine { get; private set; }

        public MachineReadyComponent(string theGuid, string theEventSuffix)
        {
            SMEMAMachineReadyAlways = false;
            StateMachine = new MachineReadySMEMAStateMachine(this);

            SMEMAMachineReadySubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Value = "1" };
            SMEMAMachineReadySubscription.Event += StateMachine.OnMachineReady;
            SMEMABoardAvailableEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = c_SMEMABoardAvailableEventId + theEventSuffix, Description = "Good Board Available", Subjects = new string[] { "value" } };
            SMEMAFailedBoardAvailableEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = c_SMEMAFailedBoardAvailableEventId + theEventSuffix, Description = "Bad Board Available", Subjects = new string[] { "value" } };
        }

        internal void UpdateProperties(MachineReadyProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (Event.Merge(SMEMABoardAvailableEvent, theNewProperties.SMEMABoardAvailableEvent))
            {
                FlagEventUpdated = true;
            }

            if (Event.Merge(SMEMAFailedBoardAvailableEvent, theNewProperties.SMEMAFailedBoardAvailableEvent))
            {
                FlagEventUpdated = true;
            }

            if (Subscription.Merge(SMEMAMachineReadySubscription, theNewProperties.SMEMAMachineReadySubscription))
            {
                FlagSubscriptionUpdated = true;
            }

            SMEMAMachineReadySubscription.Value = theNewProperties.SMEMAMachineReadySubscription.Value;

            if(theNewProperties.SMEMAMachineReadyAlways != null && theNewProperties.SMEMAMachineReadyAlways != SMEMAMachineReadyAlways)
            {
                SMEMAMachineReadyAlways = theNewProperties.SMEMAMachineReadyAlways;
                StateMachine.Init();
            }

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }
        }
    }
}
