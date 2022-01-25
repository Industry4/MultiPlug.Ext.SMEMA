using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.BoardAvailable;

namespace MultiPlug.Ext.SMEMA.Components.BoardAvailable
{
    public class BoardAvailableComponent : BoardAvailableProperties
    {

        internal static string SMEMAMachineReadyEventId = "SMEMAMachineReady";

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        public BoardAvailableComponent(string theGuid, string theEventSuffix)
        {
            SMEMAMachineReadyEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = SMEMAMachineReadyEventId + theEventSuffix, Description = "SMEMA Machine Ready", Subjects = new[] { "value" } };
            SMEMABoardAvailableSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Subjects = new ushort[] { 0 }, Value = "1" };
            SMEMAFailedBoardAvailableSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Subjects = new ushort[] { 0 }, Value = "1" };
        }

        internal void UpdateProperties(BoardAvailableProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (Subscription.Merge(SMEMABoardAvailableSubscription, theNewProperties.SMEMABoardAvailableSubscription)) { FlagSubscriptionUpdated = true; }
            if (Subscription.Merge(SMEMAFailedBoardAvailableSubscription, theNewProperties.SMEMAFailedBoardAvailableSubscription)) { FlagSubscriptionUpdated = true; }
            if (Event.Merge(SMEMAMachineReadyEvent, theNewProperties.SMEMAMachineReadyEvent)) { FlagEventUpdated = true; }

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }
        }
    }
}
