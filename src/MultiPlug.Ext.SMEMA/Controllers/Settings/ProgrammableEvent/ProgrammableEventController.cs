using System.Linq;
using System.Collections.Generic;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Models.Settings.ProgrammableEvent;
using MultiPlug.Ext.SMEMA.Models.Exchange;
using MultiPlug.Ext.SMEMA.Models.Components.BeaconTower;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.ProgrammableEvent
{
    [Route("lane/programmable-event")]
    public class ProgrammableEventController : SettingsApp
    {
        public Response Get(string id, string eventguid)
        {
            LaneComponent LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == id);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }


            BeaconTowerEvent EventSearch = LaneSearch.BeaconTower.BeaconTowerEvents.FirstOrDefault(Lane => Lane.Guid == eventguid);

            if (EventSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            return new Response
            {
                Model = new ProgrammableEventModel
                {
                    Guid = id,
                    EventGuid = eventguid,
                    EventId = EventSearch.Id,
                    EventDescription = EventSearch.Description,
                    EventSubject = EventSearch.Subjects[0],
                    EventTrueValue = EventSearch.TrueValue,
                    EventTrueDelay = EventSearch.TrueDelay.Value,
                    EventFalseValue = EventSearch.FalseValue,
                    EventFalseDelay = EventSearch.FalseDelay.Value,
                    EventTrueEnabled = EventSearch.TrueEnabled.Value,
                    EventFalseEnabled = EventSearch.FalseEnabled.Value,
                    RuleOperators = EventSearch.RuleSteps.Select( r => r.Operator).ToArray(),
                    RuleOperands = EventSearch.RuleSteps.Select(r => r.Operand).ToArray(),
                    RuleTruths = EventSearch.RuleSteps.Select(r => r.Truth).ToArray(),
                    RuleOperandsList = LaneSearch.BeaconTower.OperandsList
                },
                Template = Templates.SettingsProgrammableEvent,
            };
        }

        public Response Post(ProgrammableEventModel theModel)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            var SequenceStep = new List<BeaconTowerEventSequenceStep>();

            if (theModel.RuleOperands != null)
            {
                for (int i = 0; i < theModel.RuleOperands.Length; i++)
                {
                    SequenceStep.Add(new BeaconTowerEventSequenceStep
                    {
                        Operator = theModel.RuleOperators[i],
                        Operand = theModel.RuleOperands[i],
                        Truth = theModel.RuleTruths[i]
                    });
                }
            }

            BeaconTowerProperties Properties = new BeaconTowerProperties
            {
                BeaconTowerEvents = new BeaconTowerEvent[]
                {
                    new BeaconTowerEvent
                    {
                        Guid = theModel.EventGuid,
                        Id = theModel.EventId,
                        Description = theModel.EventDescription,
                        Subjects = new string[] { theModel.EventSubject },
                        TrueValue = theModel.EventTrueValue,
                        TrueDelay = theModel.EventTrueDelay,
                        FalseValue = theModel.EventFalseValue,
                        FalseDelay = theModel.EventFalseDelay,
                        TrueEnabled = theModel.EventTrueEnabled,
                        FalseEnabled = theModel.EventFalseEnabled,
                        RuleSteps = SequenceStep.ToArray()
                    }
                }
            };

            LaneSearch.BeaconTower.UpdateProperties(Properties);
            LaneSearch.BeaconTower.ExecuteEvent(theModel.EventGuid);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
