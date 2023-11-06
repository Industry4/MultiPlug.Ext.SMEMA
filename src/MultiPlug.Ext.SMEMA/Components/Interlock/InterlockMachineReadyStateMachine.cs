using System;
using System.Threading;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Components.Utils;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockMachineReadyStateMachine
    {
        private InterlockSMEMAStateMachine m_SMEMAUplineStateMachine;
        internal InterlockSMEMAStateMachine SMEMADownlineStateMachine { get; private set; }

        private const string c_ReadyDescription = "ready";

        private Event m_MachineReadyEvent;

        internal event Action<bool> MachineReadyUpdated;
        internal event Action<bool> MachineReadyLatchUpdated;

        private Models.Exchange.Event m_MachineReadyBlockEvent;

        internal event Action BlockedUpdated;

        private CancellationTokenSource m_MachineReadyUnblockDelayCancellationSource;

        internal bool Blocked { get; private set; }

        private bool m_MachineReady;
        private bool m_MachineReadyLatch;

        private InterlockProperties m_InterlockProperties;

        public InterlockMachineReadyStateMachine(   InterlockProperties theProperties,
                                                    InterlockSMEMAStateMachine theSMEMAUplineStateMachine,
                                                    InterlockSMEMAStateMachine theSMEMADownlineStateMachine,
                                                    Event theMachineReadyEvent,
                                                    Models.Exchange.Event theMachineReadyBlockEvent)
        {
            m_InterlockProperties = theProperties;
            m_SMEMAUplineStateMachine = theSMEMAUplineStateMachine;
            SMEMADownlineStateMachine = theSMEMADownlineStateMachine;
            m_MachineReadyEvent = theMachineReadyEvent;
            m_MachineReadyBlockEvent = theMachineReadyBlockEvent;

            m_SMEMAUplineStateMachine.MachineReady.Updated += MachineReadyStateUpdated; 
        }

        private void InvokeMachineReadyEvent()
        {
            m_MachineReadyEvent.Invoke(new Payload(m_MachineReadyEvent.Id, new PayloadSubject[] {
            new PayloadSubject(m_MachineReadyEvent.Subjects[0], GetStringValue.Invoke(MachineReady)),
            new PayloadSubject(m_MachineReadyEvent.Subjects[1], GetStringValue.Invoke(MachineReady)),
            new PayloadSubject(m_MachineReadyEvent.Subjects[2], GetStringValue.Invoke(Latch))
            }));
        }

        private void InvokeMachineReadyBlockEvent()
        {
            if(m_MachineReadyBlockEvent.Subjects.Length != 2) // Safety Net
            {
                return;
            }

            if(Blocked && m_MachineReadyBlockEvent.BlockedEnabled)
            {
                m_MachineReadyBlockEvent.Invoke(new Payload(m_MachineReadyBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_MachineReadyBlockEvent.Subjects[0], m_MachineReadyBlockEvent.BlockedValue),
                new PayloadSubject(m_MachineReadyBlockEvent.Subjects[1], c_ReadyDescription)
                }));
            }
            else if( !Blocked && m_MachineReadyBlockEvent.UnblockedEnabled)
            {
                m_MachineReadyBlockEvent.Invoke(new Payload(m_MachineReadyBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_MachineReadyBlockEvent.Subjects[0], m_MachineReadyBlockEvent.UnblockedValue),
                new PayloadSubject(m_MachineReadyBlockEvent.Subjects[1], c_ReadyDescription)
                }));
            }
        }

        private bool SetBlocked(bool theValue)
        {
            if (theValue != Blocked)
            {
                Blocked = theValue;
                InvokeMachineReadyBlockEvent();
                BlockedUpdated?.Invoke();
                return true;
            }
            return false;
        }

        internal void OnSMEMAIOMachineReady(bool theIOState)
        {
            m_SMEMAUplineStateMachine.MachineReady.Value = theIOState;
        }

        private void MachineReadyStateUpdated(bool theIOState)
        {
            if (theIOState)
            {
                if (MachineReady)
                {
                    SetBlocked(false);
                    SMEMADownlineStateMachine.MachineReady.Value = true;
                }
                else
                {
                    TransitionDelays.UnblockCancel(m_MachineReadyUnblockDelayCancellationSource);
                    SetBlocked(true);
                }
            }
            else
            {
                TransitionDelays.UnblockCancel(m_MachineReadyUnblockDelayCancellationSource);
                SetBlocked(false);

                if (!Latch)
                {
                    MachineReady = false;
                }
                SMEMADownlineStateMachine.MachineReady.Value = false;
            }
        }

        internal bool MachineReady
        {
            get
            {
                return m_MachineReady;
            }
            set
            {
                if (m_MachineReady != value)
                {
                    m_MachineReady = value;
                    MachineReadyUpdated?.Invoke(m_MachineReady);

                    if (m_MachineReady && m_SMEMAUplineStateMachine.MachineReady.Value)
                    {
                        if (SetBlocked(false))
                        {
                            m_MachineReadyUnblockDelayCancellationSource = TransitionDelays.Unblock(() =>
                            { SMEMADownlineStateMachine.MachineReady.Value = true; },
                                m_MachineReadyUnblockDelayCancellationSource,
                                m_InterlockProperties.DelayMachineReadyUnblockedThenSMEMA.Value);
                        }
                        else
                        {
                            // Don't delay if already Unblocked
                            SMEMADownlineStateMachine.MachineReady.Value = true;
                        }
                    }
                    else if(m_SMEMAUplineStateMachine.MachineReady.Value)
                    {
                        TransitionDelays.UnblockCancel(m_MachineReadyUnblockDelayCancellationSource);
                        SetBlocked(true);
                        SMEMADownlineStateMachine.MachineReady.Value = false;
                    }

                    InvokeMachineReadyEvent();
                }
            }
        }

        internal bool Latch
        {
            get
            {
                return m_MachineReadyLatch;
            }

            set
            {
                if (m_MachineReadyLatch != value)
                {
                    m_MachineReadyLatch = value;
                    MachineReadyLatchUpdated?.Invoke(m_MachineReadyLatch);
                    InvokeMachineReadyEvent();
                }
            }
        }
    }
}
