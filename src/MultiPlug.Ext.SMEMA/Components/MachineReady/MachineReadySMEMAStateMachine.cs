using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;
using MultiPlug.Ext.SMEMA.Components.Utils;

namespace MultiPlug.Ext.SMEMA.Components.MachineReady
{
    internal class MachineReadySMEMAStateMachine
    {
        private MachineReadyProperties m_Properties;

        internal event Action<bool> MachineReady;

        internal bool MachineReadyState { get; private set; }
        internal bool BadBoardAvailableState { get; private set; }
        internal bool GoodBoardAvailableState { get; private set; }

        private bool m_BadBoard;
        private bool m_GoodBoardDiverted;
        private bool m_GoodBoard;
        private bool m_BadBoardDiverted;

        public MachineReadySMEMAStateMachine(MachineReadyProperties theMachineReadyProperties)
        {
            this.m_Properties = theMachineReadyProperties;
        }

        internal void Init()
        {
            OnMachineReady(m_Properties.SMEMAMachineReadySubscription.Cache());
        }

        internal void OnMachineReady(SubscriptionEvent theEvent)
        {
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

            MachineReady?.BeginInvoke(MachineReadyState, MachineReady.EndInvoke, null);
        }

        private void InvokeGoodBoardEvent()
        {
            var Test = m_GoodBoard || m_BadBoardDiverted;

            if (GoodBoardAvailableState != Test)
            {
                GoodBoardAvailableState = Test;

                m_Properties.SMEMABoardAvailableEvent.Invoke(new Payload(m_Properties.SMEMABoardAvailableEvent.Id, new PayloadSubject[] {
                    new PayloadSubject(m_Properties.SMEMABoardAvailableEvent.Subjects[0], GetStringValue.Invoke( GoodBoardAvailableState ) )
                    }));
            }
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

            if (BadBoardAvailableState != Test)
            {
                BadBoardAvailableState = Test;

                m_Properties.SMEMAFailedBoardAvailableEvent.Invoke(new Payload(m_Properties.SMEMAFailedBoardAvailableEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_Properties.SMEMAFailedBoardAvailableEvent.Subjects[0], GetStringValue.Invoke( BadBoardAvailableState ) )
                }));
            }
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
    }
}
