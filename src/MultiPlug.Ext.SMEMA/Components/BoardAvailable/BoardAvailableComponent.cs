using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.BoardAvailable;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.BoardAvailable
{
    public class BoardAvailableComponent : BoardAvailableProperties
    {

        internal static string SMEMAMachineReadyEventId = "SMEMAMachineReady";

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal BoardAvailableSMEMAStateMachine StateMachine { get; private set; }

        public BoardAvailableComponent(string theGuid, string theEventSuffix)
        {
            SMEMABoardAvailableAlways = false;
            SMEMAFailedBoardAvailableAlways = false;
            SMEMAFlipBoardAlways = false;
            StateMachine = new BoardAvailableSMEMAStateMachine(this);

            SMEMAMachineReadyEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = SMEMAMachineReadyEventId + theEventSuffix, Description = "SMEMA Machine Ready", Subjects = new[] { "value" } };
            SMEMABoardAvailableSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Value = "1" };
            SMEMABoardAvailableSubscription.Event += StateMachine.OnGoodBoardEvent;
            SMEMAFailedBoardAvailableSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Value = "1" };
            SMEMAFailedBoardAvailableSubscription.Event += StateMachine.OnBadBoardEvent;
            SMEMAFlipBoardSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Value = "1" };
            SMEMAFlipBoardSubscription.Event += StateMachine.OnFlipBoardEvent;
        }

        internal void OnMachineReady(bool isTrue)
        {
            StateMachine.MachineReadyState = isTrue;

            SMEMAMachineReadyEvent.Invoke(new Payload(SMEMAMachineReadyEvent.Id, new PayloadSubject[] {
                new PayloadSubject(SMEMAMachineReadyEvent.Subjects[0], GetStringValue.Invoke( isTrue ) )
                }));
        }

        internal void UpdateProperties(BoardAvailableProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;
            bool FlagAvailableAlwaysUpdated = false;

            if (theNewProperties.SMEMABoardAvailableAlways != null && theNewProperties.SMEMABoardAvailableAlways != SMEMABoardAvailableAlways)
            {
                SMEMABoardAvailableAlways = theNewProperties.SMEMABoardAvailableAlways;
                FlagAvailableAlwaysUpdated = true;
            }

            if (theNewProperties.SMEMAFailedBoardAvailableAlways != null && theNewProperties.SMEMAFailedBoardAvailableAlways != SMEMAFailedBoardAvailableAlways)
            {
                SMEMAFailedBoardAvailableAlways = theNewProperties.SMEMAFailedBoardAvailableAlways;
                FlagAvailableAlwaysUpdated = true;
            }

            if(theNewProperties.SMEMAFlipBoardAlways != null && theNewProperties.SMEMAFlipBoardAlways != SMEMAFlipBoardAlways)
            {
                SMEMAFlipBoardAlways = theNewProperties.SMEMAFlipBoardAlways;
                FlagAvailableAlwaysUpdated = true;
            }

            if (theNewProperties.SMEMABoardAvailableSubscription != null && Subscription.Merge(SMEMABoardAvailableSubscription, theNewProperties.SMEMABoardAvailableSubscription)) { FlagSubscriptionUpdated = true; }
            if (theNewProperties.SMEMAFailedBoardAvailableSubscription != null && Subscription.Merge(SMEMAFailedBoardAvailableSubscription, theNewProperties.SMEMAFailedBoardAvailableSubscription)) { FlagSubscriptionUpdated = true; }
            if (theNewProperties.SMEMAFlipBoardSubscription != null && Subscription.Merge(SMEMAFlipBoardSubscription, theNewProperties.SMEMAFlipBoardSubscription)) { FlagSubscriptionUpdated = true; }
            if (theNewProperties.SMEMAMachineReadyEvent != null && Event.Merge(SMEMAMachineReadyEvent, theNewProperties.SMEMAMachineReadyEvent)) { FlagEventUpdated = true; }

            if(theNewProperties.SMEMABoardAvailableSubscription != null)
            {
                SMEMABoardAvailableSubscription.Value = theNewProperties.SMEMABoardAvailableSubscription.Value;
            }

            if(theNewProperties.SMEMAFailedBoardAvailableSubscription != null)
            {
                SMEMAFailedBoardAvailableSubscription.Value = theNewProperties.SMEMAFailedBoardAvailableSubscription.Value;
            }

            if(theNewProperties.SMEMAFlipBoardSubscription != null)
            {
                SMEMAFlipBoardSubscription.Value = theNewProperties.SMEMAFlipBoardSubscription.Value;
            }

            if (FlagAvailableAlwaysUpdated) { StateMachine.Init(); }
            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }
        }
    }
}
