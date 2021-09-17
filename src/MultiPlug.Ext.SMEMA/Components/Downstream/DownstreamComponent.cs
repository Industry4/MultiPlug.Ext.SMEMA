using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.Downstream;

namespace MultiPlug.Ext.SMEMA.Components.Downstream
{
    public class DownstreamComponent : DownstreamProperties
    {
        internal const string c_SMEMABoardAvailableEventId = "SMEMABoardAvailable";
        internal const string c_SMEMAFailedBoardAvailableEventId = "SMEMAFailedBoardAvailable";

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        public DownstreamComponent(string theGuid, string theEventSuffix)
        {
            SMEMAMachineReadySubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Subjects = new ushort[] { 0 }, Value = "1" };
            SMEMABoardAvailableEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = c_SMEMABoardAvailableEventId + theEventSuffix, Description = "Board Available", Subjects = new string[] { "value" } };
            SMEMAFailedBoardAvailableEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = c_SMEMAFailedBoardAvailableEventId + theEventSuffix, Description = "Failed Board Available", Subjects = new string[] { "value" } };
        }

        internal void UpdateProperties(DownstreamProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (Event.Merge(SMEMABoardAvailableEvent, theNewProperties.SMEMABoardAvailableEvent)) { FlagEventUpdated = true; }

            if (Event.Merge(SMEMAFailedBoardAvailableEvent, theNewProperties.SMEMAFailedBoardAvailableEvent)) { FlagEventUpdated = true; }

            if (Subscription.Merge(SMEMAMachineReadySubscription, theNewProperties.SMEMAMachineReadySubscription)) { FlagSubscriptionUpdated = true; }

            SMEMAMachineReadySubscription.Value = theNewProperties.SMEMAMachineReadySubscription.Value;

            // BUG --------- Temp Fix ------------------------------------
            //SMEMABoardAvailableEvent.Subjects = theNewProperties.SMEMAFailedBoardAvailableEvent.Subjects != null ? theNewProperties.SMEMAFailedBoardAvailableEvent.Subjects : new string[] { "value" };
            //SMEMAFailedBoardAvailableEvent.Subjects = theNewProperties.SMEMAFailedBoardAvailableEvent.Subjects != null ? theNewProperties.SMEMAFailedBoardAvailableEvent.Subjects : new string[] { "value" };
            // BUG -------------------------------------------------------

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }
        }

    }
}
