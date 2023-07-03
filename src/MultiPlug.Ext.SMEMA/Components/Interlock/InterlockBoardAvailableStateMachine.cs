using System;
using MultiPlug.Base.Exchange;
using System.Threading.Tasks;
using System.Threading;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockBoardAvailableStateMachine
    {
        private InterlockSMEMAStateMachine m_SMEMAUplineStateMachine;
        internal InterlockSMEMAStateMachine SMEMADownlineStateMachine { get; private set; }

        private Event m_BadBoardEvent;
        private Event m_GoodBoardEvent;
        private Event m_FlipBoardEvent;

        private bool m_GoodBoard;
        private bool m_GoodBoardLatch;
        private bool m_GoodBoardDivert;
        private bool m_GoodBoardDivertLatch;
        private bool m_BadBoard;
        private bool m_BadBoardLatch;

        private bool m_BadBoardDivert;
        private bool m_BadBoardDivertLatch;
        private bool m_FlipBoard;
        private bool m_FlipBoardLatch;

        private const string c_GoodBoardDescription = "good";
        private const string c_BadBoardDescription = "bad";
        private const string c_FlipBoardDescription = "flip";

        internal event Action<bool> GoodBoardUpdated;
        internal event Action<bool> GoodBoardLatchedUpdated;
        internal event Action<bool> BadBoardUpdated;
        internal event Action<bool> BadBoardLatchedUpdated;
        internal event Action<bool> FlipBoardUpdated;
        internal event Action<bool> FlipBoardLatchedUpdated;

        private Models.Exchange.Event m_GoodBoardBlockEvent;
        private Models.Exchange.Event m_BadBoardBlockEvent;
        private Models.Exchange.Event m_FlipBoardBlockEvent;

        internal event Action BlockedUpdated;

        private CancellationTokenSource m_FlipTimerCancellationToken;

        internal bool Blocked { get; private set; }
        internal bool GoodBoardBlocked { get; private set; }
        internal bool BadBoardBlocked { get; private set; }
        internal bool FlipBoardBlocked { get; private set; }

        private InterlockProperties m_InterlockProperties;

        public InterlockBoardAvailableStateMachine( InterlockProperties theProperties,
                                                    InterlockSMEMAStateMachine theSMEMAUplineStateMachine, 
                                                    InterlockSMEMAStateMachine theSMEMADownlineStateMachine, 
                                                    Event theGoodBoardEvent,
                                                    Event theBadBoardEvent,
                                                    Event theFlipBoardEvent,
                                                    Models.Exchange.Event theGoodBoardBlockEvent,
                                                    Models.Exchange.Event theBadBoardBlockEvent,
                                                    Models.Exchange.Event theFlipBoardBlockEvent)
        {
            m_InterlockProperties = theProperties;
            m_SMEMAUplineStateMachine = theSMEMAUplineStateMachine;
            SMEMADownlineStateMachine = theSMEMADownlineStateMachine;
            m_GoodBoardEvent = theGoodBoardEvent;
            m_BadBoardEvent = theBadBoardEvent;
            m_FlipBoardEvent = theFlipBoardEvent;
            m_GoodBoardBlockEvent = theGoodBoardBlockEvent;
            m_BadBoardBlockEvent = theBadBoardBlockEvent;
            m_FlipBoardBlockEvent = theFlipBoardBlockEvent;
        }

        internal void OnSMEMAIOMachineReady(bool theIOState)
        {
            if(theIOState == false)
            {
                BlockGoodBoardOnSMEMAMachineNotReady();
                BlockBadBoardOnSMEMAMachineNotReady();
                BlockFlipBoardOnSMEMAMachineNotReady();
            }
        }

        internal void OnSMEMAIOGoodBoard(bool theIOState)
        {
            m_SMEMAUplineStateMachine.GoodBoard.Value = theIOState;
            if (theIOState == false)
            {
                BlockFlipBoardOnSMEMAGoodBoardNotAvailable();
            }

            OnGoodBoardUpdate();
        }

        internal void OnSMEMAIOBadBoard(bool theIOState)
        {
            m_SMEMAUplineStateMachine.BadBoard.Value = theIOState;
            if(theIOState == false)
            {
                BlockFlipBoardOnSMEMABadBoardNotAvailable();
            }
            OnBadBoardUpdate();
        }

        internal void OnSMEMAIOFlipBoard(bool theIOState)
        {
            m_SMEMAUplineStateMachine.FlipBoard.Value = theIOState;
            OnFlipBoardUpdate();
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

        private void SetGoodBlocked(bool isBlocked)
        {
            if (isBlocked != GoodBoardBlocked)
            {
                GoodBoardBlocked = isBlocked;
                BlockUpdated();
                InvokeGoodBoardBlockEvent();
            }
        }

        private void SetBadBlocked(bool isBlocked)
        {
            if (isBlocked != BadBoardBlocked)
            {
                BadBoardBlocked = isBlocked;
                BlockUpdated();
                InvokeBadBoardBlockEvent();
            }
        }

        private void SetFlipBlocked(bool isBlocked)
        {
            if (isBlocked != FlipBoardBlocked)
            {
                FlipBoardBlocked = isBlocked;
                InvokeFlipBoardBlockEvent();
            }
        }

        private void OnGoodBoardUpdate()
        {
            if (m_SMEMAUplineStateMachine.GoodBoard.Value)
            {
                if ( ( SMEMADownlineStateMachine.GoodBoard.Value == false ) && ( SMEMADownlineStateMachine.GoodBoardDiverted.Value == false ) )
                {
                    if (GoodBoardDivert)
                    {
                        SetGoodBlocked(false);
                        SMEMADownlineStateMachine.GoodBoardDiverted.Value = true;
                    }
                    else if (GoodBoard)
                    {
                        SetGoodBlocked(false);
                        SMEMADownlineStateMachine.GoodBoard.Value = true;
                    }
                    else
                    {
                        SetGoodBlocked(true);
                    }
                }
                else if (SMEMADownlineStateMachine.GoodBoard.Value)
                {
                    if (GoodBoardDivert)
                    {
                        SMEMADownlineStateMachine.GoodBoard.Value = false;
                        SMEMADownlineStateMachine.GoodBoardDiverted.Value = true;
                        SetGoodBlocked(false);
                    }
                    else
                    {
                        SMEMADownlineStateMachine.GoodBoard.Value = false;
                        SetGoodBlocked(true);
                    }
                }
                else // SMEMADownlineStateMachine.GoodBoardDiverted.Value == true
                {
                    if (GoodBoard)
                    {
                        if (SMEMADownlineStateMachine.GoodBoardDiverted.Value)
                        {
                            GoodBoardDivertReset();
                            SMEMADownlineStateMachine.GoodBoardDiverted.Value = false;
                        }
                        SetGoodBlocked(false);
                        SMEMADownlineStateMachine.GoodBoard.Value = true;
                    }
                    else
                    {
                        GoodBoardDivertReset();
                        SMEMADownlineStateMachine.GoodBoardDiverted.Value = false;
                        SetGoodBlocked(true);
                    }
                }
            }
            else
            {
                SetGoodBlocked(false);

                if (SMEMADownlineStateMachine.GoodBoardDiverted.Value)
                {
                    GoodBoardDivertReset();
                    SMEMADownlineStateMachine.GoodBoardDiverted.Value = false;
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
                }
            }
        }

        private void OnBadBoardUpdate()
        {
            // Upline is true
            if (m_SMEMAUplineStateMachine.BadBoard.Value)
            {
                // Downline Bad Board and Good Board (Diverted) is both False
                if( ( SMEMADownlineStateMachine.BadBoard.Value == false) && ( SMEMADownlineStateMachine.BadBoardDiverted.Value == false ) )
                {
                    if (BadBoardDivert)
                    {
                        SetBadBlocked(false);
                        SMEMADownlineStateMachine.BadBoardDiverted.Value = true;
                    }
                    else if (BadBoard)
                    {
                        SetBadBlocked(false);
                        SMEMADownlineStateMachine.BadBoard.Value = true;
                    }
                    else
                    {
                        SetBadBlocked(true);
                    }
                }
                else if (SMEMADownlineStateMachine.BadBoard.Value)
                {
                    if (BadBoardDivert)
                    {
                        SMEMADownlineStateMachine.BadBoard.Value = false;
                        SMEMADownlineStateMachine.BadBoardDiverted.Value = true;
                        SetBadBlocked(false);
                    }
                    else
                    {
                        SMEMADownlineStateMachine.BadBoard.Value = false;
                        SetBadBlocked(true);
                    }
                }
                else // SMEMADownlineStateMachine.BadBoardDiverted.Value == true
                {
                    if (BadBoard)
                    {
                        if (SMEMADownlineStateMachine.BadBoardDiverted.Value)
                        {
                            BadBoardDivertReset();
                            SMEMADownlineStateMachine.BadBoardDiverted.Value = false;
                        }
                        SetBadBlocked(false);
                        SMEMADownlineStateMachine.BadBoard.Value = true;
                    }
                    else
                    {
                        BadBoardDivertReset();
                        SMEMADownlineStateMachine.BadBoardDiverted.Value = false;
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
                }
                else if (SMEMADownlineStateMachine.BadBoard.Value)
                {
                    if (!BadBoardLatch)
                    {
                        BadBoard = false;
                    }

                    SMEMADownlineStateMachine.BadBoard.Value = false;
                }
            }
        }

        private void OnFlipBoardUpdate()
        {
            if (m_SMEMAUplineStateMachine.FlipBoard.Value)
            {
                if (FlipBoard)
                {
                    SetFlipBlocked(false);
                    SMEMADownlineStateMachine.FlipBoard.Value = true;
                }
                else
                {
                    SetFlipBlocked(true);
                    SMEMADownlineStateMachine.FlipBoard.Value = false;
                }
            }
            else
            {
                SetFlipBlocked(false);

                if (SMEMADownlineStateMachine.FlipBoard.Value)
                {
                    if (!FlipBoardLatch)
                    {
                        m_FlipBoard = false;
                        InvokeFlipBoardEvent();
                    }

                    SMEMADownlineStateMachine.FlipBoard.Value = false;
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

        private void InvokeFlipBoardEvent()
        {
            m_FlipBoardEvent.Invoke(new Payload(m_FlipBoardEvent.Id, new PayloadSubject[] {
            new PayloadSubject(m_FlipBoardEvent.Subjects[0], GetStringValue.Invoke(FlipBoard)),
            new PayloadSubject(m_FlipBoardEvent.Subjects[1], GetStringValue.Invoke(FlipBoard)),
            new PayloadSubject(m_FlipBoardEvent.Subjects[2], GetStringValue.Invoke(FlipBoardLatch))
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
                new PayloadSubject(m_BadBoardBlockEvent.Subjects[1], c_BadBoardDescription)
                }));
            }
            else if (!BadBoardBlocked && m_BadBoardBlockEvent.UnblockedEnabled)
            {
                m_BadBoardBlockEvent.Invoke(new Payload(m_BadBoardBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_BadBoardBlockEvent.Subjects[0], m_BadBoardBlockEvent.UnblockedValue),
                new PayloadSubject(m_BadBoardBlockEvent.Subjects[1], c_BadBoardDescription)
                }));
            }
        }

        private void InvokeFlipBoardBlockEvent()
        {
            if (m_FlipBoardBlockEvent.Subjects.Length != 2) // Safety Net
            {
                return;
            }

            if (FlipBoardBlocked && m_FlipBoardBlockEvent.BlockedEnabled)
            {
                m_FlipBoardBlockEvent.Invoke(new Payload(m_FlipBoardBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_FlipBoardBlockEvent.Subjects[0], m_FlipBoardBlockEvent.BlockedValue),
                new PayloadSubject(m_FlipBoardBlockEvent.Subjects[1], c_FlipBoardDescription)
                }));
            }
            else if (!FlipBoardBlocked && m_FlipBoardBlockEvent.UnblockedEnabled)
            {
                m_FlipBoardBlockEvent.Invoke(new Payload(m_FlipBoardBlockEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_FlipBoardBlockEvent.Subjects[0], m_FlipBoardBlockEvent.UnblockedValue),
                new PayloadSubject(m_FlipBoardBlockEvent.Subjects[1], c_FlipBoardDescription)
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
                    OnGoodBoardUpdate();
                }
            }
        }

        internal void GoodBoardFlip(bool isEnabled)
        {
            if (isEnabled)
            {
                if (FlipBoard == false)
                {
                    FlipBoard = true;
                    FlipDelayWait();
                }

                GoodBoard = true;
            }
            else
            {
                FlipBoard = false;
                GoodBoard = false;
            }
        }

        internal void GoodBoardDivertFlip(bool isEnabled)
        {
            if (isEnabled)
            {
                if (FlipBoard == false)
                {
                    FlipBoard = true;
                    FlipDelayWait();
                }

                GoodBoardDivert = true;
            }
            else
            {
                FlipBoard = false;
                GoodBoardDivert = false;
            }
        }

        private void GoodBoardDivertReset()
        {
            if (!GoodBoardDivertLatch)
            {
                if (m_GoodBoardDivert)
                {
                    m_GoodBoardDivert = false;
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
                    OnBadBoardUpdate();
                }
            }
        }

        internal void BadBoardFlip(bool isEnabled)
        {
            if (isEnabled)
            {
                if (FlipBoard == false)
                {
                    FlipBoard = true;
                    FlipDelayWait();
                }

                BadBoard = true;
            }
            else
            {
                FlipBoard = false;
                BadBoard = false;
            }
        }

        internal void BadBoardDivertFlip(bool isEnabled)
        {
            if (isEnabled)
            {
                if (FlipBoard == false)
                {
                    FlipBoard = true;
                    FlipDelayWait();
                }

                BadBoardDivert = true;
            }
            else
            {
                FlipBoard = false;
                BadBoardDivert = false;
            }
        }

        private void BadBoardDivertReset()
        {
            if (!BadBoardDivertLatch)
            {
                if (m_BadBoardDivert)
                {
                    m_BadBoardDivert = false;
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

        private void BlockFlipBoardOnSMEMABadBoardNotAvailable()
        {
            if(m_InterlockProperties.TriggerBlockFlipBoardOnBadBoardNotAvailable.Value && ! FlipBoardLatch)
            {
                FlipBoard = false;
            }
        }

        private void BlockFlipBoardOnSMEMAGoodBoardNotAvailable()
        {
            if (m_InterlockProperties.TriggerBlockFlipBoardOnGoodBoardNotAvailable.Value && !FlipBoardLatch)
            {
                FlipBoard = false;
            }
        }

        private void BlockGoodBoardOnSMEMAMachineNotReady()
        {
            if (m_InterlockProperties.TriggerBlockGoodBoardOnMachineNotReady.Value)
            {
                if (!GoodBoardLatch)
                {
                    GoodBoard = false;
                }

                if (!GoodBoardDivertLatch)
                {
                    GoodBoardDivert = false;
                }
            }
        }

        private void BlockBadBoardOnSMEMAMachineNotReady()
        {
            if (m_InterlockProperties.TriggerBlockBadBoardOnMachineNotReady.Value)
            {
                if (!BadBoardLatch)
                {
                    BadBoard = false;
                }

                if (!BadBoardDivertLatch)
                {
                    BadBoardDivert = false;
                }
            }
        }

        private void BlockFlipBoardOnSMEMAMachineNotReady()
        {
            if (m_InterlockProperties.TriggerBlockFlipBoardOnMachineNotReady.Value)
            {
                if (!FlipBoardLatch)
                {
                    FlipBoard = false;
                }
            }
        }

        internal bool FlipBoard
        {
            get
            {
                return m_FlipBoard;
            }
            set
            {
                if (m_FlipBoard != value)
                {
                    m_FlipBoard = value;
                    FlipBoardUpdated?.Invoke(m_FlipBoard);
                    InvokeFlipBoardEvent();
                    OnFlipBoardUpdate();
                }
            }
        }

        internal bool FlipBoardLatch
        {
            get
            {
                return m_FlipBoardLatch;
            }

            set
            {
                if (m_FlipBoardLatch != value)
                {
                    m_FlipBoardLatch = value;
                    FlipBoardLatchedUpdated?.Invoke(m_FlipBoardLatch);
                    InvokeFlipBoardEvent();
                }
            }
        }

        private void FlipDelayWait()
        {
            if (m_FlipTimerCancellationToken != null)
            {
                m_FlipTimerCancellationToken.Cancel();
            }

            m_FlipTimerCancellationToken = new CancellationTokenSource();
            Task FlipDelay = Task.Delay(m_InterlockProperties.DelayFlipThenBoardAvailable.Value);
            try
            {
                FlipDelay.Wait(m_FlipTimerCancellationToken.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

    }
}
