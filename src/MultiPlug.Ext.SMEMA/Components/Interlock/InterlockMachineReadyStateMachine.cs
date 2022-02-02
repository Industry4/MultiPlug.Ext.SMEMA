using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockMachineReadyStateMachine
    {
        private InterlockSMEMAStateMachine m_SMEMAStateMachine;

        private Event m_MachineReadyEvent;

        internal event Action<bool> MachineReadyUpdated;

        private bool m_MachineReady;
        private bool m_MachineReadyLatch;

        public InterlockMachineReadyStateMachine(InterlockSMEMAStateMachine theSMEMAStateMachine, Event machineReadyEvent)
        {
            this.m_SMEMAStateMachine = theSMEMAStateMachine;
            this.m_MachineReadyEvent = machineReadyEvent;
        }

        private void InvokeMachineReadyEvent()
        {
            m_MachineReadyEvent.Invoke(new Payload(m_MachineReadyEvent.Id, new PayloadSubject[] {
            new PayloadSubject(m_MachineReadyEvent.Subjects[0], GetStringValue.Invoke(MachineReady)),
            new PayloadSubject(m_MachineReadyEvent.Subjects[1], GetStringValue.Invoke(MachineReady)),
            new PayloadSubject(m_MachineReadyEvent.Subjects[2], GetStringValue.Invoke(Latch))
            }));
        }

        internal void OnSMEMAIOMachineReady(bool theIOState)
        {
            m_SMEMAStateMachine.MachineReady = theIOState;

            if (theIOState)
            {

                if (MachineReady)
                {
                    MachineReadyUpdated?.BeginInvoke(theIOState, MachineReadyUpdated.EndInvoke, null);
                }
            }
            else
            {
                if (!Latch)
                {
                    MachineReady = false;
                }
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

                    if (m_MachineReady && m_SMEMAStateMachine.MachineReady)
                    {
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
