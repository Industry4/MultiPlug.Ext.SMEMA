using System;
using System.Threading;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.MachineReady
{
    internal class MachineReadySMEMAStateMachine
    {
        private MachineReadyProperties m_Properties;

        internal event Action<bool> MachineReady;

        private CancellationTokenSource m_GoodBoardHighDelayCancellationSource = new CancellationTokenSource();
        private CancellationTokenSource m_GoodBoardLowDelayCancellationSource = new CancellationTokenSource();
        private CancellationTokenSource m_BadBoardHighDelayCancellationSource = new CancellationTokenSource();
        private CancellationTokenSource m_BadBoardLowDelayCancellationSource = new CancellationTokenSource();
        private CancellationTokenSource m_FlipBoardHighDelayCancellationSource = new CancellationTokenSource();
        private CancellationTokenSource m_FlipBoardLowDelayCancellationSource = new CancellationTokenSource();

        internal bool MachineReadyState { get; private set; }
        internal bool BadBoardAvailableState { get; private set; }
        internal bool GoodBoardAvailableState { get; private set; }
        public bool FlipBoardState { get; internal set; }

        private bool m_BadBoard;
        private bool m_GoodBoardDiverted;
        private bool m_GoodBoard;
        private bool m_BadBoardDiverted;

        public MachineReadySMEMAStateMachine(MachineReadyProperties theMachineReadyProperties)
        {
            this.m_Properties = theMachineReadyProperties;

            m_GoodBoardHighDelayCancellationSource.Cancel();
            m_GoodBoardLowDelayCancellationSource.Cancel();
            m_BadBoardHighDelayCancellationSource.Cancel();
            m_BadBoardLowDelayCancellationSource.Cancel();
            m_FlipBoardHighDelayCancellationSource.Cancel();
            m_FlipBoardLowDelayCancellationSource.Cancel();
        }

        internal void Init()
        {
            if (m_Properties.SMEMAMachineReadyAlways == true)
            {
                MachineReadyState = true;
                MachineReady?.Invoke(MachineReadyState);
            }
            else
            {
                OnMachineReady(m_Properties.SMEMAMachineReadySubscription.Cache());
            }
        }

        internal void OnMachineReady(SubscriptionEvent theEvent)
        {
            if (m_Properties.SMEMAMachineReadyAlways == true)
            {
                return;
            }

            foreach (PayloadSubject Subject in theEvent.PayloadSubjects)
            {
                if (Subject.Value.Equals(m_Properties.SMEMAMachineReadySubscription.Value, StringComparison.OrdinalIgnoreCase))
                {
                    MachineReadyState = true;
                    break;
                }
                else
                {
                    MachineReadyState = false;
                }
            }

            MachineReady?.Invoke(MachineReadyState);
        }

        private void InvokeGoodBoardEvent()
        {
            var Test = m_GoodBoard || m_BadBoardDiverted;

            TransitionDelays.SMEMAEventChange(  GoodBoardAvailableState,
                                                Test,
                                                ref m_GoodBoardHighDelayCancellationSource,
                                                () => { GoodBoardAvailableState = true; },
                                                ref m_GoodBoardLowDelayCancellationSource,
                                                () => { GoodBoardAvailableState = false; },
                                                m_Properties.SMEMABoardAvailableEvent);
        }

        internal void OnGoodBoard(bool isTrue)
        {
            if(m_GoodBoard != isTrue)
            {
                m_GoodBoard = isTrue;
                InvokeGoodBoardEvent();
            }
        }

        internal void OnBadBoardDiverted(bool isTrue)
        {
            if (m_BadBoardDiverted != isTrue)
            {
                m_BadBoardDiverted = isTrue;
                InvokeGoodBoardEvent();
            }
        }

        private void InvokeBadBoardEvent()
        {
            var Test = m_BadBoard || m_GoodBoardDiverted;

            TransitionDelays.SMEMAEventChange(  BadBoardAvailableState,
                                                Test,
                                                ref m_BadBoardHighDelayCancellationSource,
                                                () => { BadBoardAvailableState = true; },
                                                ref m_BadBoardLowDelayCancellationSource,
                                                () => { BadBoardAvailableState = false; },
                                                m_Properties.SMEMAFailedBoardAvailableEvent);
        }

        internal void OnBadBoard(bool isTrue)
        {
            if (m_BadBoard != isTrue)
            {
                m_BadBoard = isTrue;
                InvokeBadBoardEvent();
            }
        }

        internal void OnGoodBoardDiverted(bool isTrue)
        {
            if (m_GoodBoardDiverted != isTrue)
            {
                m_GoodBoardDiverted = isTrue;
                InvokeBadBoardEvent();
            }
        }

        internal void OnFlipBoard(bool isTrue)
        {
            TransitionDelays.SMEMAEventChange(  FlipBoardState,
                                                isTrue,
                                                ref m_FlipBoardHighDelayCancellationSource,
                                                () => { FlipBoardState = true; },
                                                ref m_FlipBoardLowDelayCancellationSource,
                                                () => { FlipBoardState = false; },
                                                m_Properties.SMEMAFlipBoardEvent);
        }
    }
}
