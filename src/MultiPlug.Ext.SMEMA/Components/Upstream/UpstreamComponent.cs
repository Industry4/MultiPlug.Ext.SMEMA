using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.Upstream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlug.Ext.SMEMA.Components.Upstream
{
    public class UpstreamComponent : UpstreamProperties
    {

        internal static string SMEMAMachineReadyEventId = "SMEMAMachineReady";

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        public UpstreamComponent(string theGuid, string theEventSuffix)
        {
            SMEMAMachineReadyEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = SMEMAMachineReadyEventId + theEventSuffix, Description = "SMEMA Machine Ready", Subjects = new[] { "value" } };
            SMEMABoardAvailableSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Subjects = new ushort[] { 0 }, Value = "1" };
            SMEMAFailedBoardAvailableSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Subjects = new ushort[] { 0 }, Value = "1" };
        }

        internal void UpdateProperties(UpstreamProperties theNewProperties)
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
