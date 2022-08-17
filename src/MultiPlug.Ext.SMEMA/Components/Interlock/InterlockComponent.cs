using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    public class InterlockComponent : InterlockProperties
    {
        private bool m_RunOnce = true;

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal event Action BlockedUpdated;

        internal InterlockMachineReadyStateMachine MachineReadyStateMachine { get; private set; }
        internal InterlockBoardAvailableStateMachine BoardAvailableStateMachine { get; private set; }

        private InterlockSMEMAStateMachine m_SMEMAUplineStateMachine = new InterlockSMEMAStateMachine();
        private InterlockSMEMAStateMachine m_SMEMADownlineStateMachine = new InterlockSMEMAStateMachine();

        public InterlockComponent(string theGuid, string theEventSuffix)
        {
            MachineReadyInterlockSubscription = new Models.Exchange.MachineReadyInterlockSubscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty };
            MachineReadyInterlockSubscription.Event += OnMachineReadyInterlockEvent;

            GoodBoardInterlockSubscription = new Models.Exchange.GoodBadInterlockSubscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty };
            GoodBoardInterlockSubscription.Event += OnGoodBoardInterlockEvent;

            BadBoardInterlockSubscription = new Models.Exchange.GoodBadInterlockSubscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty };
            BadBoardInterlockSubscription.Event += OnBadBoardInterlockEvent;

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

            MachineReadyBlockEvent = new Models.Exchange.Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.MachineReady.Block",
                Description = "Machine Ready Interlock Blocking",
                Subjects = new[] { "blocked", "smema" }
            };

            GoodBoardBlockEvent = new Models.Exchange.Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.GoodBoard.Block",
                Description = "Good Board Interlock Blocking",
                Subjects = new[] { "blocked", "smema" }
            };

            BadBoardBlockEvent = new Models.Exchange.Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.BadBoard.Block",
                Description = "Bad Board Interlock Blocking",
                Subjects = new[] { "blocked", "smema" }
            };

            MachineReadyStateMachine = new InterlockMachineReadyStateMachine(
                m_SMEMAUplineStateMachine,
                m_SMEMADownlineStateMachine,
                MachineReadyEvent,
                MachineReadyBlockEvent);

            MachineReadyStateMachine.BlockedUpdated += OnBlockedStatusUpdated;

            BoardAvailableStateMachine = new InterlockBoardAvailableStateMachine(
                m_SMEMAUplineStateMachine,
                m_SMEMADownlineStateMachine,
                GoodBoardEvent,
                BadBoardEvent,
                GoodBoardBlockEvent,
                BadBoardBlockEvent);

            BoardAvailableStateMachine.BlockedUpdated += OnBlockedStatusUpdated;
        }

        private void OnBadBoardInterlockEvent(SubscriptionEvent theSubscriptionEvent)
        {
            if( PermissionInterfaceSubscriptions == false)
            {
                return;
            }

            foreach (var PayloadSubject in theSubscriptionEvent.PayloadSubjects)
            {
                if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.Unblock)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.Unblock))
                {
                    BoardAvailableStateMachine.BadBoard = true;
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.Block)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.Block))
                {
                    BoardAvailableStateMachine.BadBoard = false;
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.LatchOn)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.LatchOn))
                {
                    BoardAvailableStateMachine.BadBoardLatch = true;
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.LatchOff)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.LatchOff))
                {
                    BoardAvailableStateMachine.BadBoardLatch = false;
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.DivertOn)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.DivertOn))
                {
                    BoardAvailableStateMachine.BadBoardDivert = true;
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.DivertOff)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.DivertOff))
                {
                    BoardAvailableStateMachine.BadBoardDivert = false;
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.DivertLatchOn)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.DivertLatchOn))
                {
                    BoardAvailableStateMachine.BadBoardDivertLatch = true;
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.DivertLatchOff)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.DivertLatchOff))
                {
                    BoardAvailableStateMachine.BadBoardDivertLatch = false;
                }
            }
        }

        private void OnGoodBoardInterlockEvent(SubscriptionEvent theSubscriptionEvent)
        {
            if (PermissionInterfaceSubscriptions == false)
            {
                return;
            }

            foreach (var PayloadSubject in theSubscriptionEvent.PayloadSubjects)
            {
                if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.Unblock)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.Unblock))
                {
                    BoardAvailableStateMachine.GoodBoard = true;
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.Block)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.Block))
                {
                    BoardAvailableStateMachine.GoodBoard = false;
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.LatchOn)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.LatchOn))
                {
                    BoardAvailableStateMachine.GoodBoardLatch = true;
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.LatchOff)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.LatchOff))
                {
                    BoardAvailableStateMachine.GoodBoardLatch = false;
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.DivertOn)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.DivertOn))
                {
                    BoardAvailableStateMachine.GoodBoardDivert = true;
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.DivertOff)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.DivertOff))
                {
                    BoardAvailableStateMachine.GoodBoardDivert = false;
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.DivertLatchOn)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.DivertLatchOn))
                {
                    BoardAvailableStateMachine.GoodBoardDivertLatch = true;
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.DivertLatchOff)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.DivertLatchOff))
                {
                    BoardAvailableStateMachine.GoodBoardDivertLatch = false;
                }
            }

        }

        private void OnMachineReadyInterlockEvent(SubscriptionEvent theSubscriptionEvent)
        {
            if (PermissionInterfaceSubscriptions == false)
            {
                return;
            }

            foreach( var PayloadSubject in theSubscriptionEvent.PayloadSubjects)
            {
                if ((!string.IsNullOrEmpty(MachineReadyInterlockSubscription.Unblock)) && PayloadSubject.Value.Equals( MachineReadyInterlockSubscription.Unblock) )
                {
                    MachineReadyStateMachine.MachineReady = true;
                }
                else if ((!string.IsNullOrEmpty(MachineReadyInterlockSubscription.Block)) && PayloadSubject.Value.Equals(MachineReadyInterlockSubscription.Block))
                {
                    MachineReadyStateMachine.MachineReady = false;
                }
                else if (!string.IsNullOrEmpty((MachineReadyInterlockSubscription.LatchOn)) && PayloadSubject.Value.Equals(MachineReadyInterlockSubscription.LatchOn))
                {
                    MachineReadyStateMachine.Latch = true;
                }
                else if ((!string.IsNullOrEmpty(MachineReadyInterlockSubscription.LatchOff)) && PayloadSubject.Value.Equals(MachineReadyInterlockSubscription.LatchOff))
                {
                    MachineReadyStateMachine.Latch = false;
                }
            }
        }

        internal void Init()
        {
            if (m_RunOnce)
            {
                m_RunOnce = false;

                switch (StartupMachineReady)
                {
                    case 0:  // Blocked
                        MachineReadyStateMachine.Latch = false;
                        MachineReadyStateMachine.MachineReady = false;
                        break;
                    case 1: // Blocked and Latched
                        MachineReadyStateMachine.Latch = true;
                        MachineReadyStateMachine.MachineReady = false;
                        break;
                    case 2: // Unblocked and Latched
                        MachineReadyStateMachine.Latch = true;
                        MachineReadyStateMachine.MachineReady = true;
                        break;
                    case 3:
                        // Shutdown State
                        MachineReadyStateMachine.Latch = PersistentMachineReadyLatch;
                        MachineReadyStateMachine.MachineReady = PersistentMachineReady;
                        break;
                    default:
                        MachineReadyStateMachine.Latch = false;
                        MachineReadyStateMachine.MachineReady = false;
                        break;
                }

                switch (StartupGoodBoard)
                {
                    case 0:  // Blocked
                        BoardAvailableStateMachine.GoodBoardLatch = false;
                        BoardAvailableStateMachine.GoodBoard = false;
                        break;
                    case 1: // Blocked and Latched
                        BoardAvailableStateMachine.GoodBoardLatch = true;
                        BoardAvailableStateMachine.GoodBoard = false;
                        break;
                    case 2: // Unblocked and Latched
                        BoardAvailableStateMachine.GoodBoardLatch = true;
                        BoardAvailableStateMachine.GoodBoard = true;
                        break;
                    case 3:
                        // Shutdown State
                        BoardAvailableStateMachine.GoodBoardLatch = PersistentGoodBoardLatch;
                        BoardAvailableStateMachine.GoodBoard = PersistentGoodBoard;
                        break;
                    default:
                        BoardAvailableStateMachine.GoodBoardLatch = false;
                        BoardAvailableStateMachine.GoodBoard = false;
                        break;
                }

                switch (StartupBadBoard)
                {
                    case 0:  // Blocked
                        BoardAvailableStateMachine.BadBoardLatch = false;
                        BoardAvailableStateMachine.BadBoard = false;
                        break;
                    case 1: // Blocked and Latched
                        BoardAvailableStateMachine.BadBoardLatch = true;
                        BoardAvailableStateMachine.BadBoard = false;
                        break;
                    case 2: // Unblocked and Latched
                        BoardAvailableStateMachine.BadBoardLatch = true;
                        BoardAvailableStateMachine.BadBoard = true;
                        break;
                    case 3:
                        // Shutdown State
                        BoardAvailableStateMachine.BadBoardLatch = PersistentBadBoardLatch;
                        BoardAvailableStateMachine.BadBoard = PersistentBadBoard;
                        break;
                    default:
                        BoardAvailableStateMachine.BadBoardLatch = false;
                        BoardAvailableStateMachine.BadBoard = false;
                        break;
                }

                // Manually Sync values here as Event handlers for updates are added below
                PersistentMachineReady = MachineReadyStateMachine.MachineReady;
                PersistentMachineReadyLatch = MachineReadyStateMachine.Latch;
                PersistentGoodBoard = BoardAvailableStateMachine.GoodBoard;
                PersistentGoodBoardLatch = BoardAvailableStateMachine.GoodBoardLatch;
                PersistentBadBoard = BoardAvailableStateMachine.BadBoard;
                PersistentBadBoardLatch = BoardAvailableStateMachine.BadBoardLatch;

                // Now add Event handlers for future syncing updates
                MachineReadyStateMachine.MachineReadyUpdated += (value) => { PersistentMachineReady = value; };
                MachineReadyStateMachine.MachineReadyLatchUpdated += (value) => { PersistentMachineReadyLatch = value; };
                BoardAvailableStateMachine.GoodBoardUpdated += (value) => { PersistentGoodBoard = value; };
                BoardAvailableStateMachine.GoodBoardLatchedUpdated += (value) => { PersistentGoodBoardLatch = value; };
                BoardAvailableStateMachine.BadBoardUpdated += (value) => { PersistentBadBoard = value; };
                BoardAvailableStateMachine.BadBoardLatchedUpdated += (value) => { PersistentBadBoardLatch = value; };
            }
        }

        internal void UpdateProperties(InterlockProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (Subscription.Merge(MachineReadyInterlockSubscription, theNewProperties.MachineReadyInterlockSubscription)) { FlagSubscriptionUpdated = true; }

            MergeSubscription(MachineReadyInterlockSubscription, theNewProperties.MachineReadyInterlockSubscription);

            if (Subscription.Merge(GoodBoardInterlockSubscription, theNewProperties.GoodBoardInterlockSubscription)) { FlagSubscriptionUpdated = true; }

            MergeSubscription(GoodBoardInterlockSubscription, theNewProperties.GoodBoardInterlockSubscription);

            if (Subscription.Merge(BadBoardInterlockSubscription, theNewProperties.BadBoardInterlockSubscription)) { FlagSubscriptionUpdated = true; }

            MergeSubscription(BadBoardInterlockSubscription, theNewProperties.BadBoardInterlockSubscription);

            if (Event.Merge( MachineReadyBlockEvent, theNewProperties.MachineReadyBlockEvent)) { FlagEventUpdated = true; }

            MergeEvent(MachineReadyBlockEvent, theNewProperties.MachineReadyBlockEvent);

            if (Event.Merge(GoodBoardBlockEvent, theNewProperties.GoodBoardBlockEvent)) { FlagEventUpdated = true; }

            MergeEvent(GoodBoardBlockEvent, theNewProperties.GoodBoardBlockEvent);

            if (Event.Merge(BadBoardBlockEvent, theNewProperties.BadBoardBlockEvent)) { FlagEventUpdated = true; }

            MergeEvent(BadBoardBlockEvent, theNewProperties.BadBoardBlockEvent);

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }

            StartupMachineReady = theNewProperties.StartupMachineReady;
            StartupGoodBoard = theNewProperties.StartupGoodBoard;
            StartupBadBoard = theNewProperties.StartupBadBoard;

            if(theNewProperties.PermissionInterfaceUI != null)
            {
                PermissionInterfaceUI = theNewProperties.PermissionInterfaceUI;
            }

            if(theNewProperties.PermissionInterfaceREST != null)
            {
                PermissionInterfaceREST = theNewProperties.PermissionInterfaceREST;
            }

            if(theNewProperties.PermissionInterfaceSubscriptions != null)
            {
                PermissionInterfaceSubscriptions = theNewProperties.PermissionInterfaceSubscriptions;
            }

            if(m_RunOnce)
            {
                // Load values here temporary, to be loaded once in on Init()
                PersistentMachineReadyLatch = theNewProperties.PersistentMachineReadyLatch;
                PersistentMachineReady = theNewProperties.PersistentMachineReady;

                PersistentGoodBoardLatch = theNewProperties.PersistentGoodBoardLatch;
                PersistentGoodBoard = theNewProperties.PersistentGoodBoard;

                PersistentBadBoardLatch = theNewProperties.PersistentBadBoardLatch;
                PersistentBadBoard = theNewProperties.PersistentBadBoard;
            }
        }

        private void MergeEvent(Models.Exchange.Event Into, Models.Exchange.Event From )
        {
            if (From == null || Into == null)
            {
                return;
            }

            Into.BlockedEnabled = From.BlockedEnabled;
            Into.BlockedValue = From.BlockedValue;
            Into.UnblockedEnabled = From.UnblockedEnabled;
            Into.UnblockedValue = From.UnblockedValue;
        }

        private void MergeSubscription(Models.Exchange.MachineReadyInterlockSubscription Into, Models.Exchange.MachineReadyInterlockSubscription From)
        {
            if(From == null || Into == null)
            {
                return;
            }

            Into.Block = From.Block;
            Into.Unblock = From.Unblock;
            Into.LatchOn = From.LatchOn;
            Into.LatchOff = From.LatchOff;
        }

        private void MergeSubscription(Models.Exchange.GoodBadInterlockSubscription Into, Models.Exchange.GoodBadInterlockSubscription From)
        {
            if (From == null || Into == null)
            {
                return;
            }

            Into.Block = From.Block;
            Into.Unblock = From.Unblock;
            Into.LatchOn = From.LatchOn;
            Into.LatchOff = From.LatchOff;
            Into.DivertOn = From.DivertOn;
            Into.DivertOff = From.DivertOff;
            Into.DivertLatchOn = From.DivertLatchOn;
            Into.DivertLatchOff = From.DivertLatchOff;
        }

        private void OnBlockedStatusUpdated()
        {
            BlockedUpdated?.Invoke();
        }

        public bool Blocked
        {
            get
            {
                return MachineReadyStateMachine.Blocked || BoardAvailableStateMachine.Blocked;
            }
        }
    }
}
