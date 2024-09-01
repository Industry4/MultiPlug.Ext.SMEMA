
namespace MultiPlug.Ext.SMEMA.Models.Settings.ProgrammableEvent
{
    public class ProgrammableEventModel
    {
        public string EventDescription { get; set; }
        public int EventFalseDelay { get; set; }
        public bool EventFalseEnabled { get; set; }
        public string EventFalseValue { get; set; }
        public string EventGuid { get; set; }
        public string EventId { get; set; }
        public string EventSubject { get; set; }
        public int EventTrueDelay { get; set; }
        public bool EventTrueEnabled { get; set; }
        public string EventTrueValue { get; set; }
        public string Guid { get; set; }
        public int[] RuleOperators { get; set; }
        public int[] RuleOperands { get; set; }
        public int[] RuleTruths { get; set; }
        public string[] RuleOperandsList { get; set; }
    }
}
