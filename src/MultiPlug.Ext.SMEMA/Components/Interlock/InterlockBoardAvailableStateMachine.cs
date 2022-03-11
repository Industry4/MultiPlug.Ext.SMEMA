using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockBoardAvailableStateMachine
    {
        private InterlockSMEMAStateMachine m_SMEMAUplineStateMachine;
        internal InterlockSMEMAStateMachine SMEMADownlineStateMachine { get; private set; }

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

        private const string c_GoodBoardDescription = "good";
        private const string c_BadBoardDescription = "bad";

        internal event Action<bool> GoodBoardUpdated;
        internal event Action<bool> GoodBoardLatchedUpdated;
        internal event Action<bool> BadBoardUpdated;
        internal event Action<bool> BadBoardLatchedUpdated;

        private Models.Exchange.Event m_GoodBoardBlockEvent;
        private Models.Exchange.Event m_BadBoardBlockEvent;

        internal event Action BlockedUpdated;
        internal bool Blocked { get; private set; }

        public InterlockBoardAvailableStateMachine( InterlockSMEMAStateMachine theSMEMAUplineStateMachine, 
                                                    InterlockSMEMAStateMachine theSMEMADownlineStateMachine, 
                                                    Event theGoodBoardEvent,
                                                    Event theBadBoardEvent,
                                                    Models.Exchange.Event theGoodBoardBlockEvent,
                                                    Models.Exchange.Event theBadBoardBlockEvent)
        {
            this.m_SMEMAUplineStateMachine = theSMEMAUplineStateMachine;
            this.SMEMADownlineStateMachine = theSMEMADownlineStateMachine;
            this.m_GoodBoardEvent = theGoodBoardEvent;
            this.m_BadBoardEvent = theBadBoardEvent;
            this.m_GoodBoardBlockEvent = theGoodBoardBlockEvent;
            this.m_BadBoardBlockEvent = theBadBoardBlockEvent;
        }

        private void BlockUpdated()
        {
            var AnyBlocked = GoodBoardBlocked || BadBoardBlocked;

            if (AnyBlocked != Blocked)
            {
                Blocked = AnyBlocked;
                BlockedUpdated?.Invoke();
            }
        }

        internal void OnSMEMAIOGoodBoard(bool theIOState)
        {
            m_SMEMAUplineStateMachine.GoodBoard.Value = theIOState;

            OnGoodBoardUpdate();
        }

        private void SetGoodBlocked(bool theValue)
        {
            if (theValue != GoodBoardBlocked)
            {
                GoodBoardBlocked = theValue;
                BlockUpdated();
                InvokeGoodBoardBlockEvent();
            }
        }

        private void OnGoodBoardUpdate()
        {
            if (m_SMEMAUplineStateMachine.GoodBoard.Value)
            {
                if ( ( ! SMEMADownlineStateMachine.GoodBoard.Value ) && ( ! SMEMADownlineStateMachine.GoodBoardDiverted.Value ) )
                {
                    if (GoodBoardDivert)
                    {
                        SetGoodBlocked(false);
                        SMEMADownlineStateMachine.GoodBoardDiverted.Value = true;
                        //BadBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.GoodBoard.Value, BadBoardUpdated.EndInvoke, null);
                    }
                    else if (GoodBoard)
                    {
                        SetGoodBlocked(false);
                        SMEMADownlineStateMachine.GoodBoard.Value = true;
                       // GoodBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.GoodBoard, GoodBoardUpdated.EndInvoke, null);
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

                if (SMEMADownlineStateMachine.GoodBoardDiverted.Value)
                {
                    if (!GoodBoardDivertLatch)
                    {
                        GoodBoardDivert = false;
                       // InvokeGoodBoardEvent();
                    }

                    SMEMADownlineStateMachine.GoodBoardDiverted.Value = false;
                   // BadBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.GoodBoard.Value, BadBoardUpdated.EndInvoke, null);
                }
                else if (SMEMADownlineStateMachine.GoodBoard.Value)
                {
                    if (!GoodBoardLatch)
                    {
                        m_GoodBoard = false;
                        SMEMADownlineStateMachine.GoodBoard.Value = false;

                        InvokeGoodBoardEvent();
                    }
                    else
                    {
                        SMEMADownlineStateMachine.GoodBoard.Value = false;
                    }
                    //GoodBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.GoodBoard.Value, GoodBoardUpdated.EndInvoke, null);
                }
            }
        }

        internal void OnSMEMAIOBadBoard(bool theIOState)
        {
            m_SMEMAUplineStateMachine.BadBoard.Value = theIOState;

            OnBadBoardUpdate();
        }

        private void SetBadBlocked(bool theValue)
        {
            if(theValue != BadBoardBlocked)
            {
                BadBoardBlocked = theValue;
                BlockUpdated();
                InvokeBadBoardBlockEvent();
            }
        }

        private void OnBadBoardUpdate()
        {
            if (m_SMEMAUplineStateMachine.BadBoard.Value)
            {
                if( ( ! SMEMADownlineStateMachine.BadBoard.Value) && ( ! SMEMADownlineStateMachine.BadBoardDiverted.Value ) )
                {
                    if (BadBoardDivert)
                    {
                        SetBadBlocked(false);
                        SMEMADownlineStateMachine.BadBoardDiverted.Value = true;
                        //GoodBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.BadBoard.Value, GoodBoardUpdated.EndInvoke, null);
                    }
                    else if (BadBoard)
                    {
                        SetBadBlocked(false);
                        SMEMADownlineStateMachine.BadBoard.Value = true;
                        //BadBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.BadBoard.Value, BadBoardUpdated.EndInvoke, null);
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

                if (SMEMADownlineStateMachine.BadBoardDiverted.Value)
                {
                    if (!BadBoardDivertLatch)
                    {
                        BadBoardDivert = false;
                    }

                    SMEMADownlineStateMachine.BadBoardDiverted.Value = false;
                    //GoodBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.BadBoard.Value, GoodBoardUpdated.EndInvoke, null);
                }
                else if (SMEMADownlineStateMachine.BadBoard.Value)
                {
                    if (!BadBoardLatch)
                    {
                        BadBoard = false;
                    }

                    SMEMADownlineStateMachine.BadBoard.Value = false;
                  //  BadBoardUpdated?.BeginInvoke(m_SMEMAUplineStateMachine.BadBoard.Value, BadBoardUpdated.EndInvoke, null);
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

        private void InvokeGoodBoardBlockEvent()
        {
            if (m_GoodBoardBlockEvent.Subjects.Length != 2) // Safety Net
            {
                return;
            }

            if (GoodBoardBlocked && m_GoodBoardBlockEvent.BlockedEnabled)
            {
                m_GoodBoardBlockEvent.Invoke(new Payload(m_GoodBoardBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_GoodBoardBlockEvent.Subjects[0], m_GoodBoardBlockEvent.BlockedValue),
                new PayloadSubject(m_GoodBoardBlockEvent.Subjects[1], c_GoodBoardDescription)
                }));
            }
            else if (!GoodBoardBlocked && m_GoodBoardBlockEvent.UnblockedEnabled)
            {
                m_GoodBoardBlockEvent.Invoke(new Payload(m_GoodBoardBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_GoodBoardBlockEvent.Subjects[0], m_GoodBoardBlockEvent.UnblockedValue),
                new PayloadSubject(m_GoodBoardBlockEvent.Subjects[1], c_GoodBoardDescription)
                }));
            }
        }

        private void InvokeBadBoardBlockEvent()
        {
            if (m_BadBoardBlockEvent.Subjects.Length != 2) // Safety Net
            {
                return;
            }

            if (BadBoardBlocked && m_BadBoardBlockEvent.BlockedEnabled)
            {
                m_BadBoardBlockEvent.Invoke(new Payload(m_BadBoardBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_BadBoardBlockEvent.Subjects[0], m_BadBoardBlockEvent.BlockedValue),
                new PayloadSubject(m_GoodBoardBlockEvent.Subjects[1], c_BadBoardDescription)
                }));
            }
            else if (!Blocked && m_BadBoardBlockEvent.UnblockedEnabled)
            {
                m_BadBoardBlockEvent.Invoke(new Payload(m_BadBoardBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_BadBoardBlockEvent.Subjects[0], m_BadBoardBlockEvent.UnblockedValue),
                new PayloadSubject(m_GoodBoardBlockEvent.Subjects[1], c_BadBoardDescription)
                }));
            }
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
                    GoodBoardUpdated?.Invoke(m_GoodBoard);
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
                    GoodBoardLatchedUpdated?.Invoke(m_GoodBoardLatch);
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
        internal bool GoodBoardBlocked { get; private set; }

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
                    BadBoardUpdated?.Invoke(m_BadBoard);
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
                    BadBoardLatchedUpdated?.Invoke(m_BadBoardLatch);
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

        internal bool BadBoardBlocked { get; private set; }
    }
}
