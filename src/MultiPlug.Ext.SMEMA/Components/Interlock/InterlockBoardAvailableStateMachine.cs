using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockBoardAvailableStateMachine
    {
        private InterlockSMEMAStateMachine m_SMEMAUplineStateMachine;
        private InterlockSMEMAStateMachine m_SMEMADownlineStateMachine;

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

        private bool m_GoodBlocked;
        private bool m_BadBlocked;

        internal event Action BlockedUpdated;
        internal bool Blocked { get; private set; }

        public InterlockBoardAvailableStateMachine( InterlockSMEMAStateMachine theSMEMAUplineStateMachine,
                                                    InterlockSMEMAStateMachine theSMEMADownlineStateMachine,
                                                    Event theGoodBoardEvent,
                                                    Event theBadBoardEvent)
        {
            this.m_SMEMAUplineStateMachine = theSMEMAUplineStateMachine;
            this.m_SMEMADownlineStateMachine = theSMEMADownlineStateMachine;
            this.m_GoodBoardEvent = theGoodBoardEvent;
            this.m_BadBoardEvent = theBadBoardEvent;
        }

        private void BlockUpdated()
        {
            var AnyBlocked = m_GoodBlocked || m_BadBlocked;

            if (AnyBlocked != Blocked)
            {
                Blocked = AnyBlocked;
                BlockedUpdated?.Invoke();
            }
        }

        internal void OnSMEMAIOGoodBoard(bool theIOState)
        {
            m_SMEMAUplineStateMachine.GoodBoard = theIOState;

            OnGoodBoardUpdate();
        }

        private void SetGoodBlocked(bool theValue)
        {
            if (theValue != m_GoodBlocked)
            {
                m_GoodBlocked = theValue;
                BlockUpdated();
            }
        }

        private void OnGoodBoardUpdate()
        {
            if (m_SMEMAUplineStateMachine.GoodBoard)
            {
                if ( ( ! m_SMEMADownlineStateMachine.GoodBoard ) && ( ! m_SMEMADownlineStateMachine.GoodBoardDiverted ) )
                {
                    if (GoodBoardDivert)
                    {
                        SetGoodBlocked(false);
                        m_SMEMADownlineStateMachine.GoodBoardDiverted = true;
                        BadBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.GoodBoard, BadBoardUpdated.EndInvoke, null);
                    }
                    else if (GoodBoard)
                    {
                        SetGoodBlocked(false);
                        m_SMEMADownlineStateMachine.GoodBoard = true;
                        GoodBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.GoodBoard, GoodBoardUpdated.EndInvoke, null);
                    }
                    else
                    {
                        SetGoodBlocked(true);
                    }
                }
            }
            else
            {
                SetGoodBlocked(false);

                if (m_SMEMADownlineStateMachine.GoodBoardDiverted)
                {
                    m_SMEMADownlineStateMachine.GoodBoardDiverted = false;

                    if (!GoodBoardDivertLatch)
                    {
                        GoodBoardDivert = false;
                       // InvokeGoodBoardEvent();
                    }
                    BadBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.GoodBoard, BadBoardUpdated.EndInvoke, null);
                }
                else if (m_SMEMADownlineStateMachine.GoodBoard)
                {
                    m_SMEMADownlineStateMachine.GoodBoard = false;

                    if (!GoodBoardLatch)
                    {
                        m_GoodBoard = false;
                        InvokeGoodBoardEvent();
                    }
                    GoodBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.GoodBoard, GoodBoardUpdated.EndInvoke, null);
                }
            }
        }

        internal void OnSMEMAIOBadBoard(bool theIOState)
        {
            m_SMEMAUplineStateMachine.BadBoard = theIOState;

            OnBadBoardUpdate();
        }

        private void SetBadBlocked(bool theValue)
        {
            if(theValue != m_BadBlocked)
            {
                m_BadBlocked = theValue;
                BlockUpdated();
            }
        }

        private void OnBadBoardUpdate()
        {
            if (m_SMEMAUplineStateMachine.BadBoard)
            {
                if( ( ! m_SMEMADownlineStateMachine.BadBoard) && ( ! m_SMEMADownlineStateMachine.BadBoardDiverted ) )
                {
                    if (BadBoardDivert)
                    {
                        SetBadBlocked(false);
                        m_SMEMADownlineStateMachine.BadBoardDiverted = true;
                        GoodBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.BadBoard, GoodBoardUpdated.EndInvoke, null);
                    }
                    else if (BadBoard)
                    {
                        SetBadBlocked(false);
                        m_SMEMADownlineStateMachine.BadBoard = true;
                        BadBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.BadBoard, BadBoardUpdated.EndInvoke, null);
                    }
                    else
                    {
                        SetBadBlocked(true);
                    }
                }
            }
            else
            {
                SetBadBlocked(false);

                if (m_SMEMADownlineStateMachine.BadBoardDiverted)
                {
                    m_SMEMADownlineStateMachine.BadBoardDiverted = false;

                    if (!BadBoardDivertLatch)
                    {
                        BadBoardDivert = false;
                    }
                    GoodBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.BadBoard, GoodBoardUpdated.EndInvoke, null);
                }
                else if (m_SMEMADownlineStateMachine.BadBoard)
                {
                    m_SMEMADownlineStateMachine.BadBoard = false;

                    if (!BadBoardLatch)
                    {
                        BadBoard = false;
                    }
                    BadBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.BadBoard, BadBoardUpdated.EndInvoke, null);
                }
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
                    InvokeGoodBoardEvent();
                    OnGoodBoardUpdate();
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

                    if(m_GoodBoardDivert)
                    {
                        OnGoodBoardUpdate();
                    }
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

                    InvokeBadBoardEvent();
                    OnBadBoardUpdate();
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
                    if (m_BadBoardDivert)
                    {
                        OnBadBoardUpdate();
                    }
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
