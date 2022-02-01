using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    public class InterlockComponent : InterlockProperties
    {
        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        private bool m_MachineReadyInterlock;

        public bool MachineReadyState
        {
            get
            {
                return m_MachineReadyInterlock;
            }
            set
            {
                m_MachineReadyInterlock = value;

                if (m_MachineReadyInterlock && m_MachineReadyIOState)
                {
                    m_MachineReadyIOState = false;
                    MachineReady?.BeginInvoke(true, MachineReady.EndInvoke, null);
                }

                if (m_MachineReadyInterlock)
                {
                    MachineReadyEvent.Invoke(new Payload(MachineReadyEvent.Id, new PayloadSubject[] {
                    new PayloadSubject(MachineReadyEvent.Subjects[0], "1")
                    }));
                }
                else
                {
                    MachineReadyEvent.Invoke(new Payload(MachineReadyEvent.Id, new PayloadSubject[] {
                    new PayloadSubject(MachineReadyEvent.Subjects[0], "0")
                    }));
                }

            }
        }
        public bool MachineReadyLatch { get; set; }
        public bool GoodBoard { get; set; }
        internal bool GoodBoardLatch { get; set; } = true;
        internal bool GoodBoardDivert { get; set; } // To Bad Board
        internal bool GoodBoardDivertLatch { get; set; } = true;

        internal bool BadBoard { get; set; }
        internal bool BadBoardLatch { get; set; } = true;
        internal bool BadBoardDivert { get; set; }// To Good Board
        internal bool BadBoardDivertLatch { get; set; } = true;

        internal event Action<bool> MachineReady;

        public InterlockComponent(string theGuid, string theEventSuffix)
        {
            InterlockSubscription = new Models.Exchange.Subscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Subjects = new ushort[] { 0 }, Value = "1" };

            MachineReadyEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = "Interlock.MachineReady", Description = "Machine Ready Interlock", Subjects = new[] { "value" } };
            GoodBoardEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = "Interlock.GoodBoard", Description = "Good Board Interlock", Subjects = new[] { "value" } };
            BadBoardEvent = new Event { Guid = Guid.NewGuid().ToString(), Id = "Interlock.BadBoard", Description = "Bad Board Interlock", Subjects = new[] { "value" } };
        }

        private bool m_MachineReadyIOState;

        internal void OnMachineReady(bool theIOState)
        {
            if (theIOState)
            {

                if (MachineReadyState)
                {


                    MachineReady?.BeginInvoke(true, MachineReady.EndInvoke, null);
                }
                else
                {
                    m_MachineReadyIOState = true;
                }
            }
            else
            {
                if (!MachineReadyLatch)
                {
                    MachineReadyState = false;
                }
                m_MachineReadyIOState = false;
                MachineReady?.BeginInvoke(false, MachineReady.EndInvoke, null);
            }
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
