using System;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;
using MultiPlug.Ext.SMEMA.Models.Exchange;

namespace MultiPlug.Ext.SMEMA.Components.MachineReady
{
    public class MachineReadyComponent : MachineReadyProperties
    {
        internal const string c_SMEMABoardAvailableEventId = "SMEMABoardAvailable";
        internal const string c_SMEMAFailedBoardAvailableEventId = "SMEMAFailedBoardAvailable";
        internal const string c_SMEMAFlipBoardEventId = "SMEMAFlipBoard";

        internal event Action EventsUpdated;
        internal event Action SubscriptionsUpdated;

        internal MachineReadySMEMAStateMachine StateMachine { get; private set; }

        public MachineReadyComponent(string theGuid)
        {
            SMEMAMachineReadyAlways = false;
            StateMachine = new MachineReadySMEMAStateMachine(this);

            SMEMAMachineReadySubscription = new Models.Exchange.SMEMASubscription { Guid = Guid.NewGuid().ToString(), Id = string.Empty, Value = "1" };
            SMEMAMachineReadySubscription.Event += StateMachine.OnMachineReady;
            SMEMABoardAvailableEvent = new SMEMAEvent { Guid = Guid.NewGuid().ToString(), Id = theGuid + "-" + c_SMEMABoardAvailableEventId, Description = "Good Board Available", Subjects = new string[] { "value" }, Group = theGuid };
            SMEMAFailedBoardAvailableEvent = new SMEMAEvent { Guid = Guid.NewGuid().ToString(), Id = theGuid + "-" + c_SMEMAFailedBoardAvailableEventId, Description = "Bad Board Available", Subjects = new string[] { "value" }, Group = theGuid};
            SMEMAFlipBoardEvent = new SMEMAEvent { Guid = Guid.NewGuid().ToString(), Id = theGuid + "-" + c_SMEMAFlipBoardEventId, Description = "Flip Board", Subjects = new string[] { "value" }, Group = theGuid };
        }

        internal void UpdateProperties(MachineReadyProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;

            if (theNewProperties.SMEMABoardAvailableEvent != null && SMEMAEvent.Merge(SMEMABoardAvailableEvent, theNewProperties.SMEMABoardAvailableEvent))
            {
                FlagEventUpdated = true;
            }

            if (theNewProperties.SMEMAFailedBoardAvailableEvent != null && SMEMAEvent.Merge(SMEMAFailedBoardAvailableEvent, theNewProperties.SMEMAFailedBoardAvailableEvent))
            {
                FlagEventUpdated = true;
            }

            if (theNewProperties.SMEMAFlipBoardEvent != null && SMEMAEvent.Merge(SMEMAFlipBoardEvent, theNewProperties.SMEMAFlipBoardEvent))
            {
                FlagEventUpdated = true;
            }

            if (theNewProperties.SMEMAMachineReadySubscription != null && Models.Exchange.SMEMASubscription.Merge(SMEMAMachineReadySubscription, theNewProperties.SMEMAMachineReadySubscription))
            {
                FlagSubscriptionUpdated = true;
            }

            if(theNewProperties.SMEMAMachineReadyAlways != null && theNewProperties.SMEMAMachineReadyAlways != SMEMAMachineReadyAlways)
            {
                SMEMAMachineReadyAlways = theNewProperties.SMEMAMachineReadyAlways;
                StateMachine.Init();
            }

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }
        }

        
    }
}
