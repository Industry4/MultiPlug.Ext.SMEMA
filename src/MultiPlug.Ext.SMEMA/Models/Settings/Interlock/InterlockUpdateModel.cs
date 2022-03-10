
namespace MultiPlug.Ext.SMEMA.Models.Settings.Interlock
{
    public class InterlockUpdateModel
    {
        public string Guid { get; set; }

        public int StartupMachineReady { get; set; }
        public int StartupGoodBoard { get; set; }
        public int StartupBadBoard { get; set; }

        public string InterlockSubscriptionId { get; set; }
        public string InterlockSubscriptionReadyValue { get; set; }

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

    }
}
