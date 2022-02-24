using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockMachineReadyStateMachine
    {
        private InterlockSMEMAStateMachine m_SMEMAUplineStateMachine;
        private InterlockSMEMAStateMachine m_SMEMADownlineStateMachine;

        private const string c_ReadyDescription = "ready";

        private Event m_MachineReadyEvent;

        internal event Action<bool> MachineReadyUpdated;

        private Models.Exchange.Event m_MachineReadyBlockEvent;

        internal event Action BlockedUpdated;
        internal bool Blocked { get; private set; }

        private bool m_MachineReady;
        private bool m_MachineReadyLatch;

        public InterlockMachineReadyStateMachine(   InterlockSMEMAStateMachine theSMEMAUplineStateMachine,
                                                    InterlockSMEMAStateMachine theSMEMADownlineStateMachine,
                                                    Event theMachineReadyEvent,
                                                    Models.Exchange.Event theMachineReadyBlockEvent)
        {
            this.m_SMEMAUplineStateMachine = theSMEMAUplineStateMachine;
            this.m_SMEMADownlineStateMachine = theSMEMADownlineStateMachine;
            this.m_MachineReadyEvent = theMachineReadyEvent;
            this.m_MachineReadyBlockEvent = theMachineReadyBlockEvent;
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

        private void SetBlocked(bool theValue)
        {
            if (theValue != Blocked)
            {
                Blocked = theValue;
                InvokeMachineReadyBlockEvent();
                BlockedUpdated?.Invoke();
            }
        }

        internal void OnSMEMAIOMachineReady(bool theIOState)
        {
            m_SMEMAUplineStateMachine.MachineReady = theIOState;

            if (theIOState)
            {
                if (MachineReady)
                {
                    SetBlocked(false);
                    m_SMEMADownlineStateMachine.MachineReady = true;
                    MachineReadyUpdated?.BeginInvoke(theIOState, MachineReadyUpdated.EndInvoke, null);
                }
                else
                {
                    SetBlocked(true);
                }
            }
            else
            {
                SetBlocked(false);

                if (!Latch)
                {
                    MachineReady = false;
                }
                m_SMEMADownlineStateMachine.MachineReady = false;
                MachineReadyUpdated?.BeginInvoke(theIOState, MachineReadyUpdated.EndInvoke, null);
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

                    if (m_MachineReady && m_SMEMAUplineStateMachine.MachineReady)
                    {
                        SetBlocked(false);
                        m_SMEMADownlineStateMachine.MachineReady = true;
                        MachineReadyUpdated?.BeginInvoke(m_MachineReady, MachineReadyUpdated.EndInvoke, null);
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
                    InvokeMachineReadyEvent();
                }
            }
        }
    }
}
