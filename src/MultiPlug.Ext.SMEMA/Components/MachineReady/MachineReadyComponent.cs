using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;

namespace MultiPlug.Ext.SMEMA.Components.MachineReady
{
    public class MachineReadyComponent : MachineReadyProperties
    {
        internal const string c_SMEMABoardAvailableEventId = "SMEMABoardAvailable";
        internal const string c_SMEMAFailedBoardAvailableEventId = "SMEMAFailedBoardAvailable";
        internal const string c_SMEMAFlipBoardEventId = "SMEMAFlipBoard";

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal MachineReadySMEMAStateMachine StateMachine { get; private set; }

        public MachineReadyComponent(string theGuid)
        {
            SMEMAMachineReadyAlways = false;
            StateMachine = new MachineReadySMEMAStateMachine(this);

            SMEMAMachineReadySubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Value = "1" };
            SMEMAMachineReadySubscription.Event += StateMachine.OnMachineReady;
            SMEMABoardAvailableEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = theGuid + "-" + c_SMEMABoardAvailableEventId, Description = "Good Board Available", Subjects = new string[] { "value" }, Group = theGuid };
            SMEMAFailedBoardAvailableEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = theGuid + "-" + c_SMEMAFailedBoardAvailableEventId, Description = "Bad Board Available", Subjects = new string[] { "value" }, Group = theGuid};
            SMEMAFlipBoardEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = theGuid + "-" + c_SMEMAFlipBoardEventId, Description = "Flip Board", Subjects = new string[] { "value" }, Group = theGuid };
        }

        internal void UpdateProperties(MachineReadyProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (theNewProperties.SMEMABoardAvailableEvent != null && Event.Merge(SMEMABoardAvailableEvent, theNewProperties.SMEMABoardAvailableEvent))
            {
                FlagEventUpdated = true;
            }

            if (theNewProperties.SMEMAFailedBoardAvailableEvent != null && Event.Merge(SMEMAFailedBoardAvailableEvent, theNewProperties.SMEMAFailedBoardAvailableEvent))
            {
                FlagEventUpdated = true;
            }

            if (theNewProperties.SMEMAFlipBoardEvent != null && Event.Merge(SMEMAFlipBoardEvent, theNewProperties.SMEMAFlipBoardEvent))
            {
                FlagEventUpdated = true;
            }

            if (theNewProperties.SMEMAMachineReadySubscription != null && Subscription.Merge(SMEMAMachineReadySubscription, theNewProperties.SMEMAMachineReadySubscription))
            {
                FlagSubscriptionUpdated = true;
            }

            if(theNewProperties.SMEMAMachineReadySubscription != null)
            {
                SMEMAMachineReadySubscription.Value = theNewProperties.SMEMAMachineReadySubscription.Value;
            }

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
