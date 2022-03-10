
namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockSMEMAStateMachine
    {
        internal ObservableValue<bool> MachineReady { get; private set; } = new ObservableValue<bool>();
        internal ObservableValue<bool> GoodBoard { get; private set; } = new ObservableValue<bool>();
        internal ObservableValue<bool> GoodBoardDiverted { get; private set; } = new ObservableValue<bool>();
        internal ObservableValue<bool> BadBoard { get; private set; } = new ObservableValue<bool>();
        internal ObservableValue<bool> BadBoardDiverted { get; private set; } = new ObservableValue<bool>();
    }
}
