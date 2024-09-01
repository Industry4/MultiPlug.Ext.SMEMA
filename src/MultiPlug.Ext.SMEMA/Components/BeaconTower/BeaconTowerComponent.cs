using System;
using System.Linq;
using System.Collections.Generic;
using MultiPlug.Ext.SMEMA.Models.Components.BeaconTower;
using MultiPlug.Ext.SMEMA.Models.Exchange;
using MultiPlug.Ext.SMEMA.Components.Interlock;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.BeaconTower
{
    public class BeaconTowerComponent : BeaconTowerProperties
    {
        internal event Action EventsUpdated;
        private InterlockComponent m_Interlock;
        private PropertyDelegateLookup[] m_FunctionPointers;
        private bool m_ShuttingDown;

        internal string[] OperandsList { get; private set; }

        public BeaconTowerComponent(string theGuid, InterlockComponent theInterlock)
        {
            m_Interlock = theInterlock;

            BeaconTowerEvents = new BeaconTowerEvent[0];

            m_Interlock.MachineReadyStateMachine.BlockedUpdated += RunLogic;

            m_Interlock.MachineReadyStateMachine.MachineReadyUpdated += (v) => { RunLogic(); };
            m_Interlock.MachineReadyStateMachine.MachineReadyLatchUpdated += (v) => { RunLogic(); };

            m_Interlock.BoardAvailableStateMachine.BlockedUpdated += RunLogic;

            m_Interlock.BoardAvailableStateMachine.GoodBoardUpdated += (v) => { RunLogic(); };
            m_Interlock.BoardAvailableStateMachine.GoodBoardLatchedUpdated += (v) => { RunLogic(); };

            m_Interlock.BoardAvailableStateMachine.BadBoardUpdated += (v) => { RunLogic(); };
            m_Interlock.BoardAvailableStateMachine.BadBoardLatchedUpdated += (v) => { RunLogic(); };

            m_Interlock.BoardAvailableStateMachine.FlipBoardUpdated += (v) => { RunLogic(); };
            m_Interlock.BoardAvailableStateMachine.FlipBoardLatchedUpdated += (v) => { RunLogic(); };

            m_Interlock.BoardAvailableStateMachine.GoodBoardDivertUpdated += (v) => { RunLogic(); };
            m_Interlock.BoardAvailableStateMachine.GoodBoardDivertLatchedUpdated += (v) => { RunLogic(); };

            m_Interlock.BoardAvailableStateMachine.BadBoardDivertUpdated += (v) => { RunLogic(); };
            m_Interlock.BoardAvailableStateMachine.BadBoardDivertLatchedUpdated += (v) => { RunLogic(); };

            //
            // DO NOT Reorder these. Only add to the end (Index used for configuration)
            //
            m_FunctionPointers = new PropertyDelegateLookup[]
            {
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return ! m_Interlock.MachineReadyStateMachine.MachineReady; }), Description = "Machine Ready Interlock (Red Cross)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return ! m_Interlock.BoardAvailableStateMachine.GoodBoard; }), Description = "Good Board Interlock (Red Cross)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return ! m_Interlock.BoardAvailableStateMachine.BadBoard; }), Description = "Bad Board Interlock (Red Cross)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return ! m_Interlock.BoardAvailableStateMachine.FlipBoard; }), Description = "Flip Board Interlock (Red Cross)" },

                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.MachineReadyStateMachine.Latch; }), Description = "Machine Ready Latched (Lock Icon)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.GoodBoardLatch; }), Description = "Good Board Latched (Lock Icon)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.GoodBoardDivertLatch; }), Description = "Good Board Divert Latched (Lock Icon)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.BadBoardLatch; }), Description = "Bad Board Latched (Lock Icon)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.BadBoardDivertLatch; }), Description = "Bad Board Divert Latched (Lock Icon)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.FlipBoardLatch; }), Description = "Flip Board Latched (Lock Icon)" },

                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.GoodBoardDivert; }), Description = "Good Board Diverted (Yellow Arrow)" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.BadBoardDivert; }), Description = "Bad Board Diverted (Yellow Arrow)" },

                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.MachineReadyStateMachine.Blocked; }), Description = "Machine Ready Blocked" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.GoodBoardBlocked; }), Description = "Good Board Blocked" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.BadBoardBlocked; }), Description = "Bad Board Blocked" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_Interlock.BoardAvailableStateMachine.FlipBoardBlocked; }), Description = "Flip Board Blocked" },
                new PropertyDelegateLookup { FunctionDelegate =  new Func<bool>( () =>{ return m_ShuttingDown; }), Description = "User initiated system shutdown " },
            };

            OperandsList = m_FunctionPointers.Select(o => o.Description).ToArray();
        }

        internal void UpdateProperties(BeaconTowerProperties theNewProperties)
        {
            bool FlagEventUpdated = false;

            if (theNewProperties.BeaconTowerEvents != null)
            {
                var BeaconTowerEventsList = new List<BeaconTowerEvent>(BeaconTowerEvents);
                var NewBeaconTowerEvents = new List<BeaconTowerEvent>();

                foreach (var BeaconTowerEvent in theNewProperties.BeaconTowerEvents)
                {
                    if (string.IsNullOrEmpty(BeaconTowerEvent.Guid))
                    {
                        if (!string.IsNullOrEmpty(BeaconTowerEvent.Id) && BeaconTowerEventsList.FirstOrDefault(s => s.Id == BeaconTowerEvent.Id) == null)
                        {
                            BeaconTowerEvent.Guid = Guid.NewGuid().ToString();

                            NewBeaconTowerEvents.Add(BeaconTowerEvent);
                            FlagEventUpdated = true;
                        }
                    }
                    else
                    {
                        var ExistingSubSearch = BeaconTowerEventsList.FirstOrDefault(s => s.Guid == BeaconTowerEvent.Guid);

                        if (ExistingSubSearch == null)
                        {
                            if (!string.IsNullOrEmpty(BeaconTowerEvent.Id) && BeaconTowerEventsList.FirstOrDefault(s => s.Id == BeaconTowerEvent.Id) == null)
                            {
                                NewBeaconTowerEvents.Add(BeaconTowerEvent);
                                FlagEventUpdated = true;
                            }
                        }
                        else
                        {
                            if (BeaconTowerEvent.Merge(ExistingSubSearch, BeaconTowerEvent)) { FlagEventUpdated = true; }
                        }
                    }
                }

                if (NewBeaconTowerEvents.Any())
                {
                    BeaconTowerEventsList.AddRange(NewBeaconTowerEvents.ToArray());
                }

                BeaconTowerEvents = BeaconTowerEventsList.ToArray();
            }

            if (FlagEventUpdated)
            {
                EventsUpdated?.Invoke();
            }
        }

        internal void Init()
        {
            RunLogic();
        }

        internal void Shutdown()
        {
            if( ! m_ShuttingDown)
            {
                m_ShuttingDown = true;
                RunLogic();
            }
        }

        internal bool DeleteEvent(string theEventGuid)
        {
            var BeaconTowerEventsList = BeaconTowerEvents.ToList();

            var Search = BeaconTowerEvents.FirstOrDefault(s => s.Guid == theEventGuid);

            if(Search != null)
            {
                BeaconTowerEventsList.Remove(Search);
                BeaconTowerEvents = BeaconTowerEventsList.ToArray();
                EventsUpdated?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void ExecuteEvent(string theEventGuid)
        {
            var BeaconTowerEventsList = BeaconTowerEvents.ToList();

            var EventSearch = BeaconTowerEventsList.FirstOrDefault(s => s.Guid == theEventGuid);

            if (EventSearch != null)
            {
                ProcessEvent(EventSearch);
            }
        }

        private void RunLogic()
        {
            BeaconTowerEvent[] Events = BeaconTowerEvents;

            foreach (var theEvent in Events)
            {
                ProcessEvent(theEvent);
            }
        }

        private bool OutOfRangeCheck(int theIndex)
        {
            return m_FunctionPointers.Length > 0 && theIndex >= 0 && theIndex < m_FunctionPointers.Length;
        }

        private void ProcessEvent(BeaconTowerEvent theEvent)
        {
            bool? Result = null;

            foreach (BeaconTowerEventSequenceStep step in theEvent.RuleSteps)
            {
                if(Result.HasValue) // Has Run at least once. Should we continue?
                {
                    if(Result.Value) // Result is True so far
                    {
                        if (step.Operator == 1) // OR
                        {
                            break; // No need to continue as Left hand side of OR is already true.
                        }
                    }
                    else // Result is False already
                    {
                        if(step.Operator == 0) // AND, Looking for a OR to restart. If never found then result will remain false.
                        {
                            continue;
                        }
                    }
                }

                if (step.Truth == 0) // True
                {
                    if( ! OutOfRangeCheck( step.Operand ) )
                    {
                        Result = false;
                    }
                    else
                    {
                        Result = m_FunctionPointers[step.Operand].FunctionDelegate();
                    }
                }
                else if (step.Truth == 1) // False
                {
                    if ( ! OutOfRangeCheck( step.Operand ) )
                    {
                        Result = false;
                    }
                    else
                    {
                        Result = ! m_FunctionPointers[step.Operand].FunctionDelegate();
                    }
                }
            }

            if( ! Result.HasValue ) // RuleSteps length was zero
            {
                return;
            }

            if (Result.Value)
            {
                if ( ( ! theEvent.LastState.HasValue ) || ( ! theEvent.LastState.Value ) )
                {
                    theEvent.LastState = Result;

                    if (TransitionDelays.DelayInProgress(theEvent.FalseEventCancellationSource))
                    {
                        TransitionDelays.DelayCancel(theEvent.FalseEventCancellationSource);
                    }
                    else
                    {
                        if (theEvent.TrueEnabled.Value)
                        {
                            theEvent.TrueEventCancellationSource = TransitionDelays.InvokeEvent(() =>
                            {
                                theEvent.Invoke(new Payload(theEvent.Id, new PayloadSubject[] {
                                            new PayloadSubject(theEvent.Subjects[0], theEvent.TrueValue)
                            }));
                            },
                            theEvent.TrueEventCancellationSource,
                            theEvent.TrueDelay.Value);
                        }
                    }
                }

            }
            else
            {
                if ( ( ! theEvent.LastState.HasValue ) || ( theEvent.LastState.Value ) )
                {
                    theEvent.LastState = Result;

                    if (TransitionDelays.DelayInProgress(theEvent.TrueEventCancellationSource))
                    {
                        TransitionDelays.DelayCancel(theEvent.TrueEventCancellationSource);
                    }
                    else
                    {
                        if (theEvent.FalseEnabled.Value)
                        {
                            theEvent.FalseEventCancellationSource = TransitionDelays.InvokeEvent(() =>
                            {
                                theEvent.Invoke(new Payload(theEvent.Id, new PayloadSubject[] {
                                            new PayloadSubject(theEvent.Subjects[0], theEvent.FalseValue)
                            }));
                            },
                            theEvent.FalseEventCancellationSource,
                            theEvent.FalseDelay.Value);
                        }
                    }
                }
            }
        }
    }
}
