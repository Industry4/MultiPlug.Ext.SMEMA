using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;

namespace MultiPlug.Ext.SMEMA.Components.MachineReady
{
    internal class MachineReadySMEMAStateMachine
    {
        private MachineReadyProperties m_Properties;

        internal event Action<bool> MachineReady;

        internal bool MachineReadyState { get; private set; }
        public bool BadBoardAvailableState { get; internal set; }
        public bool GoodBoardAvailableState { get; internal set; }

        public MachineReadySMEMAStateMachine(MachineReadyProperties theMachineReadyProperties)
        {
            this.m_Properties = theMachineReadyProperties;
        }

        internal void OnEvent(SubscriptionEvent obj)
        {
            PayloadSubject PayloadSubject;

            if ( obj.TryGetValue(0, out PayloadSubject))
            {
                if (PayloadSubject.Value.Equals(m_Properties.SMEMAMachineReadySubscription.Value, StringComparison.OrdinalIgnoreCase))
                {
                    MachineReadyState = true;
                }
                else
                {
                    MachineReadyState = false;
                }

                MachineReady?.BeginInvoke(MachineReadyState, MachineReady.EndInvoke, null);
            }
        }
    }
}
