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
        private bool m_GoodBoardDivert;
        private bool m_GoodBoardDivertLatch;
        private bool m_BadBoard;
        private bool m_BadBoardLatch;
        private bool m_BadBoardDivert;
        private bool m_BadBoardDivertLatch;

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
            new PayloadSubject(m_GoodBoardEvent.Subjects[2], GetStringValue.Invoke(GoodBoardLatch)),
            new PayloadSubject(m_GoodBoardEvent.Subjects[3], GetStringValue.Invoke(GoodBoardDivert)),
            new PayloadSubject(m_GoodBoardEvent.Subjects[4], GetStringValue.Invoke(GoodBoardDivertLatch))
            }));
        }

        private void InvokeBadBoardEvent()
        {
            m_BadBoardEvent.Invoke(new Payload(m_BadBoardEvent.Id, new PayloadSubject[] {
            new PayloadSubject(m_BadBoardEvent.Subjects[0], GetStringValue.Invoke(BadBoard)),
            new PayloadSubject(m_BadBoardEvent.Subjects[1], GetStringValue.Invoke(BadBoard)),
            new PayloadSubject(m_BadBoardEvent.Subjects[2], GetStringValue.Invoke(BadBoardLatch)),
            new PayloadSubject(m_GoodBoardEvent.Subjects[3], GetStringValue.Invoke(BadBoardDivert)),
            new PayloadSubject(m_GoodBoardEvent.Subjects[4], GetStringValue.Invoke(BadBoardDivertLatch))
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

        internal bool GoodBoardDivert
        {
            get
            {
                return m_GoodBoardDivert;
            }

            set
            {
                if (m_GoodBoardDivert != value)
                {
                    m_GoodBoardDivert = value;
                    InvokeGoodBoardEvent();
                }
            }
        }

        internal bool GoodBoardDivertLatch
        {
            get
            {
                return m_GoodBoardDivertLatch;
            }

            set
            {
                if (m_GoodBoardDivertLatch != value)
                {
                    m_GoodBoardDivertLatch = value;
                    InvokeGoodBoardEvent();
                }
            }
        }

        internal bool BadBoard
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

        internal bool BadBoardDivert
        {
            get
            {
                return m_BadBoardDivert;
            }

            set
            {
                if (m_BadBoardDivert != value)
                {
                    m_BadBoardDivert = value;
                    InvokeBadBoardEvent();
                }
            }
        }
        internal bool BadBoardDivertLatch
        {
            get
            {
                return m_BadBoardDivertLatch;
            }

            set
            {
                if (m_BadBoardDivertLatch != value)
                {
                    m_BadBoardDivertLatch = value;
                    InvokeBadBoardEvent();
                }
            }
        }

    }
}
