using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;
using MultiPlug.Ext.SMEMA.Components.Utils;

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
            StateMachine = new MachineReadySMEMAStateMachine(this);

            SMEMAMachineReadySubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Value = "1" };
            SMEMAMachineReadySubscription.Event += StateMachine.OnEvent;
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

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }
        }

        internal void OnGoodBoard(bool isTrue)
        {
            StateMachine.GoodBoardAvailableState = isTrue;

            SMEMABoardAvailableEvent.Invoke(new Payload(SMEMABoardAvailableEvent.Id, new PayloadSubject[] {
                new PayloadSubject(SMEMABoardAvailableEvent.Subjects[0], GetStringValue.Invoke( isTrue ) )
                }));
        }

        internal void OnBadBoard(bool isTrue)
        {
            StateMachine.BadBoardAvailableState = isTrue;

            SMEMAFailedBoardAvailableEvent.Invoke(new Payload(SMEMAFailedBoardAvailableEvent.Id, new PayloadSubject[] {
                new PayloadSubject(SMEMAFailedBoardAvailableEvent.Subjects[0], GetStringValue.Invoke( isTrue ) )
                }));
        }
    }
}
