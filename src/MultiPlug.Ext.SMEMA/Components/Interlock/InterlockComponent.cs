using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    public class InterlockComponent : InterlockProperties
    {
        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        public InterlockComponent(string theGuid, string theEventSuffix)
        {
            InterlockSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Subjects = new ushort[] { 0 }, Value = "1" };
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
