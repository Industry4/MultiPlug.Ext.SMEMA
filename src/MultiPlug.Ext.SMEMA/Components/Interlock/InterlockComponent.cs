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
            // Defaults Start - Set them here and not in InterlockProperties
            StartupMachineReady = 0;
            StartupGoodBoard = 0;
            StartupBadBoard = 0;
            StartupFlipBoard = 0;
            PermissionInterfaceUI = true;
            PermissionInterfaceREST = true;
            PermissionInterfaceSubscriptions = true;
            TriggerBlockGoodBoardOnMachineNotReady = false;
            TriggerBlockBadBoardOnMachineNotReady = false;
            TriggerBlockFlipBoardOnMachineNotReady = false;
            TriggerBlockFlipBoardOnGoodBoardNotAvailable = false;
            TriggerBlockFlipBoardOnBadBoardNotAvailable = false;
            DelayFlipThenBoardAvailable = 0;
            // Defaults End

            MachineReadyInterlockSubscription = new Models.Exchange.MachineReadyAndFlipInterlockSubscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty };
            MachineReadyInterlockSubscription.Event += OnMachineReadyInterlockEvent;

            GoodBoardInterlockSubscription = new Models.Exchange.GoodBadInterlockSubscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty };
            GoodBoardInterlockSubscription.Event += OnGoodBoardInterlockEvent;

            BadBoardInterlockSubscription = new Models.Exchange.GoodBadInterlockSubscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty };
            BadBoardInterlockSubscription.Event += OnBadBoardInterlockEvent;

            FlipBoardInterlockSubscription = new Models.Exchange.MachineReadyAndFlipInterlockSubscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty };
            FlipBoardInterlockSubscription.Event += OnFlipBoardInterlockEvent;


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
            FlipBoardEvent = new Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.FlipBoard",
                Description = "Flip Board Interlock",
                Subjects = new[] { "value", "enabled", "interlock-latched" }
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

            FlipBoardBlockEvent = new Models.Exchange.Event
            {
                Guid = Guid.NewGuid().ToString(),
                Id = "Interlock.FlipBoard.Block",
                Description = "Flip Board Interlock Blocking",
                Subjects = new[] { "blocked", "smema" },
                BlockedEnabled = false,
                UnblockedEnabled = false
            };

            MachineReadyStateMachine = new InterlockMachineReadyStateMachine(
                m_SMEMAUplineStateMachine,
                m_SMEMADownlineStateMachine,
                MachineReadyEvent,
                MachineReadyBlockEvent);

            MachineReadyStateMachine.BlockedUpdated += OnBlockedStatusUpdated;

            BoardAvailableStateMachine = new InterlockBoardAvailableStateMachine(
                this,
                m_SMEMAUplineStateMachine,
                m_SMEMADownlineStateMachine,
                GoodBoardEvent,
                BadBoardEvent,
                FlipBoardEvent,
                GoodBoardBlockEvent,
                BadBoardBlockEvent,
                FlipBoardBlockEvent);

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
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.UnblockFlipOn)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.UnblockFlipOn))
                {
                    BoardAvailableStateMachine.BadBoardFlip(true);
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.BlockFlipOff)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.BlockFlipOff))
                {
                    BoardAvailableStateMachine.BadBoardFlip(false);
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.DivertOnFlipOn)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.DivertOnFlipOn))
                {
                    BoardAvailableStateMachine.BadBoardDivertFlip(true);
                }
                else if ((!string.IsNullOrEmpty(BadBoardInterlockSubscription.DivertOffFlipOff)) && PayloadSubject.Value.Equals(BadBoardInterlockSubscription.DivertOffFlipOff))
                {
                    BoardAvailableStateMachine.BadBoardDivertFlip(false);
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
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.UnblockFlipOn)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.UnblockFlipOn))
                {
                    BoardAvailableStateMachine.GoodBoardFlip(true);
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.BlockFlipOff)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.BlockFlipOff))
                {
                    BoardAvailableStateMachine.GoodBoardFlip(false);
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.DivertOnFlipOn)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.DivertOnFlipOn))
                {
                    BoardAvailableStateMachine.GoodBoardDivertFlip(true);
                }
                else if ((!string.IsNullOrEmpty(GoodBoardInterlockSubscription.DivertOffFlipOff)) && PayloadSubject.Value.Equals(GoodBoardInterlockSubscription.DivertOffFlipOff))
                {
                    BoardAvailableStateMachine.GoodBoardDivertFlip(false);
                }
            }
        }

        private void OnFlipBoardInterlockEvent(SubscriptionEvent theSubscriptionEvent)
        {
            if (PermissionInterfaceSubscriptions == false)
            {
                return;
            }

            foreach (var PayloadSubject in theSubscriptionEvent.PayloadSubjects)
            {
                if ((!string.IsNullOrEmpty(FlipBoardInterlockSubscription.Unblock)) && PayloadSubject.Value.Equals(FlipBoardInterlockSubscription.Unblock))
                {
                    BoardAvailableStateMachine.FlipBoard = true;
                }
                else if ((!string.IsNullOrEmpty(FlipBoardInterlockSubscription.Block)) && PayloadSubject.Value.Equals(FlipBoardInterlockSubscription.Block))
                {
                    BoardAvailableStateMachine.FlipBoard = false;
                }
                else if (!string.IsNullOrEmpty((FlipBoardInterlockSubscription.LatchOn)) && PayloadSubject.Value.Equals(FlipBoardInterlockSubscription.LatchOn))
                {
                    BoardAvailableStateMachine.FlipBoardLatch = true;
                }
                else if ((!string.IsNullOrEmpty(FlipBoardInterlockSubscription.LatchOff)) && PayloadSubject.Value.Equals(FlipBoardInterlockSubscription.LatchOff))
                {
                    BoardAvailableStateMachine.FlipBoardLatch = false;
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

                switch (StartupFlipBoard)
                {
                    case 0:  // Blocked
                        BoardAvailableStateMachine.FlipBoardLatch = false;
                        BoardAvailableStateMachine.FlipBoard = false;
                        break;
                    case 1: // Blocked and Latched
                        BoardAvailableStateMachine.FlipBoardLatch = true;
                        BoardAvailableStateMachine.FlipBoard = false;
                        break;
                    case 2: // Unblocked and Latched
                        BoardAvailableStateMachine.FlipBoardLatch = true;
                        BoardAvailableStateMachine.FlipBoard = true;
                        break;
                    case 3:
                        // Shutdown State
                        BoardAvailableStateMachine.FlipBoardLatch = PersistentFlipBoardLatch;
                        BoardAvailableStateMachine.FlipBoard = PersistentFlipBoard;
                        break;
                    default:
                        BoardAvailableStateMachine.FlipBoardLatch = false;
                        BoardAvailableStateMachine.FlipBoard = false;
                        break;
                }

                // Manually Sync values here as Event handlers for updates are added below
                PersistentMachineReady = MachineReadyStateMachine.MachineReady;
                PersistentMachineReadyLatch = MachineReadyStateMachine.Latch;
                PersistentGoodBoard = BoardAvailableStateMachine.GoodBoard;
                PersistentGoodBoardLatch = BoardAvailableStateMachine.GoodBoardLatch;
                PersistentBadBoard = BoardAvailableStateMachine.BadBoard;
                PersistentBadBoardLatch = BoardAvailableStateMachine.BadBoardLatch;
                PersistentFlipBoard = BoardAvailableStateMachine.FlipBoard;
                PersistentFlipBoardLatch = BoardAvailableStateMachine.FlipBoardLatch;

                // Now add Event handlers for future syncing updates
                MachineReadyStateMachine.MachineReadyUpdated += (value) => { PersistentMachineReady = value; };
                MachineReadyStateMachine.MachineReadyLatchUpdated += (value) => { PersistentMachineReadyLatch = value; };
                BoardAvailableStateMachine.GoodBoardUpdated += (value) => { PersistentGoodBoard = value; };
                BoardAvailableStateMachine.GoodBoardLatchedUpdated += (value) => { PersistentGoodBoardLatch = value; };
                BoardAvailableStateMachine.BadBoardUpdated += (value) => { PersistentBadBoard = value; };
                BoardAvailableStateMachine.BadBoardLatchedUpdated += (value) => { PersistentBadBoardLatch = value; };
                BoardAvailableStateMachine.FlipBoardUpdated += (value) => { PersistentFlipBoard = value; };
                BoardAvailableStateMachine.FlipBoardLatchedUpdated += (value) => { PersistentFlipBoardLatch = value; };
            }
        }

        internal void UpdateProperties(InterlockProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (theNewProperties.MachineReadyInterlockSubscription != null)
            {
                if (Subscription.Merge(MachineReadyInterlockSubscription, theNewProperties.MachineReadyInterlockSubscription)) { FlagSubscriptionUpdated = true; }
                MergeSubscription(MachineReadyInterlockSubscription, theNewProperties.MachineReadyInterlockSubscription);
            }

            if (theNewProperties.GoodBoardInterlockSubscription != null)
            {
                if (Subscription.Merge(GoodBoardInterlockSubscription, theNewProperties.GoodBoardInterlockSubscription)) { FlagSubscriptionUpdated = true; }
                MergeSubscription(GoodBoardInterlockSubscription, theNewProperties.GoodBoardInterlockSubscription);
            }

            if (theNewProperties.BadBoardInterlockSubscription != null)
            {
                if (Subscription.Merge(BadBoardInterlockSubscription, theNewProperties.BadBoardInterlockSubscription)) { FlagSubscriptionUpdated = true; }
                MergeSubscription(BadBoardInterlockSubscription, theNewProperties.BadBoardInterlockSubscription);
            }

            if (theNewProperties.FlipBoardInterlockSubscription != null)
            {
                if (Subscription.Merge(FlipBoardInterlockSubscription, theNewProperties.FlipBoardInterlockSubscription)) { FlagSubscriptionUpdated = true; }
                MergeSubscription(FlipBoardInterlockSubscription, theNewProperties.FlipBoardInterlockSubscription);
            }

            if (theNewProperties.MachineReadyBlockEvent != null)
            {
                if (Event.Merge(MachineReadyBlockEvent, theNewProperties.MachineReadyBlockEvent)) { FlagEventUpdated = true; }
                MergeEvent(MachineReadyBlockEvent, theNewProperties.MachineReadyBlockEvent);
            }

            if (theNewProperties.GoodBoardBlockEvent != null)
            {
                if (Event.Merge(GoodBoardBlockEvent, theNewProperties.GoodBoardBlockEvent)) { FlagEventUpdated = true; }
                MergeEvent(GoodBoardBlockEvent, theNewProperties.GoodBoardBlockEvent);
            }

            if (theNewProperties.BadBoardBlockEvent != null)
            {
                if (Event.Merge(BadBoardBlockEvent, theNewProperties.BadBoardBlockEvent)) { FlagEventUpdated = true; }
                MergeEvent(BadBoardBlockEvent, theNewProperties.BadBoardBlockEvent);
            }

            if(theNewProperties.FlipBoardBlockEvent != null)
            {
                if (Event.Merge(FlipBoardBlockEvent, theNewProperties.FlipBoardBlockEvent)) { FlagEventUpdated = true; }
                MergeEvent(FlipBoardBlockEvent, theNewProperties.FlipBoardBlockEvent);
            }

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }

            if(theNewProperties.StartupMachineReady != null)
            {
                StartupMachineReady = theNewProperties.StartupMachineReady;
            }

            if(theNewProperties.StartupGoodBoard != null)
            {
                StartupGoodBoard = theNewProperties.StartupGoodBoard;
            }

            if(theNewProperties.StartupBadBoard != null)
            {
                StartupBadBoard = theNewProperties.StartupBadBoard;
            }

            if(theNewProperties.StartupFlipBoard != null)
            {
                StartupFlipBoard = theNewProperties.StartupFlipBoard;
            }

            if (theNewProperties.PermissionInterfaceUI != null)
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

            if (theNewProperties.TriggerBlockGoodBoardOnMachineNotReady != null)
            {
                TriggerBlockGoodBoardOnMachineNotReady = theNewProperties.TriggerBlockGoodBoardOnMachineNotReady;
            }

            if (theNewProperties.TriggerBlockGoodBoardOnMachineNotReady != null)
            {
                TriggerBlockBadBoardOnMachineNotReady = theNewProperties.TriggerBlockBadBoardOnMachineNotReady;
            }

            if (theNewProperties.TriggerBlockFlipBoardOnMachineNotReady != null)
            {
                TriggerBlockFlipBoardOnMachineNotReady = theNewProperties.TriggerBlockFlipBoardOnMachineNotReady;
            }

            if (theNewProperties.TriggerBlockFlipBoardOnGoodBoardNotAvailable != null)
            {
                TriggerBlockFlipBoardOnGoodBoardNotAvailable = theNewProperties.TriggerBlockFlipBoardOnGoodBoardNotAvailable;
            }

            if (theNewProperties.TriggerBlockFlipBoardOnBadBoardNotAvailable != null)
            {
                TriggerBlockFlipBoardOnBadBoardNotAvailable = theNewProperties.TriggerBlockFlipBoardOnBadBoardNotAvailable;
            }

            if(theNewProperties.DelayFlipThenBoardAvailable != null)
            {
                DelayFlipThenBoardAvailable = theNewProperties.DelayFlipThenBoardAvailable;
            }

            if (m_RunOnce)
            {
                // Load values here temporary, to be loaded once in on Init()
                PersistentMachineReadyLatch = theNewProperties.PersistentMachineReadyLatch;
                PersistentMachineReady = theNewProperties.PersistentMachineReady;

                PersistentGoodBoardLatch = theNewProperties.PersistentGoodBoardLatch;
                PersistentGoodBoard = theNewProperties.PersistentGoodBoard;

                PersistentBadBoardLatch = theNewProperties.PersistentBadBoardLatch;
                PersistentBadBoard = theNewProperties.PersistentBadBoard;

                PersistentFlipBoardLatch = theNewProperties.PersistentFlipBoardLatch;
                PersistentFlipBoard = theNewProperties.PersistentFlipBoard;
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

        private void MergeSubscription(Models.Exchange.MachineReadyAndFlipInterlockSubscription Into, Models.Exchange.MachineReadyAndFlipInterlockSubscription From)
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
            Into.UnblockFlipOn = From.UnblockFlipOn;
            Into.BlockFlipOff = From.BlockFlipOff;
            Into.DivertOnFlipOn = From.DivertOnFlipOn;
            Into.DivertOffFlipOff = From.DivertOffFlipOff;
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
