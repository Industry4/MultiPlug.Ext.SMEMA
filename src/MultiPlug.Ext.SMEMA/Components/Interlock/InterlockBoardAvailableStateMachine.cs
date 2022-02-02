using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockBoardAvailableStateMachine
    {
        private InterlockSMEMAStateMachine m_SMEMAStateMachine;

        private Event m_BadBoardEvent;
        private Event m_GoodBoardEvent;
        private bool m_GoodBoard;
        private bool m_GoodBoardLatch;
        private bool m_BadBoard;
        private bool m_BadBoardLatch;

        internal event Action<bool> GoodBoardUpdated;
        internal event Action<bool> BadBoardUpdated;

        public InterlockBoardAvailableStateMachine(InterlockSMEMAStateMachine theSMEMAStateMachine, Event goodBoardEvent, Event badBoardEvent)
        {
            this.m_SMEMAStateMachine = theSMEMAStateMachine;
            this.m_GoodBoardEvent = goodBoardEvent;
            this.m_BadBoardEvent = badBoardEvent;
        }

        internal void OnSMEMAIOGoodBoard(bool theIOState)
        {
            m_SMEMAStateMachine.GoodBoard = theIOState;

            if (theIOState)
            {
                if (GoodBoard)
                {
                    GoodBoardUpdated?.BeginInvoke(theIOState, GoodBoardUpdated.EndInvoke, null);
                }
            }
            else
            {
                if (!GoodBoardLatch)
                {
                    GoodBoard = false;
                }
                GoodBoardUpdated?.BeginInvoke(theIOState, GoodBoardUpdated.EndInvoke, null);
            }
        }

        internal void OnSMEMAIOBadBoard(bool theIOState)
        {
            m_SMEMAStateMachine.BadBoard = theIOState;

            if (theIOState)
            {

                if (BadBoard)
                {
                    BadBoardUpdated?.BeginInvoke(theIOState, BadBoardUpdated.EndInvoke, null);
                }
            }
            else
            {
                if (!BadBoardLatch)
                {
                    BadBoard = false;
                }
                BadBoardUpdated?.BeginInvoke(theIOState, BadBoardUpdated.EndInvoke, null);
            }
        }

        private void InvokeGoodBoardEvent()
        {
            m_GoodBoardEvent.Invoke(new Payload(m_GoodBoardEvent.Id, new PayloadSubject[] {
            new PayloadSubject(m_GoodBoardEvent.Subjects[0], GetStringValue.Invoke(GoodBoard)),
            new PayloadSubject(m_GoodBoardEvent.Subjects[1], GetStringValue.Invoke(GoodBoard)),
            new PayloadSubject(m_GoodBoardEvent.Subjects[2], GetStringValue.Invoke(GoodBoardLatch))
            }));
        }

        private void InvokeBadBoardEvent()
        {
            m_BadBoardEvent.Invoke(new Payload(m_BadBoardEvent.Id, new PayloadSubject[] {
            new PayloadSubject(m_BadBoardEvent.Subjects[0], GetStringValue.Invoke(BadBoard)),
            new PayloadSubject(m_BadBoardEvent.Subjects[1], GetStringValue.Invoke(BadBoard)),
            new PayloadSubject(m_BadBoardEvent.Subjects[2], GetStringValue.Invoke(BadBoardLatch))
            }));
        }

        internal bool GoodBoard
        {
            get
            {
                return m_GoodBoard;
            }
            set
            {
                if (m_GoodBoard != value)
                {
                    m_GoodBoard = value;

                    if (m_GoodBoard && m_SMEMAStateMachine.MachineReady)
                    {
                        GoodBoardUpdated?.BeginInvoke(m_GoodBoard, GoodBoardUpdated.EndInvoke, null);
                    }

                    InvokeGoodBoardEvent();
                }
            }
        }

        internal bool GoodBoardLatch
        {
            get
            {
                return m_GoodBoardLatch;
            }

            set
            {
                if (m_GoodBoardLatch != value)
                {
                    m_GoodBoardLatch = value;
                    InvokeGoodBoardEvent();
                }
            }
        }

        internal bool GoodBoardDivert { get; set; }

        public bool BadBoard
        {
            get
            {
                return m_BadBoard;
            }
            set
            {
                if (m_BadBoard != value)
                {
                    m_BadBoard = value;

                    if (m_BadBoard && m_SMEMAStateMachine.MachineReady)
                    {
                        BadBoardUpdated?.BeginInvoke(true, BadBoardUpdated.EndInvoke, null);
                    }

                    InvokeBadBoardEvent();
                }
            }
        }

        internal bool BadBoardLatch
        {
            get
            {
                return m_BadBoardLatch;
            }

            set
            {
                if (m_BadBoardLatch != value)
                {
                    m_BadBoardLatch = value;
                    InvokeBadBoardEvent();
                }
            }
        }

        internal bool BadBoardDivert { get; set; }
    }
}
