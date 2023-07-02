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
        public bool FlipBoardState { get; internal set; }

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

        internal void OnFlipBoard(bool isTrue)
        {
            if (FlipBoardState != isTrue)
            {
                FlipBoardState = isTrue;
                InvokeFlipBoardEvent();
            }
        }

        private void InvokeFlipBoardEvent()
        {
            m_Properties.SMEMAFlipBoardEvent.Invoke(new Payload(m_Properties.SMEMAFlipBoardEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_Properties.SMEMAFlipBoardEvent.Subjects[0], GetStringValue.Invoke( FlipBoardState ) )
                }));
        }
    }
}
