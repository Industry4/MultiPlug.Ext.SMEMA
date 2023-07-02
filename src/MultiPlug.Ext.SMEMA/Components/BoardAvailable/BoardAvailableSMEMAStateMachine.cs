using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.BoardAvailable;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.BoardAvailable
{
    internal class BoardAvailableSMEMAStateMachine
    {
        private BoardAvailableProperties m_Properties;

        internal bool MachineReadyState { get; set; }

        internal bool GoodBoardAvailableState { get; private set; }
        internal bool BadBoardAvailableState { get; private set; }
        internal bool FlipBoardState { get; private set; }

        internal event Action<bool> GoodBoard;
        internal event Action<bool> BadBoard;
        internal event Action<bool> FlipBoard;

        public BoardAvailableSMEMAStateMachine(BoardAvailableProperties theBoardAvailableProperties)
        {
            this.m_Properties = theBoardAvailableProperties;
        }

        internal void Init()
        {
            if( m_Properties.SMEMABoardAvailableAlways == true)
            {
                GoodBoardAvailableState = true;
                GoodBoard?.BeginInvoke(GoodBoardAvailableState, GoodBoard.EndInvoke, null);
            }
            else
            {
                OnGoodBoardEvent(m_Properties.SMEMABoardAvailableSubscription.Cache());
            }

            if(m_Properties.SMEMAFailedBoardAvailableAlways == true)
            {
                BadBoardAvailableState = true;
                BadBoard?.BeginInvoke(BadBoardAvailableState, BadBoard.EndInvoke, null);
            }
            else
            {
                OnBadBoardEvent(m_Properties.SMEMAFailedBoardAvailableSubscription.Cache());
            }

            if (m_Properties.SMEMAFlipBoardAlways == true)
            {
                FlipBoardState = true;
                FlipBoard?.BeginInvoke(FlipBoardState, FlipBoard.EndInvoke, null);
            }
            else
            {
                OnFlipBoardEvent(m_Properties.SMEMAFlipBoardSubscription.Cache());
            }
        }

        internal void OnGoodBoardEvent(SubscriptionEvent theEvent)
        {
            if (m_Properties.SMEMABoardAvailableAlways == true)
            {
                return;
            }
            foreach( PayloadSubject Subject in theEvent.PayloadSubjects)
            {
                if( Subject.Value.Equals(m_Properties.SMEMABoardAvailableSubscription.Value, StringComparison.OrdinalIgnoreCase))
                {
                    GoodBoardAvailableState = true;
                    break;
                }
                else
                {
                    GoodBoardAvailableState = false;
                }
            }

            GoodBoard?.BeginInvoke(GoodBoardAvailableState, GoodBoard.EndInvoke, null);
        }

        internal void OnBadBoardEvent(SubscriptionEvent theEvent)
        {
            if (m_Properties.SMEMAFailedBoardAvailableAlways == true)
            {
                return;
            }
            foreach (PayloadSubject Subject in theEvent.PayloadSubjects)
            {
                if (Subject.Value.Equals(m_Properties.SMEMAFailedBoardAvailableSubscription.Value, StringComparison.OrdinalIgnoreCase))
                {
                    BadBoardAvailableState = true;
                    break;
                }
                else
                {
                    BadBoardAvailableState = false;
                }
            }

            BadBoard?.BeginInvoke(BadBoardAvailableState, BadBoard.EndInvoke, null);
        }

        internal void OnFlipBoardEvent(SubscriptionEvent theEvent)
        {
            if (m_Properties.SMEMAFlipBoardAlways == true)
            {
                return;
            }
            foreach (PayloadSubject Subject in theEvent.PayloadSubjects)
            {
                if (Subject.Value.Equals(m_Properties.SMEMAFlipBoardSubscription.Value, StringComparison.OrdinalIgnoreCase))
                {
                    FlipBoardState = true;
                    break;
                }
                else
                {
                    FlipBoardState = false;
                }
            }

            FlipBoard?.BeginInvoke(FlipBoardState, FlipBoard.EndInvoke, null);
        }

        internal void OnMachineReady(bool isTrue)
        {
            if (MachineReadyState != isTrue)
            {
                MachineReadyState = isTrue;

                m_Properties.SMEMAMachineReadyEvent.Invoke(new Payload(m_Properties.SMEMAMachineReadyEvent.Id, new PayloadSubject[] {
                    new PayloadSubject(m_Properties.SMEMAMachineReadyEvent.Subjects[0], GetStringValue.Invoke( isTrue ) )
                    }));
            }
        }

    }
}
