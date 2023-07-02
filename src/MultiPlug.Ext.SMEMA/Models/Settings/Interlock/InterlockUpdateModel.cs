
namespace MultiPlug.Ext.SMEMA.Models.Settings.Interlock
{
    public class InterlockUpdateModel
    {
        public string Guid { get; set; }

        public int StartupMachineReady { get; set; }
        public int StartupGoodBoard { get; set; }
        public int StartupBadBoard { get; set; }
        public int StartupFlipBoard { get; set; }

        public bool PermissionInterfaceUI { get; set; }
        public bool PermissionInterfaceREST { get; set; }
        public bool PermissionInterfaceSubscriptions { get; set; }

        public bool TriggerBlockGoodBoardOnMachineNotReady { get; set; }
        public bool TriggerBlockBadBoardOnMachineNotReady { get; set; }
        public bool TriggerBlockFlipBoardOnMachineNotReady { get; set; }
        public bool TriggerBlockFlipBoardOnGoodBoardNotAvailable { get; set; }
        public bool TriggerBlockFlipBoardOnBadBoardNotAvailable { get; set; }

        public string MachineReadyInterlockSubscriptionId { get; set; }
        public string MachineReadyInterlockSubscriptionBlock { get; set; }
        public string MachineReadyInterlockSubscriptionUnblock { get; set; }
        public string MachineReadyInterlockSubscriptionLatchOn { get; set; }
        public string MachineReadyInterlockSubscriptionLatchOff { get; set; }

        public string GoodBoardInterlockSubscriptionId { get; set; }
        public string GoodBoardInterlockSubscriptionBlock { get; set; }
        public string GoodBoardInterlockSubscriptionUnblock { get; set; }
        public string GoodBoardInterlockSubscriptionLatchOn { get; set; }
        public string GoodBoardInterlockSubscriptionLatchOff { get; set; }
        public string GoodBoardInterlockSubscriptionDivertOn { get; set; }
        public string GoodBoardInterlockSubscriptionDivertOff { get; set; }
        public string GoodBoardInterlockSubscriptionDivertLatchOn { get; set; }
        public string GoodBoardInterlockSubscriptionDivertLatchOff { get; set; }
        public string GoodBoardInterlockSubscriptionUnblockFlipOn { get; set; }
        public string GoodBoardInterlockSubscriptionBlockFlipOff { get; set; }
        public string GoodBoardInterlockSubscriptionDivertOnFlipOn { get; set; }
        public string GoodBoardInterlockSubscriptionDivertOffFlipOff { get; set; }

        public string BadBoardInterlockSubscriptionId { get; set; }
        public string BadBoardInterlockSubscriptionBlock { get; set; }
        public string BadBoardInterlockSubscriptionUnblock { get; set; }
        public string BadBoardInterlockSubscriptionLatchOn { get; set; }
        public string BadBoardInterlockSubscriptionLatchOff { get; set; }
        public string BadBoardInterlockSubscriptionDivertOn { get; set; }
        public string BadBoardInterlockSubscriptionDivertOff { get; set; }
        public string BadBoardInterlockSubscriptionDivertLatchOn { get; set; }
        public string BadBoardInterlockSubscriptionDivertLatchOff { get; set; }
        public string BadBoardInterlockSubscriptionUnblockFlipOn { get; set; }
        public string BadBoardInterlockSubscriptionBlockFlipOff { get; set; }
        public string BadBoardInterlockSubscriptionDivertOnFlipOn { get; set; }
        public string BadBoardInterlockSubscriptionDivertOffFlipOff { get; set; }

        public string FlipBoardInterlockSubscriptionId { get; set; }
        public string FlipBoardInterlockSubscriptionBlock { get; set; }
        public string FlipBoardInterlockSubscriptionUnblock { get; set; }
        public string FlipBoardInterlockSubscriptionLatchOn { get; set; }
        public string FlipBoardInterlockSubscriptionLatchOff { get; set; }

        public string MachineReadyBlockEventId { get; set; }
        public string MachineReadyBlockEventDescription { get; set; }
        public bool MachineReadyBlockEventBlockedEnabled { get; set; }
        public bool MachineReadyBlockEventUnblockedEnabled { get; set; }
        public string MachineReadyBlockEventSubject { get; set; }
        public string MachineReadyBlockEventUnblockedValue { get; set; }
        public string MachineReadyBlockEventBlockedValue { get; set; }

        public string GoodBoardBlockEventId { get; set; }
        public string GoodBoardBlockEventDescription { get; set; }
        public bool GoodBoardBlockEventBlockedEnabled { get; set; }
        public bool GoodBoardBlockEventUnblockedEnabled { get; set; }
        public string GoodBoardBlockEventSubject { get; set; }
        public string GoodBoardBlockEventUnblockedValue { get; set; }
        public string GoodBoardBlockEventBlockedValue { get; set; }

        public string BadBoardBlockEventId { get; set; }
        public string BadBoardBlockEventDescription { get; set; }
        public bool BadBoardBlockEventBlockedEnabled { get; set; }
        public bool BadBoardBlockEventUnblockedEnabled { get; set; }
        public string BadBoardBlockEventSubject { get; set; }
        public string BadBoardBlockEventUnblockedValue { get; set; }
        public string BadBoardBlockEventBlockedValue { get; set; }

        public string FlipBoardBlockEventId { get; set; }
        public string FlipBoardBlockEventDescription { get; set; }
        public bool FlipBoardBlockEventBlockedEnabled { get; set; }
        public bool FlipBoardBlockEventUnblockedEnabled { get; set; }
        public string FlipBoardBlockEventSubject { get; set; }
        public string FlipBoardBlockEventUnblockedValue { get; set; }
        public string FlipBoardBlockEventBlockedValue { get; set; }

        public int DelayFlipThenBoardAvailable { get; set; }
    }
}
