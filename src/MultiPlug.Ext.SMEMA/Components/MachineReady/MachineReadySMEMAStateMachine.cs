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

        internal void OnGoodBoard(bool isTrue)
        {
            GoodBoardAvailableState = isTrue;

            m_Properties.SMEMABoardAvailableEvent.Invoke(new Payload(m_Properties.SMEMABoardAvailableEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_Properties.SMEMABoardAvailableEvent.Subjects[0], GetStringValue.Invoke( isTrue ) )
                }));
        }

        internal void OnBadBoard(bool isTrue)
        {
            BadBoardAvailableState = isTrue;

            m_Properties.SMEMAFailedBoardAvailableEvent.Invoke(new Payload(m_Properties.SMEMAFailedBoardAvailableEvent.Id, new PayloadSubject[] {
                new PayloadSubject(m_Properties.SMEMAFailedBoardAvailableEvent.Subjects[0], GetStringValue.Invoke( isTrue ) )
                }));
        }
    }
}
