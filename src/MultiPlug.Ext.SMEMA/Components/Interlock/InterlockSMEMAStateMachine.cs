﻿
namespace MultiPlug.Ext.SMEMA.Components.Interlock
{
    internal class InterlockSMEMAStateMachine
    {
        internal bool MachineReady { get; set; }
        internal bool GoodBoard { get; set; }
        internal bool BadBoard { get; set; }
    }
}