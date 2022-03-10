using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.BoardAvailable;

namespace MultiPlug.Ext.SMEMA.Components.BoardAvailable
{
    internal class BoardAvailableSMEMAStateMachine
    {
        private BoardAvailableProperties m_Properties;

        internal bool MachineReadyState { get; set; }

        internal bool GoodBoardAvailableState { get; private set; }
        internal bool BadBoardAvailableState { get; private set; }

        internal event Action<bool> GoodBoard;
        internal event Action<bool> BadBoard;

        public BoardAvailableSMEMAStateMachine(BoardAvailableProperties theBoardAvailableProperties)
        {
            this.m_Properties = theBoardAvailableProperties;
        }

        internal void Init()
        {
            OnGoodBoardEvent(m_Properties.SMEMABoardAvailableSubscription.Cache());
            OnBadBoardEvent(m_Properties.SMEMAFailedBoardAvailableSubscription.Cache());
        }

        internal void OnGoodBoardEvent(SubscriptionEvent theEvent)
        {
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

    }
}
